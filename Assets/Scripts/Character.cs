using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterStats PA;
    public CharacterStats Strength;
    public CharacterStats Hacking;
    public CharacterStats Sense;
    public CharacterStats Agility;
    public bool isCaptured = false;

    private Character playerUnit; // Temporaire, à gerer dans le PlayerController

    void PlanPlayerAction()
    {
        PlannedAction playerAction = new PlannedAction
        {
           Performer = playerUnit,
           Type = ActionType.Move
        };

        GameManager.Instance.PlanAction(playerAction);
    }

    // À la fin de la phase de planification, exécutez les actions simultanément
    void EndPlanningPhase()
    {
        GameManager.Instance.EndPlanningPhase();
    }

    public IEnumerator PerformAction(PlannedAction plannedAction)
    {
        // Implémentez ici la logique d'exécution de l'action
        // Par exemple, déplacez l'unité, infligez des dégâts, etc.
        yield return null; // Remplacez cela par la logique réelle
    }

    public IEnumerator Move()
    {
        // Implémentation du déplacement
        // Utilisez yield return pour les actions asynchrones si nécessaire
        yield return null;
    }

    public IEnumerator Lockpick()
    {
        // Implémentation du crochetage
        yield return null;
    }

    public IEnumerator Wait()
    {
        // Implémentation de l'attente
        yield return null;
    }

    public IEnumerator OpenDoor()
    {
        // Implémentation de l'ouverture de porte
        yield return null;
    }

    public IEnumerator BreakWindow()
    {
        yield return null;
    }

    public IEnumerator PickUpDrop()
    {
        yield return null;
    }

    public IEnumerator Stun()
    {
        yield return null;
    }

    public IEnumerator Hack()
    {
        yield return null;
    }

    public IEnumerator UseItem()
    {
        yield return null;
    }
}
