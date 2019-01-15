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
    public bool pIsHost;

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pCharacters[0].pID = 1;
        pCharacters[1].pID = 2;
        Table[] tempTables = FindObjectsOfType<Table>();

        mNextCustomer = 0;
        if (GameManager.pInstance.NetMain.NET_GetPlayerID() == 1)
        {
            pIsHost = true;
        }
        pCustomer[0].SetActive(false);
        foreach (GameObject plate in pPlates)
        {
            plate.SetActive(false);
        }

        foreach (GameObject food in pFood)
        {
            food.SetActive(false);
        }

        pTables = new Table[tempTables.Length];
        for (int i = 0; i < tempTables.Length; i++)
        {
            pTables[i] = tempTables.First(p => p.pID == i);
        }
    }

    private void Update()
    {
        if (!pIsHost) return;
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

    public void tempSpawnCustomers()
    {
        SpawnCustomers(2);
        mNextCustomer += GameManager.pInstance.pRandom.Next(2, 10); //give parameters public
        NET_EventCall eventCall = new NET_EventCall("NewCustomer");
        eventCall.SetParam("Amount", 2);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
    }

    public bool CanCarry(eCarryableType type)
    {
        switch (type)
        {
            case eCarryableType.Empty:
                return true;
            case eCarryableType.Pizza:
                if (!(pCustomer[0].activeSelf || pPlates.Any(p => p.activeSelf)) && pFood.Any(p => p.activeSelf == false))
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
                if (!(pCustomer[0].activeSelf || pPlates.Any(p => p.activeSelf) || pFood.Any(p => p.activeSelf)))
                {
                    return true;
                }
                return false;
            case eCarryableType.Dishes:
                if (!(pCustomer[0].activeSelf || pFood.Any(p => p.activeSelf)) && pPlates.Any(p => p.activeSelf == false))
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
        //pCustomer[0].SetActive(false);
        //foreach (GameObject plate in pPlates)
        //{
        //    plate.SetActive(false);
        //}

        //foreach (GameObject food in pFood)
        //{
        //    food.SetActive(false);
        //}

        GameObject temp;
        switch (type)
        {
            case eCarryableType.Empty:
                break;
            case eCarryableType.Pizza:
                temp = pFood.First(p => p.activeSelf == false);
                temp.SetActive(true);
                temp.transform.GetChild(0).gameObject.SetActive(false);
                temp.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case eCarryableType.Pasta:
                temp = pFood.First(p => p.activeSelf == false);
                temp.SetActive(true);
                temp.transform.GetChild(0).gameObject.SetActive(true);
                temp.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case eCarryableType.Customer:
                pCustomer[0].SetActive(true);
                break;
            case eCarryableType.Dishes:
                pPlates.First(p => p.activeSelf == false).SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}
