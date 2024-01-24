using System.Collections;
using System.Collections.Generic;
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

    Vector3 _velocity = new Vector3();
    Vector3 _heading = new Vector3();

    float _halfHeight = 0;

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
}
