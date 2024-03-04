using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{

    [SerializeField]
    private int _range = 7;

    [SerializeField]
    private GameObject _triggerViewObjectPrefab;
    private List<ViewTrigger> _sightOfView = new List<ViewTrigger>();

    private EnemyCharacter _enemyCharacter;

    private void Awake()
    {
        _enemyCharacter = GetComponent<EnemyCharacter>();
    }

    public void UpdateSightOfView(Direction direction, Cell currentCell)
    {
        Debug.Log("Create sight of View");
        //Supprime les anciencs triggers
        foreach (ViewTrigger viewObject in _sightOfView)
        {
            Destroy(viewObject);
        }
        _sightOfView.Clear();

        //Récupère les cellules visibles
        List<Cell> viewableCells = MapManager.instance.GetSightOfView(direction, currentCell, _range);
        foreach (Cell cell in viewableCells)
        {
            //Créer un nouveau trigger sur la case actuelle de la liste, en le mettant en parent de l'ennemi propriétaire
            GameObject newViewObject = Instantiate(_triggerViewObjectPrefab, cell.transform.position, Quaternion.identity, transform);
            //Ajoute l'objet nouvellement créer dans la liste
            ViewTrigger triggerScript = newViewObject.GetComponent<ViewTrigger>();
            _sightOfView.Add(triggerScript);

        }
    }
}
