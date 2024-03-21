using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource m_WalkAudioSource;
    [SerializeField] private AudioSource m_ActionAudioSource;
    public void PlayWalkSound()
    {
        m_WalkAudioSource.Play();
    }

    public void StopWalkSound()
    {
        m_WalkAudioSource.Stop();
    }

    public void PlayActionSound(SFX_Name audioName)
    {
        m_ActionAudioSource.clip = AudioSystem.instance.GetSFX(audioName);
        m_ActionAudioSource.Play();
    }

    
}
