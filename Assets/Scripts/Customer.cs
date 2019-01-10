using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float pTempInteractionDistance;

    private Character mCharacter;


    private void OnMouseOver()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Vector3.Distance(transform.position, mCharacter.transform.position) <= pTempInteractionDistance)
        {

        }
    }
}
