#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace ResolutionMagic
{
    // this script creates the custom inspector for the Resolution Manager script
    [CustomEditor(typeof(ResolutionManager))]
    public class ResolutionManager_Editor : Editor
    {
        private ResolutionManager myTarget;
        private SerializedObject soTarget;
        private SerializedProperty DebugMode;

        private SerializedProperty ZoomTo;
        private SerializedProperty ExtraCameras;

        private SerializedProperty autoCheckResolutionChange;

        private SerializedProperty autoCheckFrequency;

        private SerializedProperty zoomMethod;
        private SerializedProperty zoomSpeed;

        private SerializedProperty zoomOvershootProtectionSpeed;

        private SerializedProperty ForceRefresh;

        private SerializedProperty centreOnCanvasWhenZoomToMax;

        // styling
        GUIStyle headingStyle;
        GUIStyle subheadingStyle;
        GUIStyle messageStyle;

        private void OnEnable()
        {
            myTarget = (ResolutionManager)target;
            soTarget = new SerializedObject(target);

            // fields
            ZoomTo = soTarget.FindProperty("ZoomTo");
            DebugMode = soTarget.FindProperty("DebugMode");
            ExtraCameras = soTarget.FindProperty("ExtraCameras");
            autoCheckResolutionChange = soTarget.FindProperty("autoCheckResolutionChange");
            autoCheckFrequency = soTarget.FindProperty("autoCheckFrequency");
            zoomMethod = soTarget.FindProperty("zoomMethod");
            zoomSpeed = soTarget.FindProperty("zoomSpeed");
            zoomOvershootProtectionSpeed = soTarget.FindProperty("zoomOvershootProtectionSpeed");
            ForceRefresh = soTarget.FindProperty("ForceRefresh");
            centreOnCanvasWhenZoomToMax = soTarget.FindProperty("centreOnCanvasWhenZoomToMax");

            // setup styles
            headingStyle = new GUIStyle();
			headingStyle.fontSize = 15;
			headingStyle.fontStyle = FontStyle.Bold;
			headingStyle.wordWrap = true;

			subheadingStyle = new GUIStyle();
			subheadingStyle.fontSize = 13;
			subheadingStyle.fontStyle = FontStyle.Italic;
			subheadingStyle.wordWrap = true;

			messageStyle = new GUIStyle();
			messageStyle.fontSize = 12;
			messageStyle.fontStyle = FontStyle.Italic;
			messageStyle.wordWrap = true;


        }

        public override void OnInspectorGUI()
        {

            soTarget.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(new GUIContent("<color=green>Main Setup</color>"), headingStyle);
            
            EditorGUILayout.PropertyField(ZoomTo);

            if(ZoomTo.intValue == 1) // background, 0 = displayareacanvas
            {
                EditorGUILayout.PropertyField(centreOnCanvasWhenZoomToMax);
            }            

            EditorGUILayout.PropertyField(zoomMethod);

            if(zoomMethod.intValue == 1) // 0 represents Instand zoom method enum value, 1 is gradual zoom
            {
                EditorGUILayout.PropertyField(zoomSpeed);
            }
            EditorGUILayout.PropertyField(zoomOvershootProtectionSpeed);
            

            EditorGUILayout.LabelField(new GUIContent("<color=green>Other Configuration</color>"), headingStyle);

            EditorGUILayout.PropertyField(autoCheckResolutionChange);
            if(autoCheckResolutionChange.boolValue)
            {
                EditorGUILayout.PropertyField(autoCheckFrequency);
            }

            EditorGUILayout.PropertyField(ExtraCameras);           

            EditorGUILayout.LabelField(new GUIContent("<color=green>Debug</color>"), headingStyle);
            EditorGUILayout.PropertyField(DebugMode);
            EditorGUILayout.PropertyField(ForceRefresh);

            EditorGUILayout.Separator();

            soTarget.ApplyModifiedProperties ();

        }
    }
}
#endif

