using System;
using System.Collections;
using System.Collections.Generic;
using NET_System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Image = UnityEngine.UI.Image;

public class Table : MonoBehaviour
{
    [HideInInspector] public eTableState pState;

    public int pID;
    public int? pPlayerID;

    private GameObject mPasta;
    private GameObject mPizza;

    public int pSize;

    public void SetDishActive(eCarryableType type)
    {
        switch (type)
        {
            case eCarryableType.Empty:
                break;
            case eCarryableType.Pizza:
                transform.GetChild(0).gameObject.SetActive(true);
                pState = eTableState.Eating;
                SendTableState(eCarryableType.Pizza);
                break;
            case eCarryableType.Pasta:
                transform.GetChild(1).gameObject.SetActive(true);
                pState = eTableState.Eating;
                SendTableState(eCarryableType.Pasta);
                break;
            case eCarryableType.Customer:
                break;
            case eCarryableType.Dishes:
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    private void SendTableState(eCarryableType? carryableType = null)
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
        transform.GetChild(1).gameObject.SetActive(false);
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
