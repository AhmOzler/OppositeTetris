using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    Spawner spawner;   
    [SerializeField] Shape storedShape = null;
    public Shape StoredShape {
        get { return storedShape; }
        set { storedShape = value; }
    }


    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
    }


    private void Start() {

        SpawnShapeInButton();
    }


    private void Update() {

        if(StoredShape == null)
        {
            spawner.StoredShapeCount = 1;
            SpawnShapeInButton();
        }
    }


    private void SpawnShapeInButton()
    {
        StoredShape = spawner.SpawnShape(transform);
        StoredShape.GetShapeAnimation("SpawnAnim");
        StoredShape.transform.localScale = new Vector2(.5f, .5f);
    }
}
