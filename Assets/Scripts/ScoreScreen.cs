using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    public Button pLeaveButton;

    //public GameObject pOwnWinCoins;
    //public GameObject pOpponentWinCoins;
    public Text pOwnScore;
    public Text pOwnScoreShaddow;
    public Text pOpponentScore;
    public Text pOpponentScoreShaddow;
    public Animator pAnimator;

    private LevelManager mLevelManager;

    private void Start()
    {
        pLeaveButton.onClick.AddListener(delegate { GameManager.pInstance.ReturnToMainMenu(); });
    }

    public void Open()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        //pLeaveButton.onClick.AddListener(delegate { GameManager.pInstance.ReturnToMainMenu(); });
        pOwnScore.text = pOwnScoreShaddow.text = mLevelManager.pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1].ToString("C", new CultureInfo("de-DE"));
        pOpponentScore.text = pOpponentScoreShaddow.text = mLevelManager.pScores[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)].ToString("C", new CultureInfo("de-DE"));
        if (mLevelManager.pScores[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1] > mLevelManager.pScores[1 - (GameManager.pInstance.NetMain.NET_GetPlayerID() - 1)])
        {
            pAnimator.SetBool("ownVictory", true);
        }
        else
        {
            pAnimator.SetBool("opponentVictory", true);

        }
    }
}