using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;

    public Shape SpawnShape() {

        return Instantiate(shapeTypes[Random.Range(0, shapeTypes.Length)]) as Shape;
    }
}
