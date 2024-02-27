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

        // Mettre à jour les actions du menu en fonction de la cellule sélectionnée
        UpdateActionMenu();
    }

    public void SetUIActionMenuON()
    {
        // Afficher le menu uniquement si la cellule sélectionnée a des actions disponibles
        if (_actionMenu.activeSelf)
            _actionMenu.SetActive(false);
        else
            _actionMenu.SetActive(true);

        // Obtenir la position de la souris en pixels par rapport à l'écran
        Vector2 mouseScreenPos = Input.mousePosition;

        // Convertir la position de la souris en coordonnées locales par rapport au canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            mouseScreenPos, parentCanvas.worldCamera,
            out Vector2 localMousePos);

        // Mettre à jour la position de l'actionMenu pour qu'elle corresponde à la position de la souris dans l'espace du canvas
        _actionMenu.transform.localPosition = localMousePos;
    }

    public void SetUIActionMenuOFF()
    {
        _actionMenu.SetActive(false);
    }

    // Méthode pour mettre à jour les actions du menu en fonction de la cellule sélectionnée
    private void UpdateActionMenu()
    {
        if (selectedCell != null)
        {
            Debug.Log("Cell selectionné : " + selectedCell);
            // Activer ou désactiver les actions en fonction des caractéristiques de la cellule sélectionnée
            // Par exemple, si la cellule est praticable, activer l'action "Move"
            ActionMenuItem moveAction = _actionMenu.transform.Find("MoveAction").GetComponent<ActionMenuItem>();
            moveAction.SetInteractable(selectedCell.walkable);

            // Vérifier si la cellule sélectionnée est adjacente à une cellule occupée par un EnemyCharacter
            bool adjacentEnemyFound = selectedCell.adjencyList.Any(adjacentCell => adjacentCell.occupant != null && adjacentCell.occupant is EnemyCharacter);

            // Si un EnemyCharacter est trouvé dans l'une des cases adjacentes, activer l'action "Attack"
            ActionMenuItem attackAction = _actionMenu.transform.Find("AttackAction").GetComponent<ActionMenuItem>();
            attackAction.gameObject.SetActive(adjacentEnemyFound);

        }
    }

    // Méthode appelée lorsqu'une action est sélectionnée dans le menu
    public void OnActionSelected()
    {
        // Exécuter l'action sélectionnée
        // Par exemple :
        // selectedCell.ExecuteSelectedAction();

        // Faire disparaître le menu
        SetUIActionMenuOFF();
    }


}

