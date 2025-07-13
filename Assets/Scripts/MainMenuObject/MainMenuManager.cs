using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuButtonPrefab; // Prefab for the main menu button
    public Transform buttonContainerObject; // Container for the main menu buttons
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMainMenuButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMainMenuButton()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject button = Instantiate(mainMenuButtonPrefab, buttonContainerObject);
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
                    btn.onClick.AddListener(() => Debug.Log("Load Game Clicked!"));
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
}
