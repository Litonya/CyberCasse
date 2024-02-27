using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Move,          // Déplacement
    Lockpicking,   // Crochetage
    Wait,          // Attendre
    OpenDoor,      // Ouvrir une porte
    BreakWindow,   // Casser une fenêtre/porte
    PickUpDrop,    // Ramasser / Lâcher un objet
    Stun,          // Assommer
    Hack,          // Pirater
    UseItem        // Utiliser un objet
}
