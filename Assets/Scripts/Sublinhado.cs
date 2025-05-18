using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextoComSublinhado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI texto;

    private void Awake()
    {
        texto = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.fontStyle |= FontStyles.Underline;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.fontStyle &= ~FontStyles.Underline;
    }
}
