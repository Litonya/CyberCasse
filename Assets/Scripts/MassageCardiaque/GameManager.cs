using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private PlayerCharacter characterSelected;

    public static GameManager instance { get { return _instance; } }
    static GameManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }


    public void UnitSelect(PlayerCharacter character)
    {
        if (characterSelected == character)
        {
            Unselect();
        }
        else
        {
            Select(character);
        }
    }


    private void Unselect()
    {
        characterSelected = null;
        Debug.Log("Unit unselect");
    }

    private void Select(PlayerCharacter character)
    {
        characterSelected = character;
        Debug.Log(character.name + " is selected");
    }
}
