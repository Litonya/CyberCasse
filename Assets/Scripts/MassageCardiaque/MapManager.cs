using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private Cell[,] _logicalMap;

    [SerializeField]
    private int _mapXSize;
    [SerializeField]
    private int _mapZSize;

    private void Awake()
    {
        _logicalMap = new Cell[_mapXSize,_mapZSize];
    }

    private void Start()
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

                _logicalMap[cellX,cellZ] = cellScript;
            }


            //Debugage de l'initialisation de la map
            for (int i = 0; i < _logicalMap.GetLength(0); i++) 
            {
                for (int j = 0; j < _logicalMap.GetLength(1); j++)
                {
                    if (_logicalMap[i, j] != null)
                    {
                        //Debug.Log("[" + i + "," + j + "] coords: " + _logicalMap[i, j].transform.position);
                    }
                }
            }
        }
    }
}
