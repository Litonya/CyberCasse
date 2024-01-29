// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("_GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private TurnManager turnManager;

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        // Ajoutez d'autres initialisations si nécessaires
    }

    public void PlanAction(PlannedAction action)
    {
        turnManager.PlanAction(action);
    }

    public void EndPlanningPhase()
    {
        turnManager.EndPlanningPhase();
    }
}
