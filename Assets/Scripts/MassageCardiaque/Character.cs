using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Faire action selection qui demande au GM de selectionner cet objet

    [SerializeField]
    private float _yOffset = 1.25f;

    private Cell _target;

    public void Acte()
    {
        Move(_target);
        _target.isSelected = false;
    }

    public void TargetCell(Cell cell)
    {
        _target = cell;
        cell.isSelected = true;
    }

    public void Move(Cell target)
    {
        Vector3 destination = new Vector3(target.transform.position.x, _yOffset, target.transform.position.z);
        transform.position = destination;
    }
}
