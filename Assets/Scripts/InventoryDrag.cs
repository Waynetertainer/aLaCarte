using Assets.Scripts;
using UnityEngine;

public class InventoryDrag : MonoBehaviour
{
    private Character mCharacter;
    private LevelManager mLevelManager;
    private GameObject mDragging;
    private GameObject mEmptyDome;
    private Vector3 mDraggedStartPosition;
    private eCarryableType mCarryableType;
    private eFood? mFood;

    private void Start()
    {
        mLevelManager = GameManager.pInstance.pLevelManager;
        mCharacter = mLevelManager.pCharacters[GameManager.pInstance.NetMain.NET_GetPlayerID() - 1];
        mEmptyDome = mLevelManager.pEmptyDome;
    }

    public void BeginDrag(GameObject obj)
    {
        if (!mLevelManager.pDragging)
        {
            mLevelManager.pDragging = true;
            mDragging = obj;
            mDraggedStartPosition = mDragging.transform.position;
            mCarryableType = mDragging.GetComponent<Carryable>().pType;
            mFood = mDragging.GetComponent<Carryable>().pFood;
        }
    }

    public void EndDrag(GameObject obj)
    {
        mLevelManager.pDragging = false;
        Ray ray = Camera.main.ScreenPointToRay(mDragging.transform.position);
        RaycastHit hit;
        if (mCarryableType == eCarryableType.Food)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
            {
                Table table = hit.transform.GetComponentInChildren<Table>();
                if (Vector3.Distance(mCharacter.transform.position, table.transform.position) <= mLevelManager.pTableInteractionDistance)
                {
                    if (table.TryDropFood(mFood))
                    {
                        mDragging.gameObject.SetActive(false);
                        mDragging.transform.parent.gameObject.SetActive(false);
                        mLevelManager.CheckFoodEmpty();
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12) && Vector3.Distance(mCharacter.transform.position, hit.transform.position) <= mLevelManager.pTableInteractionDistance)
            {
                mDragging.gameObject.SetActive(false);
                mDragging.transform.parent.gameObject.SetActive(false);
                mLevelManager.CheckFoodEmpty();
            }
        }
        else if (mCarryableType == eCarryableType.Dishes && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12) && Vector3.Distance(mCharacter.transform.position, hit.transform.position) <= mLevelManager.pTableInteractionDistance)
        {
            mDragging.gameObject.SetActive(false);
            mDragging.transform.parent.gameObject.SetActive(false);
            mCharacter.SetAnimation();
            mEmptyDome.SetActive(true);
        }
        else if (mCarryableType == eCarryableType.Customer)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
            {
                Table table = hit.transform.GetComponentInChildren<Table>();
                if (Vector3.Distance(mCharacter.transform.position, table.transform.position) <= mLevelManager.pTableInteractionDistance)
                {
                    if (table.TryDropCustomer(eCustomers.Normal))//TODO drop customertype
                    {
                        //table.pPlayerID = mCharacter.pID;  //TODO needed?
                        mDragging.gameObject.SetActive(false);
                        mDragging.transform.parent.gameObject.SetActive(false);
                        mEmptyDome.SetActive(true);
                        mCharacter.SetAnimation(eCarryableType.Customer);
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12) && Vector3.Distance(mCharacter.transform.position, hit.transform.position) <= mLevelManager.pTableInteractionDistance)
            {
                mDragging.gameObject.SetActive(false);
                mDragging.transform.parent.gameObject.SetActive(false);
                mEmptyDome.SetActive(true);
                mCharacter.SetAnimation();
            }
        }
        mDragging.transform.position = mDraggedStartPosition;
        mDragging = null;
    }

    private void Update()
    {
        if (mLevelManager.pDragging && mDragging != null)
        {
            mDragging.transform.position = Input.mousePosition;
        }
    }
}
