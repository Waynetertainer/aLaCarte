using NET_Multiplayer_V3;
using NET_System;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{

    public NET_Main NetMain;
    public string pServerName = "aLaCarte";
    public Animator pLevelSelectionAnimator;
    public GameObject pServerSelectionPanel;
    public GameObject pButtonPrefab;
    public Toggle pReady;
    public GameObject pVeniceButton;
    public GameObject pRomaButton;
    public List<GameObject> Canvases = new List<GameObject>(2);
    public LevelManager pLevelManager;
    public List<bool> pLevelLoaded = new List<bool>() { false, false };

    public int pMusicVolume;
    public int pSoundVolume;
    public Sprite[] pMusicSprites;
    public Sprite[] pSoundSprites;
    public Image pMusicImage;
    public Image pSoundImage;


    private NetworkStatus mStatus;
    private NET_ServerInfo[] Servers;
    private float nextCheck;
    NET_DataPack outPack = new NET_DataPack();
    private Level level=Level.Level_None;

    private List<bool> playerReady = new List<bool>() { false, false };
    public DateTime pLevelStart;

    public static GameManager pInstance;

    public Random pRandom = new Random();

    private void Awake()
    {
        if (pInstance == null)
        {
            pInstance = this;
        }
        else if (pInstance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ChangeCanvas(0);
        }
    }

    private void Start()
    {
        //Application.targetFrameRate = 45;
    }

    private void OnApplicationQuit()
    {
        FreePort();
    }

    void Update()
    {
        GetInfos();

        if (mStatus == NetworkStatus.Server)
        {
            if (!playerReady.Contains(false))
            {
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    StartGame();
                }
            }
        }

        if (mStatus == NetworkStatus.Server || mStatus == NetworkStatus.Client || mStatus == NetworkStatus.ListeningUDP)
        {
            HandleEvents();
            NetMain.NET_Update();
        }

        if (mStatus == NetworkStatus.ListeningUDP && Time.timeSinceLevelLoad >= nextCheck)
        {
            nextCheck = Time.timeSinceLevelLoad + 1;
            GetServers();
            //JoinFirstPossible();
        }
    }

    public void HandleEvents()
    {
        foreach (NET_EventCall eventCall in NetMain.NET_GetEventCalls())
        {
            switch (eventCall.GetEventName())
            {
                case ("StartGame"):
                    pLevelStart = (DateTime)eventCall.GetParam("StartTime");
                    SceneManager.LoadScene(level.ToString());
                    break;
                case ("PlayerReadyChange"):
                    playerReady[(int)eventCall.GetParam("PlayerID") - 1] = (bool)eventCall.GetParam("Ready");
                    Debug.Log((int)eventCall.GetParam("PlayerID"));//-1
                    SendLobbyData();
                    break;
                case ("UpdateLobby"):
                    level = (Level)eventCall.GetParam("Level");
                    ShowLevel();
                    for (int i = 0; i < playerReady.Count; i++)
                    {
                        playerReady[i] = (bool)eventCall.GetParam(i.ToString());
                    }
                    break;
                case ("UpdateAnchorPosition"):
                    pLevelManager.pNavMeshTargets[(int)eventCall.GetParam("PlayerID") - 1].transform.position = new Vector3(
                        (float)eventCall.GetParam("PositionX"),
                        (float)eventCall.GetParam("PositionY"),
                        (float)eventCall.GetParam("PositionZ")
                        );
                    pLevelManager.pCharacters[(int)eventCall.GetParam("PlayerID") - 1].Move(true);
                    pLevelManager.pCharacters[(int)eventCall.GetParam("PlayerID") - 1].SetTargetPosition(pLevelManager.pNavMeshTargets[(int)eventCall.GetParam("PlayerID") - 1].transform.position);
                    break;
                case ("StopMoving"):
                    pLevelManager.pCharacters[(int)eventCall.GetParam("PlayerID") - 1].Move(false);
                    break;
                case ("NewCustomer"):
                    pLevelManager.SpawnCustomers();
                    break;
                case ("CustomerTaken"):
                    pLevelManager.pWaitingCustomer.SetActive(false);
                    pLevelManager.pWaitingCustomer.GetComponent<Customer>().pType =(eCustomers) eventCall.GetParam("NextType");
                        pLevelManager.pWaitingCustomer.transform.GetChild((int)pLevelManager.pWaitingCustomer.GetComponent<Customer>().pType).gameObject.SetActive(true);
                        pLevelManager.pWaitingCustomer.transform.GetChild(1-(int)pLevelManager.pWaitingCustomer.GetComponent<Customer>().pType).gameObject.SetActive(false);
                    break;
                case ("SetTableState"):
                    int tableID;
                    switch ((eTableState)eventCall.GetParam("State"))
                    {
                        case eTableState.Free:
                            tableID = (int)eventCall.GetParam("TableID");
                            pLevelManager.pTables[tableID].pPlayerID = -1;
                            pLevelManager.pTables[tableID].SetTableState(eTableState.Free);
                            break;
                        case eTableState.ReadingMenu:
                            tableID = (int)eventCall.GetParam("TableID");
                            pLevelManager.pTables[tableID].pPlayerID = (int)eventCall.GetParam("PlayerID");
                            pLevelManager.pTables[tableID].SetTableState(eTableState.ReadingMenu);
                            break;
                        case eTableState.WaitingForOrder:
                            tableID = (int)eventCall.GetParam("TableID");
                            //pLevelManager.pTables[tableID].pPlayerID = (int)eventCall.GetParam("PlayerID");
                            pLevelManager.pTables[tableID].SetTableState(eTableState.WaitingForOrder);
                            break;
                        case eTableState.WaitingForFood:
                            tableID = (int)eventCall.GetParam("TableID");
                            //pLevelManager.pTables[tableID].pPlayerID = (int)eventCall.GetParam("PlayerID");
                            pLevelManager.pTables[tableID].SetTableState(eTableState.WaitingForFood);
                            break;
                        case eTableState.Eating:
                            tableID = (int)eventCall.GetParam("TableID");
                            //pLevelManager.pTables[tableID].pPlayerID = (int)eventCall.GetParam("PlayerID");
                            pLevelManager.pTables[tableID].SetTableState(eTableState.Eating, (eFood[])eventCall.GetParam("Food"));
                            break;
                        case eTableState.WaitingForClean:
                            tableID = (int)eventCall.GetParam("TableID");
                            //pLevelManager.pTables[tableID].pPlayerID = (int)eventCall.GetParam("PlayerID");
                            pLevelManager.pTables[tableID].SetTableState(eTableState.WaitingForClean);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ("FoodTaken"):
                    pLevelManager.SetFood((eFood)eventCall.GetParam("FoodType"));
                    break;
                case ("ClosedGate"):
                    pLevelManager.pGatesManager.SetGateClosed((int)eventCall.GetParam("GateID"));
                    break;
                case ("LevelLoaded"):
                    pLevelLoaded[(int)eventCall.GetParam("PlayerID") - 1] = true;
                    CheckLevelLoad();
                    break;
                case ("UpdateScore"):
                    pLevelManager.pScores[1 - (NetMain.NET_GetPlayerID() - 1)] = (float)eventCall.GetParam("Tip");
                    break;
                case ("TableStealable"):
                    Debug.Log("received table stealable");
                    pLevelManager.pTables[(int)eventCall.GetParam("TableID")].SetStealable();
                    break;
                case ("TableStolen"):
                    Debug.Log("received table stolen");
                    pLevelManager.pTables[(int)eventCall.GetParam("TableID")].Stolen((int)eventCall.GetParam("PlayerID"));
                    break;
                case ("UpdateOrders"):
                    pLevelManager.pTables[(int)eventCall.GetParam("TableID")].pOrders = (eFood[])eventCall.GetParam("Orders");
                    pLevelManager.pTables[(int)eventCall.GetParam("TableID")].pFood = (eFood[])eventCall.GetParam("Food");
                    break;

                default:
                    Debug.Log("Cant handle packets");
                    break;
            }
        }
    }

    public void CheckLevelLoad()
    {
        if (!pLevelLoaded.Contains(false)|| SceneManager.GetActiveScene().name == "Level_Tutorial")
        {
            if (pLevelManager == null)
            {
                throw new ArgumentNullException("pLevelManager");
            }
            else
            {
                pLevelManager.StartGame();
            }
        }
    }

    public void FreePort()
    {
        try
        {
            NetMain.NET_DisconnectFromServer();
            NetMain.NET_Stop();
            NetMain.NET_Shutdown();
            mStatus = NetworkStatus.Disconnected;
        }
        catch (System.Exception)
        {

        }
    }

    public void GetInfos()
    {
        if (NetMain == null) return;
        string temp = NetMain.NET_GetStates();
        if (temp != null)
        {
            //pInfoText.text = temp;
        }
    }

    private void ShowLevel()
    {
        if (level == Level.Level_Venedig)
        {
            pRomaButton.gameObject.SetActive(true);
            pVeniceButton.transform.localPosition = new Vector3(-100, 60, 0);
            pRomaButton.transform.localPosition = new Vector3(600, 60, 0);
            pVeniceButton.gameObject.SetActive(false);
            pLevelSelectionAnimator.SetBool("Venedig_Selection",true);
            pLevelSelectionAnimator.SetBool("Rom_Selection",false);
        }
        else if(level == Level.Level_Rom)
        {
            pVeniceButton.gameObject.SetActive(true);
            pVeniceButton.transform.localPosition = new Vector3(-600, 60, 0);
            pRomaButton.transform.localPosition = new Vector3(100, 60, 0);
            pRomaButton.gameObject.SetActive(false);
            pLevelSelectionAnimator.SetBool("Venedig_Selection", false);
            pLevelSelectionAnimator.SetBool("Rom_Selection", true);
        }

        if (NetMain.NET_GetPlayerID() != 1)
        {
            pVeniceButton.gameObject.SetActive(false);
            pRomaButton.gameObject.SetActive(false);
        }
        pReady.gameObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        FreePort();
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);

        //ChangeCanvas(0);
    }

    public void ChangeCanvas(int canvasID)
    {
        foreach (GameObject canvas in Canvases)
        {
            canvas.SetActive(false);
        }
        Canvases[canvasID].SetActive(true);
    }

    public void SetMusic(Toggle toggle)
    {
        if (!toggle.isOn)
        {
            pMusicVolume = 1;
            pMusicImage.sprite = pMusicSprites[1];
        }
        else
        {
            pMusicVolume = 0;
            pMusicImage.sprite = pMusicSprites[0];
        }
    }

    public void SetSound(Toggle toggle)
    {
        if (!toggle.isOn)
        {
            pSoundVolume = 1;
            pSoundImage.sprite = pSoundSprites[1];
        }
        else
        {
            pSoundVolume = 0;
            pSoundImage.sprite = pSoundSprites[0];
        }
    }

    #region ServerRegion
    public void HostGame()
    {
        if (NetMain != null) FreePort();
        NetMain = new NET_Main(true, 30f, this.pServerName, false);
        NetMain.NET_Start();
        mStatus = NetworkStatus.Server;
        ChangeCanvas(1);
    }

    public void SendLobbyData()
    {
        NET_EventCall eventCall = new NET_EventCall("UpdateLobby");
        eventCall.SetParam("Level", level);
        for (int i = 0; i < playerReady.Count; i++)
        {
            eventCall.SetParam(i.ToString(), playerReady[i]);
        }
        NetMain.NET_CallEvent(eventCall);
    }

    public void DelegateLevelSet(int inLevel)
    {
        if (NetMain.NET_GetPlayerID() != 1) return;
        SetLevel(inLevel);
        SendLobbyData();
    }

    public void SetLevel(int inLevel)
    {
        level = (Level)inLevel;
        ShowLevel();
    }

    public void StartGame()
    {
        DateTime temp = pLevelStart = DateTime.Now + new TimeSpan(0, 0, 10);
        NET_EventCall eventCall = new NET_EventCall("StartGame");
        eventCall.SetParam("StartTime", temp);
        NetMain.NET_CallEvent(eventCall);

        SceneManager.LoadScene(level.ToString());
    }

    public void StartTutorial()
    {
        if (NetMain != null) FreePort();
        NetMain = new NET_Main(true, 30f, "Tutorial", false);
        NetMain.NET_Start();
        mStatus = NetworkStatus.Client;
        SceneManager.LoadScene("Level_Tutorial");
    }
    #endregion

    #region ClientRegion
    public void JoinGame()
    {
        nextCheck = Time.timeSinceLevelLoad + 1;
        if (NetMain != null)
        {
            Debug.Log("NetMain already there");
            FreePort();
        }

        NetMain = new NET_Main(false, 30f, "Client", false);
        NetMain.NET_Start();
        mStatus = NetworkStatus.ListeningUDP;
        GetServers();
        //JoinFirstPossible();
    }

    private void JoinFirstPossible()
    {
        Servers = NetMain.NET_GetServerInfo();
        foreach (NET_ServerInfo server in Servers)
        {
            if (server.INFO_GetServerName() != "À La Carte") continue;
            NetMain.NET_ConnectToServer(server);
            ChangeCanvas(1);
            NET_EventCall eventCall = new NET_EventCall("PlayerReadyChange");
            eventCall.SetParam("PlayerID", NetMain.NET_GetPlayerID());
            eventCall.SetParam("Ready", false);
            NetMain.NET_CallEvent(eventCall);
        }
    }

    public void GetServers()
    {
        Servers = NetMain.NET_GetServerInfo();
        for (int i = pServerSelectionPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(pServerSelectionPanel.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < Servers.Length; i++)
        {
            NET_ServerInfo server = (NET_ServerInfo)Servers[i];
            GameObject temp = Instantiate(pButtonPrefab, pServerSelectionPanel.transform);
            temp.GetComponentInChildren<Text>().text = server.INFO_GetServerName();
            temp.GetComponent<Button>().onClick.AddListener(delegate
            {
                NetMain.NET_ConnectToServer(server);
                ChangeCanvas(1);
                NET_EventCall eventCall = new NET_EventCall("PlayerReadyChange");
                eventCall.SetParam("PlayerID", NetMain.NET_GetPlayerID());
                eventCall.SetParam("Ready", false);
                NetMain.NET_CallEvent(eventCall);
            });
        }
    }

    public void SendReadyStatus()
    {
        //pReady.transform.GetChild(1).GetComponent<Text>().text = pReady.isOn.ToString();
        NET_EventCall eventCall = new NET_EventCall("PlayerReadyChange");
        eventCall.SetParam("PlayerID", NetMain.NET_GetPlayerID());
        eventCall.SetParam("Ready", pReady.isOn);
        NetMain.NET_CallEvent(eventCall);
    }
    #endregion
}