using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float pTempInteractionDistance;//TODO GD
    public eFood pFood;
    public float pReactivationTime;
    public float pReactivationTimeSpan = 2;//TODO GD
    public bool pDistanceInteractable;

    private bool mTimeInteractable;
    private Character mCharacter;
    private LevelManager mLevelManager;
    private Transform mDispensePoint;

    private void Start()
    {
        mTimeInteractable = true;
        mDispensePoint = transform.GetChild(0);
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
            !(Vector3.Distance(mDispensePoint.position, mCharacter.transform.position) <= pTempInteractionDistance) ||
            !mLevelManager.TryCarry(eCarryableType.Food, pFood)) return;
        NET_EventCall eventCall = new NET_EventCall("FoodTaken");
        eventCall.SetParam("FoodType", pFood);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        SetInteractable();
    }

    public void SetInteractable()
    {
        mTimeInteractable = false;
        pReactivationTime = Time.timeSinceLevelLoad + pReactivationTimeSpan;
    }
}
