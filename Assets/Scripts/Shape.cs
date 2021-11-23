using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] Vector3 shapeOffset;
    public Vector3 ShapeOffset => shapeOffset;
    Spawner spawner;
    Animator[] animators;
    int hologramFadePropertyID;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
        animators = GetComponentsInChildren<Animator>();
    }


    public void MoveDown() {
        transform.position += Vector3.down;
    }


    public void MoveUp() {
        transform.position += Vector3.up;
    }


    public void MoveRight() {
        transform.position += Vector3.right;
    }


    public void MoveLeft() {
        transform.position += Vector3.left;
    }


    public void RotateRight(int angle) {

        transform.Rotate(0, 0, angle);
        
        foreach (Transform child in transform)
        {
            child.rotation = Quaternion.identity;
        }
    }

    
    public void GetShapeAnimation(string animationName) {

        foreach (Animator animator in animators)
        {
            animator.Play(animationName);
        }       
    }


    private void HologramFade(Transform child, float fadePercent)
    {
        Material mat = child.GetComponent<SpriteRenderer>().material;
        hologramFadePropertyID = Shader.PropertyToID("_HologramFade");
        mat.SetFloat(hologramFadePropertyID, fadePercent);
    }


    public void SetPivotOutButton() {
        
        foreach (Transform child in transform)
        {
            child.localPosition += shapeOffset;
            HologramFade(child, 0);
        }
    }

    
    public void SetPivotInButton() {
        
        foreach (Transform child in transform)
        {
            child.localPosition -= shapeOffset;
            HologramFade(child, .85f);  
        }
    }
    

    private void Update() {

        if(gameObject.CompareTag("ShadowShape")) {

            if(!Board.Instance.IsValidPosition(transform)) {
                MoveDown();
            }
        } 


        if(gameObject.CompareTag("StoredShape")) {

            foreach (Transform child in transform)
            {
                if(child.position.y < 5) Destroy(child.gameObject);
            }
        }  
    }
}
