using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject wallBlockPfb;
    public GameObject fallBlockPfb;
    public GameObject placementBlockPfb;
    public float OX, OY; // 原点座標
    public const int hight = 21;
    public const int width = 14;

    // ブロックの状態
    // 0:空
    // 1:壁
    // 2:落下ブロック
    // 3:配置ブロック
    // 4:地面ブロック

    private GameObject[,] blockObj = new GameObject[21, 14];
    private GameObject[,] fallBlockObj = new GameObject[4, 4];
    FallBlockController fallBlockController = new FallBlockController();
    private static int fallBlockInitPosX = 4;
    private static int fallBlockInitPosY = 0;
    private static int fallBlockPosX;
    private static int fallBlockPosY;
    private int blockNum;
    private int rot;
    private float fallCountTime;
    private float groundCountTime;
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
        {1,4,4,4,4,4,4,4,4,4,4,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };
    private int gameStat;
    private bool[] eraseRow =　new bool[18];
    private const int GAMEOVER = 0;
    private const int START = 1;
    private const int GROUND = 2;
    private const int ERASE = 3;
    private bool downBanFlg; // 0:下移動可 1:下移動禁止

    
    void Start()
    {
        gameStat = START;

        // 壁ブロック生成
        blockStat = wallBlockPos;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < hight; j++)
            {
                if (blockStat[j, i] == 1 || blockStat[j,i] == 4) {
                    Instantiate(wallBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
                }
            }
        }

        // 落下ブロック初期設定
        fallCountTime = 0;
        groundCountTime = 0;
        downBanFlg = false;
        fallBlockPosX = fallBlockInitPosX;
        fallBlockPosY = fallBlockInitPosY;
        blockNum = Random.Range(0, 10);
        rot = 0;
        fallBlockStat = fallBlockController.SetFallBlock(blockNum, rot);

    }

    void Update()
    {
        switch (gameStat)
        {
            case START:
                downBanFlg = false;
                
                // 1秒ごとにブロックを落下
                fallCountTime += Time.deltaTime;
                if (fallCountTime >= 1)
                {
                    fallBlockPosY++;
                    fallCountTime = 0;
                }

                // 着地判定
                if (JudgeGround(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
                {
                    gameStat = GROUND;
                }
                break;
            case GROUND:
                fallCountTime = 0;
                groundCountTime += Time.deltaTime;
                downBanFlg = true;

                // 落下ブロックの非着地判定
                if (!JudgeGround(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
                {
                    groundCountTime = 0;
                    gameStat = START;
                }
                if (groundCountTime >= 1)
                {
                    // 着地した落下ブロックを配置ブロックに置き換え
                    for (int i=0; i<4; i++)
                    {
                        for (int j=0; j<4; j++)
                        {
                            if (fallBlockStat[j, i] == 2)
                            {
                                blockStat[j + fallBlockPosY, i + fallBlockPosX] = 3;
                            }
                        }
                    }

                    // 消去判定
                    if (JudgeEraseRow())
                    {
                        gameStat = ERASE;
                    }
                    else
                    {
                        NextBlockSet();
                    }
                }
                break;
            case ERASE:
                BlockErase();
                NextBlockSet();
                gameStat = START;
                break;
            case GAMEOVER:

                break;
        }

        // 落下ブロックの水平移動操作
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") > 0 && !JudgeContactRight(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
            {
                fallBlockPosX++;
            }
            else if (Input.GetAxis("Horizontal") < 0 && !JudgeContactLeft(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
            {
                fallBlockPosX--;
            }
        }

        // 落下ブロックの回転操作
        if (Input.GetButtonDown("Jump"))
        {
            rot++;
            if (rot == 4) rot = 0;
            fallBlockStat = fallBlockController.SetFallBlock(blockNum, rot);
        }
        // 落下ブロック落下速度上昇
        if (Input.GetButtonDown("Vertical") && !downBanFlg)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                fallBlockPosY++;
                fallCountTime = 0;
            }
        }

        UpdateDisplay();
    }


    // 描画処理
    private void UpdateDisplay()
    {
        // 落下ブロック生成
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

        // 配置ブロック生成
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 18; j++)
            {
                Destroy(blockObj[j,i]);
                if (blockStat[j, i] == 3)
                {
                    blockObj[j,i] = Instantiate(placementBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
                }
            }
        }
    }

    // 新しい落下ブロックの生成
    private void NextBlockSet()
    {
        fallBlockPosX = fallBlockInitPosX;
        fallBlockPosY = fallBlockInitPosY;
        blockNum = Random.Range(0, 8);
        rot = 0;
        fallBlockStat = fallBlockController.SetFallBlock(blockNum, rot);
        gameStat = START;
        groundCountTime = 0;
    }

    // 着地判定
    private bool JudgeGround(int blockNum, int rot, int[,] blockStat, int x, int y)
    {
        bool groundFlg = false;
        int[,] block = new int[4, 4];
        block = fallBlockController.SetFallBlock(blockNum, rot);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (block[j, i] == 2)
                {
                    if (blockStat[y + j + 1, x + i] == 3 || blockStat[y + j + 1, x + i] == 4)
                    {
                        groundFlg = true;
                        break;
                    }
                }
            }
        }
        return groundFlg;
    }

    // 右側壁当たり判定
    private bool JudgeContactRight(int blockNum, int rot, int[,] blockStat, int x, int y)
    {
        bool contactFlg = false;
        int[,] block = new int[4, 4];
        block = fallBlockController.SetFallBlock(blockNum, rot);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (block[j, i] == 2)
                {
                    if (blockStat[y + j, x + i + 1] == 1 || blockStat[y + j, x + i + 1] == 3)
                    {
                        contactFlg = true;
                        break;
                    }
                }
            }
        }
        return contactFlg;
    }

    // 左側壁当たり判定
    private bool JudgeContactLeft(int blockNum, int rot, int[,] blockStat, int x, int y)
    {
        bool contactFlg = false;
        int[,] block = new int[4, 4];
        block = fallBlockController.SetFallBlock(blockNum, rot);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (block[j, i] == 2)
                {
                    if (blockStat[y + j, x + i - 1] == 1 || blockStat[y + j, x + i - 1] == 3)
                    {
                        contactFlg = true;
                        break;
                    }
                }
            }
        }
        return contactFlg;
    }

    // ブロック消去判定
    private bool JudgeEraseRow()
    {
        bool eraseFlg = false;
        for (int j=0; j<18; j++)
        {
            eraseRow[j] = false;
            bool buff = true;
            for (int i=0; i<11; i++)
            {
                if (blockStat[j,i] == 0)
                {
                    buff = false;
                }
            }
            if (buff)
            {
                eraseRow[j] = true;
                eraseFlg = true;
            }
            Debug.Log("eraseRow[" + j + "]:" + eraseRow[j]);
        }
        return eraseFlg;
    }

    // ブロック消去
    private void BlockErase()
    {
        int eraseCount;

        eraseCount = 0;
        for (int j = 0; j < 18; j++)
        {
            if (eraseRow[j] == true)
            {
                eraseCount++;
                // 1行消去
                for (int i = 1; i < 11; i++)
                {
                    blockStat[j, i] = 0;
                }
                // 消去分、下にずらす
                for (int k = j; k > 0; k--)
                {
                    for (int i = 1; i < 11; i++)
                    {
                        blockStat[k, i] = blockStat[k - 1, i];
                        blockStat[k - 1, i] = 0;
                    }
                }
            }
        }
    }
}
