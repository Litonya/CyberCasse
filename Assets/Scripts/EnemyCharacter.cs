using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    [SerializeField]
    private Direction _direction;
    private Direction _currentDirection = Direction.North;
    [SerializeField] private int _guardLevel = 2;

    [Header("Patrol settings")]
    //Sentinel
    [SerializeField] private bool _isSentinel = false;
    [SerializeField] private List<Direction> _sentinelDirection = new List<Direction>();
    //Patrol
    public bool loopingPatrol;
    [SerializeField] private List<Cell> _patrolTargets;


    EnemyFOV fieldOfView;
    PlayerCharacter player;
    
    private int _currentPatrolIndex = 0;
    
    private Cell cellDirection;

    private GuardState guardState;
    private bool _goBackOnPath = false;

    


    private void Awake()
    {
        fieldOfView = GetComponent<EnemyFOV>();
    }

    private void Start()
    {
        //cellDirection = _currentCell;
        _currentDirection = _direction;
        if (_sentinelDirection.Count == 0) _sentinelDirection.Add(_direction);
        if (_patrolTargets.Count == 0) _patrolTargets.Add(_currentCell);
        Invoke("AfterStart", 0.5f);
    }

    private void AfterStart()
    {
        fieldOfView.UpdateSightOfView(_direction, _currentCell);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////// STATE MACHINE GUARD ///////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////
    public enum GuardState
    {
        Patrol,
        Chasing
    }

    //////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////// DIRECTION CHAMP DE VISION DES GARDES //////////////////////
    //////////////////////////////////////////////////////////////////////////////////////


    

    public void PrepareTurnAction()
    {

        List<Cell> fullPath = new List<Cell>();



        if (guardState == GuardState.Chasing) // Joueur detecté
        {

            fullPath = MapManager.instance.FindPath(_currentCell, player.GetCurrentCell(), true);
            /*foreach (Cell cell in player.GetCurrentCell().adjencyList)
            {f
                List<Cell> potentielPath = new List<Cell>();
                potentielPath = MapManager.instance.FindPath(_currentCell, cell, true);
                if (potentielPath.Count > fullPath.Count) fullPath = potentielPath;
            }*/
        }
        else if (_isSentinel) return;
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

    private void SentinelRotate()
    {
        _currentPatrolIndex++;
        if(_currentPatrolIndex >= _sentinelDirection.Count) _currentPatrolIndex = 0;
        ChangeDirection(_sentinelDirection[_currentPatrolIndex]);
    }

    public override void Reset()
    {
        base.Reset();
        PrepareTurnAction();
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
        if (_nextCell != null && _nextCell!=_currentCell)
        {
            ChangeDirection(MapManager.instance.GetAdjacentCellDirection(_currentCell, _nextCell));
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

    public override void Acte()
    {
        if (!_isSentinel || guardState == GuardState.Chasing)
        {
            base.Acte();
        }
        else SentinelRotate();
    }




}
