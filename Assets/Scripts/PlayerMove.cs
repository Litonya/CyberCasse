using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        FindSelectableTiles();
    }
}
