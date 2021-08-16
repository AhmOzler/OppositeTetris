using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{

    [System.Serializable]
    public class SoundFx 
    {
        [HideInInspector] public AudioSource audioSource;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume;
        [Range(-3, 3)] public float pitch;
        public bool loop;
        public bool playOnAwake;
        public bool playOneTime;
    }

    private static SoundManager instance;
    public static SoundManager Instance => instance;

    [SerializeField] SoundFx[] sounds;

    private void Awake() {
        
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    
    private void OnDestroy() {

        instance = null;
    }


    public void Play(string name) {

        var soundFx = Array.Find(sounds, sound => sound.audioClip.name == name);

        if(soundFx.playOneTime) {

            if(!soundFx.audioSource.isPlaying) 
                soundFx.audioSource.Play();
        }
        else {

            soundFx.audioSource.Play();
        }
    }
}
