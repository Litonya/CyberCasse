using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuItem : MonoBehaviour, IPointerClickHandler
{
    // Référence à l'objet qui exécutera l'action
    public GameObject actionExecutor;

    // Référence au bouton de cette entrée de menu
    private Button button;

    // Méthode appelée lorsque le GameObject est cliqué
    public void OnPointerClick(PointerEventData eventData)
    {
        // Vérifiez si le clic est un clic gauche
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PerformAction();
        }
    }

    // Action spécifique à cette entrée de menu
    private void PerformAction()
    {
        // actionExecutor.GetComponent<ActionExecutor>().ExecuteAction();

        // Après avoir exécuté l'action, cachez le menu
        UIManager.instance.SetUIActionMenuON();
        transform.parent.gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }
}