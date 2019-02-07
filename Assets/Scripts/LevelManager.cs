using Assets.Scripts;
using NET_System;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Balancing Values")]
    [Space(20)]
    public Color pGreen;
    public Color pYellow;
    public Color pRed;
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

    public float pNormalCustomerMultiplicator;
    public float pSnobCustomerMultiplicator;

    public float pOrderWaitIntervall;
    public float pFoodWaitIntervall;
    public float pCleanWaitIntervall;
    public float pOrderIntervallTipMalus;
    public float pFoodIntervallTipMalus;
    public float pCleanIntervallTipMalus;

    public float pSymbolFeedbackDuration;
    [Space(20)]
    [Header("Scene Objects")]
    [Space(20)]
    public GameObject pWaitingCustomer;
    public Text pTimer;
    public Text pOwnScoreText;
    public Text pOtherScoreText;
    public GameObject[] pNavMeshTargets = new GameObject[2];
    public Character[] pCharacters = new Character[2];
    public GameObject pEmptyDome;
    public GameObject[] pFood = new GameObject[4];
    public GameObject pPlate;
    public GameObject pCustomer;
    public Table[] pTables;
    public Food[] pFoodDispensers;

    [HideInInspector] public bool pDragging;
    [HideInInspector] public bool pIsHost;
    [HideInInspector] public bool pIsPlaying;
    [HideInInspector] public float[] pScores = new float[2];
    [HideInInspector] public float pLevelStartTime;
    public GatesManager pGatesManager;

    private float mNextCustomer;
    private DateTime mGameEnd;

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
        pGatesManager = GetComponent<GatesManager>();
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pCharacters[0].pID = 1;
        pCharacters[1].pID = 2;
        mGameEnd = GameManager.pInstance.pLevelStart + new TimeSpan(0, 0, 240);
        pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetDecal(true);
        pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].SetDecal(false);
        Destroy(pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].GetComponent<Rigidbody>());
        pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].GetComponent<CapsuleCollider>().enabled = false;

        mNextCustomer = 0;
        if (GameManager.pInstance.NetMain.NET_GetPlayerID() == 1)
        {
            pIsHost = true;
        }
        pCustomer.transform.parent.gameObject.SetActive(false);
        pCustomer.SetActive(false);
        pPlate.transform.parent.gameObject.SetActive(false);
        pPlate.SetActive(false);
        pFood[0].transform.parent.gameObject.SetActive(false);
        foreach (GameObject food in pFood)
        {
            food.SetActive(false);
        }
        pEmptyDome.SetActive(true);

        Table[] tempTables = FindObjectsOfType<Table>();
        pTables = new Table[tempTables.Length];
        foreach (Table table in tempTables)
        {
            pTables[table.pID] = table;
        }

        pFoodDispensers = FindObjectsOfType<Food>();
    }

    private void Update()
    {
        pOwnScoreText.text = pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].ToString("C", new CultureInfo("de-DE"));
        pOtherScoreText.text = pScores[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].ToString("N2");

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
                pCustomer.transform.parent.parent.gameObject.SetActive(false);
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

    public bool TryCarry()
    {
        if (pCustomer.activeSelf || pFood.Any(p => p.activeSelf) || pPlate.activeSelf) return false;
        pPlate.SetActive(true);
        pPlate.transform.parent.gameObject.SetActive(true);
        pEmptyDome.SetActive(false);
        return true;
    }

    public bool TryCarry(eFood type)
    {
        if (!(pCustomer.activeSelf || pPlate.activeSelf) && pFood.Any(p => p.activeSelf == false))
        {
            GameObject temp = pFood.First(p => p.activeSelf == false);
            temp.SetActive(true);
            temp.transform.parent.gameObject.SetActive(true);
            pEmptyDome.SetActive(false);
            foreach (Transform transform in temp.transform)
            {
                transform.gameObject.SetActive(false);
            }
            temp.transform.GetChild((int)type-1).gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public bool TryCarry(eCustomers type)
    {
        if (pCustomer.activeSelf || pPlate.activeSelf || pFood.Any(p => p.activeSelf)) return false;
        pCustomer.SetActive(true);//vllt childobjects
        pCustomer.transform.parent.gameObject.SetActive(true);
        pEmptyDome.SetActive(false);
        return true;
    }

    public void SetFood(eFood type)
    {
        pFoodDispensers.First(p => p.pFood == type).SetInteractable();
    }

    public void SetDomeImage(eCarryableType type)
    {
        switch (type)
        {
            case eCarryableType.Food:
                break;
            case eCarryableType.Customer:
                pCustomer.transform.parent.gameObject.SetActive(false);
                pEmptyDome.SetActive(true);
                break;
            case eCarryableType.Dishes:
                pPlate.transform.parent.gameObject.SetActive(false);
                pEmptyDome.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}
