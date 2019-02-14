using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public float pWaypointInteractionDistance;
    public Animator pAnimator;
    public eTutState pState;
    public GameObject pWaypoints;

    private LevelManager mLevelManager;
    private bool mStateChanged;

    void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        pState = eTutState.Timer;
        SetAnimator(eTutState.Timer);
    }

    private void OnEnable()
    {
        LevelManager.CustomerInteraction += CustomerInteracted;
        LevelManager.CustomerPlaced += CustomerPlaced;
        LevelManager.OrderPanelOpened += OrderPanelOpened;
        LevelManager.OrderTaken += OrderTaken;
        LevelManager.TutorialEnd += EndOfTutorial;
    }

    private void OnDisable()
    {
        LevelManager.CustomerInteraction -= CustomerInteracted;
        LevelManager.CustomerPlaced -= CustomerPlaced;
        LevelManager.OrderPanelOpened -= OrderPanelOpened;
        LevelManager.OrderTaken -= OrderTaken;
        LevelManager.TutorialEnd -= EndOfTutorial;

    }

    void Update()
    {
        switch (pState)
        {
            case eTutState.Timer:
                mLevelManager.pIsPlaying = false;
                if (Input.GetMouseButtonDown(0))
                {
                    SetAnimator(eTutState.Interaction);
                    pState = eTutState.Interaction;
                    mLevelManager.pIsPlaying = true;
                }
                break;
            case eTutState.Interaction:
                break;
            case eTutState.Movement:
                break;
            case eTutState.Inventory:
                break;
            case eTutState.CustomerOrders:
                if (Input.GetMouseButtonDown(0))
                {
                    DisableAnimator();
                }
                break;
            case eTutState.OrderNote:
                break;
            case eTutState.SandBox:
                if (Input.GetMouseButtonDown(0))
                {
                    DisableAnimator();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetAnimator(eTutState state)
    {
        DisableAnimator();
        pAnimator.SetBool("Step" + ((int)state).ToString(), true);
    }

    private void DisableAnimator()
    {
        for (int i = 1; i < 8; i++)
        {
            pAnimator.SetBool("Step" + i.ToString(), false);
        }
    }

    public void CustomerInteracted()
    {
        if (pState == eTutState.Interaction)
        {
            SetAnimator(eTutState.Movement);
            pState = eTutState.Movement;
            pWaypoints.SetActive(true);
        }
    }

    public void Moved()
    {
        if (pState == eTutState.Movement)
        {
            SetAnimator(eTutState.Inventory);
            pState = eTutState.Inventory;
            pWaypoints.SetActive(false);
        }
    }

    public void CustomerPlaced()
    {
        if (pState == eTutState.Inventory)
        {
            SetAnimator(eTutState.CustomerOrders);
            pState = eTutState.CustomerOrders;
        }
    }

    public void OrderTaken()
    {
        if (pState == eTutState.CustomerOrders)
        {
            SetAnimator(eTutState.OrderNote);
            pState = eTutState.OrderNote;
        }
    }

    public void OrderPanelOpened()
    {
        if (pState == eTutState.OrderNote)
        {
            SetAnimator(eTutState.SandBox);
            pState = eTutState.SandBox;
        }
    }

    public void EndOfTutorial()
    {
        mLevelManager.StopGame();
    }
}
