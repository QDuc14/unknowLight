using System;
using System.Collections.Generic;

[Serializable]
public class SavedData
{
    public string id;
    public string name;
    public string path; // current path of the dialogue file
    public string lineIndex; // current line index in the dialogue file
}

[Serializable]
public class SavedDataFile
{
    public List<SavedData> savedDataList;
}
