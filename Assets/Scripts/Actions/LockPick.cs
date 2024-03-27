using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : CellAction
{
    float _actionDelay = 0.5f;
    float _remainDelay;

    bool _isActing = false;

    protected override void Awake()
    {
        base.Awake();
        action = Actions.LOCKPICK;
        _remainDelay = _actionDelay;
    }

    private void Update()
    {
        if (_isActing)
        {
            _remainDelay -= Time.deltaTime;
            if (_remainDelay <= 0) ContinueActe();
        }
    }

    public override bool Acte(int characterStat, PlayerCharacter character)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            EventsManager.instance.RaiseSFXEvent(SFX_Name.LOCK_PICKING);
            _isActing=true;

            return true;
        }
        return false;
    }

    private void ContinueActe()
    {
        _isActing = false;

        Unlock(_cell);

        foreach (Cell cell in _cell.linkCell)
        {
            Unlock(cell);
        }
    }


    private void Unlock(Cell cell)
    {
        _cell.SetWalkable();
        _cell.possibleActions.Remove(Actions.LOCKPICK);
        _cell.Lock_Close.SetActiveIcon(false);
        _cell.Lock_Open.SetActiveIcon(true);
        Invoke("RemoveIcon", 3);
    }

    private void RemoveIcon()
    {
        _cell.Lock_Open.SetActiveIcon(false);
    }
}
