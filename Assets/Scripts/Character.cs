using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected float _yOffset = 1.25f;
    protected Cell _target;
    [SerializeField]
    protected Cell _currentCell;

    [SerializeField]
    protected float _moveSpeed = 2f;

    public List<Cell> path = new List<Cell>();
    public bool isMoving = false;
    protected Cell _nextCell;

    public bool currentAct = false;

    public int movePoints = 4;


    protected void Update()
    {
        if (isMoving)
        {
            MoveToNextCell();
        }else if (currentAct)
        {
            Action();
        }
    }

    public virtual void Reset()
    {
        _target = null;
        path.Clear();
        path.Add(_currentCell);
    }

    public void Acte()
    {
        currentAct = true;
        if(path.Count == 0)
        {
            //Debug.LogError("path array empty.");
            return;
        }

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

    protected virtual void MoveToNextCell()
    {
        if(_nextCell == null)
        {
            Debug.LogError("Next Cell not found! Aborting");
            return;
        }

        Vector3 target = new Vector3(_nextCell.transform.position.x, _nextCell.transform.position.y + _yOffset, _nextCell.transform.position.z);
        if (Vector3.Distance(transform.position, target) >= 0.05f)
        {
            //Rendre le mouvement smooth

            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);



        }
        else if(_nextCell == _target)
        {
            isMoving = false;
            SetCurrentCell(_nextCell);
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
         isMoving = true;
         _nextCell = path[0];
    }

    public void SetCurrentCell(Cell cell)
    {
        //Debug.Log("Character = " + this);
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

    private void Action()
    {
        currentAct = false;
    }

    protected void ShowPath()
    {
        foreach (Cell cell in path)
        {
            cell.MarkPath();
        }

    }
}
