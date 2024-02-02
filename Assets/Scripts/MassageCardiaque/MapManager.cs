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
        //Récupére toutes les cellules de la scène
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
                Debug.Log("jevekané");
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
        Debug.Log(distance);
        List<Cell> cells = new List<Cell>();
        Debug.Log(origin);
        if (distance > 0)
        {
            foreach (Cell cell in origin.adjencyList)
            {
                Debug.Log(cell);
                if (cell != null && cell.walkable)
                {
                    cells.Add(cell);
                    List<Cell> cellList = GetCellsReacheable(cell, distance - 1);
                    cells.AddRange(cellList);
                }
            }
        }
        foreach (Cell cell in cells)
        {
            //Debug.Log(cell.name);
        }
        return cells;
    }

    public void ResetAllCells()
    {
        foreach (Cell cell in _logicalMap)
        {
            cell.Reset();
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
            cell.SetState(Cell.CellState.Idle);
        }
        selectableCells.Clear();
    }
}
