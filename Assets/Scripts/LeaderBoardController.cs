using LootLocker.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class LeaderBoardController : MonoBehaviour
{
    private static LeaderBoardController instance;
    public static LeaderBoardController Instance => instance; 
    [SerializeField] TMP_InputField playerName;
    [SerializeField] int leaderBoardID;
    [SerializeField] TextMeshProUGUI savedPlayerNameText;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] TextMeshProUGUI IDText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI playerRankText;
    [SerializeField] TextMeshProUGUI playerIDText;
    [SerializeField] TextMeshProUGUI playerScoreText;
    [SerializeField] RectTransform textContainer;
    [SerializeField] RectTransform textInfo;


    void Awake() 
    {
        if(instance == null)
            instance = this;
        else {
            instance = null;
            Destroy(gameObject);
        }           
    }


    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            
            if(PlayerPrefs.GetString("PlayerName") != "")
                savedPlayerNameText.text = PlayerPrefs.GetString("PlayerName");
            else
                savedPlayerNameText.text = "ENTER NAME";

            StartCoroutine(InitializeLeaderBoard());
        }           
        else {
            StartCoroutine(Setuproutine());
        }  
      
        //Debug.Log("MEMBERSCOUNT : " + members.Length);
    }


    /* private void Update() {

        if(Input.GetMouseButton(0))
        {
            Debug.Log("All playerPrefs are deleted");
            PlayerPrefs.DeleteAll();
        } 
    } */


    IEnumerator Setuproutine() 
    {
        yield return InitializeLeaderBoard();
        yield return FetchTopHighscoresRoutine();
        yield return ShowLeaderBoard();
    }


    private IEnumerator InitializeLeaderBoard()
    {    
        bool done = false;
        
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Failed to logged in");
                done = true;
            }          
        });

        yield return new WaitWhile(() => done == false);
    }


    public void SavePlayerName() //ANCHOR - PlayerName Inputfieldda event olarak kullanılıyor.
    {
        PlayerPrefs.SetString("PlayerName", playerName.text);         
    }


    public void LoadScene(string sceneName) //ANCHOR - ShowScore Buttonunda event olarak kullanılıyor.
    {
        SceneManager.LoadScene(sceneName);
    }


    public void SetPlayerName() //ANCHOR - PlayerName Inputfieldda event olarak kullanılıyor.
    {
        //if(PlayerPrefs.GetString("PlayerName") == "") return;
        
        LootLockerSDKManager.SetPlayerName(PlayerPrefs.GetString("PlayerName"), (response) =>
        {
            if(response.success)
            {
                Debug.Log("Succesfully set player name");
            }
            else
            {
                Debug.Log("Could not set player name" + response.Error);
            }
        });
    }  
    

    public void SubmitScore() //ANCHOR SubmitScore Buttonunda event olarak kullanılıyor.
    {       
        LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("PlayerID"), PlayerPrefs.GetInt("HighScore"), leaderBoardID, (response) =>
        {
            if(response.success)
            {
                Debug.Log("Succesfully uploaded score");
            }
            else
            {
                Debug.Log("Failed" + response.Error);
            }
        });
    }


    private IEnumerator ShowLeaderBoard()
    {
        bool done = false;
        
        LootLockerSDKManager.GetMemberRank(leaderBoardID, PlayerPrefs.GetString("PlayerID"), (response) =>
        {
            if (response.success)
            {
                playerRankText.text = response.rank.ToString();
                playerIDText.text = PlayerPrefs.GetString("PlayerName") != "" ? response.player.name : PlayerPrefs.GetString("PlayerID");
                playerScoreText.text = response.score.ToString();
            }
            else
            {
                Debug.Log("failed: " + response.Error);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }


    public IEnumerator FetchTopHighscoresRoutine()
    {
        bool done = false;

        LootLockerSDKManager.GetScoreList(leaderBoardID, 2000, 0, (response) =>
        {
            if(response.success)
            {
                LootLockerLeaderboardMember[] members = response.items;
                textContainer.sizeDelta = new Vector2(textContainer.sizeDelta.x, members.Length * 135);
                
                for (int i = 0; i < members.Length; i++)
                {
                    RectTransform rectText = Instantiate(textInfo, textContainer.transform);               
                    rectText.localPosition = new Vector2(0, i * -135);
                    
                    rectText.GetChild(0).GetComponent<TextMeshProUGUI>().text = members[i].rank.ToString();
                    rectText.GetChild(1).GetComponent<TextMeshProUGUI>().text = members[i].player.name != "" ? members[i].player.name : members[i].player.id.ToString();
                    rectText.GetChild(2).GetComponent<TextMeshProUGUI>().text = members[i].score.ToString();
                }
                done = true;             
            }
            else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        
        yield return new WaitWhile(() => done == false);
    }
}