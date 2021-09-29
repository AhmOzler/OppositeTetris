using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;
    [SerializeField] Transform[] shadowShapeArray;
    [SerializeField] Transform bottomSquare;
    [SerializeField] Transform bonusSquare;
    [SerializeField] Transform[] buttons;
    int storedShapeCount;
    Transform shadowShape;
    public int StoredShapeCount {
        get { return storedShapeCount; }       
        set {             
            storedShapeCount += value; 

            if(storedShapeCount >= 5 - UIController.Instance.Level) {
                storedShapeCount = 0;  
            }                        
        }
    } 


    private void Start() {
        
        CreateShadowShapes();
    }
    

    public Shape SpawnShape(Transform transform) {

        Shape randomShape = shapeTypes[UnityEngine.Random.Range(0, shapeTypes.Length)];

        var shape = Instantiate(randomShape, transform.position, Quaternion.identity, transform) as Shape;
        shape.name = randomShape.name;
        return shape;
    }

    
    void CreateShadowShapes() {

        Transform shadowShapesHolder = new GameObject("ShadowShapes").transform;
        shadowShapeArray = new Transform[shapeTypes.Length];

        for (int i = 0; i < shapeTypes.Length; i++)
        {
            Shape shadowShape = Instantiate(shapeTypes[i], shadowShapesHolder.position, Quaternion.identity, shadowShapesHolder);
            shadowShape.gameObject.SetActive(false);
            shadowShape.name = shapeTypes[i].gameObject.name;
            shadowShapeArray[i] = shadowShape.transform;
        }

        foreach (Transform child in shadowShapesHolder)
        {
            var renderers = child.GetComponentsInChildren<SpriteRenderer>();
            child.GetComponent<Shape>().SetPivotOutButton();

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = new Color(1, 1, 1, .5f);
                renderer.sortingOrder = 1;
            }
        }
    }


    public void GetShadowShape(Shape shape, bool setActive) {

        shadowShape = Array.Find<Transform>(shadowShapeArray, shadowShape => shadowShape.name == shape.name);
        
        int posX = (int) Mathf.Round(shape.transform.position.x);
        int posY = (int) Mathf.Round(shape.transform.position.y);

        shadowShape.position = new Vector2(posX, posY);
        shadowShape.rotation = shape.transform.rotation;
        
        shadowShape.gameObject.SetActive(setActive);
    }

    
    public void ResetShadowShape(Shape shape) {

        shape.transform.position = shadowShape.position;
        shadowShape.gameObject.SetActive(false);
        shadowShape.position = Vector3.zero;
    }


    public void DestroyShapeInButtons() {

        if(UIController.Instance.ChangeButtonCount <= 0) return;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i].GetComponent<Button>();

            if(!button.StoredShape) continue;

            Destroy(button.StoredShape.gameObject);
            button.StoredShape = null;
        }
    }


    private List<int> DigitList(int minValue, int maxValue)
    {
        List<int> sqrDigits = new List<int>();
        sqrDigits.Clear();
        int sqrRowNumber = UnityEngine.Random.Range(minValue, maxValue + 1);

        for (int x = 0; x < sqrRowNumber; x++)
        {
            int randomPosX = (int)UnityEngine.Random.Range(0, Board.Instance.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }


    public IEnumerator SpawnSqrAtBottom(int minValue, int maxValue, int percent)
    {
        if (storedShapeCount == 0) {
            
            Board board = Board.Instance;

            if(board.DestroyedRowsCount > 0) yield return new WaitForSeconds(1f);

            var sqrDigits = DigitList(minValue, maxValue);
                
            yield return new WaitForSeconds(0.2f);
            board.ShiftRowUp();
            SoundManager.Instance.Play("TeleportBricks");
            
            for (int r = 0; r < sqrDigits.Count; r++)
            {
                Vector2 randomXpos = new Vector2(sqrDigits.ToList()[r], transform.position.y);

                Transform sqr = Instantiate(Sqr(percent), randomXpos, Quaternion.identity);
                sqr.GetComponent<Animator>().Play("TeleportAnim");
                board.StoreShapeInGrid(sqr);
            }           
        }        
    }


    Transform Sqr(int percent) {

        if(UnityEngine.Random.Range(0, 100) < percent) {
            return bonusSquare;
        }

        return bottomSquare;
    }


    public void RotateShapesInButton() {

        SoundManager.Instance.Play("ButtonClick");

        foreach (Transform button in buttons)
        {
            if(!button.GetComponent<Button>().StoredShape) continue;

            button.GetComponent<Button>().StoredShape.RotateRight();
        }
    }
}
