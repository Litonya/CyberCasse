using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject attachedCharacter; // R�f�rence au personnage attach�

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>()) // V�rifie si le personnage entre en collision avec la win condition
        {
            attachedCharacter = other.gameObject; // Stocke la r�f�rence au personnage
            other.GetComponentInParent<PlayerCharacter>().PickUpWinCondition(this); // Informe le personnage qu'il a ramass� la win condition
           // gameObject.SetActive(false);
        }
    }
}
