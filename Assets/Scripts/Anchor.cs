using System;
using System.Collections;
using System.Collections.Generic;
using NET_System;
using UnityEngine;


public class Anchor : MonoBehaviour
{
    private LevelManager mLevelManager;
    private Vector3 mPosition;

    private void Start()
    {
        mPosition = transform.position;
        mLevelManager = GameManager.pInstance.pLevelManager;
    }

    private void Update()
    {
        NET_EventCall eventCall = new NET_EventCall("UpdateAnchorPosition");
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        eventCall.SetParam("PositionX", transform.position.x);
        eventCall.SetParam("PositionY", transform.position.y);
        eventCall.SetParam("PositionZ", transform.position.z);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        Debug.Log("sending " + transform.position + " for player " + (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1));
        mLevelManager.pNavMeshTargets[(int) eventCall.GetParam("PlayerID") - 1].transform.position = transform.position;

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
        if (!Input.GetMouseButton(0)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
        {
            transform.position = hit.point + new Vector3(0, 1, 0);
        }
#endif
    }
}
