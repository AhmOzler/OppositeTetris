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
        [Range(0, 1)] public float volume = 1;
        [Range(-3, 3)] public float pitch = 1;
        public bool loop;
        public bool playOnAwake;
        public bool playOneTime;
    }

    private static SoundManager instance;
    public static SoundManager Instance => instance;

    [SerializeField] SoundFx[] sounds;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);


        SetAudioSources();
    }


    private void OnDestroy() {

        instance = null;
    }


    private void Start() {
        SoundManager.Instance.Play("GameStart");
    }


    private void SetAudioSources()
    {
        foreach (SoundFx sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.playOnAwake = sound.playOnAwake;
        }
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
