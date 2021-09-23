using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchController : MonoBehaviour
{    
    [SerializeField] [Range(0, 15)] int minSpawnNumber = 6;
    [SerializeField] [Range(0, 15)] int maxSpawnNumber = 6;
    [SerializeField] [Range(10, 100)] int difficulty = 20;
    [SerializeField] [Range(0, 100)] int changeSqrPercentage = 1;
    Collider2D button;
    Shape shape = null;
    Spawner spawner;  
    bool isCoroutineActive = false;
    IEnumerator spawnRoutine;
    IEnumerator shiftRoutine;


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
        }
    }


    void TouchEnd(Touch touch) 
    {
        if(button == null || shape == null) return;

        if (touch.phase == TouchPhase.Ended)
        {
            if (Board.Instance.IsValidPosition(shape.transform))
            {
                Board.Instance.ResetShadowShape(shape.transform);
                Board.Instance.StoreShapeInGrid(shape.transform);
                Board.Instance.DestroyAllRows();

                SoundManager.Instance.Play("ValidPosition");
                button.GetComponent<Button>().StoredShape = null;
                shape = null;

                UIController.Instance.ScoreText(Board.Instance.DestroyedRowsCount);
                UIController.Instance.LevelText(difficulty);

                shiftRoutine = spawner.SpawnRandomSqrAtBottom(minSpawnNumber, maxSpawnNumber, changeSqrPercentage);
                StartCoroutine(shiftRoutine);                                            
            }
            else
            {
                shape.SetPivotInButton();
                shape.transform.localScale = new Vector2(.5f, .5f);
                SoundManager.Instance.Play("InvalidPosition");
                StartCoroutine(TurntoButtonPos());
                Board.Instance.ResetShadowShape(shape.transform);  //TODO Hata             
            }      
        }
    }


    public IEnumerator TurntoButtonPos()
    {
        isCoroutineActive = true;

        while ((shape.transform.position - button.transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            isCoroutineActive = true;

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
                    if (Board.Instance.IsValidPosition(shape.transform))
                    {

                        Board.Instance.ShadowShapePos(shape, true);
                    }
                    else
                    {

                        Board.Instance.ShadowShapePos(shape, false);
                    }
                }
            }
        }
    }
}
