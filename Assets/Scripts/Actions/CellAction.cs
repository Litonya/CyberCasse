using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellAction : MonoBehaviour
{
    protected Cell _cell;

    public Actions action;

    protected virtual void Awake()
    {
        _cell = GetComponent<Cell>();
    }

    public abstract bool Acte(int charcterStat);
}
