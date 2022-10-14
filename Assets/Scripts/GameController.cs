using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Common;

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
    [SerializeField]
    public ScoreData scoreData;
    public float OX, OY; // 原点座標

    // ブロックの状態
    // 0:空
    // 1:壁ブロック
    // 2:地面ブロック
    // 3:落下ブロック
    // 4:配置ブロック

    private BlockInfoList blockInfoList;
    private BlockController blockController = new BlockController();
    private BlockProperty[,] blockPropList = new BlockProperty[GrovalConst.HIGHT, GrovalConst.WIDTH];
    private BlockProperty[,] fallBlockPropList = new BlockProperty[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private BlockProperty[,] nextFallBlockPropList = new BlockProperty[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private GameObject[,] blockObj = new GameObject[GrovalConst.HIGHT, GrovalConst.WIDTH];
    private GameObject[,] fallBlockObj = new GameObject[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private GameObject[,] nextFallBlockObj = new GameObject[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private int score;
    private int gameStat;
    private float groundCountTime;
    private int[,] fallBlockStat = new int[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private int[,] nextFallBlockStat = new int[GrovalConst.FALL_BLOCK_HIGHT, GrovalConst.FALL_BLOCK_WIDTH];
    private int[,] wallBlockPos = new int[GrovalConst.HIGHT, GrovalConst.WIDTH]
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
    private const int GAMEOVER = 0;
    private const int START = 1;
    private const int GROUND = 2;
    private const int ERASE = 3;
    private bool pauseFlg; // true:一時停止
    private bool doEraseFlg; // true:ブロック消去中

    void Start()
    {
        blockInfoList = BlockInfoIO.LoadBlockList();
        gameStat = START;
        blockController.fallCountTime = 0;
        groundCountTime = 0;
        pauseFlg = false;
        doEraseFlg = false;
        blockController.rot = 0;
        blockController.downBanFlg = false;
        blockController.blockNum = Random.Range(0, blockInfoList.blockList.Count);
        blockController.nextBlockNum = Random.Range(0, blockInfoList.blockList.Count);

        // 壁・地面ブロック初期設定
        for (int i = 0; i < GrovalConst.WIDTH; i++)
        {
            for (int j = 0; j < GrovalConst.HIGHT; j++)
            {
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = wallBlockPos[j, i];
                if (blockProp.BlockStatus == 1 || blockProp.BlockStatus == 2) {
                    Instantiate(wallBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
                }
                blockPropList[j, i] = blockProp;
            }
        }

        // 落下ブロック初期設定
        blockController.fallBlockPosX = GrovalConst.FALL_BLOCK_INIT_POS_X;
        blockController.fallBlockPosY = GrovalConst.FALL_BLOCK_INIT_POS_Y;
        fallBlockStat = blockController.SetFallBlock(blockInfoList);
        for (int i=0; i<GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j=0; j<GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = fallBlockStat[j, i];
                fallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(blockController.fallBlockPosX + i + OX, -blockController.fallBlockPosY - j + OY, 0), Quaternion.identity);
                fallBlockObj[j, i].gameObject.SetActive(false);
                if (fallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList);
                }
                fallBlockPropList[j, i] = blockProp;
            }
        }

        // 次落下ブロック初期設定
        nextFallBlockStat = blockController.SetFallBlock(blockInfoList, true);
        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                BlockProperty blockProp = new BlockProperty();
                blockProp.BlockStatus = nextFallBlockStat[j, i];
                nextFallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(GrovalConst.NEXT_FALL_BLOCK_POS_X + i, GrovalConst.NEXT_FALL_BLOCK_POS_Y - j, 0), Quaternion.identity);
                if (nextFallBlockStat[j, i] == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, true);
                }
                nextFallBlockPropList[j, i] = blockProp;
            }
        }

        // 配置ブロック初期設定
        for (int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
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
                    blockController.downBanFlg = false;
                    blockController.rotBanFlg = false;

                    // 1秒ごとにブロックを落下
                    blockController.fallCountTime += Time.deltaTime;
                    if (blockController.fallCountTime >= 1)
                    {
                        blockController.fallBlockPosY++;
                        blockController.fallCountTime = 0;
                    }

                    // 着地判定
                    if (blockController.JudgeGround(blockPropList, blockInfoList))
                    {
                        gameStat = GROUND;
                    }

                    UpdateFallBlockProperty();
                    UpdateDisplay();

                    break;
                case GROUND:
                    blockController.fallCountTime = 0;
                    groundCountTime += Time.deltaTime;
                    blockController.downBanFlg = true;
                    blockController.rotBanFlg = true;

                    // 落下ブロックの非着地判定
                    if (!blockController.JudgeGround(blockPropList, blockInfoList))
                    {
                        groundCountTime = 0;
                        gameStat = START;
                    }

                    if (groundCountTime >= 1)
                    {
                        // 着地した落下ブロックを配置ブロックに置き換え
                        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
                        {
                            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
                            {
                                if (fallBlockPropList[j, i].BlockStatus == 3)
                                {
                                    blockPropList[j + blockController.fallBlockPosY, i + blockController.fallBlockPosX].BlockColor = fallBlockPropList[j, i].BlockColor;
                                    blockPropList[j + blockController.fallBlockPosY, i + blockController.fallBlockPosX].BlockStatus = 4;
                                }
                            }
                        }

                        // 配置ブロック描画
                        for (int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
                        {
                            for (int j = 1; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
                            {
                                if (blockPropList[j, i].BlockStatus == 4)
                                {
                                    blockObj[j, i].GetComponent<SpriteRenderer>().color = blockPropList[j, i].BlockColor;
                                    blockObj[j, i].gameObject.SetActive(true);
                                }
                            }
                        }

                        // 判定
                        if (blockController.JudgeEraseRow(blockPropList))
                        {
                            gameStat = ERASE;
                        }
                        else if (blockController.JudgeGameOver(blockPropList, GrovalConst.FALL_BLOCK_INIT_POS_X, GrovalConst.FALL_BLOCK_INIT_POS_Y))
                        {
                            gameStat = GAMEOVER;
                        }
                        else
                        {
                            Init();
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
            blockController.HorizontalMove(blockPropList, blockInfoList);
            moveAudioSource.PlayOneShot(moveSe);
            UpdateFallBlockProperty();
            UpdateDisplay();
        }

        // 壁際での回転禁止処理
        if (blockController.JudgeContactRight(blockPropList, blockInfoList) || blockController.JudgeContactLeft(blockPropList, blockInfoList) || gameStat == GAMEOVER)
        {
            blockController.rotBanFlg = true;
        }
        else
        {
            blockController.rotBanFlg = false;
        }

        // 落下ブロックの回転操作
        if (Input.GetButtonDown("Jump") && !blockController.rotBanFlg)
        {
            fallBlockStat = blockController.RotationMove(fallBlockStat, blockInfoList);
            rotAudioSource.PlayOneShot(rotSe);
        }

        // 落下ブロック落下速度上昇
        if (Input.GetButtonDown("Vertical") && !blockController.downBanFlg)
        {
            blockController.VerticalMove();
            moveAudioSource.PlayOneShot(moveSe);
        }
    }

    // 落下ブロック情報設定
    private void UpdateFallBlockProperty()
    {
        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                BlockProperty blockProp = fallBlockPropList[j, i];
                blockProp.BlockStatus = fallBlockStat[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList);
                }
                fallBlockPropList[j, i] = blockProp;
            }
        }

        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                BlockProperty blockProp = nextFallBlockPropList[j, i];
                blockProp.BlockStatus = nextFallBlockStat[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    blockProp.BlockColor = blockController.SetBlockColor(blockInfoList, true);
                }
                nextFallBlockPropList[j, i] = blockProp;
            }
        }
    }

    // 描画処理
    private void UpdateDisplay()
    {
        // 落下ブロック生成
        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                fallBlockObj[j, i].gameObject.SetActive(false);
                BlockProperty blockProp = fallBlockPropList[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    fallBlockObj[j, i].gameObject.SetActive(true);
                    fallBlockObj[j, i].GetComponent<SpriteRenderer>().color = fallBlockPropList[j, i].BlockColor;
                    fallBlockObj[j, i].gameObject.transform.position = new Vector3(blockController.fallBlockPosX + i + OX, -blockController.fallBlockPosY - j + OY, 0);
                }
            }
        }

        // 次落下ブロック生成
        for (int i = 0; i < GrovalConst.FALL_BLOCK_HIGHT; i++)
        {
            for (int j = 0; j < GrovalConst.FALL_BLOCK_WIDTH; j++)
            {
                nextFallBlockObj[j, i].gameObject.SetActive(false);
                BlockProperty blockProp = nextFallBlockPropList[j, i];
                if (blockProp.BlockStatus == 3)
                {
                    nextFallBlockObj[j, i].gameObject.SetActive(true);
                    nextFallBlockObj[j, i].GetComponent<SpriteRenderer>().color = nextFallBlockPropList[j, i].BlockColor;
                }
            }
        }

        // 配置ブロック生成
        for (int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
        {
            for (int j = 1; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
            {
                BlockProperty blockProp = blockPropList[j, i];
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

    // ブロック消去（コルーチン1）
    private IEnumerator BlockErase()
    {
        // ex. 
        // var seq = new CoroutineSequence(this);
        // seq.Insert(0, BlockErase());
        // seq.Append(BlockErase());

        int eraseCount = 0;
        int erasedCount = 0;
        doEraseFlg = true;
        for (int j = 0; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (blockController.eraseRow[j])
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

        for (int j = 0; j < GrovalConst.PLACEMENT_BLOCK_HIGHT; j++)
        {
            if (blockController.eraseRow[j])
            {
                // 1行消去
                for (int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
                {
                    blockPropList[j, i].BlockStatus = 0;
                }
                // 消去分、下にずらす
                for (int k = j; k > 0; k--)
                {
                    for (int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
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
        Init();
    }

    // ブロック消去（コルーチン2）
    private IEnumerator BlockEraseCoroutine(int j, System.Action callback)
    {
        Color[] blockColor = new Color[11];

        for (int i=1; i<GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
        {
            blockColor[i] = blockObj[j, i].GetComponent<SpriteRenderer>().color;
            blockObj[j, i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        yield return new WaitForSeconds(0.5f);

        for(int i = 1; i < GrovalConst.PLACEMENT_BLOCK_WIDTH; i++)
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
                addScore = scoreData.singleScore;
                break;
            case 2:
                addScore = scoreData.doubleScore;
                break;
            case 3:
                addScore = scoreData.tripleScore;
                break;
            case 4:
                addScore = scoreData.quadrupleScore;
                break;
            default:
                addScore = 0;
                break;
        }

        return addScore;
    }

    private void Init()
    {
        blockController.NextBlockInfoSet();
        blockController.nextBlockNum = Random.Range(0, blockInfoList.blockList.Count);
        fallBlockStat = blockController.SetFallBlock(blockInfoList);
        nextFallBlockStat = blockController.SetFallBlock(blockInfoList, true);
        groundCountTime = 0;
        gameStat = START;
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
