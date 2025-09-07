using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.Rendering;


public class interract : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform panelParent;
    [SerializeField] private GameObject headerText;
    [SerializeField] private GameObject detailText;
    [SerializeField] private GameObject charImg;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TEMPORARY DULICATE THE CODE , OPTIMIZE WILL CONDUCT AFTER DATA STRUCTURE DECIDED!!
        string dataSourcePath;
        if (gameObject.name.Equals("char_panel"))
        {
            // char tab
            dataSourcePath = "_DataSource/character_data";
            TextAsset jsonData = Resources.Load<TextAsset>(dataSourcePath);
            if (jsonData != null)
            {
                CharacterDatabase jsonDataObject = JsonUtility.FromJson<CharacterDatabase>(jsonData.text);
                foreach (CharacterData info in jsonDataObject?.characters ?? new List<CharacterData>())
                {
                    GameObject createdButton = Instantiate(buttonPrefab, panelParent);
                    if (createdButton == null)
                    {
                        Debug.Log("Failed to create Button Object at NoteBook Scene!");
                    }
                    else
                    {
                        createdButton.GetComponentInChildren<TMP_Text>().text = info.name;
                        createdButton.GetComponent<Button>().onClick.AddListener(() => displayMainInfo(info.name, jsonDataObject));
                    }
                }
            }
            else
            {
                print("Confusing??? There was no file at " + dataSourcePath);
            }
        }
        // example other tabs
        else
        {
            dataSourcePath = "_DataSource/keyword_test";
            TextAsset jsonData = Resources.Load<TextAsset>(dataSourcePath);
            if (jsonData != null)
            {
                KeywordDatabase jsonDataObject = JsonUtility.FromJson<KeywordDatabase>(jsonData.text);
                foreach (KeywordData info in jsonDataObject?.keywords ?? new List<KeywordData>())
                {
                    GameObject createdButton = Instantiate(buttonPrefab, panelParent);
                    if (createdButton == null)
                    {
                        Debug.Log("Failed to create Button Object at NoteBook Scene!");
                    }
                    else
                    {
                        createdButton.GetComponentInChildren<TMP_Text>().text = info.display;
                        createdButton.GetComponent<Button>().onClick.AddListener(() => displayMainInfo(info.display, jsonDataObject));
                    }
                }
            }
            else
            {
                print("Confusing??? There was no file at " + dataSourcePath);
            }
        }

    }

    void displayMainInfo<T>(string name, T jsonObject)
    {
        if (jsonObject is CharacterDatabase charJSObj)
        {
            foreach (CharacterData info in charJSObj?.characters ?? new List<CharacterData>())
            {
                if (info.name.Equals(name))
                {
                    string imgFileName = $"Art/Character/{name}/{name}_normal";
                    headerText.GetComponent<TMP_Text>().text = name;
                    detailText.GetComponent<TMP_Text>().text = info.description;

                    charImg.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgFileName);
                }
            }
        }
        // keyword object
        else if (jsonObject is KeywordDatabase keywordJSObj)
        {
            foreach (KeywordData info in keywordJSObj?.keywords ?? new List<KeywordData>())
            {
                if (info.display.Equals(name))
                {
                    //string imgFileName = $"Art/Character/{name}/{name}_normal";
                    headerText.GetComponent<TMP_Text>().text = name;
                    detailText.GetComponent<TMP_Text>().text = info.description;

                    //charImg.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgFileName);
                }
            }
        }

        Debug.Log("Clicked " + name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
