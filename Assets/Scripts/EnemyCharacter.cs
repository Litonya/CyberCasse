using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    [SerializeField]
    private List<Cell> _patrolTargets;
    private int _currentPatrolIndex = 0;

    public void PrepareTurnAction()
    {
        List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex]);

        //Initialise le chemin en utilisant la case actuelle du NPC
        path.Add(_currentCell);
        _target = _currentCell;

        //Parcours le chemin le plus cours jusqu'à la case en vérifiant que personne ne se trouve sur sa route pour le moment
        for (int i = 1; i <= movePoints && i < fullPath.Count; i++)
        {
            if (fullPath[i].occupant != null) { break; }
            else 
            { 
                path.Add(fullPath[i]); 
                _target = fullPath[i];
            }
        }

        _target.SetState(CellState.isSelected);

        ShowPath();
    }

    public override void Reset()
    {
        base.Reset();

        if (_patrolTargets.Count > 0)
        {
            PrepareTurnAction();
        }
    }

    protected override void MoveToNextCell()
    {
        base.MoveToNextCell();

        if (_currentCell == _patrolTargets[_currentPatrolIndex])
        {
            _currentPatrolIndex++;
            if (_currentPatrolIndex >= _patrolTargets.Count) {  _currentPatrolIndex = 0; }
        }
    }

    

}
