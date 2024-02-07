using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private PlayerCharacter characterSelected;

    private List<Character> _characterList;

    public static GameManager instance { get { return _instance; } }
    static GameManager _instance;

    enum GameStates { Planification, Action}
    private GameStates currentGameState;

    [SerializeField]
    private float _timePlanification = 10;

    private float _timeRemain;

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

        UIManager.instance.SetMaximumTime(_timePlanification);

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
            else if (characterSelected != null)
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
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<PlayerCharacter>())
                {
                    Debug.Log("A PlayerCharacter is clicked");
                    PlayerCharacter playerCharacter = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                    UnitSelect(playerCharacter);
                }else if (hit.collider.gameObject.GetComponent<Cell>())
                {
                    Cell cell = hit.collider.gameObject.GetComponent<Cell>();
                    CellSelect(cell);
                }
            }
        }


    }

    private void CellSelect(Cell cell)
    {
        if(characterSelected != null && cell.occupant == null && cell.currentState == Cell.CellState.isSelectable && characterSelected.path[characterSelected.path.Count-1] == cell)
        {
            characterSelected.TargetCell(cell);

            Unselect();
        }
    }

    private void UnitSelect(PlayerCharacter character)
    {
        if (characterSelected == character)
        {
            Unselect();
        }
        else
        {
            Unselect();
            Select(character);
        }
    }


    private void Unselect()
    {
        if (characterSelected != null)
        {
            Debug.Log(characterSelected.name + " unselected");
        }
        characterSelected = null;
        MapManager.instance.ResetSelectableCells();
    }

    private void Select(PlayerCharacter character)
    {
        characterSelected = character;
        Debug.Log(character.name + " is selected");

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

    private void LaunchActionPhase()
    {
        Debug.Log("Launch Action phase");
        currentGameState = GameStates.Action;
        Unselect();
        foreach (Character character in _characterList) 
        {
            character.Acte();
        }
        UIManager.instance.SetUIActionPhase();
    }

    private void LaunchPlanificationPhase()
    {
        MapManager.instance.ResetAllCells();
        ResetAllCharacter();
        currentGameState = GameStates.Planification;
        UIManager.instance.SetUIPlanificationPhase();
        _timeRemain = _timePlanification;
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
            character.path = MapManager.instance.FindPath(character.GetCurrentCell(), cell);
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

    private void ResetAllCharacter()
    {
        foreach (Character character in _characterList)
        {
            character.Reset();
        }
    }
}
