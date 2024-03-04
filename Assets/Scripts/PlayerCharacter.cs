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
            // Vérifie si le joueur se trouve à l'intérieur du cube de collider de la VictoryArea
            Collider playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
            {
                return victoryArea.GetComponent<Collider>().bounds.Contains(playerCollider.bounds.center);
            }
            else
            {
                Debug.LogWarning("Le joueur n'a pas de composant Collider attaché.");
                return false;
            }
        }//
        else
        {
            Debug.LogWarning("VictoryArea introuvable dans la scène.");
            return false;
        }
    }
}

