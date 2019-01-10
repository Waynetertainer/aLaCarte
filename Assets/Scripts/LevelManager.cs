using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NET_System;
using UnityEngine;
using UnityEngine.Networking;

public class LevelManager : MonoBehaviour
{
    public GameObject[] pNavMeshTargets = new GameObject[2];
    public Character[] pCharacters = new Character[2];
    //public eCarryableType[] pCarrying = new eCarryableType[4];
    public GameObject[][] pCarryableObjects = new GameObject[3][];

    public GameObject[] pFood = new GameObject[4];
    public GameObject[] pPlates = new GameObject[2];
    public GameObject[] pCustomer = new GameObject[1];
    public GameObject pWaitingCustomer;

    public bool pCustomerWaitingOrMoving;
    public bool pDragging;

    public Table[] pTables;

    private float mNextCustomer;
    private bool mIsHost;

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pCharacters[0].pID = 1;
        pCharacters[1].pID = 2;
        Table[] tempTables = FindObjectsOfType<Table>();
        pTables=new Table[tempTables.Length];
        for (int i = 0; i < tempTables.Length; i++)
        {
            pTables[i] = tempTables.First(p => p.pID == i);
        }
        mNextCustomer = 0;
        if (GameManager.pInstance.NetMain.NET_GetPlayerID() == 1)
        {
            mIsHost = true;
        }
    }

    private void Update()
    {
        if (!mIsHost) return;
        if (Time.timeSinceLevelLoad >= mNextCustomer)
        {
            if (pTables.Any(p => p.pState == eTableState.Free) && !pCustomerWaitingOrMoving)
            {
                int amount = pTables.Any(p => p.pSize == 4) ? 4 : 2;
                SpawnCustomers(amount);
                mNextCustomer += GameManager.pInstance.pRandom.Next(2, 10); //give parameters public
                NET_EventCall eventCall = new NET_EventCall("NewCustomer");
                eventCall.SetParam("Amount", amount);
                GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
            }
        }
    }

    public void SpawnCustomers(int amount)
    {
        switch (amount)
        {
            case 2:
                //Instantiate(pCustomerPrefab, pCustomerSpawnPoint.transform.position, pCustomerSpawnPoint.transform.rotation);
                pWaitingCustomer.gameObject.SetActive(true);
                pCustomerWaitingOrMoving = true;
                break;
            case 4:
                //Instantiate(pCustomerPrefab, pCustomerSpawnPoint.transform.position, pCustomerSpawnPoint.transform.rotation);
                pCustomerWaitingOrMoving = true;
                pWaitingCustomer.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("amount");
        }
    }

    public bool CanCarry(eCarryableType type)
    {
        switch (type)
        {
            case eCarryableType.Empty:
                return true;
            case eCarryableType.Pizza:
                if (!(pCustomer[0].activeSelf || pPlates.Any(p=>p.activeSelf)) && pFood.Any(p => p.activeSelf == false))
                {
                    return true;
                }
                return false;
            case eCarryableType.Pasta:
                if (!(pCustomer[0].activeSelf || pPlates.Any(p => p.activeSelf)) && pFood.Any(p => p.activeSelf == false))
                {
                    return true;
                }
                return false;
            case eCarryableType.Customer:
                if (!(pCustomer[0].activeSelf|| pPlates.Any(p => p.activeSelf)|| pFood.Any(p => p.activeSelf)))
                {
                    return true;
                }
                return false;
            case eCarryableType.Dishes:
                if (!(pCustomer[0].activeSelf || pFood.Any(p => p.activeSelf))&&pPlates.Any(p=>p.activeSelf==false))
                {
                    return true;
                }
                return false;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    public void ChangeCarry(eCarryableType type)
    {
        foreach (GameObject[] carryableObjectArray in pCarryableObjects)
        {
            foreach (GameObject carryableObject in carryableObjectArray)
            {
                carryableObject.SetActive(false);
            }
        }
        switch (type)
        {
            case eCarryableType.Empty:
                break;
            case eCarryableType.Pizza:

                break;
            case eCarryableType.Pasta:
                break;
            case eCarryableType.Customer:

                break;
            case eCarryableType.Dishes:
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}
