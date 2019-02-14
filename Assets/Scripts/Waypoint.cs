using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Transform pCharacter;
    public Waypoint pNext;
    public ParticleSystem pExplosion;
    public Tutorial pTutorial;

    private void Update()
    {
        if (Vector3.Distance(pCharacter.position, transform.position) <= pTutorial.pWaypointInteractionDistance)
        {
            pExplosion.Play();
            if (pNext != null)
            {
                pNext.enabled = true;
            }
            else
            {
                pTutorial.Moved();
            }
            gameObject.SetActive(false);
        }
    }
}
