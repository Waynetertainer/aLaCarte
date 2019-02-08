using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private GraphicRaycaster mRaycaster;
    private PointerEventData mPointerEventData;
    private EventSystem mEventSystem;

    private void Start()
    {
        pTables = GameManager.pInstance.pLevelManager.pTables;
        mRaycaster = GetComponent<GraphicRaycaster>();
        mEventSystem = GetComponent<EventSystem>();
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
        foreach (GameObject order in pOrders)
        {
            foreach (Transform childTransform in order.transform)
            {
                childTransform.gameObject.SetActive(false);
            }
            order.SetActive(false);
        }
        for (int i = 0; i < pTables[tabNumber].pOrders.Length; i++)
        {
            pOrders[i].SetActive(true);
            pOrders[i].transform.GetChild((int)pTables[tabNumber].pOrders[i] - 1).gameObject.SetActive(true);
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
            mPointerEventData = new PointerEventData(mEventSystem) {position = Input.mousePosition};

            List<RaycastResult> results = new List<RaycastResult>();

            mRaycaster.Raycast(mPointerEventData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
                Debug.Log("Hit " + result.gameObject.layer);
            }

            if (results.All(p => p.gameObject.layer != 13))
            {
                ClosePanel();
            }
        }
    }
}
