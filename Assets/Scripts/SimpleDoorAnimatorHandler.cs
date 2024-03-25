using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoorAnimatorHandler : MonoBehaviour, DoorAnimatorHandler
{

    private Animator _animator;
    private Collider _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }
    
    public void Open()
    {
        _animator.SetBool("isOpen", true);
    }

    public void Close()
    {
        _animator.SetBool("isOpen", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>()) Open();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Character>()) Close();
    }

}
