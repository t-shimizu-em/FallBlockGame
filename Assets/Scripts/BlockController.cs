using System;
using UnityEngine;
using Common;

public class BlockController
{
    public int fallBlockPosX;
    public int fallBlockPosY;
    public int blockNum;
    public int nextBlockNum;
    public int rot;
    public float fallCountTime;
    public bool[] eraseRow = new bool[18];
    public bool downBanFlg; // false:下移動可 true:下移動禁止
    public bool rotBanFlg; // false:回転可 true:回転禁止

    public int[,] SetFallBlock(BlockInfoList blockInfoList, bool nextBlockFlg = false)
    {
        int[,] fallBlock = new int[4, 4];

        if (nextBlockFlg)
        {
            fallBlock = SetBlockStat(blockInfoList.blockList[nextBlockNum].blockStatA);
        }
        else
        {
            switch (rot)
            {
                case 0:
                    fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatA);
                    break;
                case 1:
                    if (blockInfoList.blockList[blockNum].blockStatB.Length != 0)
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatB);
                    }
                    else
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatA);
                    }
                    break;
                case 2:
                    if (blockInfoList.blockList[blockNum].blockStatC.Length != 0)
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatC);
                    }
                    else
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatA);
                    }
                    break;
                case 3:
                    if (blockInfoList.blockList[blockNum].blockStatD.Length != 0)
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatD);
                    }
                    else if (blockInfoList.blockList[blockNum].blockStatB.Length != 0)
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatB);
                    }
                    else
                    {
                        fallBlock = SetBlockStat(blockInfoList.blockList[blockNum].blockStatA);
                    }
                    break;
            }
        }

        return fallBlock;
    }

    public int[,] SetBlockStat(int[] oneDimensionArray)
    {
        int i = 0;
        int j = 0;
        int[,] blockStat = new int[4, 4];

        for (int k = 0; k < oneDimensionArray.Length; k++)
        {
            if (oneDimensionArray[k] == 3)
            {
                blockStat[i, j] = 3;
            }
            else
            {
                blockStat[i, j] = 0;
            }

            j++;

            if (j == 4)
            {
                i++;
                j = 0;
            }

        }

        return blockStat;
    }

    public Color SetBlockColor(BlockInfoList blockInfoList, bool nextBlockFlg = false)
    {
        int red;
        int green;
        int blue;

        if (nextBlockFlg)
        {
            red = blockInfoList.blockList[nextBlockNum].red;
            green = blockInfoList.blockList[nextBlockNum].green;
            blue = blockInfoList.blockList[nextBlockNum].blue;
        }
        else
        {
            red = blockInfoList.blockList[blockNum].red;
            green = blockInfoList.blockList[blockNum].green;
            blue = blockInfoList.blockList[blockNum].blue;
        }

        Color blockColor = new Color32((byte)red, (byte)green, (byte)blue, 255);

        return blockColor;
    }

    // 落下ブロックの水平移動処理
    public void HorizontalMove(BlockProperty[,] blockPropList, BlockInfoList blockInfoList)
    {
        if (Input.GetAxis("Horizontal") > 0 && !JudgeContactRight(blockPropList, blockInfoList))
        {
            fallBlockPosX++;
        }
        else if (Input.GetAxis("Horizontal") < 0 && !JudgeContactLeft(blockPropList, blockInfoList))
        {
            fallBlockPosX--;
        }
    }

    // 落下ブロックの回転処理
    public int[,] RotationMove(int[,] fallBlockStat, BlockInfoList blockInfoList)
    {
        rot++;
        if (rot == 4) rot = 0;
        fallBlockStat = SetFallBlock(blockInfoList);

        return fallBlockStat;
    }

    // 落下ブロック下移動処理
    public void VerticalMove()
    {
        if (Input.GetAxis("Vertical") < 0)
        {
            fallBlockPosY++;
            fallCountTime = 0;
        }
    }

    // 新しい落下ブロックの設定
    public void NextBlockInfoSet()
    {
        fallBlockPosX = GrovalConst.FALL_BLOCK_INIT_POS_X;
        fallBlockPosY = GrovalConst.FALL_BLOCK_INIT_POS_Y;
        blockNum = nextBlockNum;
        rot = 0;
    }


    // 着地判定
    public bool JudgeGround(BlockProperty[,] blockPropList, BlockInfoList blockInfoList)
    {
        bool groundFlg = false;
        int[,] block = SetFallBlock(blockInfoList);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (block[j, i] == 3)
                {
                    if (blockPropList[fallBlockPosY + j + 1, fallBlockPosX + i].BlockStatus == 2 || blockPropList[fallBlockPosY + j + 1, fallBlockPosX + i].BlockStatus == 4)
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
    public bool JudgeContactRight(BlockProperty[,] blockPropList, BlockInfoList blockInfoList)
    {
        bool contactFlg = false;
        int[,] block = SetFallBlock(blockInfoList);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (block[j, i] == 3)
                {
                    if (blockPropList[fallBlockPosY + j, fallBlockPosX + i + 1].BlockStatus == 1 || blockPropList[fallBlockPosY + j, fallBlockPosX + i + 1].BlockStatus == 4)
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
    public bool JudgeContactLeft(BlockProperty[,] blockPropList, BlockInfoList blockInfoList)
    {
        bool contactFlg = false;
        int[,] block = SetFallBlock(blockInfoList);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (block[j, i] == 3)
                {
                    if (blockPropList[fallBlockPosY + j, fallBlockPosX + i - 1].BlockStatus == 1 || blockPropList[fallBlockPosY + j, fallBlockPosX + i - 1].BlockStatus == 4)
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
    public bool JudgeEraseRow(BlockProperty[,] blockPropList)
    {
        bool eraseFlg = false;
        for (int j = 0; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            eraseRow[j] = false;
            bool buff = true;
            for (int i = 0; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
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
    public bool JudgeGameOver(BlockProperty[,] blockPropList, int fallBlockInitPosX, int fallBlockInitPosY)
    {
        if (blockPropList[fallBlockInitPosY, fallBlockInitPosX].BlockStatus == 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}