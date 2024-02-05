using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private float _yOffset = 1.25f;
    private Cell _target;
    [SerializeField]
    private Cell _currentCell;

    [SerializeField]
    private float _moveSpeed = 2f;

    public List<Cell> path = new List<Cell>();
    public bool isMoving = false;
    private Cell _nextCell;

    public int movePoints = 4;


    private void Update()
    {
        if (isMoving)
        {
            MoveToNextCell();
        }
    }

    public void Reset()
    {
        _target = null;
        path.Clear();
    }

    public void Acte()
    {
        if (_target != null)
        {
            Move();
            _target.SetState(Cell.CellState.Idle);
        }
    }

    public void TargetCell(Cell cell)
    {
        _target = cell;
        cell.SetState(Cell.CellState.isSelected);
    }

    private void MoveToNextCell()
    {
        Vector3 target = new Vector3(_nextCell.transform.position.x, _nextCell.transform.position.y + _yOffset, _nextCell.transform.position.z);
        if (Vector3.Distance(transform.position, target) >= 0.05f)
        {
            //Rendre le mouvement smooth

            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);



        }
        else if(_nextCell == _target)
        {
            isMoving = false;
            _nextCell.UnmarkPath();
        }
        else
        {
            _nextCell.UnmarkPath();
            path.Remove(_nextCell);
            SetCurrentCell(_nextCell);
            _nextCell = path[0];
        }
    }

    public void Move()
    {
        if (_target != null)
        {
            isMoving = true;
            _nextCell = path[0];
        }
         //Vector3 destination = new Vector3(target.transform.position.x, _yOffset, target.transform.position.z);
         //transform.position = destination;
         //SetCurrentCell(target);
    }

    public void SetCurrentCell(Cell cell)
    {
        Debug.Log("Character = " + this);
        if (_currentCell != null)
        {
            _currentCell.occupant = null;
        }
        _currentCell = cell;
        cell.occupant = this;
    }

    public Cell GetCurrentCell()
    {
        return _currentCell;
    }
}
