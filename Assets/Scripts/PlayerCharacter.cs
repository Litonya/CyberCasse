using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    NONE,
    MOVE, //Ne pas mettre move sur un tuile
    LOCKPICK,
    HACK,
    GETITEM, //Ne pas mettre directement GetItem sur une tuile
    PLACEITEM,
    KNOCKOUT,
    LOOK,
    UNLOCK,
    BREAKGLASS,
}

public class PlayerCharacter : Character
{
    [Header("Character's stats")]
    [SerializeField] private int _lockPinckingStat = 1;
    [SerializeField] private int _hacking = 1;
    [SerializeField] private int _strenght = 1;

    private Actions _preparedAction = Actions.NONE;
    private Cell _targetActionCell;
    private Actions _previousAction = Actions.NONE;
    private Cell _previousActionCell;

    private Item _carriedItem;

    private int _movePointsBackup;

    protected override void Awake()
    {
        base.Awake();
        _movePointsBackup = movePoints;
    }

    public override void Reset()
    {
        if (_targetActionCell == null) return;
        _targetActionCell.SetState(Cell.CellState.actionTarget, this);
    }

    protected override void Action()
    {
        _previousActionCell = _targetActionCell;
        _previousAction = _preparedAction;
        if (_preparedAction != Actions.NONE) LaunchAction();
        base.Action();
    }

    private void LaunchAction()
    {
        if (_preparedAction == Actions.LOCKPICK)
        {
            if(!_targetActionCell.Acte(_preparedAction, _lockPinckingStat, this))
            {
                SetPreparedAction(_preparedAction, _targetActionCell);
                
            }else
            {
                ClearPreparedAction();
            }
        }
        else if (_preparedAction == Actions.GETITEM)
        {
            _targetActionCell.Acte(_preparedAction, 0, this);
        }
        else if (_preparedAction == Actions.UNLOCK)
        {
            _targetActionCell.Acte(_preparedAction, 0, this);
        }
        else if(_preparedAction == Actions.BREAKGLASS)
        {
            if (!_targetActionCell.Acte(_preparedAction, _strenght, this))
            {
                SetPreparedAction(_preparedAction, _targetActionCell);
            }
            else
            {
                ClearPreparedAction();
            }
            GetComponentInChildren<SpriteController>().SetAnimationState("Punch");
        }
        else if(_preparedAction == Actions.HACK)
        {
            _targetActionCell.Acte(_preparedAction, _hacking, this);
            _characterAudio.PlayActionSound(SFX_Name.CAMERA_CONNEXION);
            SetPreparedAction(_preparedAction, _targetActionCell);
        }
    }

    public override void Acte()
    {
        if (_previousActionCell != null && _previousActionCell != _targetActionCell)
        {
            _previousActionCell.ResetDifficulty();
            if (_previousAction == Actions.HACK)
            {
                foreach(SecurityCamera cameras in GameManager.instance.securityCameraList)
                {
                    cameras.UnHack();
                }
            }
        }

        base.Acte();
    }

    public void SetPreparedAction(Actions action, Cell target)
    {
        if (_targetActionCell != null) _targetActionCell.SetState(Cell.CellState.Idle, null);
        _preparedAction = action;
        _targetActionCell = target;
        target.SetState(Cell.CellState.actionTarget, this);
        //Ajouter ligne qui fait lien vers la cellule pour afficher un logo
    }

    public void ClearPreparedAction()
    {
        if (_targetActionCell != null) _targetActionCell.SetState(Cell.CellState.Idle, null);
        _preparedAction = Actions.NONE;
        _targetActionCell = null;
    }

    public void PickUpWinCondition(WinCondition winCondition)
    {
        // Attachement de la win condition au joueur
        attachedWinCondition = winCondition;

        // D�terminer la position � laquelle attacher la win condition par rapport au joueur
        Vector3 offset = new Vector3(0f, _yOffset, 0f);
        winCondition.transform.position = transform.position + offset;
        winCondition.transform.parent = transform; // Attacher la win condition au joueur pour qu'elle suive ses mouvements
    }

    public bool HasWinConditionAttached()
    {
        WinCondition winCondition = GetComponentInChildren<WinCondition>();
        Debug.Log(winCondition);
        return winCondition != null;
    }

    public bool IsInVictoryZone()
    {
        // Recherche la VictoryArea dans la sc�ne
        VictoryArea victoryArea = FindObjectOfType<VictoryArea>();
        if (victoryArea != null)
        {
           return victoryArea.playersInVictoryZone.Count == victoryArea.totalPlayer;
        }//
        else
        {
            Debug.LogWarning("VictoryArea introuvable dans la sc�ne.");
            return false;
        }
    }

    public void Caught()
    {
        PlaceCarriedItem();
        GameManager.instance.PlayerCaught(this);
        EventsManager.instance.RaiseSFXEvent(SFX_Name.PLAYER_DEAD);
        Desactivate();
        
    }

    public void Desactivate()
    {
        _currentCell.occupant = null;
        gameObject.SetActive(false);
    }

    public void SetCarriedItem(Item item)
    {
        _carriedItem = item;
        _carriedItem.gameObject.SetActive(false);
        int moveMalus = -item.movePointsMalus + _strenght;
        if (moveMalus < 0)
        {
            movePoints += moveMalus;
        }
        if (item.GetComponent<Objective>())
        {
            GameManager.instance.LaunchPartTwo();
        }
    }

    public Item GetCarriedItem()
    {
        return _carriedItem;
    }

    public void DestroyCarriedItem()
    {
        Destroy(_carriedItem.gameObject);
        _carriedItem=null;
        movePoints = _movePointsBackup;
    }

    public void PlaceCarriedItem()
    {
        if (_carriedItem == null) return;
        _currentCell.PlaceItem(_carriedItem);
        GameManager.instance.UpdateMoneyScore(-_carriedItem.value);
        _carriedItem = null;
        movePoints = _movePointsBackup;
    }
}

