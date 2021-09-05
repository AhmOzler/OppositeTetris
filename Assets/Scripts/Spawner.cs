using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Shape[] shapeTypes;

    [SerializeField] Transform[] buttons;
    public Transform[] Buttons => buttons;

    private static Spawner instance;
    public static Spawner Instance => instance;


    private void Start() {

        SpawnShapeInButtons();
    }
    

    public Shape SpawnShape() {

        return Instantiate(shapeTypes[Random.Range(0, shapeTypes.Length)], transform.position, Quaternion.identity) as Shape;
    }


    public void SpawnShapeInButtons() {

        for (int i = 0; i < buttons.Length; i++)
        {
            StoredShape storedShape = buttons[i].GetComponent<StoredShape>();
            
            storedShape.Shape = SpawnShape();
            storedShape.Shape.transform.position = buttons[i].position + storedShape.Shape.ShapeOffset;
            storedShape.Shape.transform.localScale = new Vector2(0.8f, 0.8f);
            storedShape.Shape.transform.SetParent(buttons[i]);
        }
    }


    public void DestroyAllShapes()
    {
        foreach (Transform button in buttons)
        {
            if(button.GetComponent<StoredShape>().Shape == null)
                continue;

            Destroy(button.GetComponent<StoredShape>().Shape.gameObject);
        }
        
    }
}
