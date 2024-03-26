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
    SELECTION,
    CANCEL_ACTION,
    SELECTION_ACTION,
    KEY_COLLECTED,
    PAUSE,
    AMBIENCE,
    ALERT0,
    ALERT1,
    ALERT2,
    ALERT3,
    VICOTORY,
    DEFEAT,
    STEP_PLAYER2,
    STEP_PLAYER3,
    STEP_PLAYER4,
    STEP_PLAYER5,
    STEP_PLAYER6,
    STEP_GUARD2,
    STEP_GUARD3,
    STEP_GUARD4,
    STEP_GUARD5,
    STEP_GUARD6
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
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _cityAmbiance;
    [SerializeField] AudioSource _ambience;
    [SerializeField] AudioSource _music;
    [SerializeField] List<AudioSource> _serverRoomsAmbience;
    EventsManager _eventsManager;

    public static AudioSystem instance { get { return _instance; } }
    static AudioSystem _instance;

    public List<SFX> _sfxList = new List<SFX>();

    private void Awake()
    {
        if (_instance != null) Destroy(_instance);
        _instance = this;

    }

    private void Start()
    {
        _eventsManager = EventsManager.instance;
        _eventsManager.OnPlaySFX += OnPlaySFX;
    }

    void OnPlaySFX(SFX_Name pSFXname)
    {
        if (pSFXname == SFX_Name.ALERT0 || pSFXname == SFX_Name.ALERT1 || pSFXname == SFX_Name.ALERT2 || pSFXname == SFX_Name.ALERT3)
        {
            _music.clip = GetSFX(pSFXname);
            _music.Play();
        }

        if(pSFXname == SFX_Name.AMBIENCE)
        {
            _ambience.Play();
            _music.Play();
            foreach (AudioSource source in _serverRoomsAmbience) source.Play();
            return;
        }

        if (pSFXname == SFX_Name.CITY_AMBIENCE)
        {
            _cityAmbiance.Play();
            return;
        }

        if (pSFXname == SFX_Name.PAUSE || pSFXname == SFX_Name.VICOTORY || pSFXname == SFX_Name.DEFEAT)
        {
            _cityAmbiance.Pause();
            _ambience.Pause();
            _music.Pause();
            foreach (AudioSource source in _serverRoomsAmbience) source.Pause();
        }

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

    public void PlayCityAmbience()
    {
        _cityAmbiance.Play();
    }
}
