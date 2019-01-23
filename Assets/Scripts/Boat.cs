using System.Linq;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public float pActivatonDistance;
    public Transform[] pAnchors = new Transform[2];

    private Food mFood;

    private void Start()
    {
        mFood = GetComponentInChildren<Food>();
    }

    private void Update()
    {
        mFood.enabled = pAnchors.Any(t => Vector3.Distance(t.position, transform.position) <= pActivatonDistance);
    }
}
