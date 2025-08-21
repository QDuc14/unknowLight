using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string characterId;
    public string characterImageId; // remove later if not needed
    public string text;
    public int nextLineIndex;
    public List<actor> actors;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class actor
{
    public string charId;
    public string charImgId;
    public bool actFlg;
    public int pos;
}

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public int nextLineIndex;
    public string flag; // Optional: affect story state
}

[System.Serializable]
public class DialogueFile
{
    public List<DialogueLine> lines;
}
