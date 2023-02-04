using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class InterstitialAdManager : MonoBehaviour
{
    private static InterstitialAdManager instance;
    public static InterstitialAdManager Instance => instance;
    public enum AdType {
        Real,
        Test
    }
    [SerializeField] AdType adType;
    InterstitialAd interstitialAd;
    string ID;


    private void Awake() {

        if(instance == null) {
            instance = this;
        }
        else {
            instance = null;
            Destroy(gameObject);
        }
    }


    private void Start() 
    {
        #if UNITY_ANDROID 
        {
            ID = adType == AdType.Real ? "ca-app-pub-3691073423716638/8636048313" : "ca-app-pub-3940256099942544/8691691433";
        }       
        #elif UNITY_IPHONE 
        {
            ID = adType == AdType.Real ? "ca-app-pub-3691073423716638/6612504299" : "ca-app-pub-3940256099942544/5135589807";
        }                    
        #endif

        interstitialAd = new InterstitialAd(ID);
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
    }
    


    public void ShowInterstitialAd() {

        if(interstitialAd.IsLoaded())
            interstitialAd.Show();
    }
}
