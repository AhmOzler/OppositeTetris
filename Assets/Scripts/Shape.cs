using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] Vector3 shapeOffset;
    Spawner spawner;
    Animator[] animators;
    

    private void Awake() 
    {
        spawner = FindObjectOfType<Spawner>();
        animators = GetComponentsInChildren<Animator>();
    }



    public void MoveDown() 
    {
        transform.position += Vector3.down;
    }



    public void MoveUp() 
    {
        transform.position += Vector3.up;
    }



    public void Rotate(int angle) 
    {
        transform.Rotate(0, 0, angle);
        
        foreach (Transform child in transform) //ANCHOR childların (sqrların) rotasyonunu sabitler.
        {
            child.rotation = Quaternion.identity;
        }
    }

    

    public void GetShapeAnimation(string animationName) 
    {
        foreach (Animator animator in animators)
        {
            animator.Play(animationName);
        }       
    }


    
    public void SetAlphaColor(float alpha) {

        foreach (Transform child in transform)
        {
            var tempColor = child.GetComponent<SpriteRenderer>().color;
            tempColor.a = alpha;
            child.GetComponent<SpriteRenderer>().color = tempColor;
        }
    }



    private void SetShader(Transform child, float fadePercent, string ID)
    {    
        Material mat = child.GetComponent<SpriteRenderer>().material;
        int shaderPropertyID = Shader.PropertyToID(ID);
        mat.SetFloat(shaderPropertyID, fadePercent);
    }



    public void SetPivotOutButton() 
    {       
        foreach (Transform child in transform)
        {
            child.localPosition += shapeOffset;
            SetShader(child, 0, "_HologramFade");
        }
    }



    public void SetPivotInButton() 
    {    
        foreach (Transform child in transform)
        {
            child.localPosition -= shapeOffset;
            SetShader(child, .85f, "_HologramFade");
        }
    }



    private void Update() 
    {
        if(gameObject.CompareTag("ShadowShape")) //ANCHOR - shapelerin yığınlanmamasını sağlar.
        { 
            if(!Board.Instance.IsValidPosForStoredShape(transform)) {
                MoveDown();
            }
        } 

        foreach (Transform child in transform) //ANCHOR - y 5 ten aşağıda olan shapelerin sqrlarını disable eder.
        {
            if(!child.CompareTag("DefaultSquares") && child.position.y <= 4) Destroy(child.gameObject);
        }
    }
}
