using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : CellAction
{
    float _actionDelay = 0.5f;
    float _remainDelay;

    bool _isActing = false;

    protected override void Awake()
    {
        base.Awake();
        action = Actions.BREAKGLASS;

        _remainDelay = _actionDelay;
    }

    private void Update()
    {
        if (_isActing)
        {
            _remainDelay -= Time.deltaTime;
            if (_remainDelay <= 0 ) ContinueActe();
        }
    }

    public override bool Acte(int characterStat, PlayerCharacter character)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            EventsManager.instance.RaiseSFXEvent(SFX_Name.GLASS);
            _isActing = true;
            return true;
        }
        return false;
    }

    private void ContinueActe()
    {
        _isActing=false;

        Break(_cell);

        foreach (Cell cell in _cell.linkCell)
        {
            Break(cell);
        }
    }

    private void Break(Cell cell)
    {
        _cell.SetWalkable();
        _cell.BreakGlass();
        _cell.possibleActions.Remove(Actions.BREAKGLASS);
        _cell.Icon_Break.SetActiveIcon(false);
    }
}
