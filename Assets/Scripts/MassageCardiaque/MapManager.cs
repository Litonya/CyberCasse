using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get { return _instance; } }
    static MapManager _instance;


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
    }

    private void Start()
    {
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

                GetCell(posX, posZ).occupant = characterScript;
            }
        }
    }



    private Cell GetCell(int x, int z)
    {
        return _logicalMap[x, z];
    }
}
