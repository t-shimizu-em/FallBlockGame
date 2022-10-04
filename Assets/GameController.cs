using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject wallBlockPfb;
    public GameObject fallBlockPfb;
    public float OX, OY; // 原点座標
    public const int hight = 21;
    public const int width = 14;

    // ブロックの状態
    // 0:空
    // 1:壁
    // 2:落下ブロック
    // 3:配置ブロック

    private GameObject[,] fallBlockObj = new GameObject[4, 4];
    private static int fallBlockInitPosX = 4;
    private static int fallBlockInitPosY = 0;
    private static int fallBlockPosX;
    private static int fallBlockPosY;
    private int blockNum;
    private int rot;
    private float fallCountTime;
    private int[,] fallBlockStat = new int[4, 4];
    private int[,] blockStat = new int[21, 14];
    private int[,] wallBlockPos = new int[21, 14]
    {
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,1,1,1,1,1,1,1,1,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };
    
    void Start()
    {
        // 壁ブロック生成
        blockStat = wallBlockPos;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < hight; j++)
            {
                if (blockStat[j, i] == 1) {
                    Instantiate(wallBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
                }
            }
        }

        // 落下ブロック初期設定
        fallCountTime = 0;
        fallBlockPosX = fallBlockInitPosX;
        fallBlockPosY = fallBlockInitPosY;
        blockNum = Random.Range(0, 10);
        rot = 0;
        FallBlockController fallBlockController = new FallBlockController();
        fallBlockStat = fallBlockController.setFallBlock(blockNum, rot);

    }

    void Update()
    {
        // 1秒ごとにブロックを落下
        fallCountTime += Time.deltaTime;
        if (fallCountTime >= 1)
        {
            fallBlockPosY++;
            fallCountTime = 0;
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Destroy(fallBlockObj[j, i]);
                if (fallBlockStat[j, i] == 2)
                {
                    fallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(fallBlockPosX + i + OX, -fallBlockPosY - j + OY, 0), Quaternion.identity);
                }
            }
        }
    }
}
