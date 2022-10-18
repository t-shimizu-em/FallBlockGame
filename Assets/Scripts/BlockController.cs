using System;
using System.Collections;
using UnityEngine;
using Common;
using UnityEngine.SocialPlatforms.Impl;

public class BlockController
{
    public bool doEraseFlg; // true:ブロック消去中
    public bool[] eraseRow = new bool[18];
    public int eraseCount = 0;
    public BlockProperty[,] blockPropList = new BlockProperty[GlobalConst.HIGHT, GlobalConst.WIDTH];
    public GameObject[,] blockObj = new GameObject[GlobalConst.HIGHT, GlobalConst.WIDTH];

    private int[,] wallBlockPos = new int[GlobalConst.HIGHT, GlobalConst.WIDTH]
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
        {1,2,2,2,2,2,2,2,2,2,2,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };
    private Color[] blockColor = new Color[11];

    public void CreateWallBlock(GameObject wallBlockPfb)
    {
        for (int i = 0; i < GlobalConst.WIDTH; i++)
        {
            for (int j = 0; j < GlobalConst.HIGHT; j++)
            {
                // 1マスごとにインスタンス作成し、各情報を保持
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = wallBlockPos[j, i];
                blockPropList[j, i] = blockProp;

                // 壁・床ブロック生成
                if (wallBlockPos[j, i] == 1 || wallBlockPos[j, i] == 2)
                {
                    GameObject.Instantiate(wallBlockPfb, new Vector3(i + GlobalConst.ORIGIN_POS_X, -j + GlobalConst.ORIGIN_POS_Y, 0), Quaternion.identity);
                }
            }
        }
    }

    public void CreatePlacementBlock(GameObject placementBlockPfb)
    {
        for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
            {
                // 配置ブロック生成
                blockObj[j, i] = GameObject.Instantiate(placementBlockPfb, new Vector3(i + GlobalConst.ORIGIN_POS_X, -j + GlobalConst.ORIGIN_POS_Y, 0), Quaternion.identity);
                blockObj[j, i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdatePlacementBlock()
    {
        for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
            {
                // 配置ブロック更新
                if (blockPropList[j, i].BlockStatus == 4)
                {
                    blockObj[j, i].gameObject.SetActive(true);
                    blockObj[j, i].GetComponent<SpriteRenderer>().color = blockPropList[j, i].BlockColor;
                }
                else
                {
                    blockObj[j, i].gameObject.SetActive(false);
                }
            }
        }
    }

    // ブロック消去判定
    public bool JudgeEraseRow()
    {
        bool eraseFlg = false;
        for (int j = 0; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            eraseRow[j] = false;
            bool buff = true;
            for (int i = 0; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
            {
                if (blockPropList[j, i].BlockStatus == 0)
                {
                    buff = false;
                }
            }

            if (buff)
            {
                eraseRow[j] = true;
                eraseFlg = true;
            }
        }
        return eraseFlg;
    }

    //ゲームオーバー判定
    public bool JudgeGameOver()
    {
        for (int i=0; i<GlobalConst.WIDTH; i++)
        {
            if (blockPropList[0, i].BlockStatus == 4)
            {
                return true;
            }
        }
        return false;
    }

    // ブロック消去エフェクトON
    public IEnumerator EraseEffectOn()
    {
        for (int j = 0; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (eraseRow[j])
            {
                for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
                {
                    blockColor[i] = blockObj[j, i].GetComponent<SpriteRenderer>().color;
                    blockObj[j, i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    // ブロック消去エフェクトOFF
    public IEnumerator EraseEffectOff()
    {
        for (int j = 0; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (eraseRow[j])
            {
                for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
                {
                    blockObj[j, i].GetComponent<SpriteRenderer>().color = blockColor[i];
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    // ブロック消去処理
    public IEnumerator BlockErase()
    {
        eraseCount = 0;
        for (int j = 0; j < GlobalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (eraseRow[j])
            {
                eraseCount++;
                // 1行消去
                for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
                {
                    blockPropList[j, i].BlockStatus = 0;
                }
                // 消去分、下にずらす
                for (int k = j; k > 0; k--)
                {
                    for (int i = 1; i < GlobalConst.PLACEMENT_BLOCK_WIDTH; i++)
                    {
                        blockPropList[k, i].Copy(blockPropList[k - 1, i]);
                    }
                }
            }
        }

        doEraseFlg = false;

        yield break;
    }
}