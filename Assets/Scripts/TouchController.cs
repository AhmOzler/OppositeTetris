using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchController : MonoBehaviour
{
    [SerializeField] Collider2D button;
    [SerializeField] Shape shape = null;
    Spawner spawner;  
    [SerializeField] bool isCoroutineActive = false;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
    }


    private void Update() {
        if(isCoroutineActive) return;

        if (Input.touchCount > 0) {

            Touch touch = Input.touches[0];
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            TouchBegan(touch, touchPos);

            TouchMoved(touch, touchPos);

            TouchEnd(touch);

            
        }
        Board.Instance.DestroyAllRows();
    }


    void TouchBegan(Touch touch, Vector2 touchPos) {

        if (touch.phase == TouchPhase.Began)
        {
            button = Physics2D.OverlapPoint(touchPos);

            if (button)
                shape = button.GetComponent<StoredShape>().Shape;

            if (shape != null)
            {
                shape.transform.localScale = Vector3.one;
                Board.Instance.CreateShadowShape(shape);
            }
        }
    }


    void TouchMoved(Touch touch, Vector2 touchPos)
    {
        if (shape == null) return;

        if (touch.phase == TouchPhase.Moved)
        {
            shape.transform.position = new Vector2(touchPos.x, touchPos.y + 3.5f);

            if (Board.Instance.IsValidPosition(shape.transform)) {

                Board.Instance.ShadowShapePos(shape, true);
            }
            else {
                Board.Instance.ShadowShapePos(shape, false);
            }
        }
    }


    void TouchEnd(Touch touch) 
    {
        if(button == null) return;

        if (touch.phase == TouchPhase.Ended)
        {
            if (Board.Instance.IsValidPosition(shape.transform))
            {
                Board.Instance.ResetShadowShape(shape.transform);

                button.GetComponent<StoredShape>().Shape.MoveDownOn = true;
                button.GetComponent<StoredShape>().Shape = null;
                shape = null;
                spawner.DestroyAllShapes();
                spawner.SpawnShapeInButtons();
            }
            else
            {
                StartCoroutine(TurntoButtonPos());
                Board.Instance.ResetShadowShape(shape.transform);
                shape.transform.localScale = new Vector2(.8f, .8f);
            }      
        }
    }


    public IEnumerator TurntoButtonPos()
    {
        isCoroutineActive = true;

        while ((shape.transform.position - (button.transform.position + shape.ShapeOffset)).sqrMagnitude > Mathf.Epsilon)
        {
            isCoroutineActive = true;

            shape.transform.position = Vector2.MoveTowards(
                shape.transform.position, button.transform.position + shape.ShapeOffset, 50 * Time.deltaTime);

            yield return null;
        }

        shape.transform.position = button.transform.position + shape.ShapeOffset;
        button = null;
        shape = null;
        isCoroutineActive = false;
    }


    private void LateUpdate() {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Moved)
            {
                if (button != null)
                {
                    Board.Instance.ShadowShapePos(shape, true);
                }
            }
        }
    }
}
