using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : Character, Enemy
{
    [SerializeField] private List<Direction> _directions = new List<Direction>();

    private Direction _currentDirection;

    private List<PlayerCharacter> _charactersSeen = new List<PlayerCharacter>();
    private bool _alreadyDectectThisTurn = false;

    private EnemyFOV _fov;

    public bool isHack = false;

    private int _curentIndex = 0;
    private int _fovSize;

    private void Awake()
    {
        _fov = GetComponent<EnemyFOV>();
        _fovSize = _fov.GetRange();
    }

    private void Start()
    {
        _currentCell.RemoveOccupant();
        ChangeDirection(_currentDirection = _directions[0]);
    }

    public override void Acte()
    {
        _alreadyDectectThisTurn = false;
        _curentIndex++;
        if (_curentIndex>= _directions.Count)
        {
            _curentIndex = 0;
        }

        ChangeDirection(_directions[_curentIndex]);

        List<PlayerCharacter> characters = _fov.GetAllSeePlayer();
        if (characters.Count == 0) return;
        foreach (PlayerCharacter character in _charactersSeen)
        {
            if (!characters.Contains(character)) _charactersSeen.Remove(character);
        }
    }

    public void PlayerDetected(PlayerCharacter player)
    {
        if (!_alreadyDectectThisTurn && !_charactersSeen.Contains(player))
        {
            _charactersSeen.Add(player);
            GameManager.instance.IncreaseAlertLevel();
            _alreadyDectectThisTurn = true;
        }else if (!_charactersSeen.Contains(player)) 
        {
            _charactersSeen.Add(player);
        }
    }

    void ChangeDirection(Direction pNewDirection)
    {
        _currentDirection = pNewDirection;
        Debug.Log("Update cam");
        _fov.UpdateSightOfView(_currentDirection, _currentCell);
    }

    public void Hack()
    {
        _fov.SetRange(0);
        isHack = true;
    }

    public void UnHack()
    {
        _fov.SetRange(_fovSize);
        isHack = false;
    }
}
