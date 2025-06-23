using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ClickableText : MonoBehaviour
{
    private TMP_Text textMesh;
    private Dictionary<string, KeywordData> keywordLookup;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        LoadKeywordDatabase();
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
                Debug.Log("✅ Clicked on keyword: " + clickedID);

                OnWordClicked(clickedID);
            }
        }
    }

    void LoadKeywordDatabase()
    {
        TextAsset json = Resources.Load<TextAsset>("Keyword/keyword_test"); // Adjust if needed
        if (json == null)
        {
            Debug.LogError("❌ Keyword JSON not found at Resources/Keyword/keyword_test.json");
            return;
        }

        KeywordDatabase db = JsonUtility.FromJson<KeywordDatabase>(json.text);
        keywordLookup = new Dictionary<string, KeywordData>();
        foreach (var keyword in db.keywords)
        {
            keywordLookup[keyword.id] = keyword;
        }

        Debug.Log($"✅ Loaded {keywordLookup.Count} keywords.");
    }

    void OnWordClicked(string id)
    {
        if (!keywordLookup.ContainsKey(id))
        {
            Debug.LogWarning($"⚠️ No keyword entry found for: {id}");
            return;
        }

        KeywordData keyword = keywordLookup[id];

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
        Debug.Log($"📘 Profile: {data.display}\n{data.description}");
    }

    void ShowTooltip(KeywordData data)
    {
        Debug.Log($"💬 Tooltip: {data.description}");
    }
}
