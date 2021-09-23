using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;
    [SerializeField] GameObject bottomSquare;
    [SerializeField] GameObject bonusSquare;
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
        int sqrRowNumber = Random.Range(minValue, maxValue + 1);

        for (int x = 0; x < sqrRowNumber; x++)
        {
            int randomPosX = (int)Random.Range(0, Board.Instance.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }


    public IEnumerator SpawnRandomSqrAtBottom(int minValue, int maxValue, int percent)
    {
        if (storedShapeCount == 0) {
            
            GameObject bottomShape = new GameObject("BottomShape");
            Board board = Board.Instance;

            if(board.DestroyedRowsCount > 0) yield return new WaitForSeconds(1f);

            var sqrDigits = DigitList(minValue, maxValue);
                
            yield return new WaitForSeconds(0.2f);
            board.ShiftRowUp();
            SoundManager.Instance.Play("TeleportBricks");
            
            for (int r = 0; r < sqrDigits.Count; r++)
            {
                Vector2 randomXpos = new Vector2(sqrDigits.ToList()[r], transform.position.y);

                var sqr = Instantiate(Sqr(percent), randomXpos, Quaternion.identity, bottomShape.transform);
                sqr.GetComponent<Animator>().Play("TeleportAnim");
                board.StoreShapeInGrid(bottomShape.transform);
            }           
        }        
    }


    GameObject Sqr(int percent) {

        if(Random.Range(0, 100) < percent) {
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
