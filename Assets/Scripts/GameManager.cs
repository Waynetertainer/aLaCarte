using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager pInstance;

    public Random pRandom = new Random();

    public Sprite[] pDisheSprites = new Sprite[2];
    public Sprite[] pEmotionSprites = new Sprite[4];
    public Sprite[] pDollarSprites = new Sprite[3];

    private void Awake()
    {
        if (pInstance == null)

            pInstance = this;

        else if (pInstance != this)

            Destroy(gameObject);
    }
}
