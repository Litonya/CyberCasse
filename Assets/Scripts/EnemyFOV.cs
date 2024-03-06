using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{

    [SerializeField]
    private int _range = 7;

    private List<Cell> _sightOfView = new List<Cell>();

    private EnemyCharacter _enemyCharacter;

    private void Awake()
    {
        _enemyCharacter = GetComponent<EnemyCharacter>();
    }

    public void UpdateSightOfView(Direction direction, Cell currentCell)
    {
        Debug.Log("Create sight of View");
        //Supprime les anciencs triggers
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
        _enemyCharacter.LaunchChase(character);
    }
}
