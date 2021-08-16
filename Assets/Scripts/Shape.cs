using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    
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
    }

    
    public void RotateLeft() {
        transform.Rotate(0, 0, 90);
    }
}
