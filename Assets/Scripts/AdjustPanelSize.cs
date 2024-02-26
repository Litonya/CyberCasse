using UnityEngine;
using UnityEngine.UI;

public class AdjustPanelSize : MonoBehaviour
{
    public GameObject panel;
    public float spacing = 5f; // Espacement entre les éléments
    public float minHeight = 3f; // Hauteur minimale du Panel

    void UpdatePanelSize()
    {
        // Comptez le nombre d'enfants dans le Panel
        int childCount = panel.transform.childCount;

        // Obtenez la hauteur du premier enfant
        float firstChildHeight = (childCount > 0) ? panel.transform.GetChild(0).GetComponent<RectTransform>().rect.height : 0;

        // Calculez la hauteur du Panel en fonction du nombre d'enfants et de l'espacement
        float panelHeight = firstChildHeight * childCount + spacing * (childCount - 1);

        // Assurez-vous que la hauteur du Panel ne soit pas inférieure à la hauteur minimale spécifiée
        panelHeight = Mathf.Max(panelHeight, minHeight);

        // Mettez à jour la taille du Panel
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        panelRectTransform.sizeDelta = new Vector2(panelRectTransform.sizeDelta.x, panelHeight);
    }

    void Update()
    {
        UpdatePanelSize();
    }
}
