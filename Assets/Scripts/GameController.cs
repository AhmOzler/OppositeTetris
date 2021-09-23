using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float dropInterval = 0.2f;
    [SerializeField] [Range(0, 1)] float keyDownInterval = 0.05f;
    [SerializeField] [Range(0, 1)] float keyRightLeftInterval = 0.1f;
    [SerializeField] [Range(0, 1)] float keyRotateInterval = 0.1f;

    float timeToDrop = 0;
    float timeToNextKeyDown = 0;
    float timeToNextKeyRightLeft = 0;
    float timeToNextKeyRotate = 0;
    
    Shape activeShape = null;
    Spawner spawner;

    private void Awake() {

        spawner = FindObjectOfType<Spawner>();
    }

    private void Start() {

       // activeShape = spawner.SpawnShape();
    }


    void Update()
    {      
        if(activeShape == null) return;

        PlayerInput();
    }


    /* private void LateUpdate() {
        
        if(activeShape)
            Board.Instance.CreateShadowShapeBottom(activeShape);
    } */


    void PlayerInput() {

        if (Input.GetButton("MoveDown") && Time.time >= timeToNextKeyDown || Time.time >= timeToDrop)
        {
            timeToDrop = Time.time + dropInterval;
            timeToNextKeyDown = Time.time + keyDownInterval;
            
            activeShape.MoveDown();

            if (!Board.Instance.IsValidPosition(activeShape.transform))
            {
                if (!Board.Instance.IsOverLimit())
                {                    
                    activeShape.MoveUp();
                    Board.Instance.StoreShapeInGrid(activeShape.transform);
                   // activeShape = spawner.SpawnShape();
                    
                    //Board.Instance.DestroyAllRows();
                }
                else
                {
                    activeShape = null;
                }
            }
        }
        else if (Input.GetButton("MoveRight") && Time.time >= timeToNextKeyRightLeft)
        {
            timeToNextKeyRightLeft = Time.time + keyRightLeftInterval;

            activeShape.MoveRight();

            if (!Board.Instance.IsValidPosition(activeShape.transform))
            {
                activeShape.MoveLeft();
            }
        }
        else if (Input.GetButton("MoveLeft") && Time.time >= timeToNextKeyRightLeft)
        {
            timeToNextKeyRightLeft = Time.time + keyRightLeftInterval;

            activeShape.MoveLeft();

            if (!Board.Instance.IsValidPosition(activeShape.transform))
            {
                activeShape.MoveRight();
            }
        }
        else if (Input.GetButton("Rotate") && Time.time >= timeToNextKeyRotate || Input.GetButtonDown("Rotate"))
        {
            timeToNextKeyRotate = Time.time + keyRotateInterval;

            //activeShape.RotateLeft();

            if (!Board.Instance.IsValidPosition(activeShape.transform))
            {
                activeShape.RotateRight();
            }
        }
    }


    /* private void IsValidPosition()
    {
        if (!Board.Instance.IsValidPosition(activeShape.transform))
        {
            activeShape.MoveUp();
            Board.Instance.StoreShapeInGrid(activeShape.transform);
            activeShape = spawner.SpawnShape();
        }
    } */
}
