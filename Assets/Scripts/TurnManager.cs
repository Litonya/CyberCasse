using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
    {
        private List<PlannedAction> plannedActions = new List<PlannedAction>();
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GetComponent<GameManager>();
        }

        public void PlanAction(PlannedAction action)
        {
            plannedActions.Add(action);
        }

        public void EndPlanningPhase()
        {
            StartCoroutine(ExecuteActions());
        }

        private IEnumerator ExecuteActions()
        {
            List<Coroutine> coroutines = new List<Coroutine>();

            foreach (var plannedAction in plannedActions)
            {
                coroutines.Add(StartCoroutine(plannedAction.Performer.PerformAction(plannedAction)));
            }

            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }

            plannedActions.Clear();
            // Ajoutez ici la logique pour passer au tour suivant
        }
        // Reste du code...
    }


