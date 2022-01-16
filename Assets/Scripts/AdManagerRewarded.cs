using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManagerRewarded : MonoBehaviour
{
    [SerializeField] string androidID; 
    [SerializeField] string IOSID; 
    RewardedAd rewardedAd;
    string ID;

    private void Start() {

        RequestRewardedAd();
    }
    
    void RequestRewardedAd() 
    {
        #if UNITY_ANDROID 
        {
            ID = androidID;
        }       
        #elif UNITY_IPHONE 
        {
            ID = IOSID;  
        }           
        #else 
        {
            ID = "Tanımsız Platform";
        }           
        #endif

        rewardedAd = new RewardedAd(ID);
        //ad.OnAdLoaded += IsLoaded;
        
        //ad.OnAdOpening += IsOpening;

        /* ad.OnAdClosed += delegate (object sender, EventArgs arg) {
            RequestRewardedAd(ad, androidID, IOSID);
        };

        ad.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs arg) {
            RequestRewardedAd(ad, androidID, IOSID);
        }; */
        
        rewardedAd.OnUserEarnedReward += IsUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }


    public void ShowRewardedAd() {

        if(rewardedAd.IsLoaded())
            rewardedAd.Show();
    }


    void DestroyAd() {
        rewardedAd.Destroy();
    }

    public void IsLoaded(object sender, EventArgs args)
    {
        
    }

    public void IsFailedToLoad(object sender, EventArgs args)
    {
        
    }

    public void IsOpening(object sender, EventArgs args)
    {
        
    }

    public void IsClosed(object sender, EventArgs args)
    {
        
    }

    void IsUserEarnedReward(object sender, Reward args)
    {
        string adName = args.Type;
        int adAmount = (int) args.Amount;
        UIController.Instance.IncreaseChangeButton(adAmount);
    }
}
