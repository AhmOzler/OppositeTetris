using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredShape : MonoBehaviour
{
    [SerializeField] Shape shape = null;
    public Shape Shape {
        get { return shape; }
        set { shape = value; }
    }

    
}
