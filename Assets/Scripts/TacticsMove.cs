using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool turn = false;

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

    public Tile actualTargetTile;

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _halfHeight = GetComponent<Collider>().bounds.extents.y;

        AeliTurnManager.AddUnit(this);
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

        if (Physics.Raycast(target.transform.position, -Vector3.up,out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyList(float jumpHeight, Tile target)
    {
        //_tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in _tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(jumpHeight, null);
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

            AeliTurnManager.EndTurn();

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
            movingEdge = true;

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

    protected Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];

        foreach(Tile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;

        while(next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i<= move; i++)
        {
            endTile = tempPath.Pop();
        }
        return endTile;
    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyList(jumpHeight,target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(_currentTile);
        //currentTile.parent = ??
        _currentTile.h = Vector3.Distance(_currentTile.transform.position, target.transform.position);
        _currentTile.f = _currentTile.h;

        while (openList.Count > 0) 
        {
            Tile t = FindLowestF(openList);

            closedList.Add(t);

            if(t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach(Tile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //Do nothing, it's already processed
                }
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g * tile.h;
                    }
                }
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }

        }

        //todo - what do you do is there is no path to the target tile
        Debug.Log("Path not found");
    }

    public void BeginTurn()
    {
        turn = true;
    }

    public void EndTurn()
    {
        turn = false;
    }
}
