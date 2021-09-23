using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;
    [SerializeField] GameObject bottomSquare;
    [SerializeField] Transform[] buttons;
    int storedShapeCount;
    public int StoredShapeCount {
        get { return storedShapeCount; }       
        set {             
            storedShapeCount += value; 

            if(storedShapeCount >= 5 - UIController.Instance.Level) {
                storedShapeCount = 0;  
            }                        
        }
    } 
    

    public Shape SpawnShape(Transform transform) {

        return Instantiate(shapeTypes[Random.Range(0, shapeTypes.Length)], transform.position, Quaternion.identity, transform) as Shape;
    }


    public void DestroyShapeInButtons() {

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
        int sqrRowNumber = Random.Range(minValue, maxValue + 1);

        for (int x = 0; x < sqrRowNumber; x++)
        {
            int randomPosX = (int)Random.Range(0, Board.Instance.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }


    public IEnumerator SpawnRandomSqrAtBottom(int minValue, int maxValue)
    {
        if (storedShapeCount == 0) {
            
            GameObject bottomShape = new GameObject("BottomShape");
            Board board = Board.Instance;

            if(board.DestroyedRowsCount > 0) yield return new WaitForSeconds(1f);

            var sqrDigits = DigitList(minValue, maxValue);
                
            yield return new WaitForSeconds(0.2f);
            board.ShiftRowUp();

            for (int r = 0; r < sqrDigits.Count; r++)
            {
                var sqr = Instantiate(bottomSquare, new Vector2(sqrDigits.ToList()[r], transform.position.y), Quaternion.identity, bottomShape.transform);
                sqr.GetComponent<Animator>().Play("TeleportAnim");
                board.StoreShapeInGrid(bottomShape.transform);
            }           
        }        
    }


    public void RotateShapesInButton() {

        foreach (Transform button in buttons)
        {
            if(!button.GetComponent<Button>().StoredShape) continue;

            button.GetComponent<Button>().StoredShape.RotateRight();
        }
    }
}
