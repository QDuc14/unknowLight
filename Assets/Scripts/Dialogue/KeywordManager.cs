using System.Collections.Generic;
using UnityEngine;

public class KeywordManager : MonoBehaviour
{
    public static KeywordManager Instance;

    private Dictionary<string, KeywordData> keywordLookup;

    void Awake()
    {
        Instance = this;
        LoadKeywordDatabase();
    }

    void LoadKeywordDatabase()
    {
        TextAsset json = Resources.Load<TextAsset>("Keyword/keyword_test");
        if (json == null)
        {
            Debug.LogError("Keyword database not found! Ensure the JSON file is in Resources/Keywords/ and named 'keyword_test.json'.");
            return;
        }
        KeywordDatabase db = JsonUtility.FromJson<KeywordDatabase>(json.text);

        keywordLookup = new Dictionary<string, KeywordData>();
        foreach (var entry in db.keywords)
        {
            keywordLookup[entry.id] = entry;
        }
    }

    public KeywordData GetKeyword(string id)
    {
        return keywordLookup.ContainsKey(id) ? keywordLookup[id] : null;
    }
}