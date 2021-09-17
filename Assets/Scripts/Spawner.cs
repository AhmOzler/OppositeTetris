using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;
    [SerializeField] GameObject square;
    [SerializeField] [Range(0, 15)] int sqrRowNumber = 6;
    [SerializeField] [Range(0, 5)] int sqrColumnNumber = 1;

    [SerializeField] Transform[] buttons;
    public Transform[] Buttons => buttons;


    private static Spawner instance;
    public static Spawner Instance => instance;


    private void Start() {

        SpawnShapeInButtons();
        Board.Instance.GridLineUp();
    }
    

    public Shape SpawnShape() {

        return Instantiate(shapeTypes[Random.Range(0, shapeTypes.Length)], transform.position, Quaternion.identity) as Shape;
    }


    public void SpawnShapeInButtons() {

        for (int i = 0; i < buttons.Length; i++)
        {
            StoredShape storedShape = buttons[i].GetComponent<StoredShape>();
            
            storedShape.Shape = SpawnShape();
            storedShape.Shape.GetShapeAnimation("SpawnAnim");
            storedShape.Shape.transform.position = buttons[i].position;
            storedShape.Shape.transform.localScale = new Vector2(.5f, .5f);
            storedShape.Shape.transform.SetParent(buttons[i]);
        }
    }


    private List<int> DigitList(Board board)
    {
        List<int> sqrDigits = new List<int>();
        sqrDigits.Clear();

        for (int x = 0; x < sqrRowNumber; x++)
        {
            int randomPosX = (int)Random.Range(0, board.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }


    public IEnumerator SpawnRandomSqrAtBottom()
    {
        if (AreAllButtonsNull()) {

            GameObject bottomShape = new GameObject("BottomShape");
            Board board = Board.Instance;

            var sqrDigits = DigitList(board);

            for (int i = 0; i < sqrColumnNumber; i++)
            {
                if(board.DestroyedRowsCount > 0) yield return new WaitForSeconds(1f);
                
                yield return new WaitForSeconds(0.2f);
                board.ShiftRowUp();

                for (int r = 0; r < sqrDigits.Count; r++)
                {
                    var sqr = Instantiate(square, new Vector2(sqrDigits.ToList()[r], transform.position.y), Quaternion.identity, bottomShape.transform);
                    sqr.GetComponent<Animator>().Play("TeleportAnim");
                    board.StoreShapeInGrid(bottomShape.transform);
                }
            }
        }        
    }


    public void RotateShapesInButton() {

        foreach (Transform button in buttons)
        {
            if(!button.GetComponent<StoredShape>().Shape) continue;

            button.GetComponent<StoredShape>().Shape.RotateRight();
        }
    }


    private void Update() {

        if (AreAllButtonsNull()) {

            SpawnShapeInButtons();           
        }
    }


    bool AreAllButtonsNull()
    {
        return !buttons[0].GetComponent<StoredShape>().Shape &&
            !buttons[1].GetComponent<StoredShape>().Shape &&
            !buttons[2].GetComponent<StoredShape>().Shape &&
            !buttons[3].GetComponent<StoredShape>().Shape;
    }
}
