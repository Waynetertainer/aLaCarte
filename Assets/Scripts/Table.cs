using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Image = UnityEngine.UI.Image;

public class Table : MonoBehaviour
{
    [HideInInspector] public eTableState pState;
    [HideInInspector] public GameObject pPanel;

    public int pSize;

    private Image mSprite;
    private Player mPlayer;

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
