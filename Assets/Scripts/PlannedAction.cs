// PlannedAction.cs
using System.Collections;
using UnityEngine;

public class PlannedAction
{
    public Character Performer { get; set; }
    public ActionType Type { get; set; }
    // Ajoutez d'autres propriétés selon vos besoins

    public IEnumerator Execute()
    {
        switch (Type)
        {
            case ActionType.Move:
                // action de déplacement
                yield return Performer.Move(); // Exemple, adaptez selon votre logique
                break;

            case ActionType.Lockpicking:
                // action de crochetage
                yield return Performer.Lockpick();
                break;

            case ActionType.Wait:
                // Logique pour l'action d'attente
                yield return Performer.Wait();
                break;

            case ActionType.OpenDoor:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.OpenDoor();
                break;

            case ActionType.BreakWindow:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.BreakWindow();
                break;

            case ActionType.PickUpDrop:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.PickUpDrop();
                break;

            case ActionType.Stun:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.Stun();
                break;

            case ActionType.Hack:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.Hack();
                break;

            case ActionType.UseItem:
                // Logique pour l'action d'ouverture de porte
                yield return Performer.UseItem();
                break;

            default:
                Debug.LogError("Type d'action non géré : " + Type);
                break;
        }
    }
}