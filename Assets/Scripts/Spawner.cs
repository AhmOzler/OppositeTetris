using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;
    [SerializeField] Transform[] UIButtons;
    Queue<Transform> queueShape = new Queue<Transform>(3);

    private static Spawner instance;
    public static Spawner Instance => instance;

    public Shape SpawnShape() {

        return Instantiate(shapeTypes[Random.Range(0, shapeTypes.Length)], transform.position, Quaternion.identity) as Shape;
    }


    public void Update() {

        if(queueShape.Count < 3) {

            for (int i = 0; i < 3; i++)
            {
                Transform x = SpawnShape().transform;
                queueShape.Enqueue(x);
                x.position = UIButtons[i].position; 
            }
        }       
    }

    public void UseStoredBlocks() {

        
    }
}
