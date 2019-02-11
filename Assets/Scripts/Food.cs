using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public eFood pFood;
    public float pReactivationTime;
    public bool pDistanceInteractable;
    public Transform pDispensePoint;

    private bool mTimeInteractable;
    private Character mCharacter;
    private LevelManager mLevelManager;

    private void Start()
    {
        mTimeInteractable = true;
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    private void Update()
    {
        //TODO show interactability when both interactable are true

        if (!mTimeInteractable && Time.timeSinceLevelLoad >= pReactivationTime)
        {
            mTimeInteractable = true;
        }
    }

    private void OnMouseDown()
    {
        if (!mTimeInteractable || !pDistanceInteractable ||
            !(Vector3.Distance(pDispensePoint.position, mCharacter.transform.position) <= mLevelManager.pFoodInteractionDistance) ||
            !mLevelManager.TryCarry(pFood)) return;
        NET_EventCall eventCall = new NET_EventCall("FoodTaken");
        eventCall.SetParam("FoodType", pFood);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        SetInteractable();
    }

    public void SetInteractable()
    {
        mTimeInteractable = false;
        pReactivationTime = Time.timeSinceLevelLoad + mLevelManager.pFoodDeactivationTime;
    }
}
