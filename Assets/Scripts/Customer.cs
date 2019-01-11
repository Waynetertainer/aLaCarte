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
    private LevelManager mLevelManager;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter =mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID()-1];
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))//TODO touch input
        {
            if (Input.GetMouseButtonDown(0) &&
                Vector3.Distance(transform.position, mCharacter.transform.position) <= pTempInteractionDistance &&
                mLevelManager.CanCarry(eCarryableType.Customer))
            {
                NET_EventCall eventCall = new NET_EventCall("CustomerTaken");
                eventCall.SetParam("PlayerID", mCharacter.pID);
                GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
                GameManager.pInstance.pLevelManager.ChangeCarry(eCarryableType.Customer);
                gameObject.SetActive(false);
            }
        }
    }
}
