using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItem : CellAction
{
    protected override void Awake()
    {
        base.Awake();
        action = Actions.GETITEM;
    }

    public override bool Acte(int charcterStat, PlayerCharacter character)
    {
        if (character.GetCarriedItem() == null)
        {
            character.SetCarriedItem(_cell.GetItem());
            _cell.RemoveItem();
            Destroy(this);
            return true;
        }
        else
        {
            Item playerItem = character.GetCarriedItem();
            character.SetCarriedItem(_cell.GetItem());
            _cell.PlaceItem(playerItem);
            return true;
        }
    }
}
