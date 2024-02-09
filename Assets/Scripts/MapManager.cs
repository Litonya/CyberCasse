using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

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
        //R�cup�re toutes les cellules de la sc�ne
        GameObject[] cellList = GameObject.FindGameObjectsWithTag("Tile");

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
        //R�cup�re tous les characters de la sc�ne
        GameObject[] characterList = GameObject.FindGameObjectsWithTag("NPC");

        //Lie les character � leurs cellules
        foreach(GameObject character in characterList)
        {
            Character characterScript = character.GetComponent<Character>();
            if (characterScript != null)
            {
                int posX = (int)character.transform.position.x;
                int posZ = (int)character.transform.position.z;
                Debug.Log("jevekan�");
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
        Debug.LogWarning("Try to get Cell out of map");
        return null;
    }

    public List<Cell> GetCellsReacheable(Cell origin, int distance)
    {
        List<Cell> cells = new List<Cell>();
        if (distance > 0)
        {
            foreach (Cell cell in origin.adjencyList)
            {
                Debug.Log(cell);
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
                if(neighbor.walkable == true && neighbor.occupant == null) { 
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
