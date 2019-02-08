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
    public Table[] pTables = new Table[5];
    public GameObject[] pOrders = new GameObject[4];

    private GameObject mActiveTab;

    private void Start()
    {
        pTables = GameManager.pInstance.pLevelManager.pTables;
    }

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
        ChangeTab(tableID);
        foreach (GameObject order in pOrders)
        {
            foreach (Transform childTransform in order.transform)
            {
                childTransform.gameObject.SetActive(false);
            }
            order.SetActive(false);
        }
        for (int i = 0; i < pTables[tableID].pOrders.Length; i++)
        {
            pOrders[i].SetActive(true);
            pOrders[i].transform.GetChild((int)pTables[tableID].pOrders[i]).gameObject.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (gameObject.activeSelf)
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
