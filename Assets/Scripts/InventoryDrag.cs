using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using NET_System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrag : MonoBehaviour
{
    private Character mCharacter;
    private LevelManager mLevelManager;
    private GameObject mDragging;
    private Vector3 mDraggedStartPosition;
    private eCarryableType mCarryableType;

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
            mCarryableType = mDragging.GetComponent<Carryable>().pType;
        }
    }

    public void EndDrag(GameObject obj)
    {
        mLevelManager.pDragging = false;
        Ray ray = Camera.main.ScreenPointToRay(mDragging.transform.position);
        RaycastHit hit;
        if (mCarryableType == eCarryableType.Pizza || mCarryableType == eCarryableType.Pasta)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
            {
                Table table = hit.transform.GetComponentInChildren<Table>();
                if (table.pPlayerID == mCharacter.pID && table.pState == eTableState.WaitingForFood && Vector3.Distance(mCharacter.transform.position, table.transform.position) <= 2)
                {
                    table.DelegateTableState(eTableState.Eating, mCarryableType);
                    mDragging.gameObject.SetActive(false);
                    mDragging.transform.parent.gameObject.SetActive(false);
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12) && Vector3.Distance(mCharacter.transform.position, hit.transform.position) <= 2)
            {
                mDragging.gameObject.SetActive(false);
                mDragging.transform.parent.gameObject.SetActive(false);

            }
        }
        else if (mCarryableType == eCarryableType.Dishes && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12))
        {
            mDragging.gameObject.SetActive(false);
        }
        else if (mCarryableType == eCarryableType.Customer)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
            {
                Table table = hit.transform.GetComponentInChildren<Table>();
                if (table.pState == eTableState.Free && Vector3.Distance(mCharacter.transform.position, table.transform.position) <= 2)
                {
                    table.DelegateTableState(eTableState.ReadingMenu, mCarryableType);
                    table.pPlayerID = mCharacter.pID;
                    mDragging.gameObject.SetActive(false);
                    mLevelManager.pCustomerWaitingOrMoving = false;
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12) && Vector3.Distance(mCharacter.transform.position, hit.transform.position) <= 2)
            {
                mDragging.gameObject.SetActive(false);
                mLevelManager.pCustomerWaitingOrMoving = false;
            }
        }
        mDragging.transform.position = mDraggedStartPosition;

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
