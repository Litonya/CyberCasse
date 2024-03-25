using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBreakVisual : MonoBehaviour
{
    [SerializeField] GameObject _breakGlassGO;

    [SerializeField] GameObject _glassGO;

    public void BreakGlass()
    {
        _glassGO.SetActive(false);
        _breakGlassGO.SetActive(true);
    }
}
