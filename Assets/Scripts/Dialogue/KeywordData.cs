using System.Collections.Generic;

[System.Serializable]
public class KeywordData
{
    public string id;
    public string display;
    public string description;
    public string action;
}

[System.Serializable]
public class KeywordDatabase
{
    public List<KeywordData> keywords;
}
