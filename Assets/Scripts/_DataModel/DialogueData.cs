using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public int nextLineIndex;
    public List<Actor> actors;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class Actor
{
    public string charId;
    public string charImgId;
    public bool actFlg;
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
