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

    [SerializeField] bool IsUIWallOpen = false; //ANCHOR menü ekranı geçip oyun başlamadan spawn etmemesi için yapıldı.
    Spawner spawner;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
    }


    private void Update() { //ANCHOR Buttonlar(storedShape) null olduğunda shape spawn eder.
        
        spawner.IsUIWallOpen = IsUIWallOpen;

        if(Board.Instance.IsGameOver && StoredShape)
            Destroy(StoredShape.gameObject);

        if(StoredShape == null && IsUIWallOpen && !Board.Instance.IsGameOver)
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
