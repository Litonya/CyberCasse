using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    LOCKPICK,
    HACK,
    GETITEM,
    PLACEITEM
}

public class PlayerAction : MonoBehaviour
{
    PlayerCharacter character;

    [Header("Character's stats")]
    [SerializeField] private int _lockPinckingStat = 1;
    [SerializeField] private int _hacking = 1;
    [SerializeField] private int _strenght = 1;

    private void Awake()
    {
        character = GetComponent<PlayerCharacter>();
    }

    public void LockPick()
    {

    }

    public void Hack() 
    {

    }

    public void KnockOutGard(EnemyCharacter guard)
    {

    }
}

