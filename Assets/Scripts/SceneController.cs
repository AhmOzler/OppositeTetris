using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    public static SceneController Instance => instance;


    Animator gameoverAnim;
    string currentState;

    private void Awake() {

        gameoverAnim = GetComponent<Animator>();

        if(instance == null)
            instance = this;
        else {      
            instance = null;
            Destroy(gameObject);
        }
            
    }

    public void OnRestartPressed()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }


    public void GetGameOverSceneAnim(string newState) {

        if(currentState == newState) return;

        gameoverAnim.Play(newState);
        currentState = newState;
    }
}
