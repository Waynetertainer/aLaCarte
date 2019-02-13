using Assets.Scripts;
using NET_System;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Balancing Values")]
    [Space(20)]
    public Color pGreen;
    public Color pYellow;
    public Color pRed;
    [Space(20)]
    public int pFoodAmountInLevel;
    public float pTableInteractionDistance;
    public float pCustomerInteractionDistance;
    public float pBoatInteractionDistance;
    public float pFoodInteractionDistance;
    [Space(10)]
    public float pFoodDeactivationTime;
    public float pReadingMenuTime;
    public float pEatingTime;
    [Space(10)]
    public int pCustomerRespawnTimeMin;
    public int pCustomerRespawnTimeMax;
    [Space(10)]
    public float pNormalCustomerMultiplicator;
    public float pSnobCustomerMultiplicator;
    public float pTwoTableMultiplicator;
    public float pFourTableMultiplicator;
    public int pWrongFoodMalusPercent;
    [Space(10)]
    public float pNormalWaitTime;
    public float pSnobWaitTime;
    [Space(10)]
    public float pSymbolFeedbackDuration;
    [Space(20)]
    [Header("Scene Objects")]
    [Space(20)]
    public GameObject pWaitingCustomer;
    public Text pTimer;
    public Text pTimerShaddow;
    public Text pOwnScoreText;
    public Text pOwnScoreTextShaddow;
    public Text pOtherScoreText;
    public Text pOtherScoreTextShaddow;
    public GameObject[] pNavMeshTargets = new GameObject[2];
    public Character[] pCharacters = new Character[2];
    public GameObject pEmptyDome;
    public GameObject[] pFood = new GameObject[4];
    public GameObject pPlate;
    public GameObject pCustomer;
    public Table[] pTables;
    public Food[] pFoodDispensers;
    public GameObject pReallyLeaveButton;
    public GameObject pScoreScreen;
    public eCustomers pCustomerType;

    [HideInInspector] public bool pDragging;
    [HideInInspector] public bool pIsHost;
    [HideInInspector] public bool pIsPlaying;
    [HideInInspector] public float[] pScores = new float[2];
    [HideInInspector] public float pLevelStartTime;
    public GatesManager pGatesManager;

    private float mNextCustomer;
    private float mGameEnd;

    private void Awake()
    {
        GameManager.pInstance.pLevelLoaded[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1] = true;
        NET_EventCall eventCall = new NET_EventCall("LevelLoaded");
        eventCall.SetParam("PlayerID", GameManager.pInstance.NetMain.NET_GetPlayerID());
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        GameManager.pInstance.CheckLevelLoad();
        GameManager.pInstance.pLevelManager = this;
        Table[] tempTables = FindObjectsOfType<Table>();
        pTables = new Table[tempTables.Length];
        foreach (Table table in tempTables)
        {
            pTables[table.pID] = table;
        }
    }

    private void Start()
    {
        pGatesManager = GetComponent<GatesManager>();
        pCharacters[0].pTarget = pNavMeshTargets[0].transform;
        pCharacters[1].pTarget = pNavMeshTargets[1].transform;
        pCharacters[0].pID = 1;
        pCharacters[1].pID = 2;
        pNavMeshTargets[0].transform.position = pCharacters[0].transform.position;
        pNavMeshTargets[1].transform.position = pCharacters[1].transform.position;
        pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetDecal(true);
        pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].SetDecal(false);
        Destroy(pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].GetComponent<Rigidbody>());
        pCharacters[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].GetComponent<CapsuleCollider>().enabled = false;
        pReallyLeaveButton.GetComponent<Button>().onClick.AddListener(delegate { GameManager.pInstance.ReturnToMainMenu(); });

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



        pFoodDispensers = FindObjectsOfType<Food>();
    }

    private void Update()
    {
        pOwnScoreText.text = pOwnScoreTextShaddow.text = pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].ToString("C", new CultureInfo("de-DE"));
        pOtherScoreText.text = pOtherScoreTextShaddow.text = pScores[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].ToString("C", new CultureInfo("de-DE"));

        if (Time.timeSinceLevelLoad >= mGameEnd && pIsPlaying)
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
            pScoreScreen.SetActive(true);
            pScoreScreen.GetComponent<ScoreScreen>().Open();
        }

        if (pIsPlaying)
        {
            float temp = mGameEnd - Time.timeSinceLevelLoad;
            pTimer.text = pTimerShaddow.text = Math.Floor(temp / 60) + ":" + Math.Floor(temp % 60).ToString().PadLeft(2, '0');
        }

        if (!pIsHost) return;
        if (Time.timeSinceLevelLoad >= mNextCustomer && pTables.Any(p => p.pState == eTableState.Free))
        {
            SpawnCustomers();
            mNextCustomer = Time.timeSinceLevelLoad + GameManager.pInstance.pRandom.Next(pCustomerRespawnTimeMin, pCustomerRespawnTimeMax);
            NET_EventCall eventCall = new NET_EventCall("NewCustomer");
            GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        }
    }

    public void SpawnCustomers()
    {
        pWaitingCustomer.gameObject.SetActive(true);
    }

    public bool TryCarry()//for dishes
    {
        if (pCustomer.activeSelf || pFood.Any(p => p.activeSelf) || pPlate.activeSelf) return false;
        pPlate.SetActive(true);
        pPlate.transform.parent.gameObject.SetActive(true);
        pEmptyDome.SetActive(false);
        pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetAnimation(eCarryableType.Dishes);
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
            pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetAnimation(eCarryableType.Food);
            foreach (Transform childTransform in temp.transform)
            {
                childTransform.gameObject.SetActive(false);
            }
            temp.transform.GetChild((int)type - 1).gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public bool TryCarry(eCustomers type)
    {
        if (pCustomer.activeSelf || pPlate.activeSelf || pFood.Any(p => p.activeSelf)) return false;
        pCustomerType = type;
        pCustomer.SetActive(true);
        pCustomer.transform.parent.gameObject.SetActive(true);
        pEmptyDome.SetActive(false);
        pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetAnimation(eCarryableType.Customer);
        return true;
    }

    public void SetFood(eFood type)
    {
        pFoodDispensers.First(p => p.pFood == type).SetInteractable();
    }

    public void CheckFoodEmpty()
    {
        if (pFood.All(f => !f.activeSelf))
        {
            pFood[0].transform.parent.gameObject.SetActive(false);
            pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].SetAnimation();
            pEmptyDome.SetActive(true);
        }
    }

    public void StartGame()
    {
        mGameEnd = Time.timeSinceLevelLoad + 240;
        pIsPlaying = true;
        GetComponent<AudioSource>().volume = GameManager.pInstance.pMusicVolume;
        GetComponent<AudioSource>().Play();
    }

    public void ReturnToLobby()
    {
        GameManager.pInstance.ReturnToMainMenu();
    }
}
