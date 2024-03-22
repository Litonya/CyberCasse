using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get { return _instance; } }
    static UIManager _instance;

    [Header("User Interface")]
    [SerializeField]
    private TextMeshProUGUI _phaseLabel;
    [SerializeField]
    private Image _imagePhase;
    [SerializeField] 
    private Image _imageAlertLvl;
    [SerializeField]
    private Color _actionPhaseColor = Color.red;
    [SerializeField]
    private Color _planificationPhaseColor = Color.blue;

    [SerializeField]
    private Slider _timerProgressBar;
    public Timer _dialSlider;

    [SerializeField]
    private Image _panelLockpick;
    [SerializeField]
    private Image _panelHacker;
    [SerializeField]
    private Image _panelScout;
    [SerializeField]
    private Image _panelFlorent;

    [SerializeField]
    private RawImage _SFLockpick;
    [SerializeField]
    private RawImage _SFHacker;
    [SerializeField]
    private RawImage _SFScout;
    [SerializeField]
    private RawImage _SFFlorent;


    [SerializeField] private TextMeshProUGUI _victoryLabel;

    public GameObject pauseMenu;

    [Header("Context Menu")]
    [SerializeField]
    private GameObject _actionMenu;

    [SerializeField]
    private Canvas parentCanvas;

    private Cell selectedCell;

    private void Start()
    {
        InitUICharacterColor();
    }

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
        Sprite spriteMenuAction = Resources.Load<Sprite>("Menu_Action");
        _phaseLabel.text = "ACTION";
        _phaseLabel.color = _actionPhaseColor;
        _imagePhase.sprite = spriteMenuAction;
        _dialSlider.StopTimer();
    }

    public void SetUIPlanificationPhase()
    {
        Sprite spriteMenuPreparation = Resources.Load<Sprite>("Menu_Preparations");
        _phaseLabel.text = "PLANIFICATION";
        _phaseLabel.color = _planificationPhaseColor;
        _imagePhase.sprite = spriteMenuPreparation;
        _dialSlider.StartTimer();

    }

    public void SetUIAlertLevel()
    {
        Sprite alerteLvl0 = Resources.Load<Sprite>("Menu_Alert_Level0");
        Sprite alerteLvl1 = Resources.Load<Sprite>("Menu_Alert_Level1");
        Sprite alerteLvl2 = Resources.Load<Sprite>("Menu_Alert_Level2");
        Sprite alerteLvl3 = Resources.Load<Sprite>("Menu_Alert_Level3");
        switch (GameManager.instance.GetAlertLevel())
        {
            case 0:
                _imageAlertLvl.sprite = alerteLvl0; // Traitement pour le niveau d'alerte 0
                break;
            case 1:
                _imageAlertLvl.sprite = alerteLvl1; // Traitement pour le niveau d'alerte 1
                break;
            case 2:
                _imageAlertLvl.sprite = alerteLvl2; // Traitement pour le niveau d'alerte 2
                break;
            case 3:
                _imageAlertLvl.sprite = alerteLvl3; // Traitement pour le niveau d'alerte 3
                break;
            default:
                // Traitement pour toutes les autres valeurs (si nécessaire)
                break;
        }
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

        // Mettre à jour les actions du menu en fonction de la cellule sélectionnée
        UpdateActionMenu(actions);
    }

    public void SetUIActionMenuON()
    {
        // Afficher le menu uniquement si la cellule sélectionnée a des actions disponibles
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
                      // Récupérer le Transform de l'enfant à l'index i
                      Transform child = action.GetChild(i);

                      // Désactiver l'enfant
                      child.gameObject.SetActive(false);
                  }
              } */

            Debug.Log("Cell selectionné : " + selectedCell);
            
            //Active ou desactive les actions selon les actions envoyées par le game manager
            ShowAvailibleActions(actions);
        }
    }

    private void ShowAvailibleActions(List<Actions> actions)
    {
        //En théorie le joueur peut toujours se déplacer sur la case sélectionné
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
        // Si un EnemyCharacter est trouvé dans l'une des cases adjacentes, activer l'action "Attack"
        ActionMenuItem attackAction = _actionMenu.transform.Find("Container/Image/Container/AttackAction").GetComponent<ActionMenuItem>();
        attackAction.gameObject.SetActive(showIt);
    }*/

    private void ShowLockPickAction(bool showIt)
    {
        // Activer l'action "Ouvrir" si la cellule est adjacente à une porte
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

    // Méthode appelée lorsqu'une action est sélectionnée dans le menu
    public void OnOpenDoorSelected()
    {
        // Si la cellule sélectionnée est adjacente à une porte, ouvrir la porte
        Door door = selectedCell.adjencyList.FirstOrDefault(adjacentCell => adjacentCell.TryGetComponent(out Door doorComponent))?.GetComponent<Door>();
        if (door != null && door.IsClosed())
        {
            door.OpenDoor();
        }

        // Faire disparaître le menu
        SetUIActionMenuOFF();
    }

    // Méthode appelée lorsqu'on sélectionne l'action "Observer" dans le menu
    public void OnObserveDoorSelected()
    {
        // Implémentez le comportement d'observation de la porte ici
        Debug.Log("Observing the door.");

        // Faire disparaître le menu
        SetUIActionMenuOFF();
    }

    public void PauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        // Désactiver tous les enfants du parent de _panelLockpick sauf celui nommé "imagePerso"
        foreach (Transform child in _panelLockpick.transform.parent)
        {
            if (child.gameObject != _panelLockpick && child.name != "imagePerso")
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        // Désactiver tous les enfants du parent de _panelFlorent sauf celui nommé "imagePerso"
        foreach (Transform child in _panelFlorent.transform.parent)
        {
            if (child.gameObject != _panelFlorent && child.name != "imagePerso")
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        // Désactiver tous les enfants du parent de _panelScout sauf celui nommé "imagePerso"
        foreach (Transform child in _panelScout.transform.parent)
        {
            if (child.gameObject != _panelScout && child.name != "imagePerso")
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        // Désactiver tous les enfants du parent de _panelHacker sauf celui nommé "imagePerso"
        foreach (Transform child in _panelHacker.transform.parent)
        {
            if (child.gameObject != _panelHacker && child.name != "imagePerso")
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }



    }

    public void InitUICharacterColor()
    {
        Color _LockpickUIColor = MapManager.instance.crocheteuseActionTargetColor;
        Color _HackerUIColor = MapManager.instance.hackActionTargetColor;
        Color _ScoutUIColor = MapManager.instance.scoutActionTargetColor;
        Color _FlorentUIColor = MapManager.instance.muscleActionTargetColor;

        _panelLockpick.color = _LockpickUIColor;
        _panelHacker.color = _HackerUIColor;
        _panelScout.color = _ScoutUIColor;
        _panelFlorent.color = _FlorentUIColor;

        _SFLockpick.color = _LockpickUIColor;
        _SFHacker.color = _HackerUIColor;
        _SFScout.color = _ScoutUIColor;
        _SFFlorent.color = _FlorentUIColor;
    }

    

}

