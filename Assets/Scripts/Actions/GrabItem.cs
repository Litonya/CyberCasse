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
            GameManager.instance.UpdateMoneyScore(character.GetCarriedItem().value);
            Destroy(this);
            return true;
        }
        else
        {
            Item playerItem = character.GetCarriedItem();
            GameManager.instance.UpdateMoneyScore(-playerItem.value);
            character.SetCarriedItem(_cell.GetItem());
            GameManager.instance.UpdateMoneyScore(character.GetCarriedItem().value);
            _cell.PlaceItem(playerItem);
            return true;
        }
    }
}
