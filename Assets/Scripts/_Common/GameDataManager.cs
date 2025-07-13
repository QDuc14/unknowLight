using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class GameDataManager : MonoBehaviour
{
    public static void _SaveNewGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "Saved_Data_File.json");
        SavedDataFile dataFile = LoadAllData(saveFilePath);
        if (dataFile.savedDataList.Count < 20)
        {
            SavedData newData = new()
            {
                id = dataFile.savedDataList.Count.ToString(),
                name = "Save " + dataFile.savedDataList.Count,
                path = DialogueManager.dialogueFilePath.ToString(),
                lineIndex = DialogueManager.currentLineIndex.ToString()
            };
            dataFile.savedDataList.Add(newData);
        }
        string json = JsonUtility.ToJson(dataFile, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Saved data: " + dataFile.savedDataList.Count.ToString());
    }
    private static SavedDataFile LoadAllData(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SavedDataFile>(json);
        }
        else
        {
            return new SavedDataFile(); // Return empty list
        }
    }

    public static void _LoadGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "Saved_Data_File.json");
        SavedDataFile dataFile = LoadAllData(saveFilePath);
        string savedPath = dataFile.savedDataList[0].path; // Load the first saved game as an example
    }
}
