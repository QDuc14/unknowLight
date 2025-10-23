using UnityEngine;
using TMPro;
using System.Collections;

public class DrinkUI : MonoBehaviour
{
    public static DrinkUI I { get; private set; }

    public TextMeshProUGUI label;

    void Awake() { I = this; Hide(); }

    public void Show(string text, float seconds = 2f)
    {
        label.text = text;
        label.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfter(seconds));
    }

    IEnumerator HideAfter(float s) { yield return new WaitForSeconds(s); Hide(); }

    public void Hide()
    {
        if (label) label.gameObject.SetActive(false);
    }
}
