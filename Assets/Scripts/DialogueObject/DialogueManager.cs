using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public GameObject avartarPanel;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private DialogueFile dialogueFile;
    private int currentLineIndex = 0;

    private Dictionary<string, CharacterData> characterData;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        LoadDialogue("Dialogue/mapDialouge");
        LoadData("_DataSource/character_data");
        DisplayLine();
        // HandleDialougeSystem();
    }

    void Update()
    {
        //HandleDialougeSystem();
    }
    void LoadDialogue(string path)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(path); // TextAsset is a Unity type used to store text data, such as .txt, .json, or .csv files.
        dialogueFile = JsonUtility.FromJson<DialogueFile>(jsonText.text); // jsonText.text retrieves the entire text content of the file as a string, ready for parsing.
    }
    void LoadData(string characterDataPath)
    {
        LoadCharacterData(characterDataPath); // Load character data from the specified path.
    }
    void LoadCharacterData(string path)
    {
        TextAsset characterJson = Resources.Load<TextAsset>(path);
        CharacterDatabase characterDb = JsonUtility.FromJson<CharacterDatabase>(characterJson.text);
        characterData = new Dictionary<string, CharacterData>();
        foreach (var character in characterDb.characters)
        {
            characterData[character.id] = character;
        }
    }
    public void HandleDialougeSystem()
    {
        DialogueLine currentLine = dialogueFile.lines[currentLineIndex];

        // Check if mouse is over a TMP link before advancing
        // Use null for Canvas in Screen Space - Overlay mode
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Mouse.current.position.ReadValue(), null);
        if (linkIndex != -1)
        {
            return;
        }

        if (currentLine.choices != null && currentLine.nextLineIndex >= 0)
        {
            OnChoiceSelected(currentLine.nextLineIndex + currentLineIndex);
            return;
        }

        if (currentLine.nextLineIndex < 0)
        {
            EndDialogue();
            return;
        }
    }

    void OnChoiceSelected(int nextLineIndex)
    {
        if (nextLineIndex > 0 && nextLineIndex < dialogueFile.lines.Count)
        {
            currentLineIndex = nextLineIndex;
            DisplayLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void DisplayLine()
    {
        // Clear previous choices
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        DialogueLine line = dialogueFile.lines[currentLineIndex];
        dialogueText.text = line.text;

        if (characterData.ContainsKey(line.characterId))
        {
            CharacterData character = characterData[line.characterId]; // Get the character name from the characterData dictionary using the characterId from the line.
            characterNameText.text = character.name;
            foreach (AvatarImage image in character.images)
            {
                if (image.id != null && image.id == line.characterImageId)
                {
                    avartarPanel.GetComponent<Image>().sprite = Resources.Load<Sprite>(image.path);
                    break;
                }
            }
        }
        else
        {
            characterNameText.text = "Unknown Character"; // Fallback if characterId is not found.
        }

        if (line.choices != null && line.choices.Count > 0)
        {
            foreach (DialogueChoice choice in line.choices)
            {
                GameObject choiceObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI btnText = choiceObj.GetComponentInChildren<TextMeshProUGUI>(); // Search choiceObj and all its children, and return the first TextMeshProUGUI component you find.
                btnText.text = choice.text;

                Button btn = choiceObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnChoiceSelected(choice.nextLineIndex));
            }
        }
    }

    void EndDialogue()
    {
        characterNameText.text = "";
        dialogueText.text = "The End.";
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
        Destroy(avartarPanel);
    }
}
