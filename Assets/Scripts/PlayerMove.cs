using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    private bool _isSelected = false;

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
            FindSelectableTiles();
            CheckMouse();
        }
        else
        {
            Move();
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable)
                    {
                        //todo: move target
                        MoveToTile(t);
                    }
                }else if (hit.collider.tag == "Player")
                {
                    SelectPlayer();
                }
            }
            
        }
    }

    void SelectPlayer()
    {
        if (_isSelected)
        {
            //Le personnage est d�j� selectionn�
            EndTurn();
        }
        else
        {
            //Le personnage n'est pas encore selectionner
            BeginTurn();

        }
    }
}
