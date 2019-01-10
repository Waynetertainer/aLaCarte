using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrag : MonoBehaviour
{
    private Character mCharacter;
    public GameObject pPizzaRight;
    public GameObject pPastaRight;
    public GameObject pCutomer;
    public GameObject pDishesRight;
    public GameObject pPizzaLeft;
    public GameObject pPastaLeft;
    public GameObject pDishesLeft;

    private void Start()
    {
        EventTrigger trigger = pPizzaRight.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { BeginDrag(pPizzaRight);Debug.Log("begin drag"); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((data) => { EndDrag(pPizzaRight); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { Drag(pPizzaRight); });
        trigger.triggers.Add(entry);

        mCharacter = GameManager.pInstance.pLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    public void BeginDrag(GameObject obj)
    {
        Debug.Log("BeginDrag");
    }

    public void EndDrag(GameObject obj)
    {
        Debug.Log("EndDrag");

    }

    public void Drag(GameObject obj)
    {
        Debug.Log("Drag");

    }
}
