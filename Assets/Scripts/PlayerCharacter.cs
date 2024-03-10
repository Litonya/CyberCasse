using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    NONE,
    LOCKPICK,
    HACK,
    GETITEM,
    PLACEITEM,
    KNOCKOUT
}

public class PlayerCharacter : Character
{
    [Header("Character's stats")]
    [SerializeField] private int _lockPinckingStat = 1;
    [SerializeField] private int _hacking = 1;
    [SerializeField] private int _strenght = 1;

    private Actions _preparedAction;
    private Cell _targetActionCell;

    private void LaunchAction()
    {
        if (_preparedAction == Actions.LOCKPICK)
        {
            if(!_targetActionCell.Acte(_preparedAction, _lockPinckingStat))
            {
                SetPreparedAction(_preparedAction, _target);
            }else
            {
                ClearPreparedAction();
            }
        }
    }

    public void SetPreparedAction(Actions action, Cell target)
    {
        _preparedAction = action;
        _targetActionCell = target;
        //Ajouter ligne qui fait lien vers la cellule pour afficher un logo
    }

    public void ClearPreparedAction()
    {
        _preparedAction = Actions.NONE;
        _targetActionCell = null;
    }

    public void PickUpWinCondition(WinCondition winCondition)
    {
        // Attachement de la win condition au joueur
        attachedWinCondition = winCondition;

        // Déterminer la position à laquelle attacher la win condition par rapport au joueur
        Vector3 offset = new Vector3(0f, _yOffset, 0f);
        winCondition.transform.position = transform.position + offset;
        winCondition.transform.parent = transform; // Attacher la win condition au joueur pour qu'elle suive ses mouvements
    }

    public bool HasWinConditionAttached()
    {
        WinCondition winCondition = GetComponentInChildren<WinCondition>();
        Debug.Log(winCondition);
        return winCondition != null;
    }

    public bool IsInVictoryZone()
    {
        // Recherche la VictoryArea dans la scène
        VictoryArea victoryArea = FindObjectOfType<VictoryArea>();
        if (victoryArea != null)
        {
           return victoryArea.playersInVictoryZone.Count == victoryArea.totalPlayer;
        }//
        else
        {
            Debug.LogWarning("VictoryArea introuvable dans la scène.");
            return false;
        }
    }
}

