using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighlightTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // R�f�rence au texte que vous souhaitez mettre en surbrillance
    public Text textToHighlight;

    // Couleur du texte en surbrillance
    public Color highlightColor = Color.yellow;

    // Couleur normale du texte
    private Color normalColor;

    // Initialisation
    void Start()
    {
        // Assurez-vous que vous avez une r�f�rence au texte
        if (textToHighlight != null)
        {
            // Sauvegardez la couleur normale du texte
            normalColor = textToHighlight.color;
        }
        else
        {
            Debug.LogError("Veuillez assigner une r�f�rence au texte � mettre en surbrillance.");
        }
    }

    // Appel� lorsque la souris entre dans la zone du GameObject parent
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mettez en surbrillance le texte en changeant sa couleur
        if (textToHighlight != null)
        {
            textToHighlight.color = highlightColor;
        }
    }

    // Appel� lorsque la souris quitte la zone du GameObject parent
    public void OnPointerExit(PointerEventData eventData)
    {
        // R�tablissez la couleur normale du texte
        if (textToHighlight != null)
        {
            textToHighlight.color = normalColor;
        }
    }
}
