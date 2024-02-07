using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteObject : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On; //Applique les ombres du shader à l'environnement
    }
}
