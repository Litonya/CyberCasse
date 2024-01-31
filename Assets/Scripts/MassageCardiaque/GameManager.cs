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
        currentGameState = GameStates.Planification;
        _characterList = GetAllCharacters();
    }

    private void Update()
    {
        if (currentGameState == GameStates.Planification)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetClickObject();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                LaunchActionPhase();
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
        if(characterSelected != null && cell.occupant == null && !cell.isSelected)
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
    }

    private void Select(PlayerCharacter character)
    {
        characterSelected = character;
        Debug.Log(character.name + " is selected");
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
        foreach (Character character in _characterList) 
        {
            character.Acte();
        }
        LaunchPlanificationPhase();
    }

    private void LaunchPlanificationPhase()
    {
        currentGameState = GameStates.Planification;
    }
}
