using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{

    void Awake()
    {
        GetComponent<Renderer>().enabled = false;
    }
    public void SetActiveIcon(bool state)
    {
        GetComponent<Renderer>().enabled = state;
    }
}
