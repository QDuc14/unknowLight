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
    [System.Serializable]
    protected class charInfomation
    {
        public string name;
        public string description;
    }

   [System.Serializable]
    protected class wrapperGetJsonData
    {
        public List<charInfomation> characters;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("noteData");
        wrapperGetJsonData jsonDataObject = JsonUtility.FromJson<wrapperGetJsonData>(jsonData.text);
        if (jsonData != null)
        {
            foreach (charInfomation info in jsonDataObject?.characters ?? new List<charInfomation>())
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
    }

    void displayMainInfo(string name, wrapperGetJsonData jsonObject)
    {
        //headerText.GetComponent<TMP_Text>().text = name;
        //detailText.GetComponent<TMP_Text>().text = 
        foreach (charInfomation info in jsonObject?.characters ?? new List<charInfomation>())
        {
            if (info.name.Equals(name))
            {
                headerText.GetComponent<TMP_Text>().text = name;
                detailText.GetComponent<TMP_Text>().text = info.description;
            }
        }
        Debug.Log("Clicked " + name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
