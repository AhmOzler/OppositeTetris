using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float dropInterval = 0.2f;
    [SerializeField] Vector3 shapeOffset;
    public Vector3 ShapeOffset => shapeOffset;

    [SerializeField] bool moveDownOn = false;
    public bool MoveDownOn {
        get { return moveDownOn; }
        set {
            if(moveDownOn != value)
                moveDownOn = value;
        }
    }

    float timeToDrop = 0;
    Spawner spawner;
    Animator[] animators;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
        animators = GetComponentsInChildren<Animator>();
    }


    public void MoveDown() {
        transform.position += Vector3.down;
    }


    public void MoveUp() {
        transform.position += Vector3.up;
    }


    public void MoveRight() {
        transform.position += Vector3.right;
    }


    public void MoveLeft() {
        transform.position += Vector3.left;
    }


    public void RotateRight() {

        transform.Rotate(0, 0, -90);
        
        foreach (Transform child in transform)
        {
            child.Rotate(0, 0, 90);
        }
    }

    
    public void GetShapeAnimation(string animationName) {

        foreach (Animator animator in animators)
        {
            animator.Play(animationName);
        }       
    }


    public void SetPivotOutButton() {

        foreach (Transform child in transform)
        {
            child.localPosition += shapeOffset;           
        }
    }


    public void SetPivotInButton() {

        foreach (Transform child in transform)
        {
            child.localPosition -= shapeOffset;
        }
    }
    

    /* private void Update() {

        if(!moveDownOn) return;

        if (Time.time >= timeToDrop)
        {
            timeToDrop = Time.time + dropInterval;

            MoveDown();

            if (!Board.Instance.IsValidPosition(transform))
            {
                if (!Board.Instance.IsOverLimit())
                {
                    MoveUp();
                    Board.Instance.StoreShapeInGrid(transform);
                }
            }
        }
    } */
}
