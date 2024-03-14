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
        Debug.Log("Hacked");
        return true;
    }
}
