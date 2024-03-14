using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public enum GameStates { Planification, Action }
public class GameManager : MonoBehaviour
{

    private PlayerCharacter characterSelected;

    private List<AvailibleActionsOnAdjacentCells> availibleActions;

    private Cell cellSelected;
    private Actions _actionSelected;

    private List<Character> _characterList;

    public List<SecurityCamera> securityCameraList;

    public int moneyScore = 0;

    [SerializeField] private int _moneyMalus = -300;

    public static GameManager instance { get { return _instance; } }
    static GameManager _instance;


    private GameStates currentGameState;

    private SelectionState _currentSelectionState;

    [SerializeField]
    private float _timePlanification = 10;

    [SerializeField]
    private int _knockoutLimit = 2;

    private float _timeRemain;

    //Victoire
    public int totalPlayers = 0; // Nombre total de joueurs dans la scène

    private int playersInVictoryZone = 0; // Nombre de joueurs dans la zone de victoire

    private struct AvailibleActionsOnAdjacentCells
    {
        public Cell cell;

        public List<Actions> availibleActions;
    }

    enum SelectionState
    {
        SELECT_CHARACTER,
        SELECT_DESTINATION,
        SELECT_ACTION,
        SELECT_ACTION_TARGET
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    private void Start()
    {
        _characterList = GetAllCharacters();
        securityCameraList = GetAllSecurityCameras();
        UIManager.instance.SetMaximumTime(_timePlanification);
        GetAllPlayerCharacters();
        LaunchPlanificationPhase();
    }

    private void Update()
    {
        if (currentGameState == GameStates.Planification)
        {
            /*---------------------------PLANIFICATION---------------------------------*/
            //V�rifie de le temps r�stant pour la phase de Planification
            if (_timeRemain > 0)
            {
                _timeRemain -= Time.deltaTime;
                UIManager.instance.UpdateTimeBar(_timeRemain);
            }
            else
            {
                LaunchActionPhase();
            }

            //V�rifie les Inputs
            //Input clic gauche -> Selection d'objet
            if (Input.GetMouseButtonDown(0))
            {
                GetClickObject();
            }
            //Input echap -> Fin de la phase
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                LaunchActionPhase();
            }
            //Input emplacement souris -> Si un personnage est selectionn�, r�cup�re les cellules s�lectionn�es survol�es par la souris
            else if (characterSelected != null && _currentSelectionState == SelectionState.SELECT_DESTINATION)
            {
                Cell cell = GetTargetCell();
                if (cell != null)
                {
                    AddToPath(characterSelected, cell);
                }
            }
        }

        /*-----------------------------------ACTION----------------------------*/
        else if (currentGameState == GameStates.Action)
        {
            bool allActionComplete = true;
            foreach (Character character in _characterList)
            {
                if (character.currentAct)
                {
                    allActionComplete = false;
                }
            }
            if (allActionComplete)
            {
                LaunchPlanificationPhase();
            }
        }
    }


    private void GetClickObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,Mathf.Infinity,Physics.DefaultRaycastLayers))
        {
            if(hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<PlayerCharacter>())
                {
                    Debug.Log("A PlayerCharacter is clicked");
                    PlayerCharacter playerCharacter = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                    UnitSelect(playerCharacter);
                } else if (hit.collider.gameObject.GetComponent<Cell>())
                {
                    Cell cell = hit.collider.gameObject.GetComponent<Cell>();
                    CellSelect(cell);
                }
            }
        }
    }

    private void CellSelect(Cell cell)
    {
        if (characterSelected != null && cell.occupant == null && cell.currentState == Cell.CellState.isSelectable && characterSelected.path[characterSelected.path.Count - 1] == cell && _currentSelectionState == SelectionState.SELECT_DESTINATION)
        {
            //characterSelected.TargetCell(cell);
            cellSelected = cell;
            availibleActions = GetAvailibleActions(cell);
            UIManager.instance.SetSelectedCell(cell, GetAllAvailibleActions(availibleActions));
            UIManager.instance.SetUIActionMenuON();
            MapManager.instance.ResetSelectableCells();
            _currentSelectionState = SelectionState.SELECT_ACTION;

        }else if (_currentSelectionState == SelectionState.SELECT_ACTION_TARGET && cell.currentState == Cell.CellState.isSelectable)
        {
            TargetActionSelected(cell);
        }
    }

    private void TargetActionSelected(Cell targetCell)
    {
        characterSelected.TargetCell(cellSelected);
        characterSelected.SetPreparedAction(_actionSelected, targetCell);
        Unselect();
    }

    public void ActionSelect(Actions action)
    {
        if (action == Actions.MOVE)
        {
            characterSelected.TargetCell(cellSelected);
            characterSelected.ClearPreparedAction();
            Unselect();
            return;
        }

        _actionSelected = action;
        UIManager.instance.SetUIActionMenuOFF();
        List<Cell> selectableCell = new List<Cell>();
        foreach (AvailibleActionsOnAdjacentCells actionCell in availibleActions)
        {
            if (actionCell.availibleActions.Contains(action))
            {
                selectableCell.Add(actionCell.cell);
            }
        }
        MapManager.instance.SetPreciseSelectableCells(selectableCell);
        _currentSelectionState = SelectionState.SELECT_ACTION_TARGET;
    }

    private void UnitSelect(PlayerCharacter character)
    {
        if (characterSelected == character)
        {
            Unselect();
            _currentSelectionState = SelectionState.SELECT_CHARACTER;
        }
        else
        {
            Unselect();
            Select(character);
            _currentSelectionState = SelectionState.SELECT_DESTINATION;
        }
    }


    private void Unselect()
    {
        if (characterSelected != null)
        {
            // Debug.Log(characterSelected.name + " unselected");
        }
        characterSelected = null;
        cellSelected = null;
        _actionSelected = Actions.NONE;
        UIManager.instance.SetUIActionMenuOFF();
        MapManager.instance.ResetSelectableCells();
    }

    private void Select(PlayerCharacter character)
    {
        characterSelected = character;
      // Debug.Log(character.name + " is selected");

        MapManager.instance.SetCellsSelectable(character.GetCurrentCell(), character.movePoints);

        character.path.Add(character.GetCurrentCell());
    }

    private List<Character> GetAllCharacters()
    {
        GameObject[] gameobjectList = GameObject.FindGameObjectsWithTag("NPC");
        List<Character> list = new List<Character>();
        foreach(GameObject gameObject in gameobjectList)
        {
            Character characScript = gameObject.GetComponent<Character>();
            if (characScript != null)
            {
                list.Add(characScript);
            }
        }
        return list;
    }

    private List<SecurityCamera> GetAllSecurityCameras()
    {
        List<SecurityCamera> securityCams = new List<SecurityCamera>();
        foreach(Character character in _characterList)
        {
            SecurityCamera securityCameraScript = character.GetComponent<SecurityCamera>();
            if (securityCameraScript != null)
            {
                securityCams.Add(securityCameraScript);
            }
        }
        return securityCams;
    }

    private void LaunchActionPhase()
    {
        EndPlanificationPhase();
       // Debug.Log("Launch Action phase");
        currentGameState = GameStates.Action;
        Unselect();
        foreach (Character character in _characterList) 
        {
            character.Acte();
        }
        UIManager.instance.SetUIActionPhase();
    }

    private void EndActionPhase()
    {
        UIManager.instance.SetUIActionMenuOFF();
    }

    private void LaunchPlanificationPhase()
    {
        EndActionPhase();
        MapManager.instance.ResetAllCells();
        ResetAllCharacter();
        currentGameState = GameStates.Planification;
        _currentSelectionState = SelectionState.SELECT_CHARACTER;
        UIManager.instance.SetUIPlanificationPhase();
        _timeRemain = _timePlanification;
    }

    private void EndPlanificationPhase()
    {

    }

    private void AddToPath(Character character, Cell cell)
    {
        if (character.path.Count == 0)
        {
            character.path.Add(cell);
            cell.MarkPath();
        }
        else if (cell.walkable && cell.currentState == Cell.CellState.isSelectable && !character.path.Contains(cell) && character.path[character.path.Count - 1].adjencyList.Contains(cell))
        {
            character.path.Add(cell);
            cell.MarkPath();
        }
        if (character.path.Count>character.movePoints + 1)
        {
            foreach(Cell markCell in character.path)
            {
                markCell.UnmarkPath();
            }
            character.path = MapManager.instance.FindPath(character.GetCurrentCell(), cell, false) ;
            foreach (Cell toMarkCell in character.path)
            {
                toMarkCell.MarkPath();
            }
        }
    }

    private Cell GetTargetCell()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.gameObject.GetComponent<Cell>())
        {
            return hit.collider.gameObject.GetComponent<Cell>();
        }
        return null;
    }

    public GameStates GetGameState()
    {
        return currentGameState;
    }

    private void ResetAllCharacter()
    {
        foreach (Character character in _characterList)
        {
            character.Reset();
        }
    }
    /////////////////////////////////////////////////////////////////
    /// GESTION DE LA VICTOIRE////
    /////////////////////////////////////////////////

    private void GetAllPlayerCharacters()
    {
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
        totalPlayers = players.Length;
    }

    public void TriggerVictory()
    {
        // Vérifier si toutes les conditions de victoire sont remplies
        if (VictoryConditionsMet())
        {
            // Afficher l'écran de victoire ou effectuer d'autres actions de victoire
            Debug.Log("Victory!");

            UIManager.instance.ShowVictory();

            // Exemple : Charger l'écran de victoire
            // SceneManager.LoadScene("VictoryScreen");
        }
        else
        {
            Debug.Log("Victory conditions not met.");
        }
    }

    private void OnCanvasGroupChanged()
    {
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
        totalPlayers = players.Length;
    }

    private bool VictoryConditionsMet()
    {

        // Vérifier si tous les PlayerCharacters sont dans la zone de victoire
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
        bool allPlayersInZone = true;
        foreach (PlayerCharacter player in players)
        {
            if (!player.IsInVictoryZone())
            {
                Debug.Log(player+ "n'est pas dans la zone de victoire");
                allPlayersInZone = false;
                break;
            }
        }

        // Vérifier si au moins un PlayerCharacter a attaché un objet de type WinCondition
        bool atLeastOnePlayerHasWinCondition = false;
        foreach (PlayerCharacter player in players)
        {
            if (player.GetCarriedItem() != null && player.GetCarriedItem().GetComponent<Objective>())
            {
                Debug.Log("Un joueur a bien la winCond");
                atLeastOnePlayerHasWinCondition = true;
                break;
            }
        }
        Debug.Log("allPlayerInZone = " + allPlayersInZone);
        Debug.Log("atLeastOnePlayerHasWinCondition = " + atLeastOnePlayerHasWinCondition);
        // Conditions de victoire remplies si tous les joueurs sont dans la zone ET au moins un a la winCondition attachée
        return allPlayersInZone && atLeastOnePlayerHasWinCondition;
    }




    //Gestion actions
    private List<AvailibleActionsOnAdjacentCells> GetAvailibleActions(Cell originCell)
    {
        List<AvailibleActionsOnAdjacentCells> availibleActions = new List<AvailibleActionsOnAdjacentCells>();

        foreach (Cell cell in originCell.adjencyList)
        {
            AvailibleActionsOnAdjacentCells currentCheckCell = new AvailibleActionsOnAdjacentCells();

            currentCheckCell.cell = cell;
            currentCheckCell.availibleActions = cell.possibleActions;

            if (cell.occupant != null && cell.occupant.GetComponent<EnemyCharacter>() && _knockoutLimit>0)
            {
                currentCheckCell.availibleActions.Add(Actions.KNOCKOUT);
            }

            availibleActions.Add(currentCheckCell);
        }

        return availibleActions;
    }

    private List<Actions> GetAllAvailibleActions(List<AvailibleActionsOnAdjacentCells> availibleActionsOnAdjacentCells)
    {
        List<Actions> actions = new List<Actions>();

        foreach (AvailibleActionsOnAdjacentCells actionLinkToCell in availibleActionsOnAdjacentCells)
        {
            foreach (Actions currentCellAction in  actionLinkToCell.availibleActions)
            {
                if (!actions.Contains(currentCellAction)) actions.Add(currentCellAction);
            }
        }

        return actions;
    }

    public void PlayerCaught(PlayerCharacter character)
    {
        _characterList.Remove(character);
        UpdateMoneyScore(_moneyMalus);
        if (GetAllCharacters().Count == 0) 
        {
            Debug.Log("TAPERDULOLOLOLOLOLOLOL");
        }
    }

    public void UpdateMoneyScore(int moneyGain)
    {
        moneyScore += moneyGain;
        //Add update UI score
    }

}
