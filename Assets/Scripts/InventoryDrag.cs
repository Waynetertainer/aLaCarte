using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrag : MonoBehaviour
{
    private Character mCharacter;
    private LevelManager mLevelManager;
    private GameObject mDragging;
    private Vector3 mDraggedStartPosition;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    public void BeginDrag(GameObject obj)
    {
        if (!mLevelManager.pDragging)
        {
            mLevelManager.pDragging = true;
            mDragging = obj;
            mDraggedStartPosition = mDragging.transform.position;
        }
    }

    public void EndDrag(GameObject obj)
    {
        mLevelManager.pDragging = false;
        Ray ray = Camera.main.ScreenPointToRay(mDragging.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
        {
            Debug.Log("Dropped dish");
        }
        else
        {
            mDragging.transform.position = mDraggedStartPosition;
        }
        mDragging = null;

    }

    private void Update()
    {
        if (mLevelManager.pDragging && mDragging != null)
        {
            mDragging.transform.position = Input.mousePosition;//TODO touch input
        }
    }
}
