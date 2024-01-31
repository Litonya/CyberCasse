using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Faire action selection qui demande au GM de selectionner cet objet

    [SerializeField]
    private float _yOffset = 1.25f; 
    private Cell _target;
    [SerializeField]
    private Cell _currentCell;

    public int movePoints = 4;

    public void Acte()
    {
        if (_target != null)
        {
            Move(_target);
            _target.SetState(Cell.CellState.Idle);
        }
    }

    public void TargetCell(Cell cell)
    {
        _target = cell;
        cell.SetState(Cell.CellState.isSelected);
    }

    public void Move(Cell target)
    {
         Vector3 destination = new Vector3(target.transform.position.x, _yOffset, target.transform.position.z);
         transform.position = destination;
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
