using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Image = UnityEngine.UI.Image;

public class Table : MonoBehaviour
{

    [Tooltip("Earliest possible first order") ]public int pEarliestFirstOrder;
    [Tooltip("Latest Possible first order")]public int pLatestFirstOrder;
    [Tooltip("Minimum time between orders")]public int pMinTimeNextOrder;
    [Tooltip("Maximum time between orders")]public int pMaxTimeNextOrder;
    [Tooltip("Time waited before switching to next impatience level")]public int pPatienceTime;

    [HideInInspector]public eDishes pDesire;
    [HideInInspector] public eTableState pState;
    [HideInInspector] public float pArrivalTime;
    [HideInInspector] public GameObject pPanel;

    private Image mSprite;
    private int mNextOrder;
    private float mTimestampWaiting;
    private eStatisfaction mStatisfaction;
    private Player mPlayer;

    //private void Start()
    //{
    //    mSprite = pPanel.transform.GetChild(0).GetComponent<Image>();
    //    mPlayer = FindObjectOfType<Player>();

    //    pState = eTableState.Idle;
    //    mNextOrder = GameManager.pInstance.pRandom.Next(pEarliestFirstOrder, pLatestFirstOrder);
    //    transform.GetChild(0).gameObject.SetActive(false);
    //}

    //private void Update()
    //{
    //    switch (pState)
    //    {
    //        case eTableState.Idle:
    //            if (Time.timeSinceLevelLoad >= mNextOrder)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                pDesire = (eDishes)(GameManager.pInstance.pRandom.Next(2) + 1);
    //                mStatisfaction = eStatisfaction.Good;
    //                pState = eTableState.WaitingForOrder;
    //                mSprite.sprite = GameManager.pInstance.pEmotionSprites[0];
    //                transform.GetChild(0).gameObject.SetActive(true);

    //            }
    //            break;
    //        case eTableState.WaitingForOrder:
    //            if (Time.timeSinceLevelLoad - mTimestampWaiting > pPatienceTime)
    //            {
    //                mTimestampWaiting = Time.timeSinceLevelLoad;
    //                if (mStatisfaction == eStatisfaction.Angry)
    //                {
    //                    mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    //                    transform.GetChild(0).gameObject.SetActive(false);
    //                    pState = eTableState.Idle;
    //                }
    //                else
    //                {
    //                    mStatisfaction++;
    //                    mSprite.sprite = GameManager.pInstance.pEmotionSprites[(int)mStatisfaction];
    //                }
    //            }
    //            break;
    //        case eTableState.Orders:
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
    //                    pState = eTableState.Idle;
    //                }
    //                else
    //                {
    //                    mStatisfaction++;
    //                    mSprite.sprite = GameManager.pInstance.pEmotionSprites[(int)mStatisfaction];
    //                }
    //            }
    //            break;
    //        case eTableState.Eats:
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
    //                    pState = eTableState.Idle;
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

    public void StartEating()
    {
        pState = eTableState.Eats;
        transform.GetChild(0).gameObject.SetActive(false);
        mTimestampWaiting = Time.timeSinceLevelLoad + (float)GameManager.pInstance.pRandom.Next(5, 15);
    }

    public void StartIdle()
    {
        pState = eTableState.Idle;
        transform.GetChild(0).gameObject.SetActive(false);
        mNextOrder = (int)Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pMinTimeNextOrder, pMaxTimeNextOrder);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mPlayer.PushStack(transform);
        }
    }
}
