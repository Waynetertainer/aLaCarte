using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public eCustomers pType;
    private Character mCharacter;
    private LevelManager mLevelManager;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    private void OnMouseDown()
    {
        if (!(Vector3.Distance(transform.position, mCharacter.transform.position) <= mLevelManager.pCustomerInteractionDistance) || !mLevelManager.TryCarry(pType)) return;
        pType = (eCustomers)GameManager.pInstance.pRandom.Next(2);
        transform.GetChild((int)pType).gameObject.SetActive(true);
        transform.GetChild(1 - (int)pType).gameObject.SetActive(false);
        NET_EventCall eventCall = new NET_EventCall("CustomerTaken");
        eventCall.SetParam("PlayerID", mCharacter.pID);
        eventCall.SetParam("NextType", pType);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        gameObject.SetActive(false);
    }
}