using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UnlockDoor : CellAction
{
    protected override void Awake()
    {
        base.Awake();
        action = Actions.UNLOCK;
    }

    public override bool Acte(int charcterStat, PlayerCharacter character)
    {
        Debug.Log("BOOOP");
        Key keyItem = character.GetCarriedItem().GetComponent<Key>();
        if (keyItem != null)
        {
            if (keyItem.keyColor == _cell.neededKey)
            {
                Unlock(_cell);

                foreach(Cell cell in _cell.linkCell)
                {
                    Unlock(cell);
                }
                character.DestroyCarriedItem();
                return true;
                
            }
            Debug.Log("Wrong key color");
        }
        Debug.Log("Character not carrying a key");
        return false;
    }

    private void Unlock(Cell cell)
    {
        _cell.SetWalkable();
        _cell.possibleActions.Remove(Actions.UNLOCK);  
    }
}
