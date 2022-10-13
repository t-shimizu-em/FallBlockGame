using UnityEngine;

[System.Serializable]
public class BlockInfo
{
    public int[] blockStatA = new int[16];
    public int[] blockStatB = new int[16];
    public int[] blockStatC = new int[16];
    public int[] blockStatD = new int[16];
    public int red;
    public int green;
    public int blue;
    //private Color blockColor;
    //public int[,] BlockStatA { get => blockStatA; set => blockStatA = value; }
    //public int[,] BlockStatB { get => blockStatB; set => blockStatB = value; }
    //public int[,] BlockStatC { get => blockStatC; set => blockStatC = value; }
    //public int[,] BlockStatD { get => blockStatD; set => blockStatD = value; }
    //public Color BlockColor { get => blockColor; set => blockColor = value; }

    //public int[,] GetBlockStat(int rot)
    //{
    //    int[,] blockStat = null;

    //    switch (rot)
    //    {
    //        case 0:
    //            blockStat = blockStatA;
    //            break;
    //        case 1:
    //            if(blockStatB == null)
    //            {
    //                blockStat = blockStatA;
    //            }
    //            else
    //            {
    //                blockStat = blockStatB;
    //            }
    //            break;
    //        case 2:
    //            if (blockStatC == null)
    //            {
    //                blockStat = blockStatA;
    //            }
    //            else
    //            {
    //                blockStat = blockStatC;
    //            }
    //            break;
    //        case 3:
    //            if(blockStatD == null)
    //            {
    //                blockStat = blockStatA;
    //            }
    //            else
    //            {
    //                blockStat = blockStatD;
    //            }
    //            break;
    //    }

    //    return blockStat;
    //}

    
}
