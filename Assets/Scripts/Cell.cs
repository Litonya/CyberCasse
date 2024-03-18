using JetBrains.Annotations;
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

    private Item _placeItem = null;

    [SerializeField] private float itemOffset = 0f;

    public enum CellState { Idle, isSelectable, isSelected, actionTarget }
    [HideInInspector]
    public CellState currentState = CellState.Idle;

    public List<Cell> adjencyList = new List<Cell>();

    protected UIManager uiManager;

    public bool isDoor = false;

    public List<Actions> possibleActions = new List<Actions>();

    [SerializeField] private int _diffuculty = 0;
    public int remainDifficulty;

    private List<CellAction> _cellActions = new List<CellAction>();
    private GrabItem _grabItemAction = null;

    public KeyColor neededKey;

    public List<Cell> linkCell = new List<Cell>();

    private Character _characterTarget = null;

    private Renderer _renderer;

    public void MarkPath()
    {
        _pathMarker.SetActive(true);
    }

    public void UnmarkPath()
    {
        _pathMarker.SetActive(false);
    }

    private void Awake()
    {
        InitializeActions();
        remainDifficulty = _diffuculty;
        _renderer = GetComponent<Renderer>();
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
        if (viewBy.Count != 0 && occupant != null)
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

    public PlayerCharacter DemandingCheckForPlayer()
    {
        if (occupant != null)
        {
            Debug.Log("Joueur trouvé!");
            return occupant.GetComponent<PlayerCharacter>();
        }
        return null;
    }

    public void Reset()
    {
        isVisited = false;
        UnmarkPath();
        SetState(CellState.Idle);
        _characterTarget = null;
    }

    public void ResetDifficulty()
    {
        remainDifficulty = _diffuculty;
    }

    public void VisibleBy(EnemyFOV enemy)
    {
        viewBy.Add(enemy);
        RefreshStateVisual();
        CheckForPlayer();
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
        if (currentState == CellState.Idle && viewBy.Count == 0)
        {
            SetIdleVisual();
        }
        else if (currentState == CellState.isSelectable)
        {
            SetIsVisibleVisual();
        }
        else if (currentState == CellState.isSelected)
        {
            SetIsSelectedVisual();
        }
        else if (currentState == CellState.actionTarget)
        {
            SetActionTargetVisual();
        }
        else if (viewBy.Count != 0)
        {
            SetVisibleVisual();
        }
    }

    private void SetIdleVisual()
    {
        _renderer.material.color = Color.white;
    }

    private void SetIsVisibleVisual()
    {
        _renderer.material.color = Color.magenta;
    }

    private void SetIsSelectedVisual()
    {
        _renderer.material.color = Color.blue;
    }
    private void SetVisibleVisual()
    {
        _renderer.material.color = Color.red;
    }

    private void SetActionTargetVisual()
    {
        _renderer.material.color = Color.grey;
    }


    public void SetState(CellState state)
    {
        currentState = state;

        RefreshStateVisual();
    }

    private void InitializeActions()
    {
        if (possibleActions.Contains(Actions.LOCKPICK))
        {
            LockPick lockPick = gameObject.AddComponent<LockPick>();
            _cellActions.Add(lockPick);
        }
        if (possibleActions.Contains(Actions.UNLOCK))
        {
            UnlockDoor unlockDoor = gameObject.AddComponent<UnlockDoor>();
            _cellActions.Add(unlockDoor);
        }
        if (possibleActions.Contains(Actions.BREAKGLASS))
        {
            BreakGlass breakGlass = gameObject.AddComponent<BreakGlass>();
            _cellActions.Add(breakGlass);
        }
        if (possibleActions.Contains(Actions.HACK))
        {
            Hack hack = gameObject.AddComponent<Hack>();
            _cellActions.Add(hack);
        }
    }

    public void SetWalkable()
    {
        walkable = true;
    }

    public void SetUnwalkable()
    {
        walkable = false;
    }

    public bool Acte(Actions action, int characterStat, PlayerCharacter character)
    {
        foreach (CellAction cellAction in _cellActions)
        {
            if (cellAction.action == action)
            {
                return cellAction.Acte(characterStat, character);
            }
        }

        Debug.LogError("No action \""+action+"\" find for cell " + gameObject.name);
        return true;
    }

    public void PlaceItem(Item item)
    {
        if (_placeItem != null)
        {
            Debug.LogError("This cell already have an item");
        }
        _placeItem = item;
        item.gameObject.SetActive(true);
        item.transform.position = transform.position + new Vector3(0, itemOffset, 0);
        if (_grabItemAction == null)
        {
            GrabItemActionInit();
        }

    }

    private void GrabItemActionInit()
    {
        _grabItemAction = gameObject.AddComponent<GrabItem>();
        _cellActions.Add(_grabItemAction);
        possibleActions.Add(Actions.GETITEM);
    }

    public void RemoveItem()
    {
        _placeItem.gameObject.SetActive(false);
        _placeItem = null;
        RemoveGrabItemAction();
    }

    private void RemoveGrabItemAction()
    {
        _cellActions.Remove(_grabItemAction);
        possibleActions.Remove(Actions.GETITEM);
        Destroy(_grabItemAction);
    }

    public Item GetItem()
    {
        return _placeItem;
    }
}
