using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShadowShapes : MonoBehaviour
{
    private static ShadowShapes instance;
    public static ShadowShapes Instance => instance;
    Transform[] shadowShapeArray;
    [SerializeField] Shape[] shapeTypes;
    public Shape[] ShapeTypes => shapeTypes;
    public Transform shadowShape = null;
    public Transform ShadowShape => shadowShape;
    bool isHitTop;



    private void Awake() 
    {
        if(!instance) 
            instance = this;
        else {
            instance = null;
            Destroy(gameObject);
        }
    }



    private void Start() 
    {     
        CreateShadowShapes();
        SetColorAndPivotOfShadowShapes();
    }



    private void CreateShadowShapes()
    {
        shadowShapeArray = new Transform[shapeTypes.Length];

        for (int i = 0; i < shapeTypes.Length; i++)
        {
            Shape shape = Instantiate(shapeTypes[i], transform.position, Quaternion.identity, transform);
            shape.gameObject.tag = "ShadowShape";
            shape.gameObject.SetActive(false);
            shape.name = shapeTypes[i].gameObject.name;
            shadowShapeArray[i] = shape.transform;
        }      
    }



    private void SetColorAndPivotOfShadowShapes()
    {
        foreach (Transform child in transform)
        {
            var renderers = child.GetComponentsInChildren<SpriteRenderer>();
            child.GetComponent<Shape>().SetPivotOutButton();

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = new Color(1, 1, 1, .5f);
                renderer.sortingOrder = 1;
            }
        }
    }



    private void ShadowShapePosReset(int posX, int posY) //ANCHOR - ShadowShape'in resetlenmesini sağlar.(posX değiştiğinde ve topSqr spawn olduğunda.)
    {
        int posXHolder = -1;

        if (posXHolder != posX) //ANCHOR - ShadowShape'in blokların arasına girmemesi için.
        { 
            shadowShape.position = new Vector2(posX, posY);
        }

        posXHolder = posX;
    }



    private void HitTopAndStop(Transform transform) //ANCHOR - ShadowShape'in üste vurup durmasını sağlar.
    {
        isHitTop = false;

        while (!isHitTop)
        {
            transform.GetComponent<Shape>().MoveUp();

            if (!Board.Instance.IsValidPosForStoredShape(transform))
            {
                isHitTop = true;
                transform.GetComponent<Shape>().MoveDown();
            }
        }

        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.y);
        transform.position = new Vector2(x, y);
    }



    public void GetShadowShape(Shape shape, bool setActive)
    {
        shadowShape = Array.Find<Transform>(shadowShapeArray, shadowShape => shadowShape.name == shape.name); //ANCHOR - ShadowShapeArray dizisi içinde shape isimleri ile eşleştirir.

        int posX = (int)Mathf.Round(shape.transform.position.x);
        int posY = (int)Mathf.Round(shape.transform.position.y);

        ShadowShapePosReset(posX, posY);

        shadowShape.position = new Vector2(posX, shadowShape.position.y);
        shadowShape.rotation = shape.transform.rotation;

        foreach (Transform child in shadowShape) //ANCHOR - Shape'in childlarının rotasyon yapmasını engeller.
        {
            child.rotation = Quaternion.identity;
        }

        shadowShape.gameObject.SetActive(setActive);

        HitTopAndStop(shadowShape);
    }



    public void ResetShadowShape(Shape shape) 
    {
        foreach (Transform child in shape.transform)
        {
            child.GetComponent<Animator>().Play("TeleportAnim");
        }

        HitTopAndStop(shape.transform);

        shadowShape.gameObject.SetActive(false);
        transform.position = shadowShape.position;
        shadowShape = null;
    }
}
