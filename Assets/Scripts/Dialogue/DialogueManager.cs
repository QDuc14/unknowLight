using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private DialogueFile dialogueFile;
    private int currentLineIndex = 0;


    void Start()
    {
        LoadDialogue("Dialogue/sample_dialogue");
        DisplayLine();
    }

    void Update()
    {
        if (dialogueFile.lines[currentLineIndex].choices != null && dialogueFile.lines[currentLineIndex].choices.Count == 0 && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnChoiceSelected(currentLineIndex + 1);
        }
    }

    void LoadDialogue(string path)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        dialogueFile = JsonUtility.FromJson<DialogueFile>(jsonText.text);
    }

    void DisplayLine()
    {
        // Clear previous choices
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        DialogueLine line = dialogueFile.lines[currentLineIndex];
        speakerNameText.text = line.speaker;
        dialogueText.text = line.text;

        if (line.choices != null && line.choices.Count > 0)
        {
            foreach (DialogueChoice choice in line.choices)
            {
                GameObject choiceObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI btnText = choiceObj.GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = choice.text;

                Button btn = choiceObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnChoiceSelected(choice.nextLineIndex));
            }
        }
    }


    void OnChoiceSelected(int nextIndex)
    {
        if (nextIndex > 0 && nextIndex < dialogueFile.lines.Count)
        {
            currentLineIndex = nextIndex;
            DisplayLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        speakerNameText.text = "";
        dialogueText.text = "The End.";
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
