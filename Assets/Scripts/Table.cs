using Assets.Scripts;
using NET_System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public eTableState pState;
    public eCustomers pCustomer;
    public int pID;
    public int pPlayerID;
    public int pSize;
    public float pNextState;
    public GameObject pPanel;
    public eFood[] pOrders;
    public eFood[] pFood;
    public List<Sprite> pFoodImages = new List<Sprite>();

    private Character mCharacter;
    private LevelManager mLevelManager;
    private GameObject mDecal;
    private GameObject mCustomers;
    private float mTip;

    private void Start()
    {
        pOrders = new eFood[pSize];
        pFood = new eFood[pSize];
        pPanel.SetActive(false);
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
        mDecal = transform.GetChild(2).gameObject;
        SetTableState(eTableState.Free);
    }

    private void Update()
    {
        if (pPlayerID != GameManager.pInstance.NetMain.NET_GetPlayerID()) return;
            switch (pState)
        {
            case eTableState.Free:
                break;
            case eTableState.ReadingMenu:
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForOrder);
                }
                break;
            case eTableState.WaitingForOrder:
                //TODO ungeduld
                break;
            case eTableState.WaitingForFood:
                //TODO ungeduld
                break;
            case eTableState.Eating:
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForClean);
                }
                break;
            case eTableState.WaitingForClean:
                //TODO ungeduld
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnMouseDown()
    {
        switch (pState)
        {
            case eTableState.Free:
                break;
            case eTableState.ReadingMenu:
                break;
            case eTableState.WaitingForOrder:
                if (Vector3.Distance(transform.position, mCharacter.transform.position) <= mLevelManager.pTableInteractionDistance)
                {
                    pPanel.SetActive(true);
                    for (int i = 0; i < pSize; i++)
                    {
                        int foodIdentifier = GameManager.pInstance.pRandom.Next(2) + 1;
                        pOrders[i] = (eFood)foodIdentifier;
                        pPanel.transform.GetChild(i).gameObject.SetActive(true);
                        pPanel.transform.GetChild(i).GetComponent<Image>().sprite = pFoodImages[foodIdentifier - 1];
                    }
                    DelegateTableState(eTableState.WaitingForFood);
                }
                break;
            case eTableState.WaitingForFood:
                break;
            case eTableState.Eating:
                break;
            case eTableState.WaitingForClean:
                if (Vector3.Distance(transform.position, mCharacter.transform.position) <= mLevelManager.pTableInteractionDistance)
                {
                    if (mLevelManager.TryCarry(eCarryableType.Dishes))
                    {
                        DelegateTableState(eTableState.Free);
                        mLevelManager.pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1] += mTip;
                    }
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
        if (state == eTableState.Free)
        {
            mDecal.SetActive(false);
        }
        else if (pPlayerID == GameManager.pInstance.NetMain.NET_GetPlayerID())
        {
            mDecal.SetActive(true);
        }
        pState = state;
        switch (state)
        {
            case eTableState.Free:
                DeactivateDishes();
                SetCustomer(false);
                pPlayerID = -1;
                break;
            case eTableState.ReadingMenu:
                SetCustomer(true, pCustomer);
                pNextState = Time.timeSinceLevelLoad + mLevelManager.pReadingMenuTime;
                switch (pCustomer)
                {
                    case eCustomers.Normal:
                        mTip = mLevelManager.pNormalCustomerMultiplicator * pSize;
                        break;
                    case eCustomers.Snob:
                        mTip = mLevelManager.pSnobCustomerMultiplicator * pSize;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case eTableState.WaitingForOrder:
                pPanel.SetActive(true);
                //pTempOrderPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                //pTempOrderPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                break;
            case eTableState.WaitingForFood:
                //pTempOrderPanel.SetActive(false);
                break;
            case eTableState.Eating:
                pPanel.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                if (food != null)
                {
                    for (var i = 0; i < food.Length; i++)
                    {
                        transform.GetChild(0).GetChild(i).GetChild((int)food[i]).gameObject.SetActive(true);//sets one food true
                    }
                }
                pNextState = Time.timeSinceLevelLoad + mLevelManager.pEatingTime;
                break;
            case eTableState.WaitingForClean:
                SetDishes(eFood.None);
                break;
            default:
                throw new ArgumentOutOfRangeException("state", state, null);
        }
    }

    private void SetCustomer(bool active, eCustomers type = eCustomers.Normal)
    {
        transform.GetChild(1).gameObject.SetActive(active);
        transform.GetChild(1).GetChild((int)type).gameObject.SetActive(active);
        transform.GetChild(1).GetChild(((int)type + 1) % 2).gameObject.SetActive(!active);
    }

    private void SetDishes(eFood type)
    {
        transform.GetChild(0).gameObject.SetActive(true);

        for (int j = 0; j < transform.GetChild(0).childCount; j++)
        {
            for (int i = 0; i < transform.GetChild(0).GetChild(j).childCount; i++)
            {
                transform.GetChild(0).GetChild(j).GetChild(i).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < pSize; i++)
        {
            transform.GetChild(0).GetChild(i).GetChild((int)type).gameObject.SetActive(true);
        }
    }

    private void DeactivateDishes()
    {
        transform.GetChild(0).gameObject.SetActive(false);
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

    public bool TryDropFood(eFood? food)
    {
        if (pState != eTableState.WaitingForFood || food == null) return false;
        for (int i = 0; i < pOrders.Length; i++)
        {
            if (pOrders[i] != food) continue;
            pPlayerID = GameManager.pInstance.NetMain.NET_GetPlayerID();
            pFood[i] = pOrders[i];
            pOrders[i] = eFood.None;
            pPanel.transform.GetChild(i).gameObject.SetActive(false);
            if (pOrders.All(p => p == eFood.None))
            {
                DelegateTableState(eTableState.Eating, pFood);
            }
            return true;
        }
        return false;
    }

    public bool TryDropCustomer(eCustomers type)
    {
        if (pState != eTableState.Free) return false;
        pCustomer = type;
        pPlayerID = GameManager.pInstance.NetMain.NET_GetPlayerID();
        DelegateTableState(eTableState.ReadingMenu);
        return true;
    }
}