using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{    
    private static TouchController instance;
    public static TouchController Instance => instance;
    Collider2D button;
    Shape shape = null;
    bool isCoroutineActive = false;
    IEnumerator destroyRoutine;
    int failedNum;



    private void Awake() 
    {
        if (!instance) 
        {
            instance = this;
        }
        else
        {
            instance = null;
            Destroy(gameObject);
        }
    }



    private void Update() 
    {
        if(isCoroutineActive) return;

        if (Input.touchCount > 0) {

            Touch touch = Input.touches[0];
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            TouchBegan(touch, touchPos);

            TouchMoved(touch, touchPos);

            TouchEnd(touch);
        }       
    }



    void TouchBegan(Touch touch, Vector2 touchPos) 
    {
        if (touch.phase == TouchPhase.Began)
        {
            button = Physics2D.OverlapPoint(touchPos);

            if (button)
                shape = button.GetComponent<Button>().BoxShape;

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
            shape.SetAlphaColor(1f);
            UIController.Instance.PupHintBox(true, failedNum);
            shape.transform.position = new Vector2(touchPos.x, touchPos.y + 1);
            ShapeClamp();

            if (button && shape)
            {
                if (Board.Instance.IsValidPosForDefaultShape(shape.transform))
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



    void TouchEnd (Touch touch)
    {
        if(button == null || shape == null) return;

        if (touch.phase == TouchPhase.Ended)
        {
            if (Board.Instance.IsValidPosForDefaultShape(shape.transform) && ShadowShapes.Instance.ShadowShape)
            {
                ShadowShapes.Instance.ResetShadowShape(shape);
                shape.SetAlphaColor(1f);
                Board.Instance.StoreShapeInGrid(shape.transform);
                destroyRoutine = Board.Instance.DestroyFullRows();
                StartCoroutine(destroyRoutine);
                Board.Instance.SqrSFXandVFX();

                SoundManager.Instance.Play("ValidPosition");
                button.GetComponent<Button>().BoxShape = null;
                shape = null;

                UIController.Instance.ScoreText(Board.Instance.DestroyedSquaresCount + Board.Instance.EmptyGridCountInDestroyedRow * 2);
                UIController.Instance.LevelText();

                button.GetComponent<Button>().BoxShape = button.GetComponent<Button>().SpawnShape(button.GetComponent<Button>().RandomShape1());
                failedNum = 0;
            }
            else
            {            
                StartCoroutine("TurntoButtonPos");
                SoundManager.Instance.Play("InvalidPosition");
                failedNum++;
            }  

            UIController.Instance.PupHintBox(false, failedNum);    
        }
    }



    private void ShapeClamp()
    {
        foreach (Transform child in shape.transform)
        {
            if (child.position.x >= Board.Instance.BoardWidth - 1)
            {
                var dist = Vector2.Distance(new Vector2(shape.transform.position.x, 0), new Vector2(child.position.x, 0));
                shape.transform.position = new Vector2((Board.Instance.BoardWidth - 1) - dist, shape.transform.position.y);
            }
            else if (child.position.x <= 0)
            {
                var dist = Vector2.Distance(new Vector2(shape.transform.position.x, 0), new Vector2(child.position.x, 0));
                shape.transform.position = new Vector2(0 + dist, shape.transform.position.y);
            }

            if (child.position.y >= 19)
            {
                var dist = Vector2.Distance(new Vector2(0, shape.transform.position.y), new Vector2(0, child.position.y));
                shape.transform.position = new Vector2(shape.transform.position.x, (Board.Instance.BoardHeight - 1) - dist);
            }
            else if (child.position.y <= 0)
            {
                var dist = Vector2.Distance(new Vector2(0, shape.transform.position.y), new Vector2(0, child.position.y));
                shape.transform.position = new Vector2(shape.transform.position.x, 0 + dist);
            }
        }
    }



    public IEnumerator TurntoButtonPos() //ANCHOR String olarak TouchEnd'de kullanılıyor.
    {
        isCoroutineActive = true;

        shape.SetPivotInButton();  
        shape.SetAlphaColor(1f);    

        shape.transform.localScale = new Vector2(.5f, .5f);

        while ((shape.transform.position - button.transform.position).sqrMagnitude > Mathf.Epsilon)
        {          
            shape.transform.position = Vector2.MoveTowards(shape.transform.position, button.transform.position, 50 * Time.deltaTime);
                
            yield return null;
        }
      
        shape.transform.position = button.transform.position;

        button = null;
        shape = null;
        isCoroutineActive = false;
    }



    /* private void LateUpdate() 
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Moved)
            {
                if (button && shape)
                {
                    if (Board.Instance.IsValidPosForDefaultShape(shape.transform))
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
    } */
}
