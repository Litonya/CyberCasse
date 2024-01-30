using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : TacticsMove
{
    GameObject target;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindNearestTarget();
            CalculatePath();
            FindSelectableTiles();
            actualTargetTile.target = target;
        }
        else
        {
            Move();
        }
    }

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach(GameObject obj in targets) 
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance )
            {
                distance = d;
                nearest = obj;
            }
        }


        target = nearest;
    }
}
