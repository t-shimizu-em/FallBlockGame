using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlockInfoIO : MonoBehaviour
{
    public static string dataPath;

    void Start()
    {

    }

    public static BlockInfoList LoadBlockList()
    {
        StreamReader reader = new StreamReader(dataPath);
        string json = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<BlockInfoList>(json);
    }

    public static void SaveBlockInfo(BlockInfoList blockList)
    {
        string json = JsonUtility.ToJson(blockList, true);
        StreamWriter writer = new StreamWriter(dataPath, false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }
}
