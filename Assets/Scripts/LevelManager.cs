using Assets.Scripts;
using NET_System;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Balancing Values")]
    [Space(20)]
    public float pTableInteractionDistance;
    public float pCustomerInteractionDistance;
    public float pBoatInteractionDistance;
    public float pFoodInteractionDistance;

    public float pFoodDeactivationTime;
    public float pReadingMenuTime;
    public float pEatingTime;

    public int pCustomerRespawnTimeMin;
    public int pCustomerRespawnTimeMax;
    [Space(20)]
    [Header("Scene Objects")]
    [Space(20)]
    public GameObject[] pNavMeshTargets = new GameObject[2];
    public Character[] pCharacters = new Character[2];
    public GameObject[][] pCarryableObjects = new GameObject[3][];
    public GameObject[] pFood = new GameObject[4];
    public GameObject[] pPlates = new GameObject[2];
    public GameObject[] pCustomer = new GameObject[1];
    public GameObject pWaitingCustomer;
    public Text pTimer;
    public Table[] pTables;
    public Food[] pFoodDispensers;

    [HideInInspector] public bool pDragging;
    [HideInInspector] public bool pIsHost;
    [HideInInspector] public bool pIsPlaying;

    private float mNextCustomer; //TODO make GD
    private DateTime mGameEnd;

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pCharacters[0].pID = 1;
        pCharacters[1].pID = 2;
        Table[] tempTables = FindObjectsOfType<Table>();
        mGameEnd = GameManager.pInstance.pLevelStart + new TimeSpan(0, 0, 240);
        pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetDecal(true);

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

        pFoodDispensers = FindObjectsOfType<Food>();

    }

    private void Update()
    {
        if (DateTime.Now >= GameManager.pInstance.pLevelStart)
        {
            TimeSpan temp = mGameEnd - DateTime.Now;
            if (temp >= new TimeSpan(0))
            {
                if (!pIsPlaying)
                {
                    pIsPlaying = true;
                    GetComponent<AudioSource>().Play();
                }
                pTimer.text = temp.Minutes.ToString() + ":" + temp.Seconds.ToString().PadLeft(2, '0');
            }
            else
            {
                pIsPlaying = false;
                foreach (Character character in pCharacters)
                {
                    character.Move(false);
                }
                foreach (Table table in pTables)
                {
                    table.enabled = false;
                }
                pCustomer[0].transform.parent.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            TimeSpan temp = GameManager.pInstance.pLevelStart - DateTime.Now;
            pTimer.text = temp.Minutes.ToString() + ":" + temp.Seconds.ToString().PadLeft(2, '0');

        }

        if (!pIsHost) return;
        if (Time.timeSinceLevelLoad >= mNextCustomer && pTables.Any(p => p.pState == eTableState.Free))
        {
            int amount = pTables.Any(p => p.pSize == 4) ? 4 : 2;
            SpawnCustomers(amount);
            mNextCustomer = Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pCustomerRespawnTimeMin, pCustomerRespawnTimeMax);
            NET_EventCall eventCall = new NET_EventCall("NewCustomer");
            eventCall.SetParam("Amount", amount);
            GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        }
    }

    public void SpawnCustomers(int amount)
    {
        switch (amount)
        {
            case 2:
                pWaitingCustomer.gameObject.SetActive(true);
                break;
            case 4:
                pWaitingCustomer.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("amount");
        }
    }

    //public void tempSpawnCustomers()
    //{
    //    SpawnCustomers(2);
    //    mNextCustomer += GameManager.pInstance.pRandom.Next(2, 10); 
    //    NET_EventCall eventCall = new NET_EventCall("NewCustomer");
    //    eventCall.SetParam("Amount", 2);
    //    GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
    //}

    public bool TryCarry(eCarryableType type, eFood? food = null)
    {
        switch (type)
        {
            case eCarryableType.Food:
                if (!(pCustomer[0].activeSelf || pPlates.Any(p => p.activeSelf)) && pFood.Any(p => p.activeSelf == false))
                {
                    GameObject temp;
                    switch (food)
                    {
                        case eFood.Pizza:
                            temp = pFood.First(p => p.activeSelf == false);
                            temp.SetActive(true);
                            temp.transform.GetChild(0).gameObject.SetActive(false);
                            temp.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                        case eFood.Pasta:
                            temp = pFood.First(p => p.activeSelf == false);
                            temp.SetActive(true);
                            temp.transform.GetChild(0).gameObject.SetActive(true);
                            temp.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                    }
                    return true;
                }
                return false;
            case eCarryableType.Customer:
                if (!(pCustomer[0].activeSelf || pPlates.Any(p => p.activeSelf) || pFood.Any(p => p.activeSelf)))
                {
                    pCustomer[0].SetActive(true);
                    return true;
                }
                return false;
            case eCarryableType.Dishes:
                if (!(pCustomer[0].activeSelf || pFood.Any(p => p.activeSelf)) && pPlates.Any(p => p.activeSelf == false))
                {
                    pPlates.First(p => p.activeSelf == false).SetActive(true);
                    return true;
                }
                return false;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    public void SetFood(eFood type)
    {
        pFoodDispensers.First(p => p.pFood == type).SetInteractable();
    }
}
