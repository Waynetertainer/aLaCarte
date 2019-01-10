using System;
using System.Collections;
using System.Collections.Generic;
using NET_System;
using UnityEngine;


public class Anchor : MonoBehaviour
{
    private LevelManager mLevelManager;
    private Vector3 mPosition;
    private bool mSendedStop;

    private void Start()
    {
        mPosition = transform.position;
        mLevelManager = GameManager.pInstance.pLevelManager;
    }

    private void Update()
    {

#if ANDROID
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
            {
                transform.position = hit.point + new Vector3(0, 1, 0);
            }
        }
#else
        if (Input.GetMouseButton(0))
        {
            if (!mLevelManager.pDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
                {
                    Debug.Log(hit.transform.gameObject.name);
                    return;
                }
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9)&&!Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
                {
                    transform.position = hit.point + new Vector3(0, 1, 0);
                    SendPosition();
                    mSendedStop = false;
                }
            }
        }
        else
        {
            if (mSendedStop) return;
            SendStop();
            mSendedStop = true;
        }
#endif
    }

    private void SendPosition()
    {
        int playerID = GameManager.pInstance.NetMain.NET_GetPlayerID();
        NET_EventCall eventCall = new NET_EventCall("UpdateAnchorPosition");
        eventCall.SetParam("PlayerID", playerID);
        eventCall.SetParam("PositionX", transform.position.x);
        eventCall.SetParam("PositionY", transform.position.y);
        eventCall.SetParam("PositionZ", transform.position.z);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        Debug.Log("sending " + transform.position + " for player " + (playerID - 1));
        mLevelManager.pNavMeshTargets[playerID - 1].transform.position = transform.position;
        mLevelManager.pCharacters[playerID - 1].SetTargetPosition(transform.position);
        mLevelManager.pCharacters[playerID - 1].Move(true);
    }

    private void SendStop()
    {
        NET_EventCall eventCall = new NET_EventCall("StopMoving");
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        mLevelManager.pCharacters[(int)eventCall.GetParam("PlayerID") - 1].Move(false);
    }
}
