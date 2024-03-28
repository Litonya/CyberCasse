using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UnlockDoor : CellAction
{

    float _actionDelay = 0.5f;
    float _remainDelay;

    bool _isActing = false;

    protected override void Awake()
    {
        base.Awake();
        action = Actions.UNLOCK;

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


    public override bool Acte(int charcterStat, PlayerCharacter character)
    {
        Debug.Log("BOOOP");
        Key keyItem = character.GetCarriedItem().GetComponent<Key>();
        if (keyItem != null)
        {
            if (keyItem.keyColor == _cell.neededKey)
            {
                EventsManager.instance.RaiseSFXEvent(SFX_Name.METALIC_DOOR_UNLOCKED);
                character.DestroyCarriedItem();
                _isActing = true;
                return true;
                
            }
            Debug.Log("Wrong key color");
        }
        Debug.Log("Character not carrying a key");
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
        cell.SetWalkable();
        cell.possibleActions.Remove(Actions.UNLOCK);
        
        cell.Lock_Close.SetActiveIcon(false);
        cell.Lock_Open.SetActiveIcon(true);
        cell.UnlockIcon();
    }
}
