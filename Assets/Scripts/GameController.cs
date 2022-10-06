using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private GameObject[,] nextFallBlockObj = new GameObject[4, 4];
    FallBlockController fallBlockController = new FallBlockController();
    AudioSource audioSource;
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
    private int[,] fallBlockStat = new int[4, 4];
    private int[,] nextFallBlockStat = new int[4, 4];
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
    private bool downBanFlg; // false:下移動可 true:下移動禁止
    private bool rotBanFlg; // false:回転可 true:回転禁止
    private bool pauseFlg; // true:一時停止

    
    void Start()
    {
        gameStat = START;
        pauseFlg = false;

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
        for (int i=0; i<4; i++)
        {
            for (int j=0; j<4; j++)
            {
                fallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(fallBlockPosX + i + OX, -fallBlockPosY - j + OY, 0), Quaternion.identity);
                fallBlockObj[j, i].gameObject.SetActive(false);
            }
        }

        // 次落下ブロック初期設定
        nextBlockNum = Random.Range(0, 10);
        nextFallBlockStat = fallBlockController.SetFallBlock(nextBlockNum, rot);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                nextFallBlockObj[j, i] = Instantiate(fallBlockPfb, new Vector3(nextFallBlockPosX + i, nextFallBlockPosY - j, 0), Quaternion.identity);
            }
        }

        // 配置ブロック初期設定
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 18; j++)
            {
                blockObj[j, i] = Instantiate(placementBlockPfb, new Vector3(i + OX, -j + OY, 0), Quaternion.identity);
            }
        }

        // 一時停止ボタン
        pauseButton.onClick.AddListener(Pause);
    }

    void Update()
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
                if (JudgeGround(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
                {
                    gameStat = GROUND;
                }
                break;
            case GROUND:
                fallCountTime = 0;
                groundCountTime += Time.deltaTime;
                downBanFlg = true;
                rotBanFlg = true;

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
                    }
                }
                break;
            case ERASE:
                eraseAudioSource.PlayOneShot(eraseSe);
                score += AddScore(BlockErase());
                scoreText.text = "Score:" + score;
                NextBlockSet();
                gameStat = START;
                break;
            case GAMEOVER:
                gameOverText.gameObject.SetActive(true);
                break;
        }

        // 落下ブロックの水平移動操作
        if (Input.GetButtonDown("Horizontal"))
        {
            moveAudioSource.PlayOneShot(moveSe);
            if (Input.GetAxis("Horizontal") > 0 && !JudgeContactRight(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
            {
                fallBlockPosX++;
            }
            else if (Input.GetAxis("Horizontal") < 0 && !JudgeContactLeft(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY))
            {
                fallBlockPosX--;
            }
        }

        // 壁際での回転禁止処理
        if (JudgeContactRight(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY) || JudgeContactLeft(blockNum, rot, blockStat, fallBlockPosX, fallBlockPosY) || gameStat == GAMEOVER)
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
            fallBlockStat = fallBlockController.SetFallBlock(blockNum, rot);
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
                fallBlockObj[j, i].gameObject.SetActive(false);
                if (fallBlockStat[j, i] == 2)
                {
                    fallBlockObj[j, i].gameObject.SetActive(true);
                    fallBlockObj[j, i].gameObject.transform.position = new Vector3(fallBlockPosX + i + OX, -fallBlockPosY - j + OY, 0);
                }
            }
        }

        // 次落下ブロック生成
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                nextFallBlockObj[j, i].gameObject.SetActive(false);
                if (nextFallBlockStat[j, i] == 2)
                {
                    nextFallBlockObj[j, i].gameObject.SetActive(true);
                }
            }
        }

        // 配置ブロック生成
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 18; j++)
            {
                if (blockStat[j, i] == 3)
                {
                    blockObj[j, i].gameObject.SetActive(true);
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
        nextBlockNum = Random.Range(0, 10);
        rot = 0;
        fallBlockStat = fallBlockController.SetFallBlock(blockNum, rot);
        nextFallBlockStat = fallBlockController.SetFallBlock(nextBlockNum, rot);
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
        }
        return eraseFlg;
    }

    // ブロック消去
    private int BlockErase()
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
                        blockObj[k, i].gameObject.SetActive(false);
                    }
                }
            }
        }

        return eraseCount;
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
        if (blockStat[fallBlockInitPosY, fallBlockInitPosX] == 3)
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
