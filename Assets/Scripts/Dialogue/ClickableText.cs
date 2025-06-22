using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickableText : MonoBehaviour
{
    private TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMesh, Mouse.current.position.ReadValue(), null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textMesh.textInfo.linkInfo[linkIndex];
                string clickedID = linkInfo.GetLinkID();
                Debug.Log($"Clicked on: {clickedID}");

                // Handle the clicked word (based on ID)
                OnWordClicked(clickedID);
            }
        }
    }

    void OnWordClicked(string id)
    {
        var keyword = KeywordManager.Instance.GetKeyword(id);
        if (keyword == null)
        {
            Debug.LogWarning($"No keyword entry found for: {id}");
            return;
        }

        switch (keyword.action)
        {
            case "ShowProfile":
                ShowProfile(keyword);
                break;
            case "ShowTooltip":
                ShowTooltip(keyword);
                break;
            default:
                Debug.Log($"Keyword clicked: {keyword.display}");
                break;
        }
    }

    void ShowProfile(KeywordData data)
    {
        Debug.Log($"Profile: {data.display}\n{data.description}");
    }

    void ShowTooltip(KeywordData data)
    {
        Debug.Log($"Tooltip: {data.description}");
    }
}
