using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX_Name
{
    LOCK_PICKING,
    SIMPLE_DOOR_LOCKED,
    SIMPLE_DOOR_OPEN,
    METALIC_DOOR_LOCKED,
    METALIC_DOOR_UNLOCKED,
    METALIC_DOOR_OPEN,
    MONEY_COLLECTED,
    STRONGBOX_LOOT,
    NEONS,
    STEP_GUARD,
    STEP_PLAYER,
    CAMERA_CONNEXION,
    CAMERA_OFF,
    CAMERAS_ROOM,
    CITY_AMBIENCE,
    GLASS,
    TIMER1,
    TIMER2,
    ACTION,
    PLAYER_DEAD,
    SELECTION
}

[Serializable]
public struct SFX
{
    public SFX_Name name;
    public AudioClip clip;
}
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioListener))]


public class AudioSystem : MonoBehaviour
{
    AudioSource _audioSource;
    EventsManager _eventsManager;

    public static AudioSystem instance { get { return _instance; } }
    static AudioSystem _instance;

    public List<SFX> _sfxList = new List<SFX>();

    private void Awake()
    {
        if (_instance != null) Destroy(_instance);
        _instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _eventsManager = EventsManager.instance;
        _eventsManager.OnPlaySFX += OnPlaySFX;
    }

    void OnPlaySFX(SFX_Name pSFXname)
    {
        _audioSource.clip = GetSFX(pSFXname);
        _audioSource.Play();
    }
    
    public AudioClip GetSFX(SFX_Name pSFXname)
    {
        for (int i = 0; i < _sfxList.Count; i++)
        {
            if (_sfxList[i].name == pSFXname)
            {
                if (_sfxList[i].clip == null)
                {
                    Debug.LogError(pSFXname + " is null.");
                    return null;
                }
                return _sfxList[i].clip;
            }
        }
        Debug.LogError(pSFXname + " not found in the SFX list!");
        return null;

    }

    void OnDestroy()
    {
        _eventsManager.OnPlaySFX -= OnPlaySFX;
    }
}
