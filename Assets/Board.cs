using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] GameObject grid;
    [SerializeField] [Range(0, 10)] int boardWidth;
    [SerializeField] [Range(0, 20)] int boardHeight;


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
}
