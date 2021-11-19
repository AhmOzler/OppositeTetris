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
    int topRowIndex;
    Shape shadowShape;
    [SerializeField] bool isAnimPlaying = false;
    public bool IsAnimPlaying => isAnimPlaying;
    bool isHitBottom = false;

    private void Awake() {

        if(instance) {
            Destroy(gameObject);
            return;
        }          
        else
            instance = this as Board;
        
    }


    private void Start() {

        gridArray = new Transform[boardWidth, boardHeight];
        GridLineUp();
    }


    void GridLineUp()
    {   
        for (int y = 5; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                GameObject gridInstance = Instantiate(grid, new Vector2(x, y), Quaternion.identity, transform);
                gridInstance.GetComponent<SpriteRenderer>().color = gridColor;
                gridInstance.name = x.ToString() + " " + y.ToString();
            }
        }
    }


    private void RoundPos(Transform child, out int childx, out int childy)
    {
        childx = (int) Mathf.Round(child.position.x);
        childy = (int) Mathf.Round(child.position.y);
    }


    public void StoreShapeInGrid(Transform shape)
    {
        if(shape.childCount > 0) { //ANCHOR shape objesi sqrlarım parenti ise(Shapeler için).

            foreach (Transform child in shape) {

                int childx, childy;
                RoundPos(child, out childx, out childy);

                gridArray[childx, childy] = child;
                gridArray[childx, childy].GetComponent<SpriteRenderer>().sortingOrder = 9;
            }
        }
        else { //ANCHOR sqrların parenti yok ise(TopSqr için).

            int shapeX = (int)Mathf.Round(shape.position.x);
            int shapeY = (int)Mathf.Round(shape.position.y);

            gridArray[shapeX, shapeY] = shape;
            gridArray[shapeX, shapeY].GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
    }


    public bool IsValidPosition(Transform shape) {

        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos(child, out childx, out childy);

            if (childx < 0 || childx >= boardWidth || childy < 0 || childy > 18) return false;

            if (gridArray[childx, childy] != null) return false;
        }

        return true;
    }


    public bool IsValidPosAreaShape(Transform shape) {

        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos(child, out childx, out childy);

            if (childx < 0 || childx >= boardWidth || childy < 0 || childy > 4) return false;
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


    public void SqrSFXandVFX()
    {
        destroyedRowsCount = 0;

        for (int y = 0; y < boardHeight; y++)
        {
            if(IsGridsFullInRow(y)) 
            {
                destroyedRowsCount++; // ANCHOR Yok edilen satır sayısı.

                for (int x = 0; x < boardWidth; x++)
                {
                    if (gridArray[x, y].gameObject.CompareTag("ChangeSqr"))
                    UIController.Instance.IncreaseChangeButton();

                    gridArray[x, y].GetComponent<Animator>().Play("DestroyAnim");
                    SoundManager.Instance.Play("DestroyBricks");
                } 
            }
        }             
    }


    private void DestroyRow(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            Destroy(gridArray[x, y].gameObject);
            gridArray[x, y] = null;
        }       
    }


    public void ShiftAllRowsUp(int y) {

        for (int i = y; i > 0; i--)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (gridArray[x, i] != null)
                {                  
                    gridArray[x, i].position += Vector3.up;
                    gridArray[x, i + 1] = gridArray[x, i];                  
                    gridArray[x, i] = null;
                }
            }
        }                 
    }

    
    public IEnumerator DestroyAllRows() {

        isAnimPlaying = true;

        yield return new WaitForSeconds(0.5f);      

        for (int y = 0; y < boardHeight; y++)
        {
            if (IsGridsFullInRow(y))
            {                                            
                DestroyRow(y);
                ShiftAllRowsUp(y);
                y--;
            }
        }  

        isAnimPlaying = false; 
    }
    

    public void ShiftRowDown() {

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {                
                if(gridArray[x, y] != null) {
                    
                    gridArray[x, y].position += Vector3.down;
                    gridArray[x, y - 1] = gridArray[x, y];
                    gridArray[x, y] = null;
                }
            }
        }
    }


    public bool IsOverLimit() {

        for (int x = 0; x < boardWidth; x++)
        {
            if (gridArray[x, 5] != null) return true;
        }
        
        return false;
    }
}
