using System.Collections.Generic;

[System.Serializable]
public class CharacterData
{
    public string id;
    public string name;
    public List<AvatarImage> images;
    public string description;
}
[System.Serializable]
public class AvatarImage
{
    public string id;
    public string path;
}
[System.Serializable]
public class CharacterDatabase
{
    public List<CharacterData> characters;
}
