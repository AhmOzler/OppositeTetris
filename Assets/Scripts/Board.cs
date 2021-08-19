using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance => instance;

    [SerializeField] GameObject grid;
    [SerializeField] [Range(0, 15)] int boardWidth;
    [SerializeField] [Range(0, 30)] int boardHeight;
    [SerializeField] Color shadowShapeColor = new Color(1, 1, 1, 0.2f);

    Transform[,] gridArray;  
    Shape shadowShape; 
    bool isHitBottom = false;

    private void Awake() {

        if(instance) {
            Destroy(gameObject);
            return;
        }          
        else
            instance = this as Board;

        gridArray = new Transform[boardWidth, boardHeight];
    }


    void Start()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                GameObject gridInstance = Instantiate(grid, new Vector2(x, y), Quaternion.identity, transform);
                gridInstance.name = x.ToString() + " " + y.ToString();
            }
        }
    }


    public void StoreShapeInGrid(Transform shape) {
        
        foreach (Transform child in shape)
        {
            int childx = (int) Mathf.Round(child.position.x);
            int childy = (int) Mathf.Round(child.position.y);

            gridArray[childx, childy] = child;
        }
    }


    public bool IsValidPosition(Transform shape) {

        foreach (Transform child in shape)
        {
            int childx = (int) Mathf.Round(child.position.x);
            int childy = (int) Mathf.Round(child.position.y);

            if (childx >= boardWidth || childx < 0 || childy < 0) return false;

            if (gridArray[childx, childy] != null) return false;
        }

        return true;
    }


    bool IsGridsFullInRow(int y) {

        for (int x = 0; x < boardWidth; x++)
        {
            if(gridArray[x, y] == null) return false;
        }

        return true;
    } 


    void DestroyRow(int y) {

        for (int x = 0; x < boardWidth; x++) {
            
            Destroy(gridArray[x, y].gameObject);
            gridArray[x, y] = null;
        }
    }


    void ShiftRowDown(int i) {
        
        for (int y = i; y < boardHeight; y++) {

            for (int x = 0; x < boardWidth; x++) {

                if (gridArray[x, y] != null) {

                    gridArray[x, y].position += Vector3.down;
                    gridArray[x, y - 1] = gridArray[x, y];
                    gridArray[x, y] = null;                   
                }
            }
        }
    }


    public void DestroyAllRows() {

        for (int y = 0; y < boardHeight; y++)
        {
            if(IsGridsFullInRow(y)) {

                DestroyRow(y);
                ShiftRowDown(y);
                y--; // ANCHOR y'nin sabit yerde kalıp üstteki sütunları aşağı çekmesi gerektiği için yapıldı.
            }
        }
    }


    public bool IsOverLimit() {

        for (int x = 0; x < boardWidth; x++)
        {
            if (gridArray[x, 23] != null) return true;
        }
        
        return false;
    }


    public void ShadowShape(Shape shape) {

        if(shadowShape == null) {
            
            shadowShape = Instantiate(shape, shape.transform.position, shape.transform.rotation) as Shape;
            shadowShape.name = "ShadowOf" + shape.name;

            var renderers = shadowShape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = shadowShapeColor;
            }
        }
        else {
            shadowShape.transform.position = shape.transform.position;
            shadowShape.transform.rotation = shape.transform.rotation;
        }
        

        isHitBottom = false;   
        
        while(!isHitBottom) {

            shadowShape.MoveDown();

            if(!IsValidPosition(shadowShape.transform)) {

                isHitBottom = true;
                shadowShape.MoveUp();
            }
        }
    }
}
