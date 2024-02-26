using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get { return _instance; } }
    static UIManager _instance;

    [SerializeField]
    private TextMeshProUGUI _phaseLabel;

    [SerializeField]
    private Color _actionPhaseColor = Color.red;
    [SerializeField]
    private Color _planificationPhaseColor = Color.blue;

    [SerializeField]
    private Slider _timerProgressBar;

    //CONTEXTE MENU
    [SerializeField]
    private GameObject _actionMenu;

    [SerializeField]
    private Canvas parentCanvas;

    private Camera cam;

    private Cell selectedCell;
    

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    public void SetUIActionPhase()
    {
        _phaseLabel.text = "ACTION";
        _phaseLabel.color = _actionPhaseColor;
    }

    public void SetUIPlanificationPhase()
    {
        _phaseLabel.text = "PLANIFICATION";
        _phaseLabel.color = _planificationPhaseColor;
    }

    public void SetMaximumTime(float maxTime)
    {
        _timerProgressBar.maxValue = maxTime;
    }

    public void UpdateTimeBar(float timeRemain)
    {
        _timerProgressBar.value = timeRemain;
    }

    public void SetSelectedCell(Cell cell)
    {
        selectedCell = cell;

        // Mettre � jour les actions du menu en fonction de la cellule s�lectionn�e
        UpdateActionMenu();
    }

    public void SetUIActionMenuON()
    {
        // Afficher le menu uniquement si la cellule s�lectionn�e a des actions disponibles
        if (_actionMenu.activeSelf)
            _actionMenu.SetActive(false);
        else
            _actionMenu.SetActive(true);

        // Obtenir la position de la souris en pixels par rapport � l'�cran
        Vector2 mouseScreenPos = Input.mousePosition;

        // Convertir la position de la souris en coordonn�es locales par rapport au canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            mouseScreenPos, parentCanvas.worldCamera,
            out Vector2 localMousePos);

        // Mettre � jour la position de l'actionMenu pour qu'elle corresponde � la position de la souris dans l'espace du canvas
        _actionMenu.transform.localPosition = localMousePos;
    }

    public void SetUIActionMenuOFF()
    {
        _actionMenu.SetActive(false);
    }

    // M�thode pour mettre � jour les actions du menu en fonction de la cellule s�lectionn�e
    private void UpdateActionMenu()
    {
        if (selectedCell != null)
        {
            Debug.Log("Cell selectionn� : " + selectedCell);
            // Activer ou d�sactiver les actions en fonction des caract�ristiques de la cellule s�lectionn�e
            // Par exemple, si la cellule est praticable, activer l'action "Move"
            ActionMenuItem moveAction = _actionMenu.transform.Find("MoveAction").GetComponent<ActionMenuItem>();
            moveAction.SetInteractable(selectedCell.walkable);

            // V�rifier si la cellule s�lectionn�e est adjacente � une cellule occup�e par un EnemyCharacter
            bool adjacentEnemyFound = selectedCell.adjencyList.Any(adjacentCell => adjacentCell.occupant != null && adjacentCell.occupant is EnemyCharacter);

            // Si un EnemyCharacter est trouv� dans l'une des cases adjacentes, activer l'action "Attack"
            ActionMenuItem attackAction = _actionMenu.transform.Find("AttackAction").GetComponent<ActionMenuItem>();
            attackAction.gameObject.SetActive(adjacentEnemyFound);

        }
    }

    // M�thode appel�e lorsqu'une action est s�lectionn�e dans le menu
    public void OnActionSelected()
    {
        // Ex�cuter l'action s�lectionn�e
        // Par exemple :
        // selectedCell.ExecuteSelectedAction();

        // Faire dispara�tre le menu
        SetUIActionMenuOFF();
    }


}

