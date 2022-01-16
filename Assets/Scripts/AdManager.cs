using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using TMPro;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;
    public static AdManager Instance => instance;


    [SerializeField] string androidBannerID;
    [SerializeField] string androidInterstitialID;
    [SerializeField] string androidRewardedID;

    [SerializeField] string IOSBannerID;   
    [SerializeField] string IOSInterstitialID;    
    [SerializeField] string IOSRewardedID; 

    BannerView bannerViewAd;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    string bannerID;
    string interstitialID;
    string rewardedID;


    [SerializeField] TextMeshProUGUI RewardedAdText;


    private void Awake() {

        if(instance == null) {
            instance = this;
        }
        else {
            instance = null;
            Destroy(gameObject);
        }
    }


    private void Start() {

       // UIController.Instance.ActivateRewardButtons(false);

        #if UNITY_ANDROID 
        {
            bannerID = androidBannerID;
            interstitialID = androidInterstitialID;
            rewardedID = androidRewardedID;
        }       
        #elif UNITY_IPHONE 
        {
            bannerID = IOSBannerID;
            interstitialID = IOSInterstitialID;
            rewardedID = IOSRewardedID;
        }           
        #else 
        {
            ID = "Tanımsız Platform";
        }           
        #endif

        AdSize adSize = new AdSize(320, 50);

        bannerViewAd = new BannerView(bannerID, adSize, AdPosition.Top);
        interstitialAd = new InterstitialAd(interstitialID);
        rewardedAd = new RewardedAd(rewardedID);

        interstitialAd.OnAdFailedToLoad += OnAdFailedToLoad4InterAd;
        interstitialAd.OnAdClosed += OnAdClosed;

        rewardedAd.OnAdClosed += HandleAdClosed;
        rewardedAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
        rewardedAd.OnAdLoaded += HandleAdLoaded;
        rewardedAd.OnAdOpening += HandleAdOpening;
        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
        rewardedAd.OnAdFailedToLoad += HandleAdFailedToLoad;


        AdRequest interstitialRequest = new AdRequest.Builder().Build();
        AdRequest rewardedRequest = new AdRequest.Builder().Build();

        
        bannerViewAd.Hide();
        interstitialAd.LoadAd(interstitialRequest);
        rewardedAd.LoadAd(rewardedRequest);
    }

    private void HandleAdClosed(object sender, EventArgs e)
    {
        //UIController.Instance.ActivateRewardButtons(false);
        RewardedAdText.text = "OnAdClosed";
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs e)
    {
       // UIController.Instance.ActivateRewardButtons(false);
        RewardedAdText.text = "OnAdDidRecordImpression";
    }

    private void HandleAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        RewardedAdText.text = "OnAdFailedToShow";
    }

    private void HandleAdLoaded(object sender, EventArgs e)
    {
       // UIController.Instance.ActivateRewardButtons(true);
       RewardedAdText.text = "OnAdLoaded";
    }

    private void HandleAdOpening(object sender, EventArgs e)
    {
        RewardedAdText.text = "OnAdOpening";
    }  


    private void HandleAdFailedToLoad(object sender, EventArgs e)
    {
        RewardedAdText.text = "OnAdAFailedToLoad";
        /* rewardedAd = new RewardedAd(rewardedID);
        AdRequest rewardedRequest = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(rewardedRequest); */
    }

    // INTERSİTATİALADS
    private void OnAdFailedToLoad4InterAd(object sender, EventArgs e)
    {
        interstitialAd = new InterstitialAd(interstitialID);
        AdRequest interstitialRequest = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(interstitialRequest);
    }


    private void OnAdClosed(object sender, EventArgs e)
    {
        AdRequest bannerRequest = new AdRequest.Builder().Build();
        bannerViewAd.LoadAd(bannerRequest);
    }
    


    public void ShowBannerAd() {
        
        if(interstitialAd.IsLoaded()) {
            bannerViewAd.Show();
        }           
    }


    public void DestroyBannerAd() {

        bannerViewAd.Destroy();
    }


    public void ShowInterstitialAd() {
        
        if(interstitialAd.IsLoaded()) {
            MobileAds.SetiOSAppPauseOnBackground(true);
            interstitialAd.Show();
        }           
        /*else {
            AdRequest interstitialRequest = new AdRequest.Builder().Build();
            interstitialAd.LoadAd(interstitialRequest);
        } */            
    }


    public void ShowRewardedAd() {
        
        if(rewardedAd.IsLoaded())
            rewardedAd.Show();      
    }


    void OnUserEarnedReward(object sender, Reward args)
    {
        string adName = args.Type;
        int adAmount = (int) args.Amount;
        UIController.Instance.IncreaseChangeButton(adAmount);
    }
}
