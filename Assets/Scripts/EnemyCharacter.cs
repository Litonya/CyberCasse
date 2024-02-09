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
    PlayerCharacter player;
    [SerializeField]
    private List<Cell> _patrolTargets;
    private int _currentPatrolIndex = 0;
    private Direction _direction = Direction.North;
    [SerializeField]
    private Cell cellDirection;


    private void Start()
    {
        StartCoroutine(UpdateFieldOfView());
    }

    enum Direction
    {
        North,
        West,
        East,
        South
    }

    private IEnumerator UpdateFieldOfView()
    {
        while (true)
        {
            fieldOfView.SetOrigin(transform.position);

            switch(_direction)
            {
                case Direction.North:
                    fieldOfView.SetDirection(transform.forward);
                    break;
                case Direction.South:
                    fieldOfView.SetDirection(-transform.forward);
                    break;
                case Direction.West:
                    fieldOfView.SetDirection(transform.right);
                    break;
                case Direction.East:
                    fieldOfView.SetDirection(-transform.right);
                    break;
                default:
                    break;
            }

            Debug.Log(fieldOfView.IsTarget);
            yield return null; 
        }
    }

    public void PrepareTurnAction()
    {

        if (fieldOfView.IsTarget) // Joueur detecté
        {
            List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, player.GetCurrentCell());
            _target = fullPath[movePoints];

            path = fullPath.GetRange(0, movePoints + 1);
        }
        else
        {
            List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex]);
            _target = fullPath[movePoints];

            path = fullPath.GetRange(0, movePoints + 1);
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
        if (_nextCell != null)
        {
            cellDirection.gridCoordX = _nextCell.gridCoordX - _currentCell.gridCoordX;
            cellDirection.gridCoordZ = _nextCell.gridCoordZ - _currentCell.gridCoordZ;

            // Debug.Log(cellDirection.gridCoordX);

            if (cellDirection.gridCoordX == 0 && cellDirection.gridCoordZ == 1)
                _direction = Direction.North;
            if (cellDirection.gridCoordX == 0 && cellDirection.gridCoordZ == -1)
                _direction = Direction.South;
            if (cellDirection.gridCoordX == 1 && cellDirection.gridCoordZ == 0)
                _direction = Direction.West;
            if (cellDirection.gridCoordX == -1 && cellDirection.gridCoordZ == 0)
                _direction = Direction.East;

        }

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
