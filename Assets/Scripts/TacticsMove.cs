using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    List<Tile> _selectableTiles = new List<Tile>();
    GameObject[] _tiles;

    Stack<Tile> _path = new Stack<Tile>();
    Tile _currentTile;
 
    public bool moving = false;
    public int move = 5;
    public float jumpHeight = 2;
    public float moveSpeed = 2;
    public float jumpVelocity = 4.5f;

    Vector3 _velocity = new Vector3();
    Vector3 _heading = new Vector3();

    float _halfHeight = 0;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    Vector3 jumpTarget;


    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _halfHeight = GetComponent<Collider>().bounds.extents.y;

    }

    public void GetCurrentTile()
    {
        _currentTile = GetTargetTile(gameObject);
        _currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up,out hit))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyList()
    {
        //_tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in _tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(_currentTile);
        _currentTile.visited = true;
        //_currentTile.parent = ?? leave as null

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            _selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);

                    }
                }
            }
        }

    }

    public void MoveToTile(Tile tile)
    {
        _path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while(next != null)
        {
            _path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (_path.Count > 0)
        {
            Tile t = _path.Peek();
            Vector3 target = t.transform.position;

            //calculate de unit's position on top of the target tile
            target.y += _halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                //Locomotion
                transform.forward = _heading;
                transform.position += _velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                _path.Pop();

            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;

        }
    }

    protected void RemoveSelectableTiles()
    {
        if (_currentTile != null)
        {
            _currentTile.current = false;
            _currentTile = null;
        }

        foreach (Tile tile in _selectableTiles)
        {
            tile.Reset();
        }

        _selectableTiles.Clear();
    }

    private void CalculateHeading(Vector3 target)
    {
        _heading = target - transform.position;
        _heading.Normalize();
    }

    private void SetHorizontalVelocity()
    {
        _velocity = _heading * moveSpeed;
    }

    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallingDown(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeading(target);

        if(transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            jumpTarget = transform.position +(target - transform.position) / 2f;
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            _velocity = _heading * moveSpeed / 3f;

            float difference = targetY - transform.position.y;

            _velocity.y = jumpVelocity * (0.5f + difference / 2f);
        }
    }

    void FallingDown(Vector3 target)
    {
        _velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p =transform.position;
            p.y= target.y;
            transform.position = p;

            _velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 target)
    {
        _velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;

        }
    }

    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            _velocity /= 5f;
            _velocity.y = 1.5f;
        }

    }
}
