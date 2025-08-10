using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class interract : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform panelParent;

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
    // List<charInfomation> list = new List<charInfomation>
    // {
    //     new charInfomation
    //     {
    //         name = "Thanh",
    //         description = "Dep Trai"
    //     },
    //     new charInfomation
    //     {
    //         name = "Teo",
    //         description = "Xau trai"
    //     }
    // };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("noteData");
        if (jsonData != null)
        {
            wrapperGetJsonData jsonDataObject = JsonUtility.FromJson<wrapperGetJsonData>(jsonData.text);
            foreach (charInfomation info in jsonDataObject?.characters ?? new List<charInfomation>())
                {
                    GameObject createdButton = Instantiate(buttonPrefab, panelParent);
                    if (createdButton?.GetComponentInChildren<TMP_Text>() == null)
                    {
                        Debug.Log("Failed to create Button Object at NoteBook Scene!");
                    }
                    else
                    {
                        createdButton.GetComponentInChildren<TMP_Text>().text = info.name;
                    }
                }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
