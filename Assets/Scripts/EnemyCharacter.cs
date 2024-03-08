using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    EnemyFOV fieldOfView;
    [SerializeField]
    PlayerCharacter player;
    [SerializeField]
    private List<Cell> _patrolTargets;
    private int _currentPatrolIndex = 0;
    [SerializeField]
    private Direction _direction;
    private Direction _currentDirection = Direction.North;
    [SerializeField]
    private Cell cellDirection;
    [SerializeField]
    public GuardState guardState;
    public bool loopingPatrol;
    
    private bool _goBackOnPath = false;

    [SerializeField] private int _guardLevel = 2;


    private void Awake()
    {
        fieldOfView = GetComponent<EnemyFOV>();
    }

    private void Start()
    {
        //StartCoroutine(GererEtatGarde());
        cellDirection = _currentCell;
        _currentDirection = _direction;
        fieldOfView.UpdateSightOfView(_direction, _currentCell);
        //_patrolTargets.Add(_currentCell);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////// STATE MACHINE GUARD ///////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////
    public enum GuardState
    {
        Patrol,
        Chasing
    }

    /*IEnumerator GererEtatGarde()
    {
        while (true)
        {
            switch (guardState)
            {
                case GuardState.Patrol:
                    //Debug.Log("En patrouille");
                    yield return new WaitForSeconds(1f);
                    break;
                case GuardState.Chasing:

                    //Debug.Log("En Chasse");
                    yield return new WaitForSeconds(1f); 
                    break;
            }
        }
    }*/

    //////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////// DIRECTION CHAMP DE VISION DES GARDES //////////////////////
    //////////////////////////////////////////////////////////////////////////////////////


    

    public void PrepareTurnAction()
    {

        List<Cell> fullPath = new List<Cell>();

        if (guardState == GuardState.Chasing) // Joueur detecté
        {
            foreach (Cell cell in player.GetCurrentCell().adjencyList)
            {
                List<Cell> potentielPath = new List<Cell>();
                potentielPath = MapManager.instance.FindPath(_currentCell, cell, true);
                if (potentielPath.Count > fullPath.Count) fullPath = potentielPath;
            }
        }
        else
        {
            fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex], true);
        }

        foreach (Cell cell in fullPath)
        {
            if (path.Count > _moveSpeed + 1)
            {
                break;
            }
            path.Add(cell);
        }
        _target = path[path.Count-1];

        _target.SetState(CellState.isSelected);
        ShowPath();
    }

    private void LoopPatrol()
    {
        _currentPatrolIndex++;
        if (_currentPatrolIndex >= _patrolTargets.Count) _currentPatrolIndex = 0;
    }

    private void NormalPatrol()
    {
        if (!_goBackOnPath)
        {
            _currentPatrolIndex++;
            if (_currentPatrolIndex >= _patrolTargets.Count)
            {
                _currentPatrolIndex = _patrolTargets.Count-2;
                _goBackOnPath = true;
            }
        }
        else
        {
            Debug.Log("Back");
            _currentPatrolIndex--;
            if(_currentPatrolIndex < 0)
            {
                _currentPatrolIndex = 1;
                _goBackOnPath = false;
            }
        }
    }

    public override void Reset()
    {
        base.Reset();

        if (_patrolTargets.Count > 0)
        {
            PrepareTurnAction();
        }
    }

    void ChangeDirection(Direction pNewDirection)
    {
        if (pNewDirection == _currentDirection) return;

        _direction = pNewDirection;
        _currentDirection = _direction;
        fieldOfView.UpdateSightOfView(_direction, _currentCell);
    }

    public override void SetCurrentCell(Cell cell)
    {
        base._currentCell = cell;

        fieldOfView.UpdateSightOfView(_direction, _currentCell);

    }

    protected override void MoveToNextCell()
    {
        base.MoveToNextCell();
        if (_nextCell != null)
        {
            cellDirection.gridCoordX = _nextCell.gridCoordX - _currentCell.gridCoordX;
            cellDirection.gridCoordZ = _nextCell.gridCoordZ - _currentCell.gridCoordZ;

            //Debug.Log(cellDirection.gridCoordX);

            if (cellDirection.gridCoordX == 0 && cellDirection.gridCoordZ == 1)
                ChangeDirection(Direction.North);
            if (cellDirection.gridCoordX == 0 && cellDirection.gridCoordZ == -1)
                ChangeDirection(Direction.South);
            if (cellDirection.gridCoordX == 1 && cellDirection.gridCoordZ == 0)
                ChangeDirection(Direction.East);
            if (cellDirection.gridCoordX == -1 && cellDirection.gridCoordZ == 0)
                ChangeDirection(Direction.West);
        }

        if (_currentCell == _patrolTargets[_currentPatrolIndex])
        {
            if (loopingPatrol)
            {
                LoopPatrol();
            }
            else
            {
                NormalPatrol();
            }
        }
    }

    public void LaunchChase(PlayerCharacter newTarget)
    {
        Debug.Log(newTarget.gameObject.name + " is detected by " + this.gameObject.name);
        guardState = GuardState.Chasing;
        player = newTarget;
    }

    


}
