using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

//Script pour créer différente zone de caméra si besoin, voir le tuto poster par Liam le 24/01

[RequireComponent(typeof(Collider))]

public class CameraZone : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera = null;

    private void Start()
    {
        virtualCamera.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            virtualCamera.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCamera.enabled = false;
        }
    }
    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
