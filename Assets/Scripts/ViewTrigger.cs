using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTrigger : MonoBehaviour
{
    private Cell _currentCell;

    [HideInInspector]
    public EnemyFOV owner;

    private float yOffset = 1f;

    private void Awake()
    {
        MapManager.instance.GetCell(transform.position);
        owner = transform.parent.GetComponent<EnemyFOV>();
        transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
    }

}
