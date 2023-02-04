using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;
using System;

public class RewardedAdManager : MonoBehaviour
{
    public enum AdType {
        Real,
        Test
    }

    [SerializeField] AdType adType;
    [SerializeField] [Range(30, 500)] int loadFrequency = 120;
    [SerializeField] [Range(5, 25)] int rewardCount = 10;
    float timer;
    string ID;
    RewardedAd rewardedAd;


    private void Start()
    {
        #if UNITY_ANDROID
        {
            ID = adType == AdType.Real ? "ca-app-pub-3691073423716638/7163168639" : "ca-app-pub-3940256099942544/5224354917";
        }
        #elif UNITY_IPHONE
        {
            ID = adType == AdType.Real ? "ca-app-pub-3691073423716638/4836388191" : "ca-app-pub-3940256099942544/1712485313";
        }                    
        #endif


        rewardedAd = new RewardedAd(ID);

        rewardedAd.OnAdLoaded += HandleAdLoaded;
        rewardedAd.OnAdClosed += HandleAdClosed;

        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest();
    }


    void Update() 
    {
        if(!rewardedAd.IsLoaded() && UIController.Instance.IsUIWallOpen)
        {
            timer += Time.deltaTime;
        }


        if(timer > loadFrequency) 
        {
            timer = 0;
            AdRequest();
        }
    }


    private void AdRequest()
    {
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }
    

    public void ShowRewardedAd() //ANCHOR - RewardButtonlar da event olarak kullanılıyor.
    {
        if(rewardedAd.IsLoaded())
            rewardedAd.Show();
    }


    public void HandleAdLoaded(object sender, EventArgs args)
    {
        UIController.Instance.ActivateRewardButtons(true);
    }


    public void HandleAdClosed(object sender, EventArgs args)
    {
        UIController.Instance.ActivateRewardButtons(false);
    }


    void HandleUserEarnedReward(object sender, Reward args)
    {
        UIController.Instance.IncreaseChangeButton(rewardCount);
    }
}
