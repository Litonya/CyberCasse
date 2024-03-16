using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : CellAction
{
    protected override void Awake()
    {
        base.Awake();
        action = Actions.HACK;
    }
    public override bool Acte(int characterStat, PlayerCharacter character)
    {
        _cell.remainDifficulty -= characterStat;
        if (_cell.remainDifficulty <= 0)
        {
            HackAllCams();

            return true;
        }
        return false;
    }

    private void HackAllCams()
    {
        foreach(SecurityCamera cameras in GameManager.instance.securityCameraList)
        {
            cameras.Hack();
        }
    }
}
