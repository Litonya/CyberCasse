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

    private Character playerUnit; // Temporaire, � gerer dans le PlayerController

    void PlanPlayerAction()
    {
        PlannedAction playerAction = new PlannedAction
        {
           Performer = playerUnit,
           Type = ActionType.Move
        };

        GameManager.Instance.PlanAction(playerAction);
    }

    // � la fin de la phase de planification, ex�cutez les actions simultan�ment
    void EndPlanningPhase()
    {
        GameManager.Instance.EndPlanningPhase();
    }

    public IEnumerator PerformAction(PlannedAction plannedAction)
    {
        // Impl�mentez ici la logique d'ex�cution de l'action
        // Par exemple, d�placez l'unit�, infligez des d�g�ts, etc.
        yield return null; // Remplacez cela par la logique r�elle
    }

    public IEnumerator Move()
    {
        // Impl�mentation du d�placement
        // Utilisez yield return pour les actions asynchrones si n�cessaire
        yield return null;
    }

    public IEnumerator Lockpick()
    {
        // Impl�mentation du crochetage
        yield return null;
    }

    public IEnumerator Wait()
    {
        // Impl�mentation de l'attente
        yield return null;
    }

    public IEnumerator OpenDoor()
    {
        // Impl�mentation de l'ouverture de porte
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
