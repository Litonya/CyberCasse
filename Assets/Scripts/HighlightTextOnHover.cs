using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighlightTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Référence au texte que vous souhaitez mettre en surbrillance
    public Text textToHighlight;

    // Couleur du texte en surbrillance
    public Color highlightColor = Color.yellow;

    // Couleur normale du texte
    private Color normalColor;

    // Initialisation
    void Start()
    {
        // Assurez-vous que vous avez une référence au texte
        if (textToHighlight != null)
        {
            // Sauvegardez la couleur normale du texte
            normalColor = textToHighlight.color;
        }
        else
        {
            Debug.LogError("Veuillez assigner une référence au texte à mettre en surbrillance.");
        }
    }

    // Appelé lorsque la souris entre dans la zone du GameObject parent
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mettez en surbrillance le texte en changeant sa couleur
        if (textToHighlight != null)
        {
            textToHighlight.color = highlightColor;
        }
    }

    // Appelé lorsque la souris quitte la zone du GameObject parent
    public void OnPointerExit(PointerEventData eventData)
    {
        // Rétablissez la couleur normale du texte
        if (textToHighlight != null)
        {
            textToHighlight.color = normalColor;
        }
    }
}
