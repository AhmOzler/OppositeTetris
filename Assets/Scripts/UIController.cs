using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] GameObject rewardButton1;
    [SerializeField] GameObject rewardButton2;
    [SerializeField] GameObject changeButton;
    [SerializeField] GameObject timeBar;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject hint;
    [SerializeField] RectTransform controlPanelVfx;
    [SerializeField] Transform[] buttons;
    [SerializeField] List<Shape> changeButtonList;
    [SerializeField] List<string> pastShapeList;
    public List<string> PastShapeList => pastShapeList;
    Animator UIanims;
    string currentState;
    int level = 1;
    float score = 0;
    int changeButtonCount = 2;
    float scoreMultiplier;
    public int ChangeButtonCount => changeButtonCount;  
    [HideInInspector] [SerializeField] bool isUIWallOpen;
    public bool IsUIWallOpen => isUIWallOpen;
    


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

        Application.targetFrameRate = 60;
        ScoreText();
        hint.SetActive(false);
        changeButtonText.text = changeButtonCount.ToString();
        changeButton.GetComponent<Animator>().SetInteger("PressCount", changeButtonCount);

        controlPanelVfx.sizeDelta = new Vector2(0, 355.6f);
        panel.GetComponent<Image>().material.SetFloat("_MetalHighlightDensity", .55f);
        panel.GetComponent<Image>().material.SetFloat("_MetalHighlightContrast", .35f);
        SetTimeBarShader("_SourceGlowDissolveFade", 9);

        Board.Instance.OnSquareDestroy += EmptyBoardExtraPoint;
    }


    public void PupHintBox(bool hintActivated, int failedNum) {

        if(HighScore > 40) { 
            hint.SetActive(false); 
            return;     
        }
        

        if(failedNum >= 1)
            hint.SetActive(hintActivated);
        else
            hint.SetActive(false);
    }



    private void EmptyBoardExtraPoint() 
    {
        score += level * 5;

        if(score > HighScore)
            HighScore = Mathf.RoundToInt(score);

        scoreText.text = Mathf.RoundToInt(score).ToString();
        scoreScreenText.text = Mathf.RoundToInt(score).ToString();
        highScoreScreenText.text = HighScore.ToString();
    }


    
    public void ScoreText(float value = 0) {

        AnimationCurve scoreCurve = GameManager.Instance.ScoreCurve;
        scoreMultiplier = Mathf.Lerp(1, 4, scoreCurve.Evaluate(value / 1000f));
        score += value * scoreMultiplier;

        if(score > HighScore)
            HighScore = Mathf.RoundToInt(score);

        scoreText.text = Mathf.RoundToInt(score).ToString();
        scoreScreenText.text = Mathf.RoundToInt(score).ToString();
        highScoreScreenText.text = HighScore.ToString();
    }



    public IEnumerator TimeBarHighlight(float fadeValue) { //ANCHOR - BonusTimeButton methodunda kullanılıyor.

        float fade = fadeValue;
        
        while(fade >= 0 ) 
        {
            fade -= Time.deltaTime;
            SetTimeBarShader("_RecolorFade", fade);
            yield return null;
        }
    }



    public void LevelText() {

        if(Board.Instance.TotalDestroyedSquaresCount >= 10 * level)
            level ++;
    
        levelText.text = level.ToString();
    }



    public void IncreaseChangeButton(int i) {
               
        changeButtonCount += i;
        changeButtonText.text = changeButtonCount.ToString();
    }



    public void DecreaseChangeButton(int value) { //ANCHOR - Event olarak ChangeButtonda kullanılıyor.

        if(changeButtonCount < value) return;

        changeButtonCount -= value;
        changeButton.GetComponent<Animator>().SetInteger("PressCount", changeButtonCount);
        SoundManager.Instance.Play("ButtonClick");
        changeButtonText.text = changeButtonCount.ToString();
    }

    

    public void DestroyShapeInButtons()
    {
        if (changeButtonCount <= 0) return;

        pastShapeList.Clear();
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i].GetComponent<Button>();

            if (!button.BoxShape) continue; //ANCHOR - Buttonların içinde shape yok ise devam et.
            pastShapeList.Add(button.BoxShape.name);
            Destroy(button.BoxShape.gameObject); //ANCHOR - Buttonların içindeki shapeleri yok et.
            button.BoxShape = null;
        }

        Shape[] shapeTypes = ShadowShapes.Instance.ShapeTypes;

        changeButtonList.Clear();
        
        foreach(var shape in shapeTypes)
        {
            if(pastShapeList[0] == shape.name) continue;
            if(pastShapeList[1] == shape.name) continue;
            if(pastShapeList[2] == shape.name) continue;

            changeButtonList.Add(shape);
        }


        foreach (var button in buttons)
        {
            var randomShape = changeButtonList[UnityEngine.Random.Range(0, changeButtonList.Count)];
            var shapeClone = Instantiate(randomShape, button.position, Quaternion.identity, GameObject.Find("ButtonShapeHolder").transform) as Shape;
            button.GetComponent<Button>().BoxShape = shapeClone;
            shapeClone.transform.localScale = new Vector2(.5f, .5f);
            shapeClone.GetShapeAnimation("SpawnAnim");
            shapeClone.name = randomShape.name;
        }
    }
    


    public void RotateShapesInButton(int angle) //ANCHOR - RotationButton'un(Event olarak) üstünde kullanılıyor.
    {
        SoundManager.Instance.Play("ButtonClick");

        foreach (Transform button in buttons)
        {
            if(!button.GetComponent<Button>().BoxShape) continue;

            button.GetComponent<Button>().BoxShape.Rotate(angle);
        }
    }



    public void SetPanelShader(string paramName, float value)
    {
        panel.GetComponent<Image>().material.SetFloat(paramName, value);
    }



    public void SetTimeBarShader(string paramName, float value)
    {
        timeBar.GetComponent<Image>().material.SetFloat(paramName, value);
    }



    public void OnRestartPressed()//ANCHOR - Event olarak RestartButtonda kullanılıyor.
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }



    /* private void DeleteHighScore() { //ANCHOR - UnityEvent olarak kulluanılıyor.

        PlayerPrefs.DeleteKey("HighScore");
    } */



    /* void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Delete)) {
            Debug.Log("All prefs are deleted!!");
            PlayerPrefs.DeleteKey("HighScore");
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.SetString("PlayerName", null);
        }    
    } */



    public void ActivateRewardButtons(bool activate) {

        rewardButton1.SetActive(activate);
        rewardButton2.SetActive(activate);
    }



    public void DestroyShapeButton() {

        foreach (var button in buttons)
        {
            Destroy(button.GetComponent<Button>().BoxShape.gameObject);
        }
    }



    public void GetUIAnim(string newState) {

        if(currentState == newState) return;

        UIanims.Play(newState);
        currentState = newState;
    }
}
