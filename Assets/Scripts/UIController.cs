using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance => instance;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI changeButtonText;
    [SerializeField] TextMeshProUGUI scoreScreenText;
    [SerializeField] TextMeshProUGUI highScoreScreenText;
    [SerializeField] GameObject changeButton;
    Animator UIanims;
    string currentState;
    int level = 1;
    public int Level => level;
    int score = 0;
    int changeButtonCount = 2;
    public int ChangeButtonCount => changeButtonCount;

    private int HighScore
    {
        get { return PlayerPrefs.GetInt("HighScore"); }
        set { PlayerPrefs.SetInt("HighScore", value); }
    }
    

    private void Awake() {
        
        UIanims = GetComponent<Animator>();
        
        if(instance == null)
            instance = this;
        else {
            instance = null;
            Destroy(gameObject);
        }
    }


    private void Start() {

        ScoreText();
        changeButtonText.text = changeButtonCount.ToString();
        changeButton.GetComponent<Animator>().SetInteger("PressCount", changeButtonCount);
    }


    public void ScoreText(int value = 0) {
        
        score += value;

        if(score > HighScore)
            HighScore = score;

        scoreText.text = "SCORE: " + score.ToString();
        scoreScreenText.text = score.ToString();
        highScoreScreenText.text = HighScore.ToString();
    }


    public void LevelText() {

        if(score >= 10 * level)
            level ++;
    
        levelText.text = "LEVEL: " + level.ToString();
    }


    public void IncreaseChangeButton(int i) {
        
        changeButtonCount += i;
        changeButtonText.text = changeButtonCount.ToString();
    }


    public void DecreaseChangeButton() {

        if(changeButtonCount <= 0) return;

        changeButtonCount --;
        changeButton.GetComponent<Animator>().SetInteger("PressCount", changeButtonCount);
        SoundManager.Instance.Play("ButtonClick");
        changeButtonText.text = changeButtonCount.ToString();
    }


    public void OnRestartPressed()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }


    private void DeleteHighScore() { //ANCHOR UnityEvent olarak kulluanılıyor.

        PlayerPrefs.DeleteKey("HighScore");                
    }


    public void GetUIAnim(string newState) {

        if(currentState == newState) return;

        UIanims.Play(newState);
        currentState = newState;
    }
}
