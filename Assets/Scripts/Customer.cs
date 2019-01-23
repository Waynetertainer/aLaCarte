using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float pTempInteractionDistance;//TODO GD

    private Character mCharacter;
    private LevelManager mLevelManager;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    private void OnMouseDown()
    {
        if (!(Vector3.Distance(transform.position, mCharacter.transform.position) <= pTempInteractionDistance) || !mLevelManager.TryCarry(eCarryableType.Customer)) return;
        NET_EventCall eventCall = new NET_EventCall("CustomerTaken");
        eventCall.SetParam("PlayerID", mCharacter.pID);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        gameObject.SetActive(false);
    }
}