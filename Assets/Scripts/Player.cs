using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public Transform[] pFoodDestinations = new Transform[2];
    public Stack<Transform> pDestinations = new Stack<Transform>();
    public bool pOccupied;

    public int pStack;

    private Image mSprite;
    private Transform mActiveDestination;
    private NavMeshAgent mAgent;
    private float mTimeLeaving;

    private eDishes mFood;

    //private void Start()
    //{
    //    mSprite = transform.GetChild(0).GetChild(0).transform.GetChild(0).GetComponent<Image>();
    //    mAgent = GetComponent<NavMeshAgent>();
    //}

    //private void Update()
    //{
    //    pStack = pDestinations.Count;
    //    if (pDestinations.Count > 0)
    //    {
    //        if (!pOccupied)
    //        {
    //            mActiveDestination = pDestinations.Peek();
    //            mAgent.destination = mActiveDestination.position;
    //        }

    //        if (Mathf.Sqrt(Mathf.Pow(transform.position.z - mAgent.destination.z, 2) + Mathf.Pow(transform.position.x - mAgent.destination.x, 2)) <= 0.3)
    //        {
    //            if (mActiveDestination.GetComponent<Table>())
    //            {
    //                Table targetTable = mActiveDestination.GetComponent<Table>();
    //                switch (targetTable.pState)
    //                {
    //                    case eTableState.Free:
    //                        mActiveDestination = null;
    //                        pDestinations.Pop();
    //                        break;
    //                    case eTableState.ReadingMenu:
    //                        targetTable.pState = eTableState.WaitingForOrder;
    //                        mTimeLeaving = Time.timeSinceLevelLoad + 0;
    //                        pOccupied = true;
    //                        break;
    //                    case eTableState.WaitingForOrder:
    //                        if (Time.timeSinceLevelLoad >= mTimeLeaving)
    //                        {
    //                            mActiveDestination = null;
    //                            pDestinations.Pop();
    //                            pOccupied = false;
    //                            targetTable.pState = eTableState.WaitingForFood;
    //                        }
    //                        break;
    //                    case eTableState.WaitingForFood:
    //                        if (mFood == targetTable.pDesire)
    //                        {
    //                            if (pOccupied)
    //                            {
    //                                if (Time.timeSinceLevelLoad >= mTimeLeaving)
    //                                {
    //                                    targetTable.StartEating();
    //                                    mFood = eDishes.None;
    //                                    transform.GetChild(0).gameObject.SetActive(false);
    //                                    pDestinations.Pop();
    //                                    pOccupied = false;
    //                                }
    //                            }
    //                            else
    //                            {
    //                                pOccupied = true;
    //                                mTimeLeaving = Time.timeSinceLevelLoad + 0.4f;
    //                            }
    //                        }

    //                        break;
    //                    case eTableState.Eating:
    //                        mActiveDestination = null;
    //                        pDestinations.Pop();
    //                        break;
    //                    case eTableState.WaitingForClean:
    //                        if (pOccupied)
    //                        {
    //                            if (Time.timeSinceLevelLoad >= mTimeLeaving)
    //                            {
    //                                targetTable.StartIdle();
    //                                pDestinations.Pop();
    //                                pOccupied = false;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            pOccupied = true;
    //                            mTimeLeaving = Time.timeSinceLevelLoad + 0.4f;
    //                        }
    //                        break;
    //                    default:
    //                        throw new ArgumentOutOfRangeException();
    //                }
    //            }
    //            else if (mActiveDestination.GetComponent<Food>())
    //            {
    //                mFood = mActiveDestination.GetComponent<Food>().pFoodType;
    //                mSprite.sprite = GameManager.pInstance.pDisheSprites[(int)mFood - 1];
    //                transform.GetChild(0).gameObject.SetActive(true);
    //                pDestinations.Pop();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        mAgent.destination = transform.position;
    //    }
    //}

    //public void PushStack(Transform target)
    //{
    //    if (pDestinations.Count == 0 || pDestinations.Peek() != target)
    //    {
    //        pDestinations.Push(target);
    //    }
    //}
}