using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boat : MonoBehaviour
{
    public float pDistanceToTarget;


    private NavMeshAgent mAgent;
    public Transform[] pDestinations = new Transform[8];
    private int mActiveDestination;

    // Use this for initialization
	void Start () {
        mAgent = GetComponent<NavMeshAgent>();
	    mAgent.destination = pDestinations[mActiveDestination % pDestinations.Length].position;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Mathf.Sqrt(Mathf.Pow(transform.position.z-mAgent.destination.z,2)+ Mathf.Pow(transform.position.x - mAgent.destination.x, 2)) <= pDistanceToTarget)
        {
            mActiveDestination++;
            mAgent.destination = pDestinations[mActiveDestination%pDestinations.Length].position;
        }
	}
}
