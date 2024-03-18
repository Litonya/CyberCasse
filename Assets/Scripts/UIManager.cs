using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

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
    public Timer _dialSlider;

    [SerializeField] private TextMeshProUGUI _victoryLabel;

    [Header("Context Menu")]
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
        _dialSlider.StopTimer();
    }

    public void SetUIPlanificationPhase()
    {
        _phaseLabel.text = "PLANIFICATION";
        _phaseLabel.color = _planificationPhaseColor;
        _dialSlider.StartTimer();

    }

    public void SetMaximumTime(float maxTime)
    {
        
        _timerProgressBar.maxValue = maxTime;
        // _dialSlider.DisplayFormattedTime(maxTime);
        _dialSlider.SetTimerTimeStart(maxTime);
    }

    public void UpdateTimeBar(float timeRemain)
    {
        _timerProgressBar.value = timeRemain;
        // _dialSlider.DisplayFormattedTime(timeRemain);
        _dialSlider.timeRemaining = timeRemain;
    }

    public void ShowVictory()
    {
        _victoryLabel.gameObject.SetActive(true);
    }

    public void SetSelectedCell(Cell cell, List<Actions> actions)
    {
        selectedCell = cell;

        // Mettre � jour les actions du menu en fonction de la cellule s�lectionn�e
        UpdateActionMenu(actions);
    }

    public void SetUIActionMenuON()
    {
        // Afficher le menu uniquement si la cellule s�lectionn�e a des actions disponibles
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
    private void UpdateActionMenu(List<Actions> actions)
    {
        Debug.Log("Le menu contextuel est update");
        if (selectedCell != null)
        {
            Transform action = _actionMenu.transform.Find("Container/Image/Container");
            if (!action.gameObject.activeSelf) action.gameObject.SetActive(true);

            /*  if (action != null)
              {
                  // Parcourir tous les enfants de containerTransform
                  for (int i = 0; i < action.childCount; i++)
                  {
                      // R�cup�rer le Transform de l'enfant � l'index i
                      Transform child = action.GetChild(i);

                      // D�sactiver l'enfant
                      child.gameObject.SetActive(false);
                  }
              } */

            Debug.Log("Cell selectionn� : " + selectedCell);
            
            //Active ou desactive les actions selon les actions envoy�es par le game manager
            ShowAvailibleActions(actions);
        }
    }

    private void ShowAvailibleActions(List<Actions> actions)
    {
        //En th�orie le joueur peut toujours se d�placer sur la case s�lectionn�
        ShowMoveAction();

        //ShowKnockoutAction(actions.Contains(Actions.KNOCKOUT));

        ShowLockPickAction(actions.Contains(Actions.LOCKPICK));

        ShowLookAction(actions.Contains(Actions.LOOK));

        ShowGrabItemAction(actions.Contains(Actions.GETITEM));

        ShowUnlockAction(actions.Contains(Actions.UNLOCK));

        ShowBreakGlassAction(actions.Contains(Actions.BREAKGLASS));

        ShowHackAction(actions.Contains(Actions.HACK));
    }


    private void ShowMoveAction()
    {
        ActionMenuItem moveAction = _actionMenu.transform.Find("Container/Image/Container/MoveAction").GetComponent<ActionMenuItem>();
        moveAction.gameObject.SetActive(true);
        moveAction.SetInteractable(true);
    }

    /*private void ShowKnockoutAction(bool showIt)
    {
        // Si un EnemyCharacter est trouv� dans l'une des cases adjacentes, activer l'action "Attack"
        ActionMenuItem attackAction = _actionMenu.transform.Find("Container/Image/Container/AttackAction").GetComponent<ActionMenuItem>();
        attackAction.gameObject.SetActive(showIt);
    }*/

    private void ShowLockPickAction(bool showIt)
    {
        // Activer l'action "Ouvrir" si la cellule est adjacente � une porte
        _actionMenu.transform.Find("Container/Image/Container/OpenDoorAction").gameObject.SetActive(showIt);
    }

    private void ShowUnlockAction(bool showIt)
    {
        _actionMenu.transform.Find("Container/Image/Container/UnlockAction").gameObject.SetActive(showIt);
    }

    private void ShowGrabItemAction(bool showIt)
    {
        ActionMenuItem grabAction = _actionMenu.transform.Find("Container/Image/Container/GrabAction").GetComponent<ActionMenuItem>();
        grabAction.gameObject.SetActive(showIt);
    }

    private void ShowLookAction(bool showIt)
    {
        // Activer l'action "Observer"
        _actionMenu.transform.Find("Container/Image/Container/ObserveDoorAction").gameObject.SetActive(showIt);
    }

    private void ShowBreakGlassAction(bool showIt)
    {
        _actionMenu.transform.Find("Container/Image/Container/BreakGlassAction").gameObject.SetActive(showIt);
    }

    private void ShowHackAction (bool showIt)
    {
        _actionMenu.transform.Find("Container/Image/Container/HackAction").gameObject.SetActive(showIt);
    }

    // M�thode appel�e lorsqu'une action est s�lectionn�e dans le menu
    public void OnOpenDoorSelected()
    {
        // Si la cellule s�lectionn�e est adjacente � une porte, ouvrir la porte
        Door door = selectedCell.adjencyList.FirstOrDefault(adjacentCell => adjacentCell.TryGetComponent(out Door doorComponent))?.GetComponent<Door>();
        if (door != null && door.IsClosed())
        {
            door.OpenDoor();
        }

        // Faire dispara�tre le menu
        SetUIActionMenuOFF();
    }

    // M�thode appel�e lorsqu'on s�lectionne l'action "Observer" dans le menu
    public void OnObserveDoorSelected()
    {
        // Impl�mentez le comportement d'observation de la porte ici
        Debug.Log("Observing the door.");

        // Faire dispara�tre le menu
        SetUIActionMenuOFF();
    }
}

