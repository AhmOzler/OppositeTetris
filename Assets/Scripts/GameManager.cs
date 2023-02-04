using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance; 
    [SerializeField] AnimationCurve scoreCurve;
    public AnimationCurve ScoreCurve => scoreCurve;
    private bool isAnimPlaying = false;
    public bool IsAnimPlaying {
        get { return isAnimPlaying; }
        set { isAnimPlaying = value; }
    }
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    private float timer;




    private void Awake() 
    {
        if(!instance) 
        {
            instance = this;
        }
        else {
            instance = null;
            Destroy(gameObject);
        }
    }



    void Start() 
    {
        StartCoroutine(Spawner.Instance.SpawnSqrAtStart());
    }



    void Update()
    {   
        if (UIController.Instance.IsUIWallOpen == false) return;
        
        timer += Time.deltaTime;      

        Spawner.Instance.SpawnInTime();
        GameIsOver();
    }



    private void GameIsOver()
    {
        if (Board.Instance.checkBottomGrids && !isAnimPlaying && !isGameOver)
        {
            isGameOver = true;
            Spawner.Instance.StopAllCoroutines();
            TouchController.Instance.enabled = false;
            UIController.Instance.DestroyShapeButton();
            InterstitialAdManager.Instance.Invoke("ShowInterstitialAd", 2);
            UIController.Instance.GetUIAnim ("CloseScorePanel");
            Board.Instance.DestroyAllAndClosePanel();
            ShadowShapes.Instance.ShadowShape.gameObject.SetActive(false);
        }
    }
}
