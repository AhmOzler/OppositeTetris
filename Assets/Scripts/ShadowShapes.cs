using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShadowShapes : MonoBehaviour
{
    private static ShadowShapes instance;
    public static ShadowShapes Instance => instance;
    [SerializeField] Transform[] shadowShapeArray;
    Shape[] shapeTypes;
    [SerializeField] Transform shadowShape = null;
    public Transform ShadowShape => shadowShape;
    bool isHitTop;


    private void Awake() {

        if(!instance) 
            instance = this;
        else {
            instance = null;
            Destroy(gameObject);
        }
        
        shapeTypes = FindObjectOfType<Spawner>().ShapeTypes;
    }


    private void Start() {
        
        CreateShadowShapes();
    }


    void CreateShadowShapes() {

        shadowShapeArray = new Transform[shapeTypes.Length];

        for (int i = 0; i < shapeTypes.Length; i++)
        {
            Shape shape = Instantiate(shapeTypes[i], transform.position, Quaternion.identity, transform);
            shape.gameObject.tag = "ShadowShape";
            shape.gameObject.SetActive(false);
            shape.name = shapeTypes[i].gameObject.name;
            shadowShapeArray[i] = shape.transform;
        }

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


    public void GetShadowShape(Shape shape, bool setActive)
    {
        shadowShape = Array.Find<Transform>(shadowShapeArray, shadowShape => shadowShape.name == shape.name);

        int posX = (int)Mathf.Round(shape.transform.position.x);
        int posY = (int)Mathf.Round(shape.transform.position.y);

        ShadowShapePosReset(posX, posY);

        shadowShape.position = new Vector2(posX, shadowShape.position.y);
        shadowShape.rotation = shape.transform.rotation;

        foreach (Transform child in shadowShape) //ANCHOR Shape'in childlar??n??n rotasyon yapmas??n?? engeller.
        {
            child.rotation = Quaternion.identity;
        }

        shadowShape.gameObject.SetActive(setActive);

        HitTopAndStop(shadowShape);
    }


    private void ShadowShapePosReset(int posX, int posY) //ANCHOR ShadowShape'in resetlenmesini sa??lar.(posX de??i??ti??inde ve topSqr spawn oldu??unda.)
    {
        int posXHolder = 0;
        if (posXHolder != posX)
        { //ANCHOR ShadowShape'in bloklar??n aras??na girmemesi i??in. 

            shadowShape.position = new Vector2(posX, posY);
        }

        posXHolder = posX;
    }


    private void HitTopAndStop(Transform transform) //ANCHOR ShadowShape'in ??ste vurup durmas??n?? sa??lar.
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


    public void ResetShadowShape(Shape shape) {

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
