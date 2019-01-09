using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] pNavMeshTargets = new GameObject[2];

    private void Start()
    {
        GameManager.pInstance.pLevelManager = this;
    }
}
