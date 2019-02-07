using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSpike : MonoBehaviour
{
    public GameObject pOrderPanel;

    private void OnMouseDown()
    {
        pOrderPanel.SetActive(true);
    }
}
