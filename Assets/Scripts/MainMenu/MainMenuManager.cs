using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject CanvasObject; // Reference to the Canvas object
    public GameObject mainMenuPanelPrefab;
    public GameObject mainMenuButtonContainerPrefab;
    public GameObject mainMenuButtonPrefab; // Prefab for the main menu button
    public GameObject loadGameMenuPanelPrefab;
    public GameObject loadGameMenuContainerPrefab; // Prefab for the main menu button container

    public GameObject BackButtonPrefab; // Prefab for the load game button

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
        if (mainMenuPanelPrefab == null || mainMenuButtonContainerPrefab == null || mainMenuButtonPrefab == null || loadGameMenuPanelPrefab == null || loadGameMenuContainerPrefab == null || BackButtonPrefab == null)
        {
            Debug.LogError("One or more prefabs are not assigned in the inspector.");
            return;
        }
        GenerateMainMenu();
        GenerateLoadGameMenu();
    }

    void GenerateMainMenu()
    {
        mainMenuPanelPrefab = Instantiate(mainMenuPanelPrefab, CanvasObject.transform);
        mainMenuButtonContainerPrefab = Instantiate(mainMenuButtonContainerPrefab, mainMenuPanelPrefab.transform);
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
    }

    void GenerateLoadGameMenu()
    {
        loadGameMenuPanelPrefab = Instantiate(loadGameMenuPanelPrefab, CanvasObject.transform);
        BackButtonPrefab = Instantiate(BackButtonPrefab, loadGameMenuPanelPrefab.transform);
        BackButtonPrefab.GetComponent<Button>().onClick.AddListener(() => CloseLoadGameMenu());
        loadGameMenuPanelPrefab.SetActive(false);
    }

    void OpenLoadGameMenu()
    {
        mainMenuPanelPrefab.SetActive(false);
        loadGameMenuPanelPrefab.SetActive(true);
    }

    public void CloseLoadGameMenu()
    {
        loadGameMenuPanelPrefab.SetActive(false);
        mainMenuPanelPrefab.SetActive(true);
    }
}
