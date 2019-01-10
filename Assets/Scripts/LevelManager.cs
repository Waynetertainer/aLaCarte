using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Networking;

public class LevelManager : MonoBehaviour
{
    public GameObject[] pNavMeshTargets = new GameObject[2];
    public Character[] pCharacters = new Character[2];
    public GameObject pCustomerPrefab;
    public GameObject pCustomerSpawnPoint;

    public bool pCustomerWaitingOrMoving;

    public Table[] pTables;

    private float mNextCustomer;
    private bool mIsHost;

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pTables = FindObjectsOfType<Table>();
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
                SpawnCustomers(pTables.Any(p => p.pSize == 4) ? 4 : 2);
                mNextCustomer += GameManager.pInstance.pRandom.Next(2, 10); //give parameters public
            }
        }
    }

    private void SpawnCustomers(int amount)
    {
        switch (amount)
        {
            case 2:
                Instantiate(pCustomerPrefab, pCustomerSpawnPoint.transform.position, pCustomerSpawnPoint.transform.rotation);
                pCustomerWaitingOrMoving = true;
                break;
            case 4:
                Instantiate(pCustomerPrefab, pCustomerSpawnPoint.transform.position, pCustomerSpawnPoint.transform.rotation);
                pCustomerWaitingOrMoving = true;
                break;
            default:
                throw new ArgumentOutOfRangeException("amount");
        }
    }
}
