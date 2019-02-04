using System.Linq;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Transform[] pAnchors = new Transform[2];

    private Food mFood;
    private LevelManager mLevelManager;

    private void Start()
    {
        mFood = GetComponentInChildren<Food>();
        mLevelManager = GameManager.pInstance.pLevelManager;
    }

    private void Update()
    {
        mFood.pDistanceInteractable = pAnchors.Any(t => Vector3.Distance(t.position, transform.position) <= mLevelManager.pBoatInteractionDistance);
    }
}
