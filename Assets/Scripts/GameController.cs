using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject wallBlockPfb;
    public GameObject fallBlockPfb;
    public GameObject placementBlockPfb;
    public GameObject pausePanel;
    public GameObject retryButton;
    public GameObject titleButton;
    public Button pauseButton;
    public AudioClip rotSe;
    public AudioClip moveSe;
    public AudioClip eraseSe;
    public AudioSource rotAudioSource;
    public AudioSource moveAudioSource;
    public AudioSource eraseAudioSource;
    public Text scoreText;
    public Text gameOverText;
    public float OX, OY; // 原点座標
    public const int HIGHT = 21;
    public const int WIDTH = 14;
    public const int PLACEMENT_BLOCK_HIGHT = 18;
    public const int PLACEMENT_BLOCK_WIDTH = 11;
    public const int FALL_BLOCK_HIGHT = 4;
    public const int FALL_BLOCK_WIDTH = 4;

    // ブロックの状態
    // 0:空
    // 1:壁ブロック
    // 2:地面ブロック
    // 3:落下ブロック
    // 4:配置ブロック

    private BlockInfoList blockInfoList;
    private GameObject[,] blockObj = new GameObject[HIGHT, WIDTH];
    private GameObject[,] fallBlockObj = new GameObject[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    private GameObject[,] nextFallBlockObj = new GameObject[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    BlockController blockController = new BlockController();
    private static int nextFallBlockPosX = 14;
    private static int nextFallBlockPosY = 2;
    private static int fallBlockInitPosX = 4;
    private static int fallBlockInitPosY = 0;
    private static int fallBlockPosX;
    private static int fallBlockPosY;
    private int blockNum;
    private int nextBlockNum;
    private int rot;
    private int score;
    private float fallCountTime;
    private float groundCountTime;
    private int[,] fallBlockStat = new int[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    private int[,] nextFallBlockStat = new int[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    private BlockPropertyClass[,] blockPropList = new BlockPropertyClass[HIGHT, WIDTH];
    private BlockPropertyClass[,] fallBlockPropList = new BlockPropertyClass[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    private BlockPropertyClass[,] nextFallBlockPropList = new BlockPropertyClass[FALL_BLOCK_HIGHT, FALL_BLOCK_WIDTH];
    private int[,] wallBlockPos = new int[HIGHT, WIDTH]
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
    private int gameStat;
    private bool[] eraseRow =　new bool[18];
    private const int GAMEOVER = 0;
    private const int START = 1;
    private const int GROUND = 2;
    private const int ERASE = 3;
    private bool downBanFlg; // false:下移動可 true:下移動禁止
    private bool rotBanFlg; // false:回転可 true:回転禁止
    private bool pauseFlg; // true:一時停止
    private bool doEraseFlg; // true:ブロック消去中

    
    void Start()
    {
        blockInfoList = BlockInfoIO.LoadBlockList();
        gameStat = START;
        pauseFlg = false;
        doEraseFlg = false;
        fallCountTime = 0;
        groundCountTime = 0;
        downBanFlg = false;
        blockNum = Random.Range(0, blockInfoList.blockList.Count);
        nextBlockNum = Random.Range(0, blockInfoList.blockList.Count);
        rot = 0;

        // 壁・地面ブロック初期設定
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HIGHT; j++)
            {
                BlockPropertyClass blockProp = new BlockPropertyClass();
                blockProp.BlockStatus = wallBlockPos[j, i];
                if (blockProp.BlockStatus == 1 || blockProp.BlockStatus == 2) {
                    Instantiate(wallBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);

                }
                blockPropList[j, i] = blockProp;
            }
        }

        // 落下ブロック初期設定
        fallBlockPosX = fallBlockInitPosX;
        fallBlockPosY = fallBlockInitPosY;
        fallBlockStat = blockController.SetFallBlock(blockInfoList, blockNum, rot);
        for (int i=0; i<FALL_BLOCK_HIGHT; i++)
        {
            for (int j=0; j<FALL_BLOCK_WIDTH; j++)
            {
                BlockPropertyClass blockProp = new BlockPropertyClass();
                blockProp.BlockStatus = fallBlockStat[j, i];
                fallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(fallBlockPosX + i + OX, -fallBlockPosY - j + OY, 0), Quaternion.identity);
                fallBlockObj[j, i].gameObject.SetActive(false);
                if (fallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, blockNum);
                }
                fallBlockPropList[j, i] = blockProp;
            }
        }

        // 次落下ブロック初期設定
        nextFallBlockStat = blockController.SetFallBlock(blockInfoList, nextBlockNum, rot);
        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
            {
                BlockPropertyClass blockProp = new BlockPropertyClass();
                blockProp.BlockStatus = nextFallBlockStat[j, i];
                nextFallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(nextFallBlockPosX + i, nextFallBlockPosY - j, 0), Quaternion.identity);
                if (nextFallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, nextBlockNum);
                }
                nextFallBlockPropList[j, i] = blockProp;
            }
        }

        // 配置ブロック初期設定
        for (int i = 1; i < PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < PLACEMENT_BLOCK_HIGHT; j++)
            {
                blockObj[j, i] = Instantiate(placementBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
                blockObj[j, i].gameObject.SetActive(false);
            }
        }

        // 一時停止ボタン
        pauseButton.onClick.AddListener(Pause);

        UpdateDisplay();
    }

    void Update()
    {
        if (!doEraseFlg)
        {
            if (pauseFlg)
            {
                return;
            }

            switch (gameStat)
            {
                case START:
                    downBanFlg = false;
                    rotBanFlg = false;

                    // 1秒ごとにブロックを落下
                    fallCountTime += Time.deltaTime;
                    if (fallCountTime >= 1)
                    {
                        fallBlockPosY++;
                        fallCountTime = 0;
                    }

                    // 着地判定
                    if (JudgeGround())
                    {
                        gameStat = GROUND;
                    }

                    UpdateFallBlockProperty();
                    UpdateDisplay();

                    break;
                case GROUND:
                    fallCountTime = 0;
                    groundCountTime += Time.deltaTime;
                    downBanFlg = true;
                    rotBanFlg = true;

                    // 落下ブロックの非着地判定
                    if (!JudgeGround())
                    {
                        groundCountTime = 0;
                        gameStat = START;
                    }

                    if (groundCountTime >= 1)
                    {
                        // 着地した落下ブロックを配置ブロックに置き換え
                        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
                        {
                            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
                            {
                                if (fallBlockPropList[j, i].BlockStatus == 3)
                                {
                                    blockPropList[j + fallBlockPosY, i + fallBlockPosX].BlockColor = fallBlockPropList[j, i].BlockColor;
                                    blockPropList[j + fallBlockPosY, i + fallBlockPosX].BlockStatus = 4;
                                }
                            }
                        }

                        // 配置ブロック描画
                        for (int i = 1; i < PLACEMENT_BLOCK_WIDTH; i++)
                        {
                            for (int j = 1; j < PLACEMENT_BLOCK_HIGHT; j++)
                            {
                                if (blockPropList[j, i].BlockStatus == 4)
                                {
                                    blockObj[j, i].GetComponent<SpriteRenderer>().color = blockPropList[j, i].BlockColor;
                                    blockObj[j, i].gameObject.SetActive(true);
                                }
                                
                            }
                        }

                        // 判定
                        if (JudgeEraseRow())
                        {
                            gameStat = ERASE;
                        }
                        else if (JudgeGameOver())
                        {
                            gameStat = GAMEOVER;
                        }
                        else
                        {
                            NextBlockSet();
                            gameStat = START;
                        }
                    }
                    break;
                case ERASE:
                    StartCoroutine(BlockErase());
                    gameStat = START;
                    break;
                case GAMEOVER:
                    gameOverText.gameObject.SetActive(true);
                    retryButton.gameObject.SetActive(true);
                    titleButton.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(false);
                    break;
            }
        }

        // 落下ブロックの水平移動操作
        if (Input.GetButtonDown("Horizontal"))
        {
            moveAudioSource.PlayOneShot(moveSe);
            if (Input.GetAxis("Horizontal") > 0 && !JudgeContactRight())
            {
                fallBlockPosX++;
            }
            else if (Input.GetAxis("Horizontal") < 0 && !JudgeContactLeft())
            {
                fallBlockPosX--;
            }

            UpdateFallBlockProperty();
            UpdateDisplay();
        }

        // 壁際での回転禁止処理
        if (JudgeContactRight() || JudgeContactLeft() || gameStat == GAMEOVER)
        {
            rotBanFlg = true;
        }
        else
        {
            rotBanFlg = false;
        }

        // 落下ブロックの回転操作
        if (Input.GetButtonDown("Jump") && !rotBanFlg)
        {
            rotAudioSource.PlayOneShot(rotSe);
            rot++;
            if (rot == 4) rot = 0;
            fallBlockStat = blockController.SetFallBlock(blockInfoList, blockNum, rot);
        }

        // 落下ブロック落下速度上昇
        if (Input.GetButtonDown("Vertical") && !downBanFlg)
        {
            moveAudioSource.PlayOneShot(moveSe);
            if (Input.GetAxis("Vertical") < 0)
            {
                fallBlockPosY++;
                fallCountTime = 0;
            }
        }
    }

    // 落下ブロック情報設定
    private void UpdateFallBlockProperty()
    {
        // 落下ブロック情報設定
        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
            {
                BlockPropertyClass blockProp = fallBlockPropList[j, i];
                blockProp.BlockStatus = fallBlockStat[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, blockNum);
                }
                fallBlockPropList[j, i] = blockProp;
            }
        }

        // 次落下ブロック情報設定
        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
            {
                BlockPropertyClass blockProp = nextFallBlockPropList[j, i];
                blockProp.BlockStatus = nextFallBlockStat[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, nextBlockNum);
                }
                nextFallBlockPropList[j, i] = blockProp;
            }
        }
    }

    // 描画処理
    private void UpdateDisplay()
    {
        // 落下ブロック生成
        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
            {
                fallBlockObj[j, i].gameObject.SetActive(false);
                BlockPropertyClass blockProp = fallBlockPropList[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    fallBlockObj[j, i].gameObject.SetActive(true);
                    fallBlockObj[j, i].GetComponent<SpriteRenderer>().color = fallBlockPropList[j, i].BlockColor;
                    fallBlockObj[j, i].gameObject.transform.position = new Vector3(fallBlockPosX + i + OX, -fallBlockPosY - j + OY, 0);
                }
            }
        }

        // 次落下ブロック生成
        for (int i = 0; i < FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < FALL_BLOCK_WIDTH; j++)
            {
                nextFallBlockObj[j, i].gameObject.SetActive(false);
                BlockPropertyClass blockProp = nextFallBlockPropList[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    nextFallBlockObj[j, i].gameObject.SetActive(true);
                    nextFallBlockObj[j, i].GetComponent<SpriteRenderer>().color = nextFallBlockPropList[j, i].BlockColor;
                }
            }
        }

        // 配置ブロック生成
        for (int i = 1; i < PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < PLACEMENT_BLOCK_HIGHT; j++)
            {
                BlockPropertyClass blockProp = blockPropList[j, i];
                if (blockProp.BlockStatus == 4)
                {
                    blockObj[j, i].gameObject.SetActive(true);
                    blockObj[j, i].GetComponent<SpriteRenderer>().color = blockProp.BlockColor;
                }
                else
                {
                    blockObj[j, i].gameObject.SetActive(false);
                }
            }
        }
    }

    // 新しい落下ブロックの設定
    private void NextBlockSet()
    {
        fallBlockPosX = fallBlockInitPosX;
        fallBlockPosY = fallBlockInitPosY;
        blockNum = nextBlockNum;
        nextBlockNum = Random.Range(0, 7);
        rot = 0;
        fallBlockStat = blockController.SetFallBlock(blockInfoList, blockNum, rot);
        nextFallBlockStat = blockController.SetFallBlock(blockInfoList, nextBlockNum, rot);
        gameStat = START;
        groundCountTime = 0;
    }

    // 着地判定
    private bool JudgeGround()
    {
        bool groundFlg = false;
        int[,] block  = blockController.SetFallBlock(blockInfoList, blockNum, rot);
        for (int i=0; i<4; i++)
        {
            for (int j=3; j>=0; j--)
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
    private bool JudgeContactRight()
    {
        bool contactFlg = false;
        int[,] block = blockController.SetFallBlock(blockInfoList, blockNum, rot);
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
    private bool JudgeContactLeft()
    {
        bool contactFlg = false;
        int[,] block = blockController.SetFallBlock(blockInfoList, blockNum, rot);
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
    private bool JudgeEraseRow()
    {
        bool eraseFlg = false;
        for (int j=0; j<18; j++)
        {
            eraseRow[j] = false;
            bool buff = true;
            for (int i=0; i<11; i++)
            {
                if (blockPropList[j,i].BlockStatus == 0)
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

    // ブロック消去（コルーチン1）
    private IEnumerator BlockErase()
    {
        int eraseCount = 0;
        int erasedCount = 0;
        doEraseFlg = true;
        for (int j = 0; j < 18; j++)
        {
            if (eraseRow[j])
            {
                eraseCount++;
                StartCoroutine(BlockEraseCoroutine(j, () =>
                {
                    erasedCount++;
                }));
            }
        }

        while (erasedCount < eraseCount)
        {
            yield return null;
        }

        for (int j = 0; j < PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (eraseRow[j])
            {
                // 1行消去
                for (int i = 1; i < PLACEMENT_BLOCK_WIDTH; i++)
                {
                    blockPropList[j, i].BlockStatus = 0;
                }
                // 消去分、下にずらす
                for (int k = j; k > 0; k--)
                {
                    for (int i = 1; i < PLACEMENT_BLOCK_WIDTH; i++)
                    {
                        blockPropList[k, i].Copy(blockPropList[k - 1, i]);
                    }
                }
            }
        }

        doEraseFlg = false;
        score += AddScore(eraseCount);
        scoreText.text = "Score:" + score;
        eraseAudioSource.PlayOneShot(eraseSe);
        NextBlockSet();
    }

    // ブロック消去（コルーチン2）
    private IEnumerator BlockEraseCoroutine(int j, System.Action callback)
    {
        Color[] blockColor = new Color[11];

        for (int i=1; i<11; i++)
        {
            blockColor[i] = blockObj[j, i].GetComponent<SpriteRenderer>().color;
            blockObj[j, i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        yield return new WaitForSeconds(0.5f);

        for(int i = 1; i < 11; i++)
        {
            blockObj[j, i].GetComponent<SpriteRenderer>().color = blockColor[i];
        }

        yield return new WaitForSeconds(0.5f);

        callback();
    }

    // スコア加算
    private int AddScore(int rowNum)
    {
        int addScore;

        switch (rowNum)
        {
            case 1:
                addScore = 100;
                break;
            case 2:
                addScore = 300;
                break;
            case 3:
                addScore = 500;
                break;
            case 4:
                addScore = 1000;
                break;
            default:
                addScore = 0;
                break;
        }

        return addScore;
    }

    //ゲームオーバー判定
    private bool JudgeGameOver()
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

    // 一時停止処理
    public void Pause()
    {
        if (pauseFlg)
        {
            pauseFlg = false;
            pausePanel.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(false);
            titleButton.gameObject.SetActive(false);
        }
        else
        {
            pauseFlg = true;
            pausePanel.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(true);
            titleButton.gameObject.SetActive(true);
        }
    }
}
