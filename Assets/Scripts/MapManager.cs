using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public enum Direction
{
    North,
    West,
    East,
    South
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get { return _instance; } }
    static MapManager _instance;

    public List<Cell> selectableCells = new List<Cell>(); 
    private Cell[,] _logicalMap;

    [SerializeField]
    private int _mapXSize;
    [SerializeField]
    private int _mapZSize;

    public GameObject[] cellArrayTemp;
    
    enum Side
    {
        LEFT,
        RIGHT,
        BACK
    }


    private void Awake()
    {
        //Singleton
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;


        //Initialisation tableau carte
        _logicalMap = new Cell[_mapXSize,_mapZSize];

        InitMap();
        InitCharacterPos();
    }

    private void InitMap()
    {
        //Récupére toutes les cellules de la scène
        GameObject[] cellList = GameObject.FindGameObjectsWithTag("Tile");
        cellArrayTemp = cellList;

        //Place les cellules dans le champs _logicalMap selon leurs coords
        foreach (GameObject cell in cellList)
        {
            Cell cellScript = cell.GetComponent<Cell>();

            if (cellScript != null)
            {
                int cellX = (int)cell.transform.position.x;
                int cellZ = (int)cell.transform.position.z;

                _logicalMap[cellX, cellZ] = cellScript;
                cellScript.gridCoordX = cellX;
                cellScript.gridCoordZ = cellZ;
            }


            //Debugage de l'initialisation de la map
            /*
            for (int i = 0; i < _logicalMap.GetLength(0); i++)
            {
                for (int j = 0; j < _logicalMap.GetLength(1); j++)
                {
                    if (_logicalMap[i, j] != null)
                    {
                        Debug.Log("[" + i + "," + j + "] coords: " + _logicalMap[i, j].transform.position);
                    }
                }
            }*/
        }
    }

    private void InitCharacterPos()
    {
        //Récupére tous les characters de la scène
        GameObject[] characterList = GameObject.FindGameObjectsWithTag("NPC");

        //Lie les character à leurs cellules
        foreach(GameObject character in characterList)
        {
            Character characterScript = character.GetComponent<Character>();
            if (characterScript != null)
            {
                int posX = (int)character.transform.position.x;
                int posZ = (int)character.transform.position.z;
                characterScript.SetCurrentCell(GetCell(posX, posZ));
                
            }
        }
    }


    public Cell GetCell(int x, int z)
    {
        if (x>= 0 && z>= 0 && x < _logicalMap.GetLength(0) && z < _logicalMap.GetLength(1)) 
        {
            return _logicalMap[x, z];
        }
        //Debug.LogWarning("Try to get Cell out of map");
        return null;
    }

    public List<Cell> GetCellsReacheable(Cell origin, int distance)
    {
        List<Cell> cells = new List<Cell>();
        if (distance > 0)
        {
            foreach (Cell cell in origin.adjencyList)
            {
                // Debug.Log(cell);
                if (cell != null && cell.walkable && cell.currentState == Cell.CellState.Idle)
                {
                    cells.Add(cell);
                    List<Cell> cellList = GetCellsReacheable(cell, distance - 1);
                    cells.AddRange(cellList);
                }
            }
        }
        return cells;
    }

    public Vector3 DirectionToVector3 (Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Vector3.forward;
            case Direction.South:
                return Vector3.back;
            case Direction.East:
                return Vector3.right;
            case Direction.West:
                return Vector3.left;
            default:
                Debug.LogError("Can't convert null Direction into Vector3");
                return Vector3.zero;
        }
    }

    public void ResetAllCells()
    {
        foreach (Cell cell in _logicalMap)
        {
            if (cell != null)
            {
                cell.Reset();
            }
        }
    }

    public void SetCellsSelectable(Cell origin, int distance)
    {
        selectableCells = GetCellsReacheable(origin, distance);

        foreach(Cell cell in selectableCells)
        {
            cell.SetState(Cell.CellState.isSelectable);
        }
    }

    public void ResetSelectableCells()
    {
        foreach(Cell cell in selectableCells)
        {
            if (cell != null && cell.currentState != Cell.CellState.isSelected)
            {
                cell.SetState(Cell.CellState.Idle);
            }
        }
        selectableCells.Clear();
    }

    public List<Cell> GetSightOfView(Direction direction,Cell origin, int range)
    {
        List<Cell> visibleCells = new List<Cell>();
        Cell lookingCell = GetAdjacentCell(direction, origin);
        if (lookingCell != null && lookingCell.seeThrough && range>0) 
        {
            visibleCells.Add(lookingCell);
            visibleCells.AddRange(RecursiveSightOfView(direction, lookingCell, range - 1));
            visibleCells = visibleCells.Distinct().ToList();
        }
        return visibleCells;
    }

    private List<Cell> RecursiveSightOfView(Direction direction, Cell origin, int range)
    {
        List<Cell> visibleCells = new List<Cell>();
        Cell looking = GetAdjacentCell(direction, origin);

        if (looking !=null && looking.seeThrough && range > 0)
        {
            visibleCells.Add(looking);
            Cell leftCell = GetAdjacentCell(GetDirectionSide(direction, Side.LEFT), origin);
            visibleCells.AddRange(RecursiveSightOfView(direction, leftCell, range - 1));

            Cell rightCell = GetAdjacentCell(GetDirectionSide(direction,Side.RIGHT), origin);
            visibleCells.AddRange(RecursiveSightOfView(direction, rightCell, range -1));

            visibleCells = visibleCells.Distinct().ToList();
        }
        return visibleCells;
    }

    private Direction GetDirectionSide(Direction direction, Side side)
    {
        switch(direction)
        {
            case Direction.North:
                if (side == Side.LEFT) return Direction.West;
                if (side == Side.RIGHT) return Direction.East;
                if (side == Side.BACK) return Direction.South;
                else
                {
                    Debug.LogError("Sides can't be null");
                    return Direction.North;
                }
            case Direction.South:
                if(side == Side.LEFT) return Direction.East;
                if (side==Side.RIGHT) return Direction.West;
                if (side == Side.BACK) return Direction.North;
                else
                {
                    Debug.LogError("Sides can't be null");
                    return Direction.North;
                }
            case Direction.West:
                if (side == Side.LEFT) return Direction.North;
                if (side == Side.RIGHT) return Direction.South;
                if (side == Side.BACK) return Direction.East;
                else
                {
                    Debug.LogError("Sides can't be null");
                    return Direction.North;
                }
            case Direction.East:
                if (side == Side.LEFT) return Direction.South;
                if (side == Side.RIGHT) return Direction.North;
                if (side == Side.BACK) return Direction.West;
                else
                {
                    Debug.LogError("Sides can't be null");
                    return Direction.North;
                }
            default:
                 Debug.LogError("null Direction can't have sides");
                 return Direction.North;
        }
    }

    public Cell GetAdjacentCell(Direction direction,Cell origin)
    {
        Vector3 adjacentCellPos = origin.transform.position + DirectionToVector3(direction);
        foreach (Cell cell in origin.adjencyList)
        {
            if (cell.transform.position == adjacentCellPos) return cell;
        }
        return null;
    }


    //ADD Liam

    public List<Cell> FindPath(Cell startCell, Cell targetCell)
    {
        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();
        Dictionary<Cell, float> gScore = new Dictionary<Cell, float>();
        Dictionary<Cell, float> fScore = new Dictionary<Cell, float>();

        openSet.Add(startCell);
        gScore[startCell] = 0;
        fScore[startCell] = HeuristicCostEstimate(startCell, targetCell);
        while (openSet.Count > 0)
        {
            Cell currentCell = GetLowestFScore(openSet, fScore);
            if (currentCell == targetCell)
            {
                return ReconstructPath(cameFrom, targetCell);
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            foreach (Cell neighbor in currentCell.adjencyList)
                // on vérifie si la case voisine est walkable
                if(neighbor.walkable == true) {
                    {
                        if (closedSet.Contains(neighbor))
                            continue;

                        float tentativeGScore = gScore[currentCell] + DistanceBetween(currentCell, neighbor);

                        if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                        {
                            cameFrom[neighbor] = currentCell;
                            gScore[neighbor] = tentativeGScore;
                            fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, targetCell);

                            if (!openSet.Contains(neighbor))
                                openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

 

    private Cell GetLowestFScore(List<Cell> openSet, Dictionary<Cell, float> fScore)
    {
        float lowestFScore = float.MaxValue;
        Cell lowestCell = null;

        foreach (Cell cell in openSet)
        {
            if (fScore.ContainsKey(cell) && fScore[cell] < lowestFScore)
            {
                lowestFScore = fScore[cell];
                lowestCell = cell;
            }
        }

        return lowestCell;
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell currentCell)
    {
        List<Cell> path = new List<Cell> { currentCell };

        while (cameFrom.ContainsKey(currentCell))
        {
            currentCell = cameFrom[currentCell];
            path.Insert(0, currentCell);
        }

        return path;
    }

    private float HeuristicCostEstimate(Cell startCell, Cell targetCell)
    {
        // Example: Manhattan distance heuristic
        return Mathf.Abs(startCell.gridCoordX - targetCell.gridCoordX) + Mathf.Abs(startCell.gridCoordZ - targetCell.gridCoordZ);
    }

    private float DistanceBetween(Cell cellA, Cell cellB)
    {
        // Example: Euclidean distance
        return Vector2.Distance(new Vector2(cellA.gridCoordX, cellA.gridCoordZ), new Vector2(cellB.gridCoordX, cellB.gridCoordZ));
    }
}
