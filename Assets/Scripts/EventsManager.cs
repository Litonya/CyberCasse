using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance { get { return _instance; } }
    static EventsManager _instance;

    //public event Action OnPlaySFXAttack;
    //public event Action OnPlaySFXJump;
    public event Action<SFX_Name> OnPlaySFX;

    private void Awake()
    {
        if (_instance != null) Destroy(_instance);
        _instance = this;
    }

    public void RaiseSFXEvent(SFX_Name pSFXName)
    {
        if (OnPlaySFX != null)
        {
            OnPlaySFX.Invoke(pSFXName);
        }
    }
}
