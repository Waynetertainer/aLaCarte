using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : MonoBehaviour
{
    public Color pTextActive;
    public Color pTextPassive;
    public GameObject[] pTabs = new GameObject[5];
    public GameObject[] pTableSprites = new GameObject[5];
    public Table[] pTables=new Table[5];

    private GameObject mActiveTab;

    public void ChangeTab(int tabNumber)
    {
        foreach (GameObject tab in pTabs)
        {
            tab.transform.GetChild(0).gameObject.SetActive(false);
            tab.transform.GetChild(1).GetComponent<Text>().color = pTextPassive;
        }
        pTabs[tabNumber].transform.GetChild(0).gameObject.SetActive(true);
        pTabs[tabNumber].transform.GetChild(1).GetComponent<Text>().color = pTextActive;
    }

    public void ShowOrder(int tableID)
    {

    }

    public void ClosePanel()
    {
        if(gameObject.activeSelf)
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 13))
            {
                //ClosePanel();
            }
        }
    }
}
