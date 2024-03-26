using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameStates { Preparation,Planification, Action }
public class GameManager : MonoBehaviour
{

    private PlayerCharacter characterSelected;

    private List<AvailibleActionsOnAdjacentCells> availibleActions;

    private Cell cellSelected;
    private Actions _actionSelected;

    private List<Character> _characterList;

    public List<SecurityCamera> securityCameraList;

    private List<EnemyCharacter> _guardList;

    private List<PlayerCharacter> _playerCharacterList;

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

    [SerializeField] private List<SecondPhasePatrols> _secondePhasePatrols = new List<SecondPhasePatrols>();

    private int _alertLevel = 0;

    public int maxAlertLevel = 2;

    [SerializeField] private int _guardPatrolMovePointsIncrease = 2;
    [SerializeField] private int _guardChassingMovePointsIncrease = 2;
    [SerializeField] private int _guardFOVRangeIncrease = 1;
    [SerializeField] private float _timeReducePlanificationTime = 5f;

    private bool isPaused = false;

    private List<Cell> _potentialPath;

    private struct AvailibleActionsOnAdjacentCells
    {
        public Cell cell;

        public List<Actions> availibleActions;
    }

    [Serializable]
    public struct SecondPhasePatrols
    {
        public EnemyCharacter enemy;

        public bool isSentinel;

        public List<Cell> patrolCells;

        public bool looping;
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

        _potentialPath = new List<Cell>();

        _instance = this;
    }

    private void Start()
    {
        _characterList = GetAllCharacters();
        securityCameraList = GetAllSecurityCameras();
        _guardList = GetAllEnemyCharacter();
        _playerCharacterList = GetAllPlayerCharacter();
        InitGame();
    }

    private void Update()
    {
        if (currentGameState == GameStates.Preparation)
        {
            if (Input.GetKeyDown(KeyCode.Space)) LaunchPlanificationPhase();
            return;
        }

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
            //Input clic droit -> Annulation action
            if (Input.GetMouseButtonDown(1))
            {
                GetRightClickObject();
            }
            //Input espace -> Fin de la phase
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchActionPhase();
            }
            //Input emplacement souris -> Si un personnage est selectionn�, r�cup�re les cellules s�lectionn�es survol�es par la souris
            else if (characterSelected != null && _currentSelectionState == SelectionState.SELECT_DESTINATION)
            {
                Cell cell = GetTargetCell();
                if (cell != null)
                {
                    ChangePotentialPath(characterSelected, cell);
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                // Inverser l'état de pause
                TogglePause();
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

    private void InitGame()
    {

        UIManager.instance.SetMaximumTime(_timePlanification);
        UIManager.instance.SetUIAlertLevel();
        GetAllPlayerCharacters();
        currentGameState = GameStates.Preparation;
    }

    private void GetRightClickObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<PlayerCharacter>() && _currentSelectionState == SelectionState.SELECT_CHARACTER)
                {
                    PlayerCharacter characterScript = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                    characterScript.ClearPreparedAction();
                    characterScript.Reset();
                }
                else if (_currentSelectionState == SelectionState.SELECT_DESTINATION)
                {
                    Unselect();
                }
                else if (_currentSelectionState == SelectionState.SELECT_ACTION)
                {
                    UIManager.instance.SetUIActionMenuOFF();
                    Unselect();
                }
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
                    //Debug.Log("A PlayerCharacter is clicked");
                    EventsManager.instance.RaiseSFXEvent(SFX_Name.SELECTION);
                    PlayerCharacter playerCharacter = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                    UnitSelect(playerCharacter);
                } else if (hit.collider.gameObject.GetComponent<Cell>())
                {
                    EventsManager.instance.RaiseSFXEvent(SFX_Name.SELECTION);
                    Cell cell = hit.collider.gameObject.GetComponent<Cell>();
                    CellSelect(cell);
                }
            }
        }
    }

    private void CellSelect(Cell cell)
    {
        if (characterSelected != null && cell.occupant == null && cell.currentState == Cell.CellState.isSelectable &&  _currentSelectionState == SelectionState.SELECT_DESTINATION)
        {
            cellSelected = cell;
            availibleActions = GetAvailibleActions(cell);
            UIManager.instance.SetSelectedCell(cell, GetAllAvailibleActions(availibleActions));
            UIManager.instance.SetUIActionMenuON();
            MapManager.instance.ResetSelectableCells();
            _currentSelectionState = SelectionState.SELECT_ACTION;
            foreach (Cell cellPath in characterSelected.path)
            {
                cellPath.UnmarkPath();
            }

        }
        else if (_currentSelectionState == SelectionState.SELECT_ACTION_TARGET && cell.currentState == Cell.CellState.isSelectable)
        {
            Unselect();
        }
    }

    private void TargetActionSelected(Cell targetCell)
    {
        characterSelected.TargetCell(cellSelected);
        characterSelected.path = _potentialPath;
        characterSelected.SetPreparedAction(_actionSelected, targetCell);
        Unselect();
    }

    public void ActionSelect(Actions action)
    {
        if (action == Actions.MOVE)
        {
            characterSelected.path = _potentialPath;
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

        if (selectableCell.Count == 1)
        {
            TargetActionSelected(selectableCell[0]);
            return;
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

    private void SimpleUnselect()
    {
        characterSelected = null;
        cellSelected = null;
        _actionSelected = Actions.NONE;
        UIManager.instance.SetUIActionMenuOFF();
        MapManager.instance.ResetSelectableCells();
    }

    private void Unselect()
    {
        characterSelected = null;
        cellSelected = null;
        _actionSelected = Actions.NONE;
        _currentSelectionState = SelectionState.SELECT_CHARACTER;
        foreach (Cell cell in _potentialPath) if (cell != null) cell.UnmarkPath();
        UIManager.instance.SetUIActionMenuOFF();
        MapManager.instance.ResetSelectableCells();
    }

    private void Select(PlayerCharacter character)
    {
        characterSelected = character;

        _potentialPath.Add(character.GetCurrentCell());
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

    private List<EnemyCharacter> GetAllEnemyCharacter() 
    {
        List<EnemyCharacter> enemyCharacters = new List<EnemyCharacter>();
        foreach(Character character in _characterList)
        {
            EnemyCharacter enemyCharacterScript = character.GetComponent<EnemyCharacter>();
            if (enemyCharacterScript != null)
            {
                enemyCharacters.Add(enemyCharacterScript);
            }
        }
        return enemyCharacters;
    }

    private List<PlayerCharacter> GetAllPlayerCharacter()
    {
        List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();
        foreach (Character character in _characterList)
        {
            PlayerCharacter playerCharacterScript = character.GetComponent<PlayerCharacter>();
            if (playerCharacterScript != null)
            {
                playerCharacters.Add(playerCharacterScript);
            }
        }
        return playerCharacters;
    }

    private void LaunchActionPhase()
    {
        EndPlanificationPhase();
        EventsManager.instance.RaiseSFXEvent(SFX_Name.ACTION);
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
        foreach (EnemyCharacter enemyCharacter in _guardList) enemyCharacter.EndActionPhase();

        foreach(PlayerCharacter character in _playerCharacterList)
        {
            if (character.GetCurrentCell().occupant == null) character.GetCurrentCell().SetOccupant(character);
        }
    }

    private void LaunchPlanificationPhase()
    {
        EndActionPhase();
        _timeRemain = _timePlanification;
        MapManager.instance.ResetAllCells();
        ResetAllCharacter();
        currentGameState = GameStates.Planification;
        _currentSelectionState = SelectionState.SELECT_CHARACTER;
        UIManager.instance.SetUIPlanificationPhase();
        
    }

    private void EndPlanificationPhase()
    {
        MapManager.instance.UnmarkAllCells();
        UIManager.instance.SetUIActionMenuOFF();
    }

    private void AddToPath(Character character, Cell cell)
    {
        /*if (character.path.Count == 0)
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
        {*/
        if (cell.currentState != Cell.CellState.isSelectable) return;
        foreach (Cell markCell in character.path)
        {
            if (markCell != null) markCell.UnmarkPath();
        }
        character.path = MapManager.instance.FindPath(character.GetCurrentCell(), cell, false);
        foreach (Cell toMarkCell in character.path)
        {
            toMarkCell.MarkPath();
        }
        //}
    }

    private void ChangePotentialPath(Character character, Cell cell)
    {
        if (cell.currentState != Cell.CellState.isSelectable) return;
        foreach (Cell markCell in _potentialPath)
        {
            if (markCell != null) markCell.UnmarkPath();
        }
        _potentialPath = MapManager.instance.FindPath(character.GetCurrentCell(), cell, false);
        foreach (Cell toMarkCell in _potentialPath)
        {
            toMarkCell.MarkPath();
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
            if(isPaused) { TogglePause(); }
            Time.timeScale = 0f;

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
        _playerCharacterList.Remove(character);
        UpdateMoneyScore(_moneyMalus);
        if (_playerCharacterList.Count == 0) 
        {
            if(isPaused) { TogglePause(); }
            UIManager.instance.ShowLoose();
            Time.timeScale = 0;
            Debug.Log("TAPERDULOLOLOLOLOLOLOL");
        }
    }

    public void UpdateMoneyScore(int moneyGain)
    {
        moneyScore += moneyGain;
        //Add update UI score
    }


    public void LaunchPartTwo()
    {
        foreach (SecondPhasePatrols newPatrol in _secondePhasePatrols)
        {
            newPatrol.enemy.SetSentinel(newPatrol.isSentinel);
            newPatrol.enemy.SetPatrolTarget(newPatrol.patrolCells);
            newPatrol.enemy.loopingPatrol = newPatrol.looping;
        }
    }

    public void IncreaseAlertLevel()
    {
        if (_alertLevel == maxAlertLevel) return;
        _alertLevel++;
        _timePlanification -= _timeReducePlanificationTime;
        UIManager.instance.SetUIAlertLevel();
        foreach (EnemyCharacter enemyCharacter in _guardList)
        {
            enemyCharacter.IncreaseMovePatrolAndChase(_guardPatrolMovePointsIncrease, _guardChassingMovePointsIncrease);
            enemyCharacter.IncreaseVisionRange(_guardFOVRangeIncrease);
        }
        if (_alertLevel == maxAlertLevel)
        {
            foreach(EnemyCharacter enemyCharacter2 in _guardList)
            {
                enemyCharacter2.LaunchGeneralAlert();
            }
            foreach(SecurityCamera camera in securityCameraList)
            {
                camera.LaunchGeneralAlert();
            }
        }
    }

    public PlayerCharacter GetClosestPlayer(Character character)
    {
        int minDistance = int.MaxValue;
        PlayerCharacter closestPlayer = null;

        foreach (PlayerCharacter playerCharacter in _playerCharacterList)
        {
            List<Cell> path = MapManager.instance.FindPath(character.GetCurrentCell(), playerCharacter.GetCurrentCell(), true);
            if (path.Count < minDistance && !playerCharacter.IsCaught())
            {
                minDistance = path.Count;
                closestPlayer = playerCharacter;
            }
        }
        return closestPlayer;
    }

    public int GetAlertLevel() { return _alertLevel; }

    public void TogglePause()
    {
        // Inverser l'état de pause
        isPaused = !isPaused;

        // Mettre en pause ou reprendre le jeu en fonction de l'état de pause
        if (isPaused)
        {
            Time.timeScale = 0f; // Mettre le temps à zéro pour mettre en pause le jeu
        }
        else
        {
            Time.timeScale = 1f; // Remettre le temps à sa valeur normale pour reprendre le jeu
        }
        UIManager.instance.PauseMenu();
    }

    public void ResetScene()
    {
        if (isPaused) TogglePause();
        // Récupérer le numéro de la scène actuelle
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Recharger la scène actuelle
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ReturnToTitleScreen()
    {
        if(isPaused) TogglePause();
        // Charger la scène de l'écran titre 
        SceneManager.LoadScene("TitleScreen"); 
    }

    public GameStates GetCurrentPhase()
    {
        return currentGameState;
    }
}
