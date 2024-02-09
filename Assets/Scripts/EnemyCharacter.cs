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
    [SerializeField]
    public GuardState guardState;


    private void Start()
    {
        StartCoroutine(UpdateFieldOfView());
        StartCoroutine(GererEtatGarde());
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////// STATE MACHINE GUARD ///////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////
    public enum GuardState
    {
        Patrol,
        Chasing
    }

    IEnumerator GererEtatGarde()
    {
        while (true)
        {
            switch (guardState)
            {
                case GuardState.Patrol:
                    Debug.Log("En patrouille");
                    yield return new WaitForSeconds(1f);
                    break;
                case GuardState.Chasing:

                    Debug.Log("En Chasse");
                    yield return new WaitForSeconds(1f); 
                    break;
            }

            if (fieldOfView.IsTarget)
            {
                guardState = GuardState.Chasing;
            }
            else
            {
                guardState = GuardState.Patrol;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////// DIRECTION CHAMP DE VISION DES GARDES //////////////////////
    //////////////////////////////////////////////////////////////////////////////////////


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

        if (guardState == GuardState.Chasing) // Joueur detecté
        {
            List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, player.GetCurrentCell());
            _target = fullPath[movePoints];

            path = fullPath.GetRange(0, movePoints + 1);
        }
        else
        {
            List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex]);
            _target = fullPath[movePoints];

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

    



}
