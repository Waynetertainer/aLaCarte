
using System.Collections;
using System.Linq;
using NET_System;
using UnityEngine;

public class GatesManager : MonoBehaviour
{
    private readonly Gate[] mGates= new Gate[3];
    private bool[] mGatesClosed = new bool[3];

    private void Start()
    {
        foreach (Gate gate in FindObjectsOfType<Gate>())
        {
            mGates[gate.pId] = gate;
        }
    }

    public void DelegateGateClosed(int id)
    {
        SendClosedMsg(id);
        SetGateClosed(id);
    }

    public void SetGateClosed(int id)
    {
        mGatesClosed[id] = true;
        mGates[id].SetGate(true);
        if (!mGatesClosed.All(p => p)) return;
        StartCoroutine(OpenAll());
    }

    private void SendClosedMsg(int id)
    {
        NET_EventCall eventCall = new NET_EventCall("ClosedGate");
        eventCall.SetParam("GateID", id);
        GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
    }

    IEnumerator OpenAll()
    {
        yield return new WaitForSeconds(3);
        for (var i = 0; i < mGatesClosed.Length; i++)
        {
            mGatesClosed[i] = false;
        }
        foreach (Gate gate in mGates)
        {
            gate.SetGate(false);
        }
    }
}
