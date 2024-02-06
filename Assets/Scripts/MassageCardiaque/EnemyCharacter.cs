using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    [SerializeField]
    private List<Cell> _patrolPath;
    private int _currentPatrolIndex = 0;

    private void Start()
    {
        path = MapManager.instance.FindPath(_currentCell, _patrolPath[0]);
    }

}
