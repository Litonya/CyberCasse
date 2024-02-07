using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    [SerializeField] 
    EnemyFOV fieldOfView;

    [SerializeField]
    private List<Cell> _patrolTargets;
    private int _currentPatrolIndex = 0;


    private void Start()
    {
        StartCoroutine(UpdateFieldOfView());
    }

    private IEnumerator UpdateFieldOfView()
    {
        while (true)
        {
            fieldOfView.SetOrigin(transform.position);
            fieldOfView.SetDirection(transform.forward);
            yield return null; 
        }
    }

    public void PrepareTurnAction()
    {
        List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex]);
        _target = fullPath[movePoints];

        path = fullPath.GetRange(0, movePoints + 1);

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

    private void ShowPath()
    {
        foreach (Cell cell in path)
        {
            cell.MarkPath();
        }
        
    }



}
