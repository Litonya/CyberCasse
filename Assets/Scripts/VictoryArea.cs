using UnityEngine;
using System.Collections.Generic;

public class VictoryArea : MonoBehaviour
{
    // public Collider collider;
    public int totalPlayer = 4; // Nombre total de joueurs dans la scène
    [SerializeField]
    public List<PlayerCharacter> playersInVictoryZone = new List<PlayerCharacter>();

    void OnTriggerEnter(Collider other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            playersInVictoryZone.Add(player);
            CheckVictory();
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            playersInVictoryZone.Remove(player);
        }
    }

    void CheckVictory()
    {
        if (playersInVictoryZone.Count == totalPlayer)
        {
            Debug.Log("Tous les joueurs sont dans la zone de victoire !");
            GameManager.instance.TriggerVictory();
            // Mettre ici le code pour afficher l'écran de victoire ou effectuer d'autres actions de victoire
        }
    }
}