using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance => instance;

    [SerializeField] GameObject grid;
    [SerializeField] Color gridColor = Color.white;
    [SerializeField] [Range(0, 15)] int boardWidth;
    public int BoardWidth => boardWidth;

    [SerializeField] [Range(0, 30)] int boardHeight;
    [SerializeField] Color shadowShapeColor = new Color(1, 1, 1, 0.2f);

    Transform[,] gridArray;
    [SerializeField] int destroyedRowsCount;
    public int DestroyedRowsCount => destroyedRowsCount;
    [SerializeField] int topRowIndex;
    /* bool checkGrid = false;
    public bool CheckGrid {
        get { return checkGrid; }
        set {
            if(checkGrid != value)
                checkGrid = value;
        }
    } */
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


    public void GridLineUp()
    {   
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                GameObject gridInstance = Instantiate(grid, new Vector2(x, y), Quaternion.identity, transform);
                gridInstance.GetComponent<SpriteRenderer>().color = gridColor;
                gridInstance.name = x.ToString() + " " + y.ToString();
            }
        }
    }


    public void StoreShapeInGrid(Transform shape)
    {
        foreach (Transform child in shape)
        {
            int childx = (int)Mathf.Round(child.position.x);
            int childy = (int)Mathf.Round(child.position.y);

            gridArray[childx, childy] = child;
            gridArray[childx, childy].GetComponent<SpriteRenderer>().sortingOrder = 1;
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

    
    public void DestroyAllRows() {

        destroyedRowsCount = 0;

        for (int y = 0; y < boardHeight; y++)
        {
            if (IsGridsFullInRow(y))
            {
                destroyedRowsCount ++; // ANCHOR Yok edilen satır sayısı.
                topRowIndex = y; // ANCHOR En üstteki full satırın kaçıncı stünda olduğu.

                for (int x = 0; x < boardWidth; x++)
                {
                    gridArray[x, y].GetComponent<Animator>().Play("DestroyAnim");
                    float animLength = gridArray[x, y].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                    Destroy(gridArray[x, y].gameObject, animLength);
                    gridArray[x, y] = null;
                }
            }
        }       
    }


    public IEnumerator ShiftAllRowsDown() {

        if(destroyedRowsCount <= 0) yield break;

        yield return new WaitForSeconds(1);

        for (int i = topRowIndex; i < boardHeight; i++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (gridArray[x, i] != null)
                {
                    gridArray[x, i].position += Vector3.down * destroyedRowsCount;
                    gridArray[x, i - destroyedRowsCount] = gridArray[x, i];
                    gridArray[x, i] = null;
                }
            }
        }                 
    }


    public void ShiftRowUp() {

        for (int y = boardHeight - 2; y >= 0; y--)
        {
            for (int x = 0; x < boardWidth; x++)
            {                
                if(gridArray[x, y] != null) {
                    
                    gridArray[x, y].position += Vector3.up;
                    gridArray[x, y].GetComponent<Animator>().Play("TeleportAnim");
                    gridArray[x, y + 1] = gridArray[x, y];
                    gridArray[x, y] = null;
                }
            }
        }
    }


    /* public IEnumerator DestroyAllRows() {

        //gridCheckList.Clear();

        for (int y = 0; y < boardHeight; y++)
        {
            //CheckGrid = IsThereFullRow(IsGridsFullInRow(y));
            
            if (IsGridsFullInRow(y)) {

                DestroyRow(y);
                yield return new WaitForSeconds(1f);
                ShiftRowDown(y);
                y--; // ANCHOR y'nin sabit yerde kalıp üstteki sütunları aşağı çekmesi gerektiği için yapıldı.
            }
        }       
    } */


    public bool IsOverLimit() {

        for (int x = 0; x < boardWidth; x++)
        {
            if (gridArray[x, 23] != null) return true;
        }
        
        return false;
    }


    public void CreateShadowShape(Shape shape) {

        if(shadowShape) return;

        shadowShape = Instantiate(shape, shape.transform.position, shape.transform.rotation) as Shape;
        shadowShape.name = "ShadowOf" + shape.name;

        var renderers = shadowShape.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = shadowShapeColor;
            renderer.sortingOrder = 0;
        }
    }


    public void ResetShadowShape(Transform shape)
    {
        if (shadowShape && shape)
        {
            shape.transform.position = shadowShape.transform.position;
            Destroy(shadowShape.gameObject);
        }
    }


    public void ShadowShapePos(Shape shape, bool setActive)
    {
        if(!shadowShape) return;

        int posX = (int) Mathf.Round(shape.transform.position.x);
        int posY = (int) Mathf.Round(shape.transform.position.y);

        shadowShape.transform.position = new Vector2(posX, posY);
        shadowShape.transform.rotation = shape.transform.rotation;
        shadowShape.gameObject.SetActive(setActive);

        /* isHitBottom = false;

        while (!isHitBottom)
        {
            shadowShape.MoveDown();

            if (!IsValidPosition(shadowShape.transform))
            {
                isHitBottom = true;
                shadowShape.MoveUp();
            }
        } */
    }


    public void CreateShadowShapeBottom(Shape shape) {

        CreateShadowShape(shape);
        
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
