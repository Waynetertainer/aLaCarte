using Assets.Scripts;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float pTempInteractionDistance;
    public eFood pFood;

    private Character mCharacter;
    private LevelManager mLevelManager;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
    }

    private void Update()
    {
        ////Debug.Log(transform.parent.parent.position);
        //if (Input.GetMouseButtonDown(0)) //TODO touch input
        //{
        //    if (Input.GetMouseButtonDown(0) &&
        //        //Vector3.Distance(transform.parent.parent.position, mCharacter.transform.position) <= pTempInteractionDistance &&
        //        mLevelManager.CanCarry(pFoodtype))
        //    {
        //        //NET_EventCall eventCall = new NET_EventCall("CustomerTaken");
        //        //eventCall.SetParam("PlayerID", mCharacter.pID);
        //        //GameManager.pInstance.NetMain.NET_CallEvent(eventCall);
        //        GameManager.pInstance.pLevelManager.ChangeCarry((eCarryableType)pFoodTypeInt);
        //        //gameObject.SetActive(false);
        //    }
        //}
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && (Input.GetMouseButtonDown(0)))//TODO check distance
        {
            if (hit.collider.gameObject == gameObject)
            {
                mLevelManager.TryCarry(eCarryableType.Food, pFood);

                //if (mLevelManager.CanCarry((eCarryableType)pFoodTypeInt))
                // {
                //     GameManager.pInstance.pLevelManager.ChangeCarry((eCarryableType)pFoodTypeInt);
                // }
            }
        }
    }
}
