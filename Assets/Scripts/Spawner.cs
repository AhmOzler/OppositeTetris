using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Spawner : MonoBehaviour
{
    [SerializeField] [Range(1, 8)] int sqrDensity = 7;
    [SerializeField] [Range(5, 15)] float spawnSpeed = 10;
    [SerializeField] [Range(0, 100)] int changeSqrPercentage = 1;
    [SerializeField] Shape[] shapeTypes;
    public Shape[] ShapeTypes => shapeTypes;
    [SerializeField] Transform topSquare;
    [SerializeField] Transform bonusSquare;
    [SerializeField] Transform[] buttons;
    [SerializeField] bool letSpawn = false; //ANCHOR menü ekranı geçip oyun başlamadan spawn etmemesi için yapıldı.
    public bool LetSpawn {
        set { if(value != letSpawn) letSpawn = value; }
    }
    Transform buttonShapeHolder;
    Transform topShapeHolder;
    

    private void Start() {
        
        buttonShapeHolder = new GameObject("buttonShapeHolder").transform;
        topShapeHolder = new GameObject("topShapeHolder").transform;
        StartCoroutine(SpawnSqrAtTop(changeSqrPercentage));       
    }
    
    
    public Shape SpawnShape(Transform transform) {

        Shape randomShape = shapeTypes[UnityEngine.Random.Range(0, shapeTypes.Length)];

        var shape = Instantiate(randomShape, transform.position, Quaternion.identity, buttonShapeHolder) as Shape;
        shape.name = randomShape.name;
        return shape;
    }


    public void DestroyShapeInButtons() { //ANCHOR ChangeButton'un(Eventolarak) üstünde kullanılıyor.

        if(UIController.Instance.ChangeButtonCount <= 0) return;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i].GetComponent<Button>();

            if(!button.StoredShape) continue;

            Destroy(button.StoredShape.gameObject);
            button.StoredShape = null;
        }
    }


    private List<int> DigitList()
    {
        List<int> sqrDigits = new List<int>();
        sqrDigits.Clear();

        for (int x = 0; sqrDigits.Distinct().ToList().Count < sqrDensity; x++)
        {
            int randomPosX = (int)UnityEngine.Random.Range(0, Board.Instance.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }


    public IEnumerator SpawnSqrAtTop(int percent)
    {   
        Board board = Board.Instance;
        List<int> sqrDigits;

        for (int i = 0; i < 2; i++) //ANCHOR Oyun başlangıcında TopSqr üretmek için.
        {
            yield return new WaitUntil(() => letSpawn);

            sqrDigits = DigitList();
            board.ShiftRowDown();
            SoundManager.Instance.Play("TeleportBricks");
            SpawnTopSqr(percent, board, sqrDigits);           
        }
        

        while(true) //ANCHOR Rutin TopSqr üretmek için.
        {   
            yield return new WaitWhile(() => Board.Instance.IsAnimPlaying);
            yield return new WaitUntil(() => letSpawn);
            yield return new WaitForSeconds(spawnSpeed);

           
            sqrDigits = DigitList();
            board.ShiftRowDown();
            SoundManager.Instance.Play("TeleportBricks");

            SpawnTopSqr(percent, board, sqrDigits);
        }
    }


    private void SpawnTopSqr(int percent, Board board, List<int> sqrDigits)
    {
        for (int r = 0; r < sqrDigits.Count; r++)
        {
            Vector2 randomXpos = new Vector2(sqrDigits[r], transform.position.y);
        
            Transform sqr = Instantiate(Sqr(percent), randomXpos, Quaternion.identity, topShapeHolder);
            board.StoreShapeInGrid(sqr);
        }
    }


    Transform Sqr(int percent) {

        if(UnityEngine.Random.Range(0, 100) < percent) {
            return bonusSquare;
        }

        return topSquare;
    }


    public void RotateShapesInButton(int angle) { //ANCHOR RotationButton'un(Event olarak) üstünde kullanılıyor.

        SoundManager.Instance.Play("ButtonClick");

        foreach (Transform button in buttons)
        {
            if(!button.GetComponent<Button>().StoredShape) continue;

            button.GetComponent<Button>().StoredShape.RotateRight(angle);
        }
    }
}
