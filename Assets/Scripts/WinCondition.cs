using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject attachedCharacter; // Référence au personnage attaché

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>()) // Vérifie si le personnage entre en collision avec la win condition
        {
            attachedCharacter = other.gameObject; // Stocke la référence au personnage
            other.GetComponentInParent<PlayerCharacter>().PickUpWinCondition(this); // Informe le personnage qu'il a ramassé la win condition
           // gameObject.SetActive(false);
        }
    }
}
