using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cell;
using static MapManager;

public class EnemyCharacter : Character
{
    [SerializeField]
    private List<Cell> _patrolTargets;
    private int _currentPatrolIndex = 0;

    public void PrepareTurnAction()
    {
        List<Cell> fullPath = MapManager.instance.FindPath(_currentCell, _patrolTargets[_currentPatrolIndex]);
        int index = movePoints;
        
        //Vérifie la présence d'une autre cible ou non au bout du chemin
        while (fullPath[index].occupant != null)
        {
            index--;
            if (index <= 0) { break; } //S'il ne reste que l'origine alors on arrête et le NPC ne bougera donc pas
        }

        _target = fullPath[index];
        
        if(index > 0) { path = fullPath.GetRange(0, index + 1);} //On vérifie si le NPC bouge effectivement
        else { path.Add(_currentCell); }
        

        _target.SetState(CellState.isSelected);

        ShowPath();
    }

    public override void Reset()
    {
        base.Reset();

        if (_patrolTargets.Count > 0)
        {
            PrepareTurnAction();
        }
    }

    protected override void MoveToNextCell()
    {
        base.MoveToNextCell();

        if (_currentCell == _patrolTargets[_currentPatrolIndex])
        {
            _currentPatrolIndex++;
            if (_currentPatrolIndex >= _patrolTargets.Count) {  _currentPatrolIndex = 0; }
        }
    }

    

}
