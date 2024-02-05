using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private GameObject _pathMarker;

    public int gridCoordX;
    public int gridCoordZ;

    public Character occupant;

    public bool walkable;

    public bool isVisited = false;

    public enum CellState { Idle, isSelectable, isSelected }
    public CellState currentState;

    public List<Cell> adjencyList = new List<Cell>();

    private void Awake()
    {
        SetState(CellState.Idle);
    }

    public void MarkPath()
    {
        _pathMarker.SetActive(true);
    }

    public void UnmarkPath()
    {
        _pathMarker.SetActive(false);
    }

    private void Start()
    {
        adjencyList.Add(MapManager.instance.GetCell(gridCoordX + 1, gridCoordZ));
        adjencyList.Add(MapManager.instance.GetCell(gridCoordX - 1, gridCoordZ));
        adjencyList.Add(MapManager.instance.GetCell(gridCoordX, gridCoordZ + 1));
        adjencyList.Add(MapManager.instance.GetCell(gridCoordX, gridCoordZ - 1));
    }

    public void Reset()
    {
        isVisited = false;
        UnmarkPath();
        SetState(CellState.Idle);
    }

    public void SetState(CellState state)
    {
        currentState = state;

        Renderer cellMat = this.GetComponent<Renderer>();

        if (currentState == CellState.Idle)
        {
            cellMat.material.color = Color.white;
        }
        else if(currentState == CellState.isSelectable)
        {
            cellMat.material.color= Color.magenta;
        }
        else if (currentState == CellState.isSelected)
        {
            cellMat.material.color = Color.blue;
        }
    }
}
