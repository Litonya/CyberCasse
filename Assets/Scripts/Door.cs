using UnityEngine;

public class Door : Cell
{
    // Booléen pour suivre l'état de la porte
    private bool isOpen = false;

    // Référence au gameObject pour l'animation d'ouverture
    public GameObject doorObject;

    // Méthode pour ouvrir la porte
    public void OpenDoor()
    {
        // Changer l'état de la porte
        isOpen = true;

        // Rendre la cellule praticable
        walkable = true;

        // Effectuer une rotation de 90 degrés en Y sur le gameObject de la porte
        if (doorObject != null)
            doorObject.transform.Rotate(0, 90, 0);
    }

    // Méthode pour observer la porte
    public void ObserveDoor()
    {
        // Implémentez le comportement pour observer la porte, si nécessaire
        Debug.Log("Observing the door.");
    }

    // Méthode pour vérifier si la porte est ouverte
    public bool IsOpen()
    {
        return isOpen;
    }

    // Méthode pour vérifier si la porte est fermée
    public bool IsClosed()
    {
        return !isOpen;
    }
}