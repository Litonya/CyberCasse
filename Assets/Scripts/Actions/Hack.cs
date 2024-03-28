using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : CellAction
{
    float _actionDelay = 0.5f;
    float _remainDelay;

    bool _isActing = false;

    protected override void Awake()
    {
        base.Awake();
        action = Actions.HACK;
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
        if (Random.Range(0f,1f) < character.hackStunChance)
        {
            character.Stun();
            return true;
        }
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            EventsManager.instance.RaiseSFXEvent(SFX_Name.CAMERA_CONNEXION);
            _isActing=true;


            return true;
        }
        return false;
    }

    private void ContinueActe()
    {
        _isActing = false;

        HackAllCams();

        foreach (Cell cell in _cell.linkCell)
        {
            HackAllCams();
        }

    }


    private void HackAllCams()
    {
        
        foreach(SecurityCamera cameras in GameManager.instance.securityCameraList)
        {
            cameras.Hack();
        }
    }
}
