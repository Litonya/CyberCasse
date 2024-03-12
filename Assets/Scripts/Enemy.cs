using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    public void PlayerDetected(PlayerCharacter target);
}
