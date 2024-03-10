using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellAction : MonoBehaviour
{
    protected Cell _cell;

    protected virtual void Awake()
    {
        _cell = GetComponent<Cell>();
    }

    public abstract void Acte(int charcterStat);
}
