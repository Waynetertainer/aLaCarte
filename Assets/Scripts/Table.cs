using Assets.Scripts;
using NET_System;
using System;
using System.Collections;
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
    public OrderPanel pOrderPanel;
    public eFood[] pOrders;
    public eFood[] pFood;
    public bool pStealable;


    private Character mCharacter;
    private LevelManager mLevelManager;
    private GameObject mDecal;
    private GameObject mCustomers;
    private float mTip;
    private eSymbol mPreviousSymbol;
    private Image mStatisfactionBar;
    private float mWaitingTime;
    private float mMaxWaitingTime;
    private bool mStealableSended = false;

    private void Start()
    {
        mStatisfactionBar = pPanel.transform.GetChild(0).GetChild(3).GetComponent<Image>();
        pOrders = new eFood[pSize];
        pFood = new eFood[pSize];
        pPanel.SetActive(false);
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
        mDecal = transform.GetChild(2).gameObject;
        SetTableState(eTableState.Free);
        mMaxWaitingTime = mLevelManager.pOrderWaitIntervall + mLevelManager.pFoodWaitIntervall + mLevelManager.pCleanWaitIntervall;
    }

    private void Update()
    {
        switch (pState)
        {
            case eTableState.Free:
                break;
            case eTableState.ReadingMenu:
                if (pPlayerID != GameManager.pInstance.NetMain.NET_GetPlayerID()) return;
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForOrder);
                }
                break;
            case eTableState.WaitingForOrder:
                Displeasement();
                break;
            case eTableState.WaitingForFood:
                Displeasement();
                break;
            case eTableState.Eating:
                if (pPlayerID != GameManager.pInstance.NetMain.NET_GetPlayerID()) return;
                if (Time.timeSinceLevelLoad >= pNextState)
                {
                    DelegateTableState(eTableState.WaitingForClean);
                }
                break;
            case eTableState.WaitingForClean:
                Displeasement();
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
                if (Vector3.Distance(transform.position, mCharacter.transform.position) <= mLevelManager.pTableInteractionDistance && (pPlayerID == mCharacter.pID || pStealable))
                {
                    if (pPlayerID != mCharacter.pID)//&&pStealable
                    {
                        Steal();
                    }
                    pOrderPanel.gameObject.SetActive(true);
                    for (int i = 0; i < pSize; i++)
                    {
                        int foodIdentifier = GameManager.pInstance.pRandom.Next(mLevelManager.pFoodAmountInLevel) + 1;
                        pOrders[i] = (eFood)foodIdentifier;
                    }
                    NET_EventCall eventCall = new NET_EventCall("UpdateOrders");
                    eventCall.SetParam("TableID", pID);
                    eventCall.SetParam("Orders", pOrders);
                    eventCall.SetParam("Food", pFood);
                    GameManager.pInstance.NetMain.NET_CallEvent(eventCall);

                    pOrderPanel.ChangeTab(pID);
                    StartCoroutine(FrameDelayer());
                    DelegateTableState(eTableState.WaitingForFood);
                }
                break;
            case eTableState.WaitingForFood:
                if (pStealable && pPlayerID != mCharacter.pID)
                {
                    Steal(true);
                }
                break;
            case eTableState.Eating:
                break;
            case eTableState.WaitingForClean:
                if (Vector3.Distance(transform.position, mCharacter.transform.position) <= mLevelManager.pTableInteractionDistance && (pPlayerID == mCharacter.pID || pStealable))
                {
                    if (mLevelManager.TryCarry())
                    {
                        if (pPlayerID != mCharacter.pID)//&&pStealable
                        {
                            Steal();
                        }
                        DelegateTableState(eTableState.Free);
                        mLevelManager.pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1] += mTip * mStatisfactionBar.fillAmount;
                        NET_EventCall eventCall = new NET_EventCall("UpdateScore");
                        eventCall.SetParam("Tip", mLevelManager.pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1]);
                        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
                    }
                    else
                    {
                        ActivateSymbol(eSymbol.Failure, false);
                        StartCoroutine(SymbolFeedback());
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Displeasement()
    {
        mWaitingTime += Time.deltaTime;
        mStatisfactionBar.fillAmount = 1 - (mWaitingTime / mMaxWaitingTime);
        if (mStatisfactionBar.fillAmount <= 2f / 3)
        {
            if (mStatisfactionBar.fillAmount <= 1f / 3)
            {
                if (mStatisfactionBar.fillAmount <= 0)
                {
                    DelegateTableState(eTableState.Free);
                }
                else
                {
                    mStatisfactionBar.color = mLevelManager.pRed;
                    if (!mStealableSended && pPlayerID == GameManager.pInstance.NetMain.NET_GetPlayerID())
                    {
                        mStealableSended = true;
                        NET_EventCall eventCall = new NET_EventCall("TableStealable");
                        eventCall.SetParam("TableID", pID);
                        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
                        Debug.Log("sent table stealable");
                    }
                }
            }
            else
            {
                mStatisfactionBar.color = mLevelManager.pYellow;
            }
        }
    }

    public void DelegateTableState(eTableState state, eFood[] food = null)
    {
        SetTableState(state, food);
        SendTableState(state, food);
    }

    public void SetTableState(eTableState state, eFood[] food = null)
    {
        mDecal.SetActive(state != eTableState.Free && pPlayerID == GameManager.pInstance.NetMain.NET_GetPlayerID());
        pState = state;

        switch (state)
        {
            case eTableState.Free:
                pPanel.SetActive(false);
                DeactivateDishes();
                SetCustomer(false);
                pPlayerID = -1;
                mStatisfactionBar.color = mLevelManager.pGreen;
                mWaitingTime = 0;
                mStealableSended = false;
                break;
            case eTableState.ReadingMenu:
                SetCustomer(true, pCustomer);
                pNextState = Time.timeSinceLevelLoad + mLevelManager.pReadingMenuTime;
                float sizeMultiplicator =pSize == 2 ? mLevelManager.pTwoTableMultiplicator : mLevelManager.pFourTableMultiplicator;
                switch (pCustomer)
                {
                    case eCustomers.Normal:
                        mTip = mLevelManager.pNormalCustomerMultiplicator * sizeMultiplicator;
                        break;
                    case eCustomers.Snob:
                        mTip = mLevelManager.pSnobCustomerMultiplicator * sizeMultiplicator;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case eTableState.WaitingForOrder:
                ActivateSymbol(eSymbol.ExclamationMark, true);
                if (pPlayerID == GameManager.pInstance.NetMain.NET_GetPlayerID())
                {
                    pPanel.SetActive(true);
                }
                break;
            case eTableState.WaitingForFood:
                ActivateSymbol(eSymbol.ServingDome, true);
                break;
            case eTableState.Eating:
                pPanel.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
                if (food != null)
                {
                    for (var i = 0; i < pFood.Length; i++)
                    {
                        transform.GetChild(0).GetChild(i).GetChild((int)food[i]).gameObject.SetActive(true);//sets one food true
                    }
                }
                pNextState = Time.timeSinceLevelLoad + mLevelManager.pEatingTime;
                break;
            case eTableState.WaitingForClean:
                ActivateSymbol(eSymbol.Euro, true);
                SetDishes(eFood.None);
                if (pPlayerID == GameManager.pInstance.NetMain.NET_GetPlayerID())
                {
                    pPanel.SetActive(true);
                }
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
        for (int j = 0; j < transform.GetChild(0).childCount; j++)
        {
            for (int i = 0; i < transform.GetChild(0).GetChild(j).childCount; i++)
            {
                transform.GetChild(0).GetChild(j).GetChild(i).gameObject.SetActive(false);
            }
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

    private void ActivateSymbol(eSymbol symbol, bool save)
    {
        foreach (Transform child in pPanel.transform.GetChild(0).transform)
        {
            child.gameObject.SetActive(false);
        }
        mStatisfactionBar.gameObject.SetActive(true);
        pPanel.transform.GetChild(0).GetChild((int)symbol).gameObject.SetActive(true);
        if (save)
        {
            mPreviousSymbol = symbol;
        }
    }

    IEnumerator SymbolFeedback()
    {
        yield return new WaitForSeconds(mLevelManager.pSymbolFeedbackDuration);
        ActivateSymbol(mPreviousSymbol, true);
    }

    private IEnumerator FrameDelayer()
    {
        yield return null;
        pOrderPanel.gameObject.SetActive(true);
    }

    public bool TryDropFood(eFood? food)
    {
        if (pState != eTableState.WaitingForFood || food == null) return false;
        if (pPlayerID == mCharacter.pID || pStealable)
        {
            if (pStealable)
            {
                Steal();
            }
            pPlayerID = GameManager.pInstance.NetMain.NET_GetPlayerID();
            for (int i = 0; i < pOrders.Length; i++)
            {
                //Das macht Sinn, trust me!
                if (pOrders[i] != food) continue;
                pFood[i] = pOrders[i];
                pOrders[i] = eFood.None;
                if (pOrders.All(p => p == eFood.None))
                {
                    DelegateTableState(eTableState.Eating, pFood);
                }

                ActivateSymbol(eSymbol.Success, false);
                StartCoroutine(SymbolFeedback());
                NET_EventCall eventCall = new NET_EventCall("UpdateOrders");
                eventCall.SetParam("TableID", pID);
                eventCall.SetParam("Orders", pOrders);
                eventCall.SetParam("Food", pFood);
                GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
                return true;
            }
        }
        ActivateSymbol(eSymbol.Failure, false);
        StartCoroutine(SymbolFeedback());
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

    public void SetStealable()
    {
        pStealable = true;
        if (pState == eTableState.WaitingForOrder || pState == eTableState.WaitingForFood || pState == eTableState.WaitingForClean)
        {
            pPanel.SetActive(true);
        }
    }

    private void Steal(bool iWillComeSoon = false)
    {
        if (iWillComeSoon)
        {
            pOrderPanel.gameObject.SetActive(true);
            pOrderPanel.ChangeTab(pID);
            StartCoroutine(FrameDelayer());
            mWaitingTime = mMaxWaitingTime / 3;
        }
        else
        {
            mStatisfactionBar.color = mLevelManager.pGreen;
            mWaitingTime = 0;
        }
        mStealableSended = false;

        pStealable = false;
        pPlayerID = GameManager.pInstance.NetMain.NET_GetPlayerID();
        NET_EventCall eventCall = new NET_EventCall("TableStolen");
        eventCall.SetParam("TableID", pID);
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        Debug.Log("sent table stolen");
    }

    public void Stolen(int id)
    {
        pPlayerID = id;
        pPanel.SetActive(false);
        mStatisfactionBar.color = mLevelManager.pGreen;
        mWaitingTime = 0;
        mStealableSended = false;
        pStealable = false;
    }
}