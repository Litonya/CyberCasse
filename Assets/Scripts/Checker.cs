using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Check", 2f);
    }

    void Check()
    {
        bool isInList = false;

        for (int i = 0; i < MapManager.instance.cellArrayTemp.Length; i++)
        {
            if (MapManager.instance.cellArrayTemp[i] == this.gameObject)
            {
                isInList = true;
            }
        }

        Debug.Log("Is dectected by MM: " + isInList);
    }
}
