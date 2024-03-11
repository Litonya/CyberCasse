using UnityEngine;

public class Door : Cell
{
    // Bool�en pour suivre l'�tat de la porte
    private bool isOpen = false;

    // R�f�rence au gameObject pour l'animation d'ouverture
    public GameObject doorObject;

    // M�thode pour ouvrir la porte
    public void OpenDoor()
    {
        // Changer l'�tat de la porte
        isOpen = true;

        // Rendre la cellule praticable
        walkable = true;

        // Effectuer une rotation de 90 degr�s en Y sur le gameObject de la porte
        if (doorObject != null)
            doorObject.transform.Rotate(0, 90, 0);
    }

    // M�thode pour observer la porte
    public void ObserveDoor()
    {
        // Impl�mentez le comportement pour observer la porte, si n�cessaire
        Debug.Log("Observing the door.");
    }

    // M�thode pour v�rifier si la porte est ouverte
    public bool IsOpen()
    {
        return isOpen;
    }

    // M�thode pour v�rifier si la porte est ferm�e
    public bool IsClosed()
    {
        return !isOpen;
    }
}