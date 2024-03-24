using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyCharacter;

public interface Enemy
{
    public void PlayerDetected(PlayerCharacter target);

    public bool GetIsAlerted();
}
