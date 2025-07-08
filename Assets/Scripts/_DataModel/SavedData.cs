using System;
using System.Collections.Generic;

[Serializable]
public class SavedData
{
    public string id;
    public string name;
    public string path;
    public string lineIndex;
}

[Serializable]
public class SavedDataFile
{
    public List<SavedData> savedDataList;
}
