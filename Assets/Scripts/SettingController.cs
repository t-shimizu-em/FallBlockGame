using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingController : MonoBehaviour
{
    public Toggle[] blockA;
    public Toggle[] blockB;
    public Toggle[] blockC;
    public Toggle[] blockD;
    public GameObject[] blockInfoA;
    public GameObject[] blockInfoB;
    public GameObject[] blockInfoC;
    public GameObject[] blockInfoD;
    public TMP_InputField colorInputFieldRed;
    public TMP_InputField colorInputFieldGreen;
    public TMP_InputField colorInputFieldBlue;
    public GameObject panel;
    public TextMeshProUGUI resultText;
    private int red;
    private int green;
    private int blue;
    private Color blockColor = new Color32(0, 0, 0, 0);
    private bool submitFlg;

    void Start()
    {
        BlockInfoIO.dataPath = Application.dataPath + "/Json/BlockList.json";
        red = ConvertInputValue(colorInputFieldRed.GetComponent<TMP_InputField>().text);
        green = ConvertInputValue(colorInputFieldGreen.GetComponent<TMP_InputField>().text);
        blue = ConvertInputValue(colorInputFieldBlue.GetComponent<TMP_InputField>().text);
    }

    void Update()
    {

    }

    public void ChangeColorRed()
    {
        red = ConvertInputValue(colorInputFieldRed.GetComponent<TMP_InputField>().text);
        blockColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
        //Debug.Log(blockColor);
        SetTileColor(blockInfoA);
        SetTileColor(blockInfoB);
        SetTileColor(blockInfoC);
        SetTileColor(blockInfoD);
        colorInputFieldRed.GetComponent<TMP_InputField>().text = red.ToString("000");
    }

    public void ChangeColorGreen()
    {
        green = ConvertInputValue(colorInputFieldGreen.GetComponent<TMP_InputField>().text);
        blockColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
        SetTileColor(blockInfoA);
        SetTileColor(blockInfoB);
        SetTileColor(blockInfoC);
        SetTileColor(blockInfoD);
        colorInputFieldGreen.GetComponent<TMP_InputField>().text = green.ToString("000");
    }

    public void ChangeColorBlue()
    {
        blue = ConvertInputValue(colorInputFieldBlue.GetComponent<TMP_InputField>().text);
        blockColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
        SetTileColor(blockInfoA);
        SetTileColor(blockInfoB);
        SetTileColor(blockInfoC);
        SetTileColor(blockInfoD);
        colorInputFieldRed.GetComponent<TMP_InputField>().text = red.ToString("000");
    }

    // 登録処理
    public void SubmitBlock()
    {
        submitFlg = true;

        // ブロック情報格納
        BlockInfo blockInfo = new BlockInfo();
        blockInfo.blockStatA = SetBlockStat(blockA, true);
        blockInfo.blockStatB = SetBlockStat(blockB);
        blockInfo.blockStatC = SetBlockStat(blockC);
        blockInfo.blockStatD = SetBlockStat(blockD);
        blockInfo.red = red;
        blockInfo.green = green;
        blockInfo.blue = blue;

        // 登録用リストに追加
        BlockInfoList blockInfoList = BlockInfoIO.LoadBlockList();
        blockInfoList.blockList.Add(blockInfo);
        // ブロック登録
        if (submitFlg)
        {
            BlockInfoIO.SaveBlockInfo(blockInfoList);
        }
        StartCoroutine(DisplayResult());
    }

    private int[] SetBlockStat(Toggle[] block, bool isBlockA = false)
    {
        int[] blockStat = new int[16];
        bool successFlg = false;

        for (int i = 0; i < block.Length; i++)
        {
            if (block[i].isOn)
            {
                blockStat[i] = 3;
                successFlg = true;
            }
            else
            {
                blockStat[i] = 0;
            }

        }

        if (!successFlg)
        {
            blockStat = null;
            if (isBlockA)
            {
                submitFlg = false;
            }
        }

        return blockStat;
    }

    private void SetTileColor(GameObject[] blockInfo)
    {
        for (int i=0; i<blockInfo.Length; i++)
        {
            GameObject tile = blockInfo[i].transform.Find("Color").gameObject;
            tile.GetComponent<Image>().color = blockColor;
        }
    }

    private int ConvertInputValue(string str)
    {
        int num = int.Parse(str);
        if (num < 0)
        {
            num = 0;
        }
        else if (num > 255)
        {
            num = 255;
        }

        return num;
    }

    private IEnumerator DisplayResult()
    {
        panel.gameObject.SetActive(true);

        if (submitFlg)
        {
            resultText.text = "Successed";
            resultText.color = new Color(0, 1, 0, 1);
        }
        else
        {
            resultText.text = "Failed";
            resultText.color = new Color(1, 0, 0, 1);
        }

        yield return new WaitForSeconds(2);

        panel.gameObject.SetActive(false);
    }

    //public int[,] SetBlockStatX(Toggle[] block)
    //{
    //    int i = 0;
    //    int j = 0;
    //    int[,] blockStat = new int[4, 4];
    //    bool successFlg = false;

    //    for (int k = 0; k < block.Length; k++)
    //    {
    //        if (block[k].isOn)
    //        {
    //            blockStat[i, j] = 3;
    //            successFlg = true;
    //        }
    //        else
    //        {
    //            blockStat[i, j] = 0;
    //        }

    //        j++;

    //        if (j == 4)
    //        {
    //            i++;
    //            j = 0;
    //        }

    //    }

    //    if (!successFlg)
    //    {
    //        blockStat = null;
    //    }

    //    return blockStat;
    //}
}
