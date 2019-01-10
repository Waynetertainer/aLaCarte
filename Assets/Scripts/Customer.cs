using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float pTempInteractionDistance;

    private Character mCharacter;

    private void Start()
    {
        mCharacter =GameManager.pInstance.pLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID()-1];
    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) &&
            Vector3.Distance(transform.position, mCharacter.transform.position) <= pTempInteractionDistance &&
            mCharacter.pCarrying.All(p => p == eCarryableType.Empty))
        {

            NET_EventCall eventCall = new NET_EventCall("CustomerTaken");
            eventCall.SetParam("PlayerID", mCharacter.pID);
            GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
            TakeCustomer(mCharacter.pID-1);
        }
    }

    public void TakeCustomer(int id)
    {
        GameManager.pInstance.pLevelManager.pCharacters[id].ChangeCarry(eCarryableType.Customer);
        gameObject.SetActive(false);
    }
}
