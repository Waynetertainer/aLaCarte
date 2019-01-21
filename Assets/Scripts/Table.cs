using Assets.Scripts;
using NET_System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Table : MonoBehaviour
{
    public eTableState pState;
    public int pID;
    public int pPlayerID;
    public int pSize;
    public float pNextState;
    public GameObject pTempOrderPanel;
    public eFood[] pOrders;
    public eFood[] pFood;
    public List<Sprite> pFoodImages = new List<Sprite>();
    private Character mCharacter;
    private LevelManager mLevelManager;
    private bool mIsHost;
    private GameObject mPanel;

    private void Start()
    {
        pOrders = new eFood[pSize];
        pFood = new eFood[pSize];
        mPanel = transform.GetChild(2).GetChild(0).gameObject;
        mPanel.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        mPanel.SetActive(false);
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
        SetTableState(eTableState.Free);
        mIsHost = mLevelManager.pIsHost;
    }

    private void Update()
    {
        switch (pState)
        {
            case eTableState.Free:
                break;
            case eTableState.ReadingMenu:
                if (!mIsHost) return;
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForOrder);
                }

                break;
            case eTableState.WaitingForOrder:
                if (Input.GetMouseButtonDown(0) &&
                    Vector3.Distance(transform.position, mCharacter.transform.position) <= 2) //TODO make fpr GD
                {
                    mPanel.SetActive(true);
                    for (int i = 0; i < pSize; i++)
                    {
                        int foodIdentifier = GameManager.pInstance.pRandom.Next(2) + 1;
                        pOrders[i] = (eFood)foodIdentifier;
                        mPanel.transform.GetChild(i).gameObject.SetActive(true);
                        mPanel.transform.GetChild(i).GetComponent<Image>().sprite = pFoodImages[foodIdentifier - 1];
                    }

                    // Show Order
                    DelegateTableState(eTableState.WaitingForFood);
                }

                break;
            case eTableState.WaitingForFood:
                break;
            case eTableState.Eating:
                if (!mIsHost) return;
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForClean);
                }

                break;
            case eTableState.WaitingForClean:
                if (Input.GetMouseButtonDown(0) &&
                    Vector3.Distance(transform.position, mCharacter.transform.position) <= 2) //TODO make fpr GD
                {
                    if (mLevelManager.TryCarry(eCarryableType.Dishes))
                    {
                        DelegateTableState(eTableState.Free);
                    }

                    //TODO AddMoney
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DelegateTableState(eTableState state, eFood[] food = null)
    {
        SetTableState(state, food);
        SendTableState(state, food);
    }

    public void SetTableState(eTableState state, eFood[] food = null)
    {
        pState = state;
        switch (state)
        {
            case eTableState.Free:
                transform.GetChild(0).gameObject.SetActive(false);
                for (int j = 0; j < transform.GetChild(0).childCount; j++)
                {
                    for (int i = 0; i < transform.GetChild(0).GetChild(j).childCount; i++)
                    {
                        transform.GetChild(0).GetChild(j).GetChild(i).gameObject.SetActive(false);
                    }
                }

                transform.GetChild(1).gameObject.SetActive(false);
                pPlayerID = -1;
                break;
            case eTableState.ReadingMenu:
                transform.GetChild(1).gameObject.SetActive(true);
                pNextState = Time.timeSinceLevelLoad + 8;
                break;
            case eTableState.WaitingForOrder:
                //TODO: ungeduld
                pTempOrderPanel.SetActive(true);
                pTempOrderPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                pTempOrderPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                break;
            case eTableState.WaitingForFood:
                //TODO: ungeduld
                pTempOrderPanel.SetActive(false);
                break;
            case eTableState.Eating:
                mPanel.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                if (food != null)
                {
                    for (var i = 0; i < food.Length; i++)
                    {
                        transform.GetChild(0).GetChild(i).GetChild((int)food[i]).gameObject.SetActive(true);
                    }
                }

                pNextState = Time.timeSinceLevelLoad + 8;
                break;
            case eTableState.WaitingForClean:
                for (int j = 0; j < transform.GetChild(0).childCount; j++)
                {
                    for (int i = 0; i < transform.GetChild(0).GetChild(j).childCount; i++)
                    {
                        transform.GetChild(0).GetChild(j).GetChild(i).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < pSize; i++)
                {
                    transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);

                }

                break;
            default:
                throw new ArgumentOutOfRangeException("state", state, null);
        }
    }



    private void SendTableState(eTableState state, eFood[] food = null)
    {
        NET_EventCall eventCall = new NET_EventCall("SetTableState");
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        eventCall.SetParam("TableID", pID);
        eventCall.SetParam("State", pState);
        if (pState == eTableState.Eating)
        {
            eventCall.SetParam("Food", food);
        }

        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
    }

    public void DeactivateDishes()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool TryDrop(eCarryableType type, eFood? food = null)
    {
        switch (type)
        {
            case eCarryableType.Customer:
                if (pState != eTableState.Free) return false;
                DelegateTableState(eTableState.ReadingMenu);
                return true;
            case eCarryableType.Food:
                if (pState != eTableState.WaitingForFood || food == null) return false;
                for (int i = 0; i < pOrders.Length; i++)
                {
                    if (pOrders[i] != food) continue;
                    pFood[i] = pOrders[i];
                    pOrders[i] = eFood.None;
                    mPanel.transform.GetChild(i).gameObject.SetActive(false);
                    if (pOrders.All(p => p == eFood.None))
                    {
                        DelegateTableState(eTableState.Eating, pFood);
                    }

                    //TODO adjust panel
                    return true;
                }

                return false;
        }

        return false;
    }
}