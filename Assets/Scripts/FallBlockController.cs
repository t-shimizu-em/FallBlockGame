using UnityEngine;
using Random = UnityEngine.Random;
using Common;

public class FallBlockController
{
    public int fallBlockPosX;
    public int fallBlockPosY;
    public int blockNum;
    public int nextBlockNum;
    public int rot;
    public float fallCountTime;
    public bool downBanFlg; // false:下移動可 true:下移動禁止
    public bool rotBanFlg; // false:回転可 true:回転禁止
    public BlockProperty[,] fallBlockPropList = new BlockProperty[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    public BlockProperty[,] nextFallBlockPropList = new BlockProperty[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    public GameObject[,] fallBlockObj = new GameObject[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    public GameObject[,] nextFallBlockObj = new GameObject[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];

    private int[,] fallBlockStat = new int[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    private int[,] nextFallBlockStat = new int[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];

    public void InitBlockNum(int listSize)
    {
        blockNum = Random.Range(0, listSize);
        nextBlockNum = Random.Range(0, listSize);
    }

    public void CreateFallBlock(GameObject fallBlockPfb, BlockInfoList blockInfoList)
    {
        fallBlockPosX = GlobalConst.FALL_BLOCK_INIT_POS_X;
        fallBlockPosY = GlobalConst.FALL_BLOCK_INIT_POS_Y;
        fallBlockStat = SetFallBlock(blockInfoList);
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 1マスごとにインスタンス作成し、各情報を保持
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = fallBlockStat[j, i];
                if (fallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = SetBlockColor(blockInfoList);
                }
                fallBlockPropList[j, i] = blockProp;

                // 落下ブロック生成
                fallBlockObj[j, i] = GameObject.Instantiate(fallBlockPfb, new Vector3(fallBlockPosX + i + GlobalConst.ORIGIN_POS_X, -fallBlockPosY - j + GlobalConst.ORIGIN_POS_Y, 0), Quaternion.identity);
                fallBlockObj[j, i].gameObject.SetActive(false);
            }
        }
    }

    public void CreateNextFallBlock(GameObject fallBlockPfb, BlockInfoList blockInfoList)
    {
        nextFallBlockStat = SetFallBlock(blockInfoList, true);
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 1マスごとにインスタンス作成し、各情報を保持
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = nextFallBlockStat[j, i];
                if (nextFallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = SetBlockColor(blockInfoList, true);
                }
                nextFallBlockPropList[j, i] = blockProp;

                //次落下ブロック生成
                nextFallBlockObj[j, i] = GameObject.Instantiate(fallBlockPfb, new Vector3(GlobalConst.NEXT_FALL_BLOCK_POS_X + i, GlobalConst.NEXT_FALL_BLOCK_POS_Y - j, 0), Quaternion.identity);
            }
        }
    }

    public void UpdateFallBlockProperty(BlockInfoList blockInfoList)
    {
        fallBlockPosX = GlobalConst.FALL_BLOCK_INIT_POS_X;
        fallBlockPosY = GlobalConst.FALL_BLOCK_INIT_POS_Y;
        fallBlockStat = SetFallBlock(blockInfoList);
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 1マスごとのインスタンスに格納されている各情報を更新
                fallBlockPropList[j, i].BlockStatus = fallBlockStat[j, i];
                if (fallBlockStat[j, i] == 3)
                {
                    fallBlockPropList[j, i].BlockColor = SetBlockColor(blockInfoList);
                }
            }
        }
    }

    public void UpdateNextFallBlockPropery(BlockInfoList blockInfoList)
    {
        nextFallBlockStat = SetFallBlock(blockInfoList, true);
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 1マスごとのインスタンスに格納されている各情報を更新
                nextFallBlockPropList[j, i].BlockStatus = nextFallBlockStat[j, i];
                if (nextFallBlockStat[j, i] == 3)
                {
                    nextFallBlockPropList[j, i].BlockColor = SetBlockColor(blockInfoList, true);
                }
            }
        }
    }

    public void UpdateFallBlock()
    {
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                //落下ブロック更新
                if (fallBlockPropList[j, i].BlockStatus == 3)
                {
                    fallBlockObj[j, i].gameObject.SetActive(true);
                    fallBlockObj[j, i].GetComponent<SpriteRenderer>().color = fallBlockPropList[j, i].BlockColor;
                    fallBlockObj[j, i].gameObject.transform.position = new Vector3(fallBlockPosX + i + GlobalConst.ORIGIN_POS_X, -fallBlockPosY - j + GlobalConst.ORIGIN_POS_Y, 0);
                }
                else
                {
                    fallBlockObj[j, i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateNextFallBlock()
    {
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 次落下ブロック更新
                if (nextFallBlockPropList[j, i].BlockStatus == 3)
                {
                    nextFallBlockObj[j, i].gameObject.SetActive(true);
                    nextFallBlockObj[j, i].GetComponent<SpriteRenderer>().color = nextFallBlockPropList[j, i].BlockColor;
                }
                else
                {
                    nextFallBlockObj[j, i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void Fall()
    {
        fallCountTime += Time.deltaTime;
        if (fallCountTime >= 1)
        {
            fallBlockPosY++;
            fallCountTime = 0;
        }
    }

    public bool JudgeGround(BlockProperty[,] blockPropList)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (fallBlockPropList[j, i].BlockStatus == 3)
                {
                    if (blockPropList[fallBlockPosY + j + 1, fallBlockPosX + i].BlockStatus == 2 || blockPropList[fallBlockPosY + j + 1, fallBlockPosX + i].BlockStatus == 4)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public BlockProperty[,] ReplaceBlock(BlockProperty[,] blockPropList)
    {
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                if (fallBlockPropList[j, i].BlockStatus == 3)
                {
                    blockPropList[j + fallBlockPosY, i + fallBlockPosX].BlockStatus = 4;
                    blockPropList[j + fallBlockPosY, i + fallBlockPosX].BlockColor = fallBlockPropList[j, i].BlockColor;
                }
            }
        }

        return blockPropList;
    }

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

    // 落下ブロックの水平移動処理
    public void HorizontalMove(BlockProperty[,] blockPropList)
    {
        if (Input.GetAxis("Horizontal") > 0 && !JudgeContactRight(blockPropList))
        {
            fallBlockPosX++;
        }
        else if (Input.GetAxis("Horizontal") < 0 && !JudgeContactLeft(blockPropList))
        {
            fallBlockPosX--;
        }
    }

    // 落下ブロックの水平移動禁止処理
    public void BanRotationMove(BlockProperty[,] blockPropList)
    {
        if (JudgeContactRight(blockPropList) || JudgeContactLeft(blockPropList))
        {
            rotBanFlg = true;
        }
        else
        {
            rotBanFlg = false;
        }
    }

    // 落下ブロックの回転処理
    public void RotationMove(BlockInfoList blockInfoList)
    {
        rot++;
        if (rot == 4) rot = 0;
        fallBlockStat = SetFallBlock(blockInfoList);
        for (int i = 0; i < GlobalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GlobalConst.FALL_BLOCK_WIDTH; j++)
            {
                // 1マスごとにインスタンス作成し、各情報を保持
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = fallBlockStat[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    blockProp.BlockColor = SetBlockColor(blockInfoList);
                }
                fallBlockPropList[j, i] = blockProp;
            }
        }
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

    // 右側壁当たり判定
    private bool JudgeContactRight(BlockProperty[,] blockPropList)
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (fallBlockPropList[j, i].BlockStatus == 3)
                {
                    if (blockPropList[fallBlockPosY + j, fallBlockPosX + i + 1].BlockStatus == 1 || blockPropList[fallBlockPosY + j, fallBlockPosX + i + 1].BlockStatus == 4)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // 左側壁当たり判定
    private bool JudgeContactLeft(BlockProperty[,] blockPropList)
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (fallBlockPropList[j, i].BlockStatus == 3)
                {
                    if (blockPropList[fallBlockPosY + j, fallBlockPosX + i - 1].BlockStatus == 1 || blockPropList[fallBlockPosY + j, fallBlockPosX + i - 1].BlockStatus == 4)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // 新しい落下ブロックの設定
    public void NextBlockInfoSet()
    {
        fallBlockPosX = GlobalConst.FALL_BLOCK_INIT_POS_X;
        fallBlockPosY = GlobalConst.FALL_BLOCK_INIT_POS_Y;
        blockNum = nextBlockNum;
        rot = 0;
    }
    

    private int[,] SetBlockStat(int[] oneDimensionArray)
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

    // セーブデータから色情報取得
    private Color SetBlockColor(BlockInfoList blockInfoList, bool nextBlockFlg = false)
    {
        int red;
        int green;
        int blue;
        int key;

        if (nextBlockFlg)
        {
            key = nextBlockNum;
        }
        else
        {
            key = blockNum;
        }

        red = blockInfoList.blockList[key].red;
        green = blockInfoList.blockList[key].green;
        blue = blockInfoList.blockList[key].blue;

        return new Color32((byte)red, (byte)green, (byte)blue, 255);
    }

    public void SetBlockNum(int listSize)
    {
        blockNum = nextBlockNum;
        nextBlockNum = Random.Range(0, listSize);
    }
}

