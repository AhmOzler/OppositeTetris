using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{    
    [SerializeField] [Range(10, 100)] int difficulty = 20;
    Collider2D button;
    Shape shape = null;
    Spawner spawner;  
    bool isCoroutineActive = false;
    IEnumerator destroyRoutine;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
    }


    private void Update() {

        if(Board.Instance.IsOverLimit()) {
            StartCoroutine(Board.Instance.DestroyAllRows());
            return;
        }

        if(isCoroutineActive) return;

        if (Input.touchCount > 0) {

            Touch touch = Input.touches[0];
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            TouchBegan(touch, touchPos);

            TouchMoved(touch, touchPos);

            TouchEnd(touch);            
        }       
    }


    void TouchBegan(Touch touch, Vector2 touchPos) {

        if (touch.phase == TouchPhase.Began)
        {
            button = Physics2D.OverlapPoint(touchPos);

            if (button) 
                shape = button.GetComponent<Button>().StoredShape;

            if (shape != null)
            {              
                shape.transform.localScale = Vector3.one;
                shape.SetPivotOutButton();
            }
        }
    }


    void TouchMoved(Touch touch, Vector2 touchPos)
    {
        if (shape == null) return;

        if (touch.phase == TouchPhase.Moved)
        {
            var x = Mathf.Round(touchPos.x);
            var y = Mathf.Round(touchPos.y);
            shape.transform.position = new Vector2(x, y + 1);
        }
    }


    void TouchEnd(Touch touch) 
    {
        if(button == null || shape == null) return;

        if (touch.phase == TouchPhase.Ended)
        {
            if (Board.Instance.IsValidPosAreaShape(shape.transform) && ShadowShapes.Instance.ShadowShape)
            {
                ShadowShapes.Instance.ResetShadowShape(shape);
                Board.Instance.StoreShapeInGrid(shape.transform);
                destroyRoutine = Board.Instance.DestroyFullRows();
                StartCoroutine(destroyRoutine);
                Board.Instance.SqrSFXandVFX();

                SoundManager.Instance.Play("ValidPosition");
                button.GetComponent<Button>().StoredShape = null;
                shape = null;

                UIController.Instance.ScoreText(Board.Instance.DestroyedRowsCount);
                UIController.Instance.LevelText(difficulty);                                      
            }
            else
            {              
                StartCoroutine("TurntoButtonPos");
                SoundManager.Instance.Play("InvalidPosition");                           
            }      
        }
    }


    public IEnumerator TurntoButtonPos()
    {
        isCoroutineActive = true;

        shape.SetPivotInButton();
        shape.transform.localScale = new Vector2(.5f, .5f);

        while ((shape.transform.position - button.transform.position).sqrMagnitude > Mathf.Epsilon)
        {   
            shape.transform.position = Vector2.MoveTowards(
                shape.transform.position, button.transform.position, 50 * Time.deltaTime);

            yield return null;
        }

        shape.transform.position = button.transform.position;
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
                if (button && shape)
                {
                    if (Board.Instance.IsValidPosAreaShape(shape.transform))
                    {
                        ShadowShapes.Instance.GetShadowShape(shape, true);
                    }
                    else
                    {
                        ShadowShapes.Instance.GetShadowShape(shape, false);
                    }
                }
            }
        }
    }
}
