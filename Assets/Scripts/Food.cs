using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public eDishes pFoodType;
    private Player mPlayer;

    private void Start()
    {
        mPlayer = FindObjectOfType<Player>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mPlayer.PushStack(transform);
        }
    }
}
