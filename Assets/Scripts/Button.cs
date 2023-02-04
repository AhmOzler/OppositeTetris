using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Button : MonoBehaviour
{   
    [SerializeField] Shape boxShape = null;

    public Shape BoxShape {
        get { return boxShape; }
        set { boxShape = value; }
    }

    [SerializeField] List<Shape> shapeTypes;
    [SerializeField] List<Shape> buttonShapeList;
    [SerializeField] List<Shape> exceptList;
    [SerializeField] List<Shape> changeButtonList;



    IEnumerator Start() {
        yield return new WaitWhile(() => GameManager.Instance.IsGameOver || !UIController.Instance.IsUIWallOpen);

        boxShape = SpawnShape(shapeTypes[UnityEngine.Random.Range(0, shapeTypes.Count)]);
    }


    private void FillButtonShapeList()
    {
        buttonShapeList.Clear();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (i == transform.GetSiblingIndex()) continue;

            buttonShapeList.Add(transform.parent.GetChild(i).GetComponent<Button>().BoxShape);
        }
    }

    

    private void FillExceptShapeList()
    {
        if (buttonShapeList.Count > 0 && IsThereFullButton())
        {
            exceptList.Clear();

            foreach (var shape in shapeTypes)
            {
                if (buttonShapeList[0].name == shape.name) continue;
                if (buttonShapeList[1].name == shape.name) continue;

                exceptList.Add(shape);
            }
        }
    }



    private void FillChangeButtonShapeList()
    {
        if (buttonShapeList.Count > 0 && !IsThereFullButton())
        {
            changeButtonList.Clear();

            foreach (var shape in shapeTypes)
            {
                if (UIController.Instance.PastShapeList[0] == shape.name) continue;
                if (UIController.Instance.PastShapeList[1] == shape.name) continue;
                if (UIController.Instance.PastShapeList[2] == shape.name) continue;
                
                changeButtonList.Add(shape);
            }
        }
    }



    private bool IsThereFullButton() {

        for (int i = 0; i < transform.parent.childCount; i++)
        {           
            if(!transform.parent.GetChild(i).GetComponent<Button>().BoxShape) return false;
        }

        return true;
    }



    public Shape RandomShape1()
    {       
        return exceptList[UnityEngine.Random.Range(0, exceptList.Count)];
    }



    public Shape RandomShape2()
    {       
        return changeButtonList[UnityEngine.Random.Range(0, changeButtonList.Count)];
    }



    public Shape SpawnShape(Shape shape) //ANCHOR - Belirtilen pozisyonda shapeTypes'taki bir şekli random yaratıp return eder.
    {
        var randomShape = shape;
        var shapeClone = Instantiate(randomShape, transform.position, Quaternion.identity, GameObject.Find("ButtonShapeHolder").transform) as Shape;
        shapeClone.transform.localScale = new Vector2(.5f, .5f);
        shapeClone.GetShapeAnimation("SpawnAnim");
        shapeClone.name = randomShape.name;
        return shapeClone;
    }



    private void Update() //ANCHOR - Buttonlar(storedShape) null olduğunda shape spawn eder.
    {        
        if(GameManager.Instance.IsGameOver || !UIController.Instance.IsUIWallOpen) return;
        
       /*  if(BoxShape == null)
        {
            boxShape = SpawnShape();
        } */

        FillButtonShapeList();
        FillExceptShapeList();
    }
}
