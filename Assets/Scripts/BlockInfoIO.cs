using System.IO;
using UnityEngine;

public class BlockInfoIO
{
    public static string dataPath = Application.dataPath + "/Json/BlockList.json";

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
