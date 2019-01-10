using System;
using System.Collections;
using System.Collections.Generic;
using NET_Messages;
using NET_System;
using UnityEngine;
using NET_Multiplayer_V3;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{

    public NET_Main NetMain;
    public string pServerName = "Default Name";
    public GameObject pServerSelectionPanel;
    public GameObject pButtonPrefab;
    public TextMeshProUGUI pInfoText;
    public TextMeshProUGUI pServerInfoText;
    public Toggle pReady;
    public GameObject pVeniceButton;
    public GameObject pRomaButton;
    public List<GameObject> Canvases = new List<GameObject>(2);
    public LevelManager pLevelManager;

    private NetworkStatus mStatus;
    private NET_ServerInfo[] Servers;
    private float nextCheck;
    NET_DataPack outPack = new NET_DataPack();
    private Level level;

    private List<bool> playerReady = new List<bool>() { false, false };

    public static GameManager pInstance;

    public Random pRandom = new Random();

    public Sprite[] pDisheSprites = new Sprite[2];
    public Sprite[] pEmotionSprites = new Sprite[4];
    public Sprite[] pDollarSprites = new Sprite[3];

    private void Awake()
    {
        if (pInstance == null)

            pInstance = this;

        else if (pInstance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {

            ChangeCanvas(0);
        }
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
            Servers = NetMain.NET_GetServerInfo();
            if (Servers != null && Servers.Length > 0)
            {
                pServerInfoText.text = Servers[0].INFO_GetServerName() + Servers[0].INFO_GetAddress();
                GetServers();
            }
        }
    }

    public void HandleEvents()
    {
        foreach (NET_EventCall eventCall in NetMain.NET_GetEventCalls())
        {
            switch (eventCall.GetEventName())
            {
                case ("StartGame"):
                    SceneManager.LoadScene(level.ToString());
                    break;
                case ("PlayerReadyChange"):
                    playerReady[(int)eventCall.GetParam("PlayerID") - 1] = (bool)eventCall.GetParam("Ready");
                    Debug.Log((int)eventCall.GetParam("PlayerID") - 1);
                    SendLobbyData();
                    break;
                case ("UpdateLobby"):
                    Debug.Log("Updated Lobby");
                    level = (Level)eventCall.GetParam("Level");
                    ShowLevel();
                    for (int i = 0; i < playerReady.Count; i++)
                    {
                        playerReady[i] = (bool)eventCall.GetParam(i.ToString());//TODO synchronise player ready status
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
                    pLevelManager.SpawnCustomers((int)eventCall.GetParam("Amount"));
                    break;
                case ("CustomerTaken"):
                    pLevelManager.pWaitingCustomer.GetComponent<Customer>().TakeCustomer((int)eventCall.GetParam("PlayerID")-1);
                    break;
                default:
                    Debug.Log("Cant handle packets");
                    break;
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
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void GetInfos()
    {
        if (NetMain == null) return;
        string temp = NetMain.NET_GetStates();
        if (temp != null)
        {
            pInfoText.text = temp;
        }
    }

    private void ShowLevel()
    {
        if (level == Level.Level_Venedig)
        {
            pVeniceButton.transform.localScale = new Vector3(1, 1, 1);
            pRomaButton.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        }
        else
        {
            pRomaButton.transform.localScale = new Vector3(1, 1, 1);
            pVeniceButton.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        }
    }

    public void ChangeCanvas(int canvasID)
    {
        foreach (GameObject canvas in Canvases)
        {
            canvas.SetActive(false);
        }
        Canvases[canvasID].SetActive(true);
    }

    #region ServerRegion

    public void HostGame()
    {
        if (NetMain != null) return;
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

    public void SetLevel(int inLevel)
    {
        level = (Level)inLevel;
        ShowLevel();
        SendLobbyData();
    }

    public void StartGame()
    {
        NetMain.NET_CallEvent(new NET_EventCall("StartGame"));
        SceneManager.LoadScene(level.ToString());
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
    }

    public void GetServers()
    {
        Servers = NetMain.NET_GetServerInfo();
        Debug.Log("found " + Servers.Length + " servers");
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
        pReady.transform.GetChild(1).GetComponent<Text>().text = pReady.isOn.ToString();
        NET_EventCall eventCall = new NET_EventCall("PlayerReadyChange");
        eventCall.SetParam("PlayerID", NetMain.NET_GetPlayerID());
        eventCall.SetParam("Ready", pReady.isOn);
        NetMain.NET_CallEvent(eventCall);
    }
    #endregion
}
