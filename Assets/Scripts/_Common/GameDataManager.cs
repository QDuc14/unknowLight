using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static void SaveNewGame() // call from onClick event in the UI to save the current game state as a new save
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
    
    public static void DeleteSavedGame(int index) // call from runtime with the index of the selected to delete
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

    public static void LoadGame(int index) // call from runtime with the index of the selected to load saved game
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "Saved_Data_File.json");
        SavedDataFile dataFile = LoadAllData(saveFilePath);
        DialogueManager.dialogueFilePath = dataFile.savedDataList[index].path;
        DialogueManager.currentLineIndex = int.Parse(dataFile.savedDataList[index].lineIndex);
        SceneManager.LoadScene("_SampleScene");
    }

    public static void NewGame() // call from onClick event in the UI to start a new game
    {
        DialogueManager.dialogueFilePath = "Dialogue/Demo/Demo_0.0.1a";
        DialogueManager.currentLineIndex = 0;
        SceneManager.LoadScene("DialogueScene"); 
    }
}
