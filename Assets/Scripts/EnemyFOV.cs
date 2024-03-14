using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{

    [SerializeField]
    private int _range = 7;

    private List<Cell> _sightOfView = new List<Cell>();

    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    public void UpdateSightOfView(Direction direction, Cell currentCell)
    {
        //Supprime les anciens triggers
        foreach (Cell cell in _sightOfView)
        {
            cell.OutOfView(this);
        }
        _sightOfView.Clear();

        //Récupère les cellules visibles
        List<Cell> viewableCells = MapManager.instance.GetSightOfView(direction, currentCell, _range);
        foreach (Cell cell in viewableCells)
        {
            cell.VisibleBy(this);
            _sightOfView.Add(cell);

        }
    }

    public void PlayerDetected(PlayerCharacter character)
    {
        _enemy.PlayerDetected(character);
    }

    public List<PlayerCharacter> GetAllSeePlayer()
    {
        List<PlayerCharacter> playerSee = new List<PlayerCharacter>();
        foreach(Cell cell in _sightOfView)
        {
            PlayerCharacter character = cell.DemandingCheckForPlayer();
            if (character != null)
            {
                playerSee.Add(character);
            }
        }
        return playerSee;
    }

    public virtual PlayerCharacter GetClosestVisiblePlayer()
    {
        PlayerCharacter closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Cell cell in _sightOfView)
        {
            PlayerCharacter detectedPlayer = cell.DemandingCheckForPlayer();
            if (detectedPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, detectedPlayer.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = detectedPlayer;
                }
            }
        }

        Debug.Log(closestPlayer);

        return closestPlayer;
    }

    public int GetRange()
    {
        return _range;
    }

    public void SetRange(int range)
    {
        _range = range;
    }

    public void IncreaseRange(int range)
    {
        _range += range;
    }
}

