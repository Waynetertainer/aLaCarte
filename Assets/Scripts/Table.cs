﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Table : MonoBehaviour
{
    public eTableState pState;
    public int pID;
    public int? pPlayerID;
    public int pSize;
    public float pNextState;
    private Character mCharacter;
    private LevelManager mLevelManager;
    private bool mIsHost;

    private void Start()
    {
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
                    Vector3.Distance(transform.position, mCharacter.transform.position) <= 2 && //TODO make fpr GD
                    mLevelManager.CanCarry(eCarryableType.Dishes))
                {
                    mLevelManager.ChangeCarry(eCarryableType.Dishes);
                    DelegateTableState(eTableState.Free);
                    //TODO AddMoney
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DelegateTableState(eTableState state, eCarryableType? type = null)
    {
        SetTableState(state, type);
        SendTableState(state, type);
    }

    public void SetTableState(eTableState state, eCarryableType? type = null)
    {
        pState = state;
        switch (state)
        {
            case eTableState.Free:
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                pPlayerID = null;
                break;
            case eTableState.ReadingMenu:
                transform.GetChild(1).gameObject.SetActive(true);
                pNextState = Time.timeSinceLevelLoad + 4;
                break;
            case eTableState.WaitingForOrder:
                //TODO: ungeduld

                break;
            case eTableState.WaitingForFood:
                //TODO: ungeduld
                break;
            case eTableState.Eating:
                switch (type)
                {
                    case eCarryableType.Pizza:
                        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        break;
                    case eCarryableType.Pasta:
                        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
                pNextState = Time.timeSinceLevelLoad + 4;
                break;
            case eTableState.WaitingForClean:
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

                break;
            default:
                throw new ArgumentOutOfRangeException("state", state, null);
        }
    }

    private void SendTableState(eTableState state, eCarryableType? carryableType = null)
    {
        NET_EventCall eventCall = new NET_EventCall("SetTableState");
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        eventCall.SetParam("TableID", pID);
        eventCall.SetParam("State", pState);
        if (pState == eTableState.Eating)
        {
            eventCall.SetParam("Carryable", carryableType);
        }
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
    }

    public void DeactivateDishes()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }



    //[HideInInspector] public GameObject pPanel;
    //private Image mSprite;
    //private Player mPlayer;

    //private void Start()
    //{
    //    mSprite = pPanel.transform.GetChild(0).GetComponent<Image>();
    //    mPlayer = FindObjectOfType<Player>();

    //    pState = eTableState.Free;
    //    mNextOrder = GameManager.pInstance.pRandom.Next(pEarliestFirstOrder, pLatestFirstOrder);
    //    transform.GetChild(0).gameObject.SetActive(false);
    //}

    //private void Update()
    //{
    //    switch (pState)
    //    {
    //        case eTableState.Free:
    //            if (Time.timeSinceLevelLoad >= mNextOrder)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                pDesire = (eDishes)(GameManager.pInstance.pRandom.Next(2) + 1);
    //                mStatisfaction = eStatisfaction.Good;
    //                pState = eTableState.ReadingMenu;
    //                mSprite.sprite = GameManager.pInstance.pEmotionSprites[0];
    //                transform.GetChild(0).gameObject.SetActive(true);

    //            }
    //            break;
    //        case eTableState.ReadingMenu:
    //            if (Time.timeSinceLevelLoad - mTimestampWaiting > pPatienceTime)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                if (mStatisfaction == eStatisfaction.Angry)
    //                {
    //                    mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    //                    transform.GetChild(0).gameObject.SetActive(false);
    //                    pState = eTableState.Free;
    //                }
    //                else
    //                {
    //                    mStatisfaction++;
    //                    mSprite.sprite = GameManager.pInstance.pEmotionSprites[(int)mStatisfaction];
    //                }
    //            }
    //            break;
    //        case eTableState.WaitingForOrder:
    //            mSprite.sprite = GameManager.pInstance.pDisheSprites[(int)pDesire - 1];
    //            mTimestampWaiting = Time.timeSinceLevelLoad + 5;
    //            break;
    //        case eTableState.WaitingForFood:
    //            if (Time.timeSinceLevelLoad - mTimestampWaiting > pPatienceTime)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                if (mStatisfaction == eStatisfaction.Angry)
    //                {
    //                    mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    //                    transform.GetChild(0).gameObject.SetActive(false);
    //                    pState = eTableState.Free;
    //                }
    //                else
    //                {
    //                    mStatisfaction++;
    //                    mSprite.sprite = GameManager.pInstance.pEmotionSprites[(int)mStatisfaction];
    //                }
    //            }
    //            break;
    //        case eTableState.Eating:
    //            if (Time.timeSinceLevelLoad >= mTimestampWaiting)
    //            {
    //                pState = eTableState.WaitingForClean;
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                mSprite.sprite = GameManager.pInstance.pDollarSprites[0];
    //                transform.GetChild(0).gameObject.SetActive(true);
    //            }
    //            break;
    //        case eTableState.WaitingForClean:
    //            if (Time.timeSinceLevelLoad - mTimestampWaiting > pPatienceTime)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                if (mStatisfaction == eStatisfaction.Displeased)
    //                {
    //                    mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    //                    transform.GetChild(0).gameObject.SetActive(false);
    //                    pState = eTableState.Free;
    //                }
    //                else
    //                {
    //                    mStatisfaction++;
    //                    mSprite.sprite = GameManager.pInstance.pDollarSprites[(int)mStatisfaction];
    //                }
    //            }
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    //public void StartEating()
    //{
    //    pState = eTableState.Eating;
    //    transform.GetChild(0).gameObject.SetActive(false);
    //    mTimestampWaiting = Time.timeSinceLevelLoad + (float)GameManager.pInstance.pRandom.Next(5, 15);
    //}

    //public void StartIdle()
    //{
    //    pState = eTableState.Free;
    //    transform.GetChild(0).gameObject.SetActive(false);
    //    mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    //}

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        mPlayer.PushStack(transform);
    //    }
    //}
}
