using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject CanvasObject; // Reference to the Canvas object
    public GameObject mainMenuPanel;
    public GameObject mainMenuButtonContainerPrefab;
    public GameObject mainMenuButtonPrefab; // Prefab for the main menu button
    public GameObject loadGameMenuPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMenuAttribute();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMenuAttribute() {        
        if (CanvasObject == null)
        {
            CanvasObject = GameObject.Find("Canvas");
            if (CanvasObject == null)
            {
                return;
            }
        }
        if (mainMenuPanel == null || mainMenuButtonContainerPrefab == null || mainMenuButtonPrefab == null || loadGameMenuPanel == null)
        {
            Debug.LogError("One or more prefabs are not assigned in the inspector.");
            return;
        }
        GenerateMainMenu();
        GenerateLoadGameMenu();
    }

    void GenerateMainMenu()
    {
        mainMenuButtonContainerPrefab = Instantiate(mainMenuButtonContainerPrefab, mainMenuPanel.transform);
        for (int i = 0; i < 4; i++)
        {
            GameObject button = Instantiate(mainMenuButtonPrefab, mainMenuButtonContainerPrefab.transform);
            TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
            Button btn = button.GetComponent<Button>();
            switch (i)
            {
                case 0:
                    btnText.text = "New Game";
                    btn.onClick.AddListener(() => GameDataManager.NewGame());
                    break;
                case 1:
                    btnText.text = "Load Game";
                    btn.onClick.AddListener(() => OpenLoadGameMenu());
                    break;
                case 2:
                    btnText.text = "Settings";
                    btn.onClick.AddListener(() => Debug.Log("Settings Clicked!"));
                    break;
                case 3:
                    btnText.text = "Exit Game";
                    btn.onClick.AddListener(() => Debug.Log("Exit Game Clicked!"));
                    break;
            }
        }
        mainMenuPanel.SetActive(true);
    }

    void GenerateLoadGameMenu()
    {
        var t = loadGameMenuPanel.transform.Find("TitlePanel/BackButton");
        if (t && t.TryGetComponent(out Button btn))
            btn.onClick.AddListener(CloseLoadGameMenu);
        loadGameMenuPanel.SetActive(false);
    }

    void OpenLoadGameMenu()
    {
        mainMenuPanel.SetActive(false);
        loadGameMenuPanel.SetActive(true);
    }

    public void CloseLoadGameMenu()
    {
        loadGameMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
