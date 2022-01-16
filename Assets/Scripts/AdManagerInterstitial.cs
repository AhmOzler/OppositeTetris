using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManagerInterstitial : MonoBehaviour
{
    [SerializeField] string androidID; 
    [SerializeField] string IOSID; 
    [SerializeField] string androidVideoID; 
    [SerializeField] string IOSVideoID; 
    InterstitialAd ad;
    InterstitialAd adVideo;
    string ID;

    private void Start() {
        RequestBanner(androidID, IOSID);
        RequestBanner(androidVideoID, IOSVideoID);
    }
    
    InterstitialAd RequestBanner(string androidID, string IOSID) 
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        
        #if UNITY_ANDROİD
            ID = androidID;
        #elif UNITY_IOS
            ID = IOSID;  
        #else
            ID = "Tanımsız Platform";
        #endif

        ad = new InterstitialAd(ID);
        //ad.OnAdLoaded += IsLoaded;
        
        //ad.OnAdOpening += IsOpening;

        /* ad.OnAdClosed += delegate (object sender, EventArgs arg) {
            RequestBanner(ad, androidID, IOSID);
        };

        ad.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs arg) {
            RequestBanner(ad, androidID, IOSID);
        }; */
        
        //ad.OnPaidEvent += IsPaidEvent;
        AdRequest request = new AdRequest.Builder().Build();
        ad.LoadAd(request);
        return ad;
    }


    public void ShowInterstitialAd() {

        if(ad.IsLoaded())
            RequestBanner(androidID, IOSID).Show();
        else   
            RequestBanner(androidID, IOSID);
    }


    void DestroyAd() {
        ad.Destroy();
    }


    public void ShowInterstitialVideoAd() {

        if(ad.IsLoaded())
            RequestBanner(androidVideoID, IOSVideoID).Show();
        else   
            RequestBanner(androidVideoID, IOSVideoID);
    }

    void DestroyVideoAd() {
        ad.Destroy();
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

    public void IsPaidEvent(object sender, EventArgs args)
    {
        
    }
}
