using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public GameObject avartarPanel1;
    public GameObject avartarPanel2;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    public static int currentLineIndex = 0;
    public static string dialogueFilePath = "";

    private DialogueFile dialogueFile;
    private Dictionary<string, CharacterData> characterData;

    void Awake()
    {
        if (dialogueFilePath != "")
        {
            LoadDialogue(dialogueFilePath);
            LoadData("_DataSource/character_data");
        }
    }

    void Start()
    {
        if (dialogueFilePath != "")
        {
            DisplayLine();
        }
    }

    void LoadDialogue(string path)
    {
        dialogueFilePath = path;
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
    public void HandleDialogueSystem()
    {
        // Check if dialogueFile is loaded and has lines
        if (dialogueFile == null || dialogueFile.lines == null || dialogueFile.lines.Count == 0 || currentLineIndex >= dialogueFile.lines.Count)
        {
            return;
        }

        DialogueLine currentLine = dialogueFile.lines[currentLineIndex];

        // Check if mouse is over a TMP link before advancing
        // Use null for Canvas in Screen Space - Overlay mode
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Mouse.current.position.ReadValue(), null);
        if (linkIndex != -1)
        {
            return;
        }

        if (currentLine.choices != null && currentLine.nextLineIndex >= 0 && currentLine.choices.Count == 0)
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

        GetActorsInfo(line);

        if (line.choices != null && line.choices.Count > 0)
        {
            foreach (DialogueChoice choice in line.choices)
            {
                GameObject choiceObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI btnText = choiceObj.GetComponentInChildren<TextMeshProUGUI>(); // Search choiceObj and all its children, and return the first TextMeshProUGUI component you find.
                btnText.text = choice.text;

                Button btn = choiceObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnChoiceSelected(choice.nextLineIndex + currentLineIndex));
            }
        }
    }

    void GetActorsInfo(DialogueLine line)
    {
        List<Actor> actors = line.actors;
        CharacterData character1 = characterData[actors[0].charId];
        CharacterData character2 = characterData[actors[1].charId];

        if (actors.Count == 0 || (actors.Count == 1 && !actors[0].actFlg) || actors.Count == 2 && !actors[0].actFlg && !actors[1].actFlg)
        {
            characterNameText.text = "";
        }
        else
        {
            characterNameText.text = actors[0].actFlg ? character1.name : actors[1].actFlg ? character2.name : "Unknown";
        }

        if (characterData.ContainsKey(actors[0].charId))
        {
            foreach (AvatarImage image in characterData[actors[0].charId].images)
            {
                if (image.id != null && image.id == actors[0].charImgId)
                {
                    avartarPanel1.GetComponent<Image>().sprite = Resources.Load<Sprite>(image.path);
                    break;
                }
                else
                {
                    avartarPanel1.GetComponent<Image>().sprite = null;
                }
            }
            avartarPanel1.GetComponent<Image>().color = !actors[0].actFlg ? new Color(0.6f, 0.6f, 0.6f, 1f) : new Color(1f, 1f, 1f, 1f);
        }

        if (characterData.ContainsKey(actors[1].charId))
        {
            foreach (AvatarImage image in characterData[actors[1].charId].images)
            {
                if (image.id != null && image.id == actors[1].charImgId)
                {
                    avartarPanel2.GetComponent<Image>().sprite = Resources.Load<Sprite>(image.path);
                    break;
                }
                else
                {
                    avartarPanel2.GetComponent<Image>().sprite = null;
                }
            }
            avartarPanel2.GetComponent<Image>().color = !actors[1].actFlg ? new Color(0.6f, 0.6f, 0.6f, 1f) : new Color(1f, 1f, 1f, 1f);
        }
    }

    void EndDialogue()
    {
        currentLineIndex += 1;
        dialogueText.text = "The End.";
        Destroy(avartarPanel1);
        Destroy(avartarPanel2);
        Destroy(choicesContainer.gameObject);
        Destroy(characterNameText);
    }
}
