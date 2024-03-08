using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
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

