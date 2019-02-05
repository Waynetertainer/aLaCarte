using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int pId;

    private GatesManager mGatesManager;
    private Animator mAnimator;

    private void Start()
    {
        mGatesManager = GameManager.pInstance.pLevelManager.GetComponent<GatesManager>();
        mAnimator = transform.GetChild(0).GetComponent<Animator>();
        SetGate(false);
    }

    private void OnTriggerExit(Collider other)
    {
        mGatesManager.DelegateGateClosed(pId);
    }

    public void SetGate(bool closed)
    {
        mAnimator.SetBool("CloseDoor",closed);
    }
}
