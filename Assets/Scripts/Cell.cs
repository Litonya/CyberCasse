using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField]
    protected GameObject _pathMarker;

    public int gridCoordX;
    public int gridCoordZ;

    public Character occupant;

    public bool walkable;
    public bool seeThrough = true;

    public bool isVisited = false;

    private List<EnemyFOV> viewBy = new List<EnemyFOV>();

    public enum CellState { Idle, isSelectable, isSelected }
    [HideInInspector]
    public CellState currentState = CellState.Idle;

    public List<Cell> adjencyList = new List<Cell>();

    protected UIManager uiManager;

    public bool isDoor = false;

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
        if (MapManager.instance.GetCell(gridCoordX + 1, gridCoordZ) != null) adjencyList.Add(MapManager.instance.GetCell(gridCoordX + 1, gridCoordZ));
        if (MapManager.instance.GetCell(gridCoordX - 1, gridCoordZ) != null) adjencyList.Add(MapManager.instance.GetCell(gridCoordX - 1, gridCoordZ));
        if (MapManager.instance.GetCell(gridCoordX, gridCoordZ + 1) != null) adjencyList.Add(MapManager.instance.GetCell(gridCoordX, gridCoordZ + 1));
        if (MapManager.instance.GetCell(gridCoordX, gridCoordZ - 1) != null) adjencyList.Add(MapManager.instance.GetCell(gridCoordX, gridCoordZ - 1));

        uiManager = FindObjectOfType<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("UI Manager non trouvé dans la scène !");
        }

        Vector3 cellPosition = new Vector3(gridCoordX, 0.5f, gridCoordZ);
    }

    public void SetOccupant(Character character)
    {
        occupant = character;
        CheckForPlayer();
        
    }

    public void RemoveOccupant()
    {
        occupant = null;
    }

    private void CheckForPlayer()
    {
        if (viewBy.Count != 0)
        {
            PlayerCharacter playerCharater = occupant.GetComponent<PlayerCharacter>();
            if (playerCharater != null)
            {
                foreach (EnemyFOV enemy in viewBy)
                {
                    enemy.PlayerDetected(playerCharater);
                }
            }
        }
    }

    public void Reset()
    {
        isVisited = false;
        UnmarkPath();
        SetState(CellState.Idle);
    }

    public void VisibleBy(EnemyFOV enemy)
    {
        viewBy.Add(enemy);
        RefreshStateVisual();
    }

    public void OutOfView(EnemyFOV enemy)
    {
        if (viewBy.Remove(enemy) && viewBy.Count == 0)
        {
            RefreshStateVisual();
        }
    }

    public void RefreshStateVisual()
    {
        Renderer cellMat = this.GetComponent<Renderer>();

        if (currentState == CellState.Idle && viewBy.Count == 0)
        {
            cellMat.material.color = Color.white;
        }
        else if (currentState == CellState.isSelectable)
        {
            cellMat.material.color = Color.magenta;
        }
        else if (currentState == CellState.isSelected)
        {
            cellMat.material.color = Color.blue;
        }
        else if (viewBy.Count != 0)
        {
            cellMat.material.color = Color.red;
        }
    }

    public void SetState(CellState state)
    {
        currentState = state;

        RefreshStateVisual();
    }
}
