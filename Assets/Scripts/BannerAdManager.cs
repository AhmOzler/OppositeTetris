using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class BannerAdManager : MonoBehaviour
{
    [SerializeField] string androidID; 
    [SerializeField] string IOSID; 
    BannerView bannerView;
    string ID;

    private void Start() {
        RequestBanner();
    }

    
    void RequestBanner()
    {
        #if UNITY_ANDROID
            ID = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            ID = "ca-app-pub-3940256099942544/2934735716";  
        #else
            ID = "Tanımsız Platform";
        #endif


        AdSize adSize = new AdSize(320, 50);
        bannerView = new BannerView(ID, adSize, AdPosition.Top);
        /* bannerView.OnAdLoaded += IsLoaded;
        bannerView.OnAdFailedToLoad += IsFailedToLoad;
        bannerView.OnAdOpening += IsOpening;
        bannerView.OnAdClosed += IsClosed;
        bannerView.OnPaidEvent += IsPaidEvent; */
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
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
