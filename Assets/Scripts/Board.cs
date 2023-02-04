using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance => instance;
    [SerializeField] GameObject grid;
    [SerializeField] [Range(0, 15)] int boardWidth;
    public int BoardWidth => boardWidth;
    [SerializeField] [Range(0, 30)] int boardHeight;
    public int BoardHeight => boardHeight;
    private Transform[,] gridArray;
    private int destroyedSquaresCount;
    public int DestroyedSquaresCount => destroyedSquaresCount;
    private int totalDestroyedSquaresCount;
    public int TotalDestroyedSquaresCount => totalDestroyedSquaresCount;
    private int emptyGridCountInDestroyedRow;
    public int EmptyGridCountInDestroyedRow => emptyGridCountInDestroyedRow;
    private Action onSquareDestroy;  
    public Action OnSquareDestroy { 
        get { return onSquareDestroy; } 
        set { onSquareDestroy = value; } 
    }
    public bool checkBottomGrids => gridArray[0, 5] || gridArray[1, 5]
                                || gridArray[2, 5] || gridArray[3, 5]
                                || gridArray[4, 5] || gridArray[5, 5]
                                || gridArray[6, 5] || gridArray[7, 5]
                                || gridArray[8, 5];

    
    
    private void Awake() 
    {
        if(instance) {
            Destroy(gameObject);
            return;
        }          
        else
            instance = this as Board;
    }



    private void Start() 
    {
        gridArray = new Transform[boardWidth, boardHeight];  
        GridLineUp(); 
    }



    public bool IsBoardEmpty()
    {
        for (int y = boardHeight - 1; y > 13; y--)
        {
            for (int x = 0; x < boardWidth - 1; x++)
            {
                if(gridArray[x, y])
                    if (gridArray[x, y].tag == "PointSquares" || gridArray[x, y].tag == "BonusSquares") return false;
            }
        }

        return true;
    }



    private void GridLineUp() //ANCHOR - Gridler oyun başlangıcında sıralanıyor.
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



    private void RoundPos(Transform child, out int childx, out int childy) //ANCHOR - Gridlerin x,y kordinatlarını tamsayıya yuvarlar.
    {
        childx = (int) Mathf.Round(child.position.x);
        childy = (int) Mathf.Round(child.position.y);
    }



    public void StoreShapeInGrid(Transform shape) //ANCHOR - Gridlere shapeleri yada sqrları yerleştirir.
    {      
        if(shape.childCount > 0) { //ANCHOR - Parentı olanlar ise shapelerdir(tetris şekilleri gibi).
            
            foreach (Transform child in shape) {

                int childx, childy;
                RoundPos(child, out childx, out childy);

                gridArray[childx, childy] = child;
                gridArray[childx, childy].GetComponent<SpriteRenderer>().sortingOrder = 9;

                if(child.CompareTag("DefaultSquares")) child.gameObject.tag = "StoredSquares";
            }
        }
        else { //ANCHOR - Parentı olmayanlar sqr6lar(tag'i pointShape olanlar).

            int shapeX = (int)Mathf.Round(shape.position.x);
            int shapeY = (int)Mathf.Round(shape.position.y);

            gridArray[shapeX, shapeY] = shape;
            gridArray[shapeX, shapeY].GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
    }



    public bool IsValidPosForStoredShape (Transform shape)  //ANCHOR - Shapelerin childı olan sqrların gridlere yerleşmesi için gridlerin dolu olup olmadığını kontrol eder.
    {
        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos (child, out childx, out childy);

            if (childx < 0 || childx > boardWidth - 1 || childy < 0 || childy > boardHeight - 1) return false; //ANCHOR board sınırları.

            if (gridArray[childx, childy] != null) return false;
        }

        return true;
    }



    public bool IsValidPosForDefaultShape (Transform shape) //ANCHOR - Freeshapelerin (alttaki 3 shape) board içindeki hareket limitini belirler. 
    {
        foreach (Transform child in shape)
        {
            int childx, childy;
            RoundPos(child, out childx, out childy);

            if (child.position.x < 0 || child.position.x > 8 || child.position.y < -9) return false;
            if (childy < 0 || childy > 4) return false;
        }

        return true;
    }



    bool IsGridsFullAtRow (int y) //ANCHOR - Bir satırda bütün gridler dolu olup olmadığını kontrol eder.
    { 
        for (int x = 0; x < boardWidth; x++)
        {
            if(gridArray[x, y] == null) return false;
        }

        return true;
    } 
    


    public void SqrSFXandVFX()
    {     
        List<int> rowIndexes = new List<int>();
        rowIndexes.Clear();

        destroyedSquaresCount = 0;        
        emptyGridCountInDestroyedRow = 0;

        for (int y = 0; y < boardHeight; y++)
        {
            if(IsGridsFullAtRow(y))
            {               
                StartCoroutine(CameraShake.Instance.ShakeCamera()); 

                for (int x = 0; x < boardWidth; x++)
                {   
                    if (gridArray[x, y].gameObject.CompareTag("PointSquares") || gridArray[x, y].gameObject.CompareTag("BonusSquares"))   //TODO bonusshape'ede puan ekelenecek.
                    {
                        if (gridArray[x, y].gameObject.CompareTag("BonusSquares")) UIController.Instance.IncreaseChangeButton(1); //ANCHOR - BonusShape tagli sqr ları kırdıkça changebuttonı arttırır.

                        rowIndexes.Add(y);
                        destroyedSquaresCount++;
                        emptyGridCountInDestroyedRow = (9 * rowIndexes.Distinct().ToList().Count) - destroyedSquaresCount;
                    }                                 
                    
                    gridArray[x, y].GetComponent<Animator>().Play("DestroyAnim");
                    SoundManager.Instance.Play("DestroyBricks");
                }
            }
        }

        totalDestroyedSquaresCount += destroyedSquaresCount;
    }



    private void DestroyRow(int y) //ANCHOR - Belirlenen satırı yok eder.
    {
        for (int x = 0; x < boardWidth; x++)
        {
            Destroy(gridArray[x, y].gameObject);
            gridArray[x, y] = null;
        }       
    }



    public void ShiftRowDown() //ANCHOR - Tüm sqrları bir grid aşağı kaydırır. 
    {
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



    public void ShiftAllRowsUp(int y) //ANCHOR - Belirlenen satırdan en alt satıra kadar sqrları bir birim yukarı kaydırır.
    {
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



    public IEnumerator DestroyFullRows() //ANCHOR - Dolu olan tüm satırları yok eder ve tüm sqrları yok olan satır miktarınca yukarı kaydırır.
    {
        GameManager.Instance.IsAnimPlaying = true;
        
        yield return new WaitForSeconds(0.5f);

        for (int y = 0; y < boardHeight; y++) //ANCHOR - En alt satırdan en üst satıra kadar.
        {
            if (IsGridsFullAtRow(y)) //ANCHOR - Satırdaki bütün gridler dolu ise.
            {                                                        
                DestroyRow(y); //ANCHOR - Satırı yok eder.
                ShiftAllRowsUp(y); //ANCHOR - Satırı yukarı kaydırır.
                y--; //ANCHOR - Satır başlangıcını en alttan başlatmak için sürekli 0. satırda tutar.
            }
        }  

        GameManager.Instance.IsAnimPlaying = false; 

        if(IsBoardEmpty() && OnSquareDestroy != null) onSquareDestroy();
    }



    public void DestroyAllAndClosePanel() //ANCHOR - Oyun bittiğinde bütün gridlerdeki sqrları yok eder(Görsellik için).
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (gridArray[x, y] == null) continue;
                gridArray[x, y].GetComponent<Animator>().Play("DestroyAnim");
                Destroy (gridArray[x, y].gameObject, .5f);
            }       
        }    
    }
}
