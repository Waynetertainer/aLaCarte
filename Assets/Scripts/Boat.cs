using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boat : MonoBehaviour
{
    public int pFirstTarget;
    public float pSpeed;
    public float pActivatonDistance;

    public Transform[] pAnchors = new Transform[2];
    public Transform[] pDestinations = new Transform[8];
    private int mActiveDestination;
    private Transform mNextTarget;
    private Food mFood;

    void Start()
    {
        mActiveDestination = pFirstTarget;
        mNextTarget = pDestinations[pFirstTarget % pDestinations.Length];
        mFood = GetComponentInChildren<Food>();
    }

    void Update()
    {
        //Debug.Log(transform.position);
        mFood.enabled = pAnchors.Any(t => Vector3.Distance(t.position, transform.position) <= pActivatonDistance);

        if (Vector3.Distance(transform.position, mNextTarget.position) <= 0.5f)
        {
            mNextTarget = pDestinations[mActiveDestination++ % pDestinations.Length];
        }
        transform.position = Vector3.MoveTowards(transform.position, mNextTarget.position, pSpeed * Time.deltaTime);
        Vector3 AimAt = mNextTarget.transform.position;
        AimAt.x -= transform.position.x;
        AimAt.z -= transform.position.z;

        float angle = (Mathf.Atan2(AimAt.x, AimAt.z) * Mathf.Rad2Deg);
        if (angle < 0)
        {
            angle += 360;
        }
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
