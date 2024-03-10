using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : CellAction
{
    public override void Acte(int characterStat)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            _cell.SetWalkable();
            _cell.possibleActions.Remove(Actions.LOCKPICK);
        }
    }
}
