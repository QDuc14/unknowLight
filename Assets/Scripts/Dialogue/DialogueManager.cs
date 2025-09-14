using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    // --- Public Variables ---
    [Header("Scenes")]
    public GameObject DialogueScene;
    public GameObject Cutscene;

    [Header("Dialogue Elements")]
    public TextMeshProUGUI characterNameText;
    public GameObject avartarPanel1;
    public GameObject avartarPanel2;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    // --- Static Variables ---
    public static int currentLineIndex = 0;
    public static string dialogueFilePath = "";

    // --- Private Variables ---
    private DialogueFile dialogueFile;
    private Dictionary<string, CharacterData> characterData;
    private Dictionary<string, KeywordData> keywordLookup;
    private Coroutine typingCo;
    private bool isTyping = false;

    void Awake()
    {
        if (dialogueFilePath != "")
        {
            LoadDialogue(dialogueFilePath);
            LoadData("_DataSource/character_data", "_DataSource/keyword_test");
        }
    }

    void Start()
    {
        if (dialogueFilePath != "")
        {
            DialogueLine currentLine = dialogueFile.lines[currentLineIndex];
            StartDialogue();
            //if (currentLine.type == "dialogue")
            //{
            //    StartDialogue();
            //}
        }
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        HandleDialogueSystem();
    }

    void LoadDialogue(string path)
    {
        dialogueFilePath = path;
        TextAsset jsonText = Resources.Load<TextAsset>(path); // TextAsset is a Unity type used to store text data, such as .txt, .json, or .csv files.
        dialogueFile = JsonUtility.FromJson<DialogueFile>(jsonText.text); // jsonText.text retrieves the entire text content of the file as a string, ready for parsing.
    }
    void LoadData(string characterDataPath, string keyWordDataPath)
    {
        LoadCharacterData(characterDataPath); // Load character data from the specified path.
        LoadKeywordDatabase(keyWordDataPath);
    }
    void LoadCharacterData(string path)
    {
        TextAsset characterJson = Resources.Load<TextAsset>(path);
        if (characterJson == null)
        {
            Debug.LogError("Character JSON not found at " + path);
            return;
        }

        CharacterDatabase characterDb = JsonUtility.FromJson<CharacterDatabase>(characterJson.text);
        characterData = new Dictionary<string, CharacterData>();
        foreach (var character in characterDb.characters)
        {
            characterData[character.id] = character;
        }
    }
    void LoadKeywordDatabase(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path); // Adjust if needed
        if (json == null)
        {
            Debug.LogError("Keyword JSON not found at " + path);
            return;
        }

        KeywordDatabase db = JsonUtility.FromJson<KeywordDatabase>(json.text);
        keywordLookup = new Dictionary<string, KeywordData>();
        foreach (var keyword in db.keywords)
        {
            keywordLookup[keyword.id] = keyword;
        }

        Debug.Log($"Loaded {keywordLookup.Count} keywords.");
    }
    void HandleDialogueSystem()
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
            KeyWordOnPressed();
            return;
        }

        if (currentLine.choices != null && currentLine.nextLineIndex >= 0 && currentLine.choices.Count == 0)
        {
            if (isTyping)
            {
                RevealAllDialogueline(dialogueText);
            }
            else {
                ProceedToNextLine(currentLine.nextLineIndex + currentLineIndex);
            }
            return;
        }

        if (currentLine.nextLineIndex < 0)
        {
            EndDialogue();
            return;
        }
    }

    void ProceedToNextLine(int nextLineIndex)
    {
        if (nextLineIndex > 0 && nextLineIndex < dialogueFile.lines.Count)
        {
            currentLineIndex = nextLineIndex;
            StartDialogue();
            //if (currentLine.type == "dialogue")
            //{
            //    Cutscene.SetActive(false);
            //    DialogueScene.SetActive(true);
            //    StartDialogue();
            //}
        }
        else
        {
            EndDialogue();
        }
    }

    void StartDialogue()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        DialogueLine line = dialogueFile.lines[currentLineIndex];
        dialogueText.text = line.text;
        isTyping = true;
        typingCo = StartCoroutine(ShowDialogueText(dialogueText));

        List<Actor> actors = line.actors;

        if (actors.Count == 0)
        {
            Color bgColor = new Color(0f, 0f, 0f, 0f);
            characterNameText.text = "";
            avartarPanel1.GetComponent<Image>().sprite = null;
            avartarPanel2.GetComponent<Image>().sprite = null;
            avartarPanel1.GetComponent<Image>().color = bgColor;
            avartarPanel2.GetComponent<Image>().color = bgColor;
            return;
        }

        CharacterData character1 = characterData[actors[0].charId];
        CharacterData character2 = characterData[actors[1].charId];

        if (actors.Count == 1 && !actors[0].actFlg || actors.Count == 2 && !actors[0].actFlg && !actors[1].actFlg)
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
            avartarPanel1.GetComponent<Image>().color = !actors[0].actFlg ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(1f, 1f, 1f, 1f);
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
            avartarPanel2.GetComponent<Image>().color = !actors[1].actFlg ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(1f, 1f, 1f, 1f);
        }

        if (line.choices != null && line.choices.Count > 0)
        {
            foreach (DialogueChoice choice in line.choices)
            {
                GameObject choiceObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI btnText = choiceObj.GetComponentInChildren<TextMeshProUGUI>(); // Search choiceObj and all its children, and return the first TextMeshProUGUI component you find.
                btnText.text = choice.text;

                Button btn = choiceObj.GetComponent<Button>();
                btn.onClick.AddListener(() => ProceedToNextLine(choice.nextLineIndex + currentLineIndex));
            }
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

    IEnumerator ShowDialogueText(TextMeshProUGUI TMP_UI)
    {
        TMP_UI.maxVisibleCharacters = 0;
        string fullText = TMP_UI.text;

        for (int i = 0; i < fullText.Length; i++)
        {
            TMP_UI.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    void RevealAllDialogueline(TextMeshProUGUI TMP_UI)
    {
        if (typingCo != null) StopCoroutine(typingCo);
        dialogueText.maxVisibleCharacters = TMP_UI.text.Length;
        isTyping = false;
    }

    void KeyWordOnPressed()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Mouse.current.position.ReadValue(), null);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = dialogueText.textInfo.linkInfo[linkIndex];
            string clickedID = linkInfo.GetLinkID();
            Debug.Log("Clicked on keyword: " + clickedID);

            OnWordClicked(clickedID);
        }
    }
    void OnWordClicked(string id)
    {
        if (!keywordLookup.ContainsKey(id))
        {
            Debug.LogWarning($"No keyword entry found for: {id}");
            return;
        }

        KeywordData keyword = keywordLookup[id];

        switch (keyword.action)
        {
            case "ShowProfile":
                ShowProfile(keyword);
                break;
            case "ShowDetail":
                ShowDetail(keyword);
                break;
            default:
                Debug.Log($"Keyword clicked: {keyword.display}");
                break;
        }
    }

    void ShowProfile(KeywordData data)
    {
        Debug.Log($"Profile: {data.display}\n{data.description}");
    }

    void ShowDetail(KeywordData data)
    {
        Debug.Log($"Tooltip: {data.description}");
    }
}
