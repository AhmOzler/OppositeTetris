using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance => instance;

    [SerializeField] GameObject grid;
    [SerializeField] [Range(0, 10)] int boardWidth;
    [SerializeField] [Range(0, 20)] int boardHeight;

    Transform[,] gridArray;   

    private void Awake() {

        if(instance) {
            Destroy(instance);
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
                GameObject gridInstance = Instantiate(grid, new Vector2(x, y), Quaternion.identity);
                gridInstance.name = x.ToString() + " " + y.ToString();
            }
        }
    }


    public void StoreShapeInGrid(Transform shape) {
        
        foreach (Transform child in shape)
        {
            float childx = Mathf.Round(child.position.x);
            float childy = Mathf.Round(child.position.y);

            gridArray[(int) childx, (int) childy] = child;
            shape.GetComponent<Shape>().MoveUp();
        }
    }


    public bool IsValidPosition(Transform shape) {

        foreach (Transform child in shape)
        {
            float childx = Mathf.Round(child.position.x);
            float childy = Mathf.Round(child.position.y);
            Debug.Log(child.name + ": " + new Vector2(childx, childy));

            if (gridArray[(int)childx, (int)childy]) { 
                
                return false;
            }
            
            if (childx > boardWidth || childx < 0) return false;

            if (childy <= 0) return false;
        }

        return true;
    }
}
