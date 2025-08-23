using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text tmpText;

    void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        tmpText.color = new Color(1f, 0.8f, 0f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        tmpText.color = new Color(1f, 1f, 1f, 1f);
    }
}
