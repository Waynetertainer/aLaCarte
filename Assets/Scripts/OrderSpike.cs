using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSpike : MonoBehaviour
{
    public GameObject pOrderPanel;
    private Transform mCharacter;
    private LevelManager mLevelManager;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].transform;
    }

    private void OnMouseDown()
    {
        if (Vector3.Distance(mCharacter.position,transform.position)<=mLevelManager.pTableInteractionDistance)
        {
            StartCoroutine(FrameDelayer()); 
        }
    }

    private IEnumerator FrameDelayer()
    {
        yield return null;
        pOrderPanel.SetActive(true);
    }
}
