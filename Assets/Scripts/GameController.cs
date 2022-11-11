using Common;
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
    private FallBlockController fallBlockController = new FallBlockController();
    private GameObject[,] blockObj = new GameObject[GlobalConst.HIGHT, GlobalConst.WIDTH];
    private GameObject[,] fallBlockObj = new GameObject[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    private GameObject[,] nextFallBlockObj = new GameObject[GlobalConst.FALL_BLOCK_HIGHT, GlobalConst.FALL_BLOCK_WIDTH];
    private int score;
    private int gameStat;
    private float groundCountTime;
    private const int GAMEOVER = 0;
    private const int START = 1;
    private const int GROUND = 2;
    private const int ERASE = 3;
    private bool pauseFlg; // true:一時停止

    void Start()
    {
        blockInfoList = BlockInfoIO.LoadBlockList();
        gameStat = START;
        groundCountTime = 0;
        pauseFlg = false;
        blockController.doEraseFlg = false;
        fallBlockController.fallCountTime = 0;
        fallBlockController.rot = 0;
        fallBlockController.InitBlockNum(blockInfoList.blockList.Count);

        // 壁・地面ブロック生成、ステータス保存
        blockController.CreateWallBlock(wallBlockPfb);

        // 落下ブロック生成、ステータス保存
        fallBlockController.CreateFallBlock(fallBlockPfb, blockInfoList);

        // 次落下ブロック生成、ステータス保存
        fallBlockController.CreateNextFallBlock(fallBlockPfb, blockInfoList);

        // 配置ブロック生成
        blockController.CreatePlacementBlock(placementBlockPfb);

        // 一時停止ボタン
        pauseButton.onClick.AddListener(Pause);

        UpdateDisplay();
    }

    async void Update()
    {
        if (!blockController.doEraseFlg)
        {
            if (pauseFlg)
            {
                return;
            }

            switch (gameStat)
            {
                case START:
                    fallBlockController.downBanFlg = false;
                    fallBlockController.rotBanFlg = false;

                    // 一定時間ごとにブロックを落下
                    fallBlockController.Fall();

                    // 着地判定
                    if (fallBlockController.JudgeGround(blockController.blockPropList))
                    {
                        gameStat = GROUND;
                    }

                    UpdateDisplay();

                    break;
                case GROUND:
                    fallBlockController.downBanFlg = true;
                    fallBlockController.rotBanFlg = true;
                    fallBlockController.fallCountTime = 0;
                    groundCountTime += Time.deltaTime;

                    // 落下ブロックの非着地判定
                    if (!fallBlockController.JudgeGround(blockController.blockPropList))
                    {
                        groundCountTime = 0;
                        gameStat = START;
                    }

                    if (groundCountTime >= 1)
                    {
                        // 着地した落下ブロックを配置ブロックに置き換え
                        blockController.blockPropList = fallBlockController.ReplaceBlock(blockController.blockPropList);

                        // 配置ブロック更新
                        blockController.UpdatePlacementBlock();

                        // 判定
                        if (blockController.JudgeEraseRow())
                        {
                            gameStat = ERASE;
                        }
                        else if (blockController.JudgeGameOver())
                        {
                            gameStat = GAMEOVER;
                        }
                        else
                        {
                            Init();
                            gameStat = START;
                        }
                    }
                    break;
                case ERASE:
                    Init();
                    await blockController.BlockErase();
                    score += AddScore(blockController.eraseCount);
                    scoreText.text = "Score:" + score;
                    eraseAudioSource.PlayOneShot(eraseSe);
                    gameStat = START;
                    break;
                case GAMEOVER:
                    gameOverText.gameObject.SetActive(true);
                    retryButton.gameObject.SetActive(true);
                    titleButton.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(false);
                    break;
            }

            // 落下ブロックの水平移動操作
            if (Input.GetButtonDown("Horizontal"))
            {
                fallBlockController.HorizontalMove(blockController.blockPropList);
                moveAudioSource.PlayOneShot(moveSe);
                UpdateDisplay();
            }

            // 壁際での回転禁止処理
            fallBlockController.BanRotationMove(blockController.blockPropList);

            // 落下ブロックの回転操作
            if (Input.GetButtonDown("Jump") && !fallBlockController.rotBanFlg)
            {
                fallBlockController.RotationMove(blockInfoList);
                rotAudioSource.PlayOneShot(rotSe);
            }

            // 落下ブロック落下速度上昇
            if (Input.GetButtonDown("Vertical") && !fallBlockController.downBanFlg)
            {
                fallBlockController.VerticalMove();
                moveAudioSource.PlayOneShot(moveSe);
            }
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

    // 描画処理
    private void UpdateDisplay()
    {
        // 落下ブロック更新
        fallBlockController.UpdateFallBlock();

        // 次落下ブロック更新
        fallBlockController.UpdateNextFallBlock();

        // 配置ブロック更新
        blockController.UpdatePlacementBlock();
    }

    // スコア加算
    private int AddScore(int eraseCount)
    {
        int addScore;

        switch (eraseCount)
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
        fallBlockController.rot = 0;
        fallBlockController.SetBlockNum(blockInfoList.blockList.Count);
        fallBlockController.UpdateFallBlockProperty(blockInfoList);
        fallBlockController.UpdateNextFallBlockPropery(blockInfoList);
        groundCountTime = 0;
    }
}
