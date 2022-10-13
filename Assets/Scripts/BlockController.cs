using System;
using UnityEngine;

public class BlockController
{
    public int[,] SetFallBlock(BlockInfoList blockInfoList, int blockNum, int rot)
    {
        int[,] fallBlock = new int[4, 4];

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

    public Color SetBlockColor(BlockInfoList blockInfoList, int blockNum)
    {
        int red = blockInfoList.blockList[blockNum].red;
        int green = blockInfoList.blockList[blockNum].green;
        int blue = blockInfoList.blockList[blockNum].blue;
        Color blockColor = new Color32((byte)red, (byte)green, (byte)blue, 255);

        return blockColor;
    }
}

