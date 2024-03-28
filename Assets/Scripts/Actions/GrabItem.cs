using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GrabItem : CellAction
{

    float _actionDelay = 0.5f;
    float _remainDelay;

    bool _isActing = false;

    PlayerCharacter _character;

    protected override void Awake()
    {
        base.Awake();
        action = Actions.GETITEM;

        _remainDelay = _actionDelay;
    }

    private void Update()
    {
        if (_isActing)
        {
            _remainDelay -= Time.deltaTime;
            if (_remainDelay <= 0) ContinueActe(_character);
        }
    }

    public override bool Acte(int charcterStat, PlayerCharacter character)
    {
        if (_cell.GetItem().GetComponent<Key>()) EventsManager.instance.RaiseSFXEvent(SFX_Name.KEY_COLLECTED);
        else if (_cell.GetItem().GetComponent<Money>()) EventsManager.instance.RaiseSFXEvent(SFX_Name.MONEY_COLLECTED);
        else if (_cell.GetItem().GetComponent<Objective>()) EventsManager.instance.RaiseSFXEvent(SFX_Name.MONEY_COLLECTED);
        _isActing=true;
        _character = character;

        return true;


    }


    private void ContinueActe(PlayerCharacter character)
    {
        _isActing = false;

        if (character.GetCarriedItem() == null)
        {
            character.SetCarriedItem(_cell.GetItem());
            _cell.RemoveItem();
            GameManager.instance.UpdateMoneyScore(character.GetCarriedItem().value);
            Destroy(this);
        }
        else
        {
            Item playerItem = character.GetCarriedItem();
            GameManager.instance.UpdateMoneyScore(-playerItem.value);
            character.SetCarriedItem(_cell.GetItem());
            GameManager.instance.UpdateMoneyScore(character.GetCarriedItem().value);
            _cell.PlaceItem(playerItem);
        }

    }
}
