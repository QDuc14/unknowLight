using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{

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

    public static void _SaveNewGame() // call from onClick event in the UI to save the current game state as a new save
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
    
    public static void _DeleteSavedGame(int index) // call from runtime with the index of the selected to delete
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "Saved_Data_File.json");
        SavedDataFile dataFile = LoadAllData(saveFilePath);
        if (index >= 0 && index < dataFile.savedDataList.Count)
        {
            dataFile.savedDataList.RemoveAt(index);
            string json = JsonUtility.ToJson(dataFile, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Deleted save at index: " + index);
        }
        else
        {
            Debug.LogWarning("Invalid index for deletion: " + index);
        }
    }

    public static void _LoadGame(int index) // call from runtime with the index of the selected to load saved game
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "Saved_Data_File.json");
        SavedDataFile dataFile = LoadAllData(saveFilePath);
        string savedPath = dataFile.savedDataList[index].path; // Load the first saved game as an example
    }
}
