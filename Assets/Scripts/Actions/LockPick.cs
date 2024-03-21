using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : CellAction
{
    protected override void Awake()
    {
        base.Awake();
        action = Actions.LOCKPICK;
    }

    public override bool Acte(int characterStat, PlayerCharacter character)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            Unlock(_cell);

            foreach(Cell cell in _cell.linkCell)
            {
                Unlock(cell);
            }

            return true;
        }
        return false;
    }

    private void Unlock(Cell cell)
    {
        _cell.SetWalkable();
        _cell.possibleActions.Remove(Actions.LOCKPICK);
        _cell.Lock_Close.SetActiveIcon(false);
        _cell.Lock_Open.SetActiveIcon(true);
        new WaitForSeconds(1);
        _cell.Lock_Open.SetActiveIcon(false);
    }
}
