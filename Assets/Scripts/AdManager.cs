using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

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

        AdSize adSize = new AdSize(320, 100);

        bannerViewAd = new BannerView(bannerID, adSize, AdPosition.Top);
        interstitialAd = new InterstitialAd(interstitialID);
        rewardedAd = new RewardedAd(rewardedID);

        interstitialAd.OnAdFailedToLoad += OnAdFailedToLoad;
        interstitialAd.OnAdClosed += OnAdClosed;
        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;


        AdRequest interstitialRequest = new AdRequest.Builder().Build();
        AdRequest rewardedRequest = new AdRequest.Builder().Build();

        
        bannerViewAd.Hide();
        interstitialAd.LoadAd(interstitialRequest);
        rewardedAd.LoadAd(rewardedRequest);
    }


    private void OnAdClosed(object sender, EventArgs e)
    {
        AdRequest bannerRequest = new AdRequest.Builder().Build();
        bannerViewAd.LoadAd(bannerRequest);
    }


    private void OnAdFailedToLoad(object sender, EventArgs e)
    {
        interstitialAd = new InterstitialAd(interstitialID);
        AdRequest interstitialRequest = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(interstitialRequest);
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
