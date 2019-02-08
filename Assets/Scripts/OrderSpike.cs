using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSpike : MonoBehaviour
{
    public GameObject pOrderPanel;

    private void OnMouseDown()
    {
        StartCoroutine(FrameDelayer());
    }

    private IEnumerator FrameDelayer()
    {
        yield return null;
        pOrderPanel.SetActive(true);
    }
}
