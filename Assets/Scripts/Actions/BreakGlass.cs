using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : CellAction
{
    protected override void Awake()
    {
        base.Awake();
        action = Actions.BREAKGLASS;
    }

    public override bool Acte(int characterStat, PlayerCharacter character)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            EventsManager.instance.RaiseSFXEvent(SFX_Name.GLASS);
            Break(_cell);

            foreach (Cell cell in _cell.linkCell)
            {
                Break(cell);
            }

            return true;
        }
        return false;
    }

    private void Break(Cell cell)
    {
        _cell.SetWalkable();
        _cell.BreakGlass();
        _cell.possibleActions.Remove(Actions.LOCKPICK);
        _cell.Icon_Break.SetActiveIcon(false);
    }
}
