using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] Shape storedShape = null;
    public Shape StoredShape {
        get { return storedShape; }
        set { storedShape = value; }
    }

    [SerializeField] bool letSpawn = false; //ANCHOR menü ekranı geçip oyun başlamadan spawn etmemesi için yapıldı.
    Spawner spawner;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
    }


    private void Start() {

        if(letSpawn)
            SpawnShapeInButton();
    }


    private void Update() {
        
       spawner.LetSpawn = letSpawn;

        if(StoredShape == null && letSpawn)
        {
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
