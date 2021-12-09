using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance => instance;
    [SerializeField] GameObject grid;
    [SerializeField] [Range(0, 15)] int boardWidth;
    public int BoardWidth => boardWidth;
    [SerializeField] [Range(0, 30)] int boardHeight;
    Transform[,] gridArray;
    [SerializeField] int destroyedRowsCount;
    public int DestroyedRowsCount => destroyedRowsCount;
    [SerializeField] bool isAnimPlaying = false;
    public bool IsAnimPlaying => isAnimPlaying;
    bool isGameOver;
    public bool IsGameOver => isGameOver;
    bool isCoroutineActive = false;

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
                if(child.CompareTag("FreeShape")) child.gameObject.tag = "StoredShape";
            }
        }
        else { //ANCHOR sqrların parenti yok ise(TopSqr için).

            int shapeX = (int)Mathf.Round(shape.position.x);
            int shapeY = (int)Mathf.Round(shape.position.y);

            gridArray[shapeX, shapeY] = shape;
            gridArray[shapeX, shapeY].GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
    }


    public bool IsValidPosForStoredShape(Transform shape) {

        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos(child, out childx, out childy);

            if (childx < 0 || childx >= boardWidth || childy < 0 || childy > 18) return false;

            if (gridArray[childx, childy] != null) return false;
        }

        return true;
    }


    public bool IsValidPosForFreeShape(Transform shape) {

        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos(child, out childx, out childy);

            if (childy < 0 || childy > 4) return false;
        }

        return true;
    }


    bool IsGridsFullAtRow(int y) {

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
            if(IsGridsFullAtRow(y)) 
            {               
                for (int x = 0; x < boardWidth; x++)
                {
                    if (gridArray[x, y].gameObject.CompareTag("BonusShape"))
                        UIController.Instance.IncreaseChangeButton();

                    if (gridArray[x, y].gameObject.CompareTag("PointShape"))
                        destroyedRowsCount++; // ANCHOR Yok edilen satır sayısı.

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

    
    public IEnumerator DestroyFullRows() {

        isAnimPlaying = true;

        yield return new WaitForSeconds(0.5f);      

        for (int y = 0; y < boardHeight; y++)
        {
            if (IsGridsFullAtRow(y))
            {                                            
                DestroyRow(y);
                ShiftAllRowsUp(y);
                y--;
            }
        }  

        isAnimPlaying = false; 
    }


    public IEnumerator DestroyAllRows() {   

        for (int y = 0; y < boardHeight; y++)
        {     
            for (int x = 0; x < boardWidth; x++)
            {
                if(gridArray[x, y] == null) continue;
                gridArray[x, y].GetComponent<Animator>().Play("DestroyAnim");
                Destroy(gridArray[x, y].gameObject, gridArray[x, y].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            }       
        }  

        yield return new WaitForSeconds(0.5f);

        UIController.Instance.GetUIAnim("CloseScorePanel");
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


    IEnumerator IsStillOverLimit() {

        isCoroutineActive = true;

        if(isAnimPlaying) yield return new WaitForSeconds(.6f);
        else yield return new WaitForSeconds(.01f);

        if(IsOverLimit()) isGameOver = true;
        else isGameOver = false;

        isCoroutineActive = false;
    }


    private void Update() {

        if(IsOverLimit() && !isCoroutineActive) {

            StartCoroutine("IsStillOverLimit");
        }
    }
}
