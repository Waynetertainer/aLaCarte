using Assets.Scripts;
using NET_System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float pTempInteractionDistance;//TODO GD
    public eFood pFood;
    public bool pInteractable;
    public float pReactivationTime;
    public float pReactivationTimeSpan;

    private Character mCharacter;
    private LevelManager mLevelManager;
    private Transform mDispensePoint;

    private void Start()
    {
        pInteractable = true;
        mDispensePoint = transform.GetChild(0);
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    private void Update()
    {
        if (!pInteractable&&Time.timeSinceLevelLoad>=pReactivationTime)
        {
            pInteractable = true;
        }
    }

    private void OnMouseDown()
    {
        if (!pInteractable || !enabled || !(Vector3.Distance(mDispensePoint.position, mCharacter.transform.position) <= pTempInteractionDistance)) return;
        if (mLevelManager.TryCarry(eCarryableType.Food, pFood))
        {
            NET_EventCall eventCall = new NET_EventCall("FoodTaken");
            eventCall.SetParam("FoodType", pFood);
            GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
            pInteractable = false;
            pReactivationTime = Time.timeSinceLevelLoad + pReactivationTimeSpan;
        }
    }
}
