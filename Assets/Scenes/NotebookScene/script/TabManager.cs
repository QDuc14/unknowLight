using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public List<GameObject> tabContentList;
    public List<Button> tabList;

    public void tabOnClick(int tabIndex)
    {
        resetState();
        tabContentList[tabIndex].SetActive(true);
    }

    void resetState()
    {
        foreach (GameObject tabContent in tabContentList)
        {
            tabContent.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
