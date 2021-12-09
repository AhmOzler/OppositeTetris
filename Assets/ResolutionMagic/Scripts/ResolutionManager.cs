
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


namespace ResolutionMagic
{
    public class ResolutionManager : MonoBehaviour
    {
        #region SETTINGS

        /// USER DEFINED SETTINGS
        [Header("Settings")]
        public ZoomType ZoomTo;
        // choose to zoom to the canvas (show canvas at maximum size) or screen (show as much content as possible)
   
        [SerializeField] Camera[] ExtraCameras;

        float cameraSpeed = 0.25f;

        [SerializeField] bool autoCheckResolutionChange = true; // by default automatically detect when the resolution changes
        [SerializeField] float autoCheckFrequency = 0.5f; // by default check for a resolution change every 0.5 seconds

        float previousCameraSize;
        float _calculatedCameraSize;

        public float CalculatedCameraSize
        {
            get
            {
                return _calculatedCameraSize;
            }
            private set{
                _calculatedCameraSize = value;
            }
        }

        [SerializeField] bool DebugMode;

        #endregion

        bool resetBorders = false; // used internally to notify that the borders needs to move because the screen resolution has changed TODO: check this with new refactoring


        [Tooltip("Choose to zoom instantly to the correct level or gradually zoom in/out")]
        [SerializeField] ZoomingMethod zoomMethod;

        [Tooltip("This is how quickly the camera will change to a new zoom level.")]
        [SerializeField] float zoomSpeed = 0.5f;

        [Tooltip("If the zoom speed is too fast, the zoom might go too far. Therefore the zooming will be backtracked to compensate. This is the zoom increment for the backtracking. This should be lower than the zoom speed. Values 0 or lower will be ignored.")]
        [SerializeField] float zoomOvershootProtectionSpeed = 0.05f;

        public CameraBoundary CanvasBoundary;
        public CameraBoundary MaxAreaBoundary;

        float resHeight = 0f; // and resWidth - used to store the most recent resolution so we can check if it has changed
        float resWidth = 0f;
        static ResolutionManager myInstance;

        // the developer can choose to use a renderer to define the areas to be shown or use the new method of a box collider (box collider is easier to modify in the Unity editor)
        private bool getSizeFromCollider = false;

        public static ResolutionManager Instance
        {
            get { return myInstance; }
        }
        float canvasWidth;
        float canvasHeight;

        [SerializeField] bool centreOnCanvasWhenZoomToMax = true;
        public bool ForceRefresh = false;

        public event CameraZoomChangedEvent CameraZoomChanged;
        public delegate void CameraZoomChangedEvent(float previousSize, float newSize);

        void Awake()
        {
            myInstance = this;
            previousCameraSize = Camera.main.orthographicSize;
        }

        void Start()
        {
            InvokeRepeating("CheckForResolutionChange", 2, autoCheckFrequency);
            RefreshResolution();
        }


        #region PUBLIC METHODS
        // these methods are accessible from anywhere in your game as long as there is an instance of this script available
        // you access them like the following example:
        // ResolutionManager.Instance.MoveCamera(CameraDirections.Up, 0.5f);

        public void MoveCameraInDirection(CameraDirection direction, float moveDistance, bool moveSafely = true)
        {
            // move the camera in the specified direction and the specified distance
            // if RestrictScreenToBackground or moveSafely is true the camera will only move if the movement won't cause content outside the background region to show on the screen

            Vector3 newCameraPos = Camera.main.transform.position;

            switch (direction) // to move it two directions (e.g. up and left), call this function  for each direction separately
            {
                case CameraDirection.Up:
                    newCameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + moveDistance, -10f);
                    break;
                case CameraDirection.Down:
                    newCameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - moveDistance, -10f);
                    break;
                case CameraDirection.Left:
                    newCameraPos = new Vector3(Camera.main.transform.position.x - moveDistance, Camera.main.transform.position.y, -10f);
                    break;
                case CameraDirection.Right:
                    newCameraPos = new Vector3(Camera.main.transform.position.x + moveDistance, Camera.main.transform.position.y, -10f);
                    break;

            }

            if (moveSafely)
                MoveCameraSafely(newCameraPos, direction); // only move the camera if the movement will not reveal space outside the background area
            else
                MoveCameraUnsafely(newCameraPos); // move the camera regardless
        }

        public void MoveCameraPosition(Vector2 newPosition, bool moveSafely = true)
        {
            Vector2 currentPos = (Vector2)Camera.main.transform.position;

            if (moveSafely)
            {
                Vector3 newCameraYPos = new Vector3(Camera.main.transform.position.x, newPosition.y, Camera.main.transform.position.z);
                var dirY = newCameraYPos.y >= currentPos.y ? CameraDirection.Up : CameraDirection.Down;
                MoveCameraSafely(newCameraYPos, dirY);

                Vector3 newCameraXPos = new Vector3(newPosition.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
                var dirX = newCameraXPos.x >= currentPos.x ? CameraDirection.Right : CameraDirection.Left;
                MoveCameraSafely(newCameraXPos, dirX); // only move the camera if the movement will not reveal space outside the background area
            }
            else
            {
                var newCameraPos = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);
            
                MoveCameraUnsafely(newCameraPos); // move the camera regardless
            }
        }

        public void RefreshResolution()
        {
            previousCameraSize = Camera.main.orthographicSize;
            // force the resolution to be checked/changed as per the script's properties
            // this shouldn't be required unless you turn off the automatic checking

            // choose the correct zoom method based on the ZoomTo property
            if (ZoomTo == ZoomType.AlwaysDisplayedArea)
                ZoomCameraToCanvas();
            else if (ZoomTo == ZoomType.MaximumBounds)
                ZoomCameraToMaxGameArea();

           
        }

        void CompleteResolutionRefresh()
        {
             // store the current screen resolution for use elsewhere
            resHeight = Screen.height;
            resWidth = Screen.width;

            // ensure the borders are refreshed on the next frame update in case the resolution has changed
            resetBorders = true;

            // store the canvas dimensions for later use
            canvasHeight = CanvasHeight;
            canvasWidth = CanvasWidth;
            _scaleFactor = 0;

            foreach (Camera cam in ExtraCameras)
            {
                cam.orthographicSize = Camera.main.orthographicSize;
            }

            // remember the size in case it needs to be retrieved later
            CalculatedCameraSize = Camera.main.orthographicSize;

            // raise the camerazoomchanged event if it has been subscribed to anywhere
            if(CameraZoomChanged != null) CameraZoomChanged(previousCameraSize, CalculatedCameraSize);            
        }


        private float MaxHeight
        {
            get
            {
                return Mathf.Abs(MaxAreaBoundary.topLeftPos.y - MaxAreaBoundary.bottomRightPos.y);
            }
        }

        private float MaxWidth
        {
            get
            {
                return Mathf.Abs(MaxAreaBoundary.bottomRightPos.x - MaxAreaBoundary.topLeftPos.y);
            }
        }


        public void TurnOnBlackBars()
        {
            SetBlackBarsState(true);
        }

        public void TurnOffBlackBars()
        {
            SetBlackBarsState(false);
        }


        #endregion

        #region PRIVATE METHODS
        void MoveCameraUnsafely(Vector3 newCameraPosition)
        {
            // move the camera to the specified position
            // this method will always move the camera regardless of what content will be shown to the player
            // it may reveal 'black bars' or objects outside the game area
            Camera.main.transform.position = newCameraPosition;
        }

        void MoveCameraSafely(Vector3 newCameraPosition, CameraDirection direction)
        {
            // an example of moving the camera only if it doesn't make space outside the background visible
            bool safe = false;

            switch (direction)
            {
                case CameraDirection.Up:
                    safe = !PointIsVisibleToCamera(new Vector2(Camera.main.transform.position.x, MaxTopEdge - cameraSpeed));
                    break;
                case CameraDirection.Down:
                    safe = !PointIsVisibleToCamera(new Vector2(Camera.main.transform.position.x, MaxBottomEdge + cameraSpeed));
                    break;
                case CameraDirection.Left:
                    safe = !PointIsVisibleToCamera(new Vector2(MaxLeftEdge + cameraSpeed, Camera.main.transform.position.y));
                    break;
                case CameraDirection.Right:
                    safe = !PointIsVisibleToCamera(new Vector2(MaxRightEdge - cameraSpeed, Camera.main.transform.position.y));
                    break;
            }

            if (safe)
                Camera.main.transform.position = newCameraPosition;

        }


        void CheckForResolutionChange()
        {
            if (!autoCheckResolutionChange)
                return;

            if (HasResolutionChanged())
                RefreshResolution();

        }
        bool HasResolutionChanged()
        {
            if (resHeight == 0 || resWidth == 0)
                return false; // no resolution is set yet if these values are zero, so ignore

            if (Math.Abs(resWidth - Screen.width) > 2 || Math.Abs(resHeight - Screen.height) > 2)
                return true;

            return false;
        }
        void LateUpdate()
        {

            if (ForceRefresh)
            {
                RefreshResolution();
                ForceRefresh = false;
            }
        }

        void ZoomCameraToCanvas()
        {
            // zoom the camera in until the specified object is at the edge of the screen
            CentreCameraOn(CanvasBoundary.Centre);
            // get the position of the object we're zooming in on in viewport co-ordinates
            // Vector2 canvasTopLeft = new Vector2((Canvas.transform.position.x - CanvasWidth / 2), Canvas.transform.position.y - CanvasHeight / 2);

            if (PointIsVisibleToCamera(CanvasBoundary.topLeftPos))
            {
                ZoomIn(new Vector2[] { CanvasBoundary.topLeftPos });
            }
            else
            {
                ZoomOut(new Vector2[] { CanvasBoundary.topLeftPos });
            }            
        }

        void ZoomCameraToMaxGameArea()
        {
            // centre on the canvas first, as this is where all zooming originates
            if (centreOnCanvasWhenZoomToMax) CentreCameraOn(CanvasBoundary.Centre); else CentreCameraOn(MaxAreaBoundary.Centre);


            if (PointIsVisibleToCamera(MaxAreaBoundary.topLeftPos) || PointIsVisibleToCamera(MaxAreaBoundary.bottomRightPos))
            {
                ZoomIn(new Vector2[] { MaxAreaBoundary.topLeftPos, MaxAreaBoundary.bottomRightPos });
            }
            else
            {
                ZoomOut(new Vector2[] { MaxAreaBoundary.topLeftPos, MaxAreaBoundary.bottomRightPos });
            }

        }

        static bool PointIsVisibleToCamera(Vector2 point)
        {
            if (Camera.main.WorldToViewportPoint(point).x < 0 || Camera.main.WorldToViewportPoint(point).x > 1 || Camera.main.WorldToViewportPoint(point).y > 1 || Camera.main.WorldToViewportPoint(point).y < 0)
                return false;

            return true;
        }

        bool PointIsWithinBackground(Vector2 point)
        {

            if (point.x > MaxTopEdge || point.x < MaxBottomEdge || point.y > MaxRightEdge || point.y < MaxLeftEdge)
                return false;

            return true;


        }

        void ZoomOut(Vector2[] points)
        {
            StopCoroutine("ZoomOutCo");
            StopCoroutine("ZoomInCo");
            StartCoroutine(ZoomOutCo(points));
        }

        void ZoomIn(Vector2[] points)
        {
            StopCoroutine("ZoomOutCo");
            StopCoroutine("ZoomInCo");
            StartCoroutine(ZoomInCo(points));
        }

        IEnumerator ZoomInCo(Vector2[] points)
        {
            foreach (var point in points)
            {
                while (PointIsVisibleToCamera(point))
                {
                    Camera.main.orthographicSize -= zoomSpeed;
                    if (zoomMethod == ZoomingMethod.Gradual) yield return null;
                }


                while (!PointIsVisibleToCamera(point) && zoomOvershootProtectionSpeed > 0)
                {
                    Camera.main.orthographicSize += zoomOvershootProtectionSpeed;
                }

            }

            CompleteResolutionRefresh();
        }

        IEnumerator ZoomOutCo(Vector2[] points)
        {
            foreach (var point in points)
            {
                while (!PointIsVisibleToCamera(point))
                {
                    Camera.main.orthographicSize += zoomSpeed;
                    if (zoomMethod == ZoomingMethod.Gradual) yield return null;
                }
                while (PointIsVisibleToCamera(point) && zoomOvershootProtectionSpeed > 0)
                {
                    Camera.main.orthographicSize -= zoomOvershootProtectionSpeed; ;
                }

            }

            CompleteResolutionRefresh();
        }

        void CentreCameraOn(Vector2 pos)
        {
            // this centres the camera on a specific transform's location
            //Resolution Magic uses this to centre on the canvas before zooming the camera
            Vector3 newCameraPos = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
            Camera.main.transform.position = newCameraPos;
        }

        public void ToggleBlackBars()
        {
            var barsPrefab = GameObject.FindGameObjectWithTag("RM_Black_Bars");
            if (barsPrefab != null)
            {
                barsPrefab.GetComponent<BlackBars>().Enabled = !barsPrefab.GetComponent<BlackBars>().Enabled;
            }
            else
            {
                Debug.Log("Resolution Magic warning: trying to toggle black bars, but black bars prefab not found.");
            }
        }
        public void SetBlackBarsState(bool isActive)
        {
            var barsPrefab = GameObject.FindGameObjectWithTag("RM_Black_Bars");
            if (barsPrefab != null)
            {
                if (isActive)
                    barsPrefab.GetComponent<BlackBars>().Enabled = true;
                else
                    barsPrefab.GetComponent<BlackBars>().Enabled = false;
            }
            else
            {
                Debug.Log("Resolution Magic warning: trying to toggle black bars, but black bars prefab not found.");
            }
        }

        #endregion

        #region PROPERTIES


        // the EDGE properties return the furthest point on the relevant edge, e.g. the CameraLeftEdge is the leftmost position the camera can see
        // and CanvasTopEdge is the topmost point on the canvas (which will not necessarily be the same as the top of the screen)
        // these values are in regular vector space with (0,0) representing the centre of the screen
        public float ScreenLeftEdge
        {
            get
            {
                Vector2 topLeft = new Vector2(0, 1);
                Vector2 topLeftInScreen = Camera.main.ViewportToWorldPoint(topLeft);
                return topLeftInScreen.x;
            }
        }

        public float ScreenRightEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 0);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.x;
            }
        }

        public float ScreenTopEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 1);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.y;
            }
        }

        public float ScreenBottomEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 0);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.y;
            }
        }

        public Vector2 ScreenTopLeft
        {
            get
            {
                return new Vector2(ScreenLeftEdge, ScreenTopEdge);
            }
        }

        public Vector2 ScreenTopRight
        {
            get
            {
                return new Vector2(ScreenRightEdge, ScreenTopEdge);
            }
        }

        public Vector2 ScreenBottomLeft
        {
            get
            {
                return new Vector2(ScreenLeftEdge, ScreenBottomEdge);
            }
        }

        public Vector2 ScreenBottomRight
        {
            get
            {
                return new Vector2(ScreenRightEdge, ScreenBottomEdge);
            }
        }

        public float CanvasLeftEdge
        {
            get
            {
                return CanvasBoundary.topLeftPos.x;
            }
        }

        public float CanvasRightEdge
        {
            get
            {
                return CanvasBoundary.bottomRightPos.x;
            }
        }

        public float CanvasTopEdge
        {
            get
            {
                return CanvasBoundary.topLeftPos.y;
            }
        }

        public float CanvasBottomEdge
        {
            get
            {
                return CanvasBoundary.bottomRightPos.y;
            }
        }


        public float CanvasWidth
        {
            get
            {
                return Mathf.Abs(CanvasBoundary.bottomRightPos.x - CanvasBoundary.topLeftPos.x);
            }
        }

        public float CanvasHeight
        {
            get
            {
                return Mathf.Abs(CanvasBoundary.topLeftPos.y - CanvasBoundary.bottomRightPos.y);
            }
        }

        public float ScreenWidth
        {
            get
            {
                Vector2 topRightCorner = new Vector2(ScreenRightEdge, 0);
                float width = topRightCorner.x * 2;
                return width;
            }
        }

        public float ScreenHeight
        {
            get
            {

                Vector2 topRightCorner = new Vector2(0, ScreenTopEdge);
                float height = topRightCorner.y * 2;
                return height;
            }
        }

        public float MaxLeftEdge
        {
            get
            {
                return MaxAreaBoundary.topLeftPos.x;
            }
        }

        public float MaxRightEdge
        {
            get
            {
                return MaxAreaBoundary.bottomRightPos.x;
            }
        }

        public float MaxTopEdge
        {
            get
            {
                return MaxAreaBoundary.topLeftPos.y;
            }
        }

        public float MaxBottomEdge
        {
            get
            {
                return MaxAreaBoundary.bottomRightPos.y;
            }
        }

        private float _scaleFactor = 0;
        public float ScaleFactor
        {
            get
            {

                if (_scaleFactor == 0)
                {
                    float ratio;
                    float canvasArea;
                    float screenArea;

                    canvasArea = canvasHeight * canvasWidth;

                    float screenX = ScreenRightEdge * 2; // double the distance from the centre to the screen edge
                    float screenY = ScreenTopEdge * 2; // double the distance from the centre to the screen edge
                    screenArea = screenX * screenY;
                    ratio = screenArea / canvasArea;
                    _scaleFactor = Mathf.Sqrt(ratio);
                    return _scaleFactor;
                }
                return _scaleFactor;
            }
        }

        public Vector2 ScreenEdgeAsVector(AlignPoint AlignedEdge)
        {

            switch (AlignedEdge)
            {

                case ResolutionManager.AlignPoint.Centre:
                    return Vector2.zero;

                case ResolutionManager.AlignPoint.Left:
                    return new Vector2(ScreenLeftEdge, ScreenCentre.y);


                case ResolutionManager.AlignPoint.Bottom:
                    return new Vector2(ScreenCentre.x, ScreenBottomEdge);


                case ResolutionManager.AlignPoint.Right:
                    return new Vector2(ScreenRightEdge, ScreenCentre.y);

                case ResolutionManager.AlignPoint.Top:
                    return new Vector2(ScreenCentre.x, ScreenTopEdge);

                case ResolutionManager.AlignPoint.TopLeftCorner:
                    return new Vector2(ScreenLeftEdge, ScreenTopEdge);


                case ResolutionManager.AlignPoint.TopRightCorner:
                    return new Vector2(ScreenRightEdge, ScreenTopEdge);


                case ResolutionManager.AlignPoint.BottomLeftCorner:
                    return new Vector2(ScreenLeftEdge, ScreenBottomEdge);


                case ResolutionManager.AlignPoint.BottomRightCorner:
                    return new Vector2(ScreenRightEdge, ScreenBottomEdge);


            }
            return new Vector2(0, 0);


        }
        public Vector2 ScreenEdgeAsVector(Edge AlignedEdge)
        {

            switch (AlignedEdge)
            {

                case ResolutionManager.Edge.Left:
                    return new Vector2(ScreenLeftEdge, 0);


                case ResolutionManager.Edge.Bottom:
                    return new Vector2(0, ScreenBottomEdge);


                case ResolutionManager.Edge.Right:
                    return new Vector2(ScreenRightEdge, 0);

                case ResolutionManager.Edge.Top:
                    return new Vector2(0, ScreenTopEdge);


            }
            return new Vector2(0, 0);
        }

        public Vector2 ScreenCentre
        {
            // returns the x-coordinate that is the centre of the screen on the x axis regardless of where the camera is
            get
            {
                Vector2 zeroZero = new Vector2(0.5f, 0.5f);

                Vector2 zeroZeroToWorld = Camera.main.ViewportToWorldPoint(zeroZero);


                return zeroZeroToWorld;
            }

        }
        public Vector2 CanvasEdgeAsVector(AlignPoint AlignedEdge)
        {

            switch (AlignedEdge)
            {
                case ResolutionManager.AlignPoint.Left:
                    return new Vector2(CanvasLeftEdge, 0);

                case ResolutionManager.AlignPoint.Bottom:
                    return new Vector2(0, CanvasBottomEdge);

                case ResolutionManager.AlignPoint.Right:
                    return new Vector2(CanvasRightEdge, 0);

                case ResolutionManager.AlignPoint.Top:
                    return new Vector2(0, CanvasTopEdge);

                case ResolutionManager.AlignPoint.TopLeftCorner:
                    return new Vector2(CanvasLeftEdge, CanvasTopEdge);


                case ResolutionManager.AlignPoint.TopRightCorner:
                    return new Vector2(CanvasRightEdge, CanvasTopEdge);


                case ResolutionManager.AlignPoint.BottomLeftCorner:
                    return new Vector2(CanvasLeftEdge, CanvasBottomEdge);


                case ResolutionManager.AlignPoint.BottomRightCorner:
                    return new Vector2(CanvasRightEdge, CanvasBottomEdge);


            }
            return new Vector2(0, 0);


        }

        public Vector2 CanvasEdgeAsVector(Edge AlignedEdge)
        {
            // overload to only allow edges (i.e. not corners) for placing the border around the screen/canvas
            switch (AlignedEdge)
            {
                case ResolutionManager.Edge.Left:
                    return new Vector2(CanvasLeftEdge, 0);


                case ResolutionManager.Edge.Bottom:
                    return new Vector2(0, CanvasBottomEdge);


                case ResolutionManager.Edge.Right:
                    return new Vector2(CanvasRightEdge, 0);

                case ResolutionManager.Edge.Top:
                    return new Vector2(0, CanvasTopEdge);

            }
            return new Vector2(0, 0);


        }

        public string ScreenAspectRatio()
        {

            // Resolution Magic doesn't use this, but you can use it to get the current screen ratio
            // you can add/remove resolutions to suit your project

            // needs to be fleshed out
            float ratio;
            string orientation;
            if (Screen.width > Screen.height)
            {
                ratio = (float)Screen.width / Screen.height;
                orientation = "landscape";
            }
            else
            {
                ratio = (float)Screen.height / Screen.width;
                orientation = "portrait";
            }

            // NOTE: because screen sizes can vary slightly
            // we need to use fuzzy logic to get the closest match rather than checking for the exact ratio
            // e.g. a screen might have an actual ratio of 1.59999 instead of 1.6

            if (ratio < 1.38f)
                return "4x3" + orientation;

            if (ratio < 1.59f)
                return "3x2" + orientation;

            if (ratio < 1.63f)
                return "16x10" + orientation;

            if (ratio < 1.7f)
                return "5x3" + orientation;

            if (ratio < 1.82f)
                return "16x9" + orientation;

            return "not_detected"; //fallback for very narrow or weird screens

        }
        #endregion

        #region ENUMS

        [System.Serializable]
        public enum ZoomType
        {
            AlwaysDisplayedArea,
            MaximumBounds

        }

        [System.Serializable]
        public enum ZoomingMethod
        {
            Instant,
            Gradual
        }

        public enum AlignPoint
        {
            Centre,
            Top,
            Bottom,
            Left,
            Right,
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner

        }

        public enum CameraDirection
        {
            Up,
            Down,
            Left,
            Right

        }
        public enum Edge
        {
            Left,
            Top,
            Right,
            Bottom

        }

        public enum AlignObject
        {
            Screen,
            AlwaysDisplayedArea
        }

        #endregion

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(DebugMode)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(CanvasEdgeAsVector(AlignPoint.TopLeftCorner), CanvasEdgeAsVector(AlignPoint.BottomRightCorner));
            }
        }
#endif

    }
}
