using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get { return _instance; } }
    static UIManager _instance;

    [SerializeField]
    private TextMeshPro _phaseLabel;

    [SerializeField]
    private Color _actionPhaseColor = Color.red;
    [SerializeField]
    private Color _planificationPhaseColor = Color.blue;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    public void SetUIActionPhase()
    {
        _phaseLabel.text = "ACTION";
        _phaseLabel.color = _actionPhaseColor;
    }

    public void SetUIPlanificationPhase()
    {
        _phaseLabel.text = "PLANIFICATION";
        _phaseLabel.color = _planificationPhaseColor;
    }
}

