using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuItem : MonoBehaviour, IPointerClickHandler
{
    // R�f�rence � l'objet qui ex�cutera l'action
    public GameObject actionExecutor;

    // R�f�rence au bouton de cette entr�e de menu
    private Button button;

    // M�thode appel�e lorsque le GameObject est cliqu�
    public void OnPointerClick(PointerEventData eventData)
    {
        // V�rifiez si le clic est un clic gauche
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PerformAction();
        }
    }

    // Action sp�cifique � cette entr�e de menu
    private void PerformAction()
    {
        // actionExecutor.GetComponent<ActionExecutor>().ExecuteAction();

        // Apr�s avoir ex�cut� l'action, cachez le menu
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