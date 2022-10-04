using System;
public class FallBlockController
{
    private int[,] block00 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {2, 2, 2, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block01 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 2, 0, 0},
        {2, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block02 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {0, 2, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block03 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {2, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block10 = new int[4, 4]
    {
        {2, 2, 0, 0},
        {2, 0, 0, 0},
        {2, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block11 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {0, 0, 2, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block12 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {0, 2, 0, 0},
        {2, 2, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block13 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 2, 2, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block20 = new int[4, 4]
    {
        {2, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block21 = new int[4, 4]
    {
        {0, 0, 2, 0},
        {2, 2, 2, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block22 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 0, 0, 0},
        {2, 2, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block23 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {2, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block30 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block31 = new int[4, 4]
    {
        {0, 2, 2, 0},
        {2, 2, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block40 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {2, 2, 0, 0},
        {2, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block41 = new int[4, 4]
    {
        {2, 2, 0, 0},
        {0, 2, 2, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block50 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 0, 0, 0},
        {2, 0, 0, 0},
        {2, 0, 0, 0}
    };

    private int[,] block51 = new int[4, 4]
    {
        {2, 2, 2, 2},
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block60 = new int[4, 4]
    {
        {2, 2, 0, 0},
        {2, 2, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block70 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {0, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 2, 0, 0}
    };

    private int[,] block71 = new int[4, 4]
    {
        {0, 0, 0, 2},
        {2, 2, 2, 2},
        {0, 0, 0, 2},
        {0, 0, 0, 0}
    };

    private int[,] block72 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {0, 2, 0, 0},
        {0, 2, 0, 0},
        {2, 2, 2, 0}
    };

    private int[,] block73 = new int[4, 4]
    {
        {2, 0, 0, 0},
        {2, 2, 2, 2},
        {2, 0, 0, 0},
        {0, 0, 0, 0}
    };

    private int[,] block80 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {2, 0, 2, 0},
        {2, 2, 2, 0},
        {0, 0, 0, 0}
    };

    private int[,] block90 = new int[4, 4]
    {
        {0, 2, 0, 0},
        {2, 2, 2, 0},
        {2, 0, 2, 2},
        {2, 0, 0, 0}
    };

    private int[,] block91 = new int[4, 4]
    {
        {2, 2, 2, 0},
        {0, 0, 2, 2},
        {0, 2, 2, 0},
        {0, 2, 0, 0}
    };

    private int[,] block92 = new int[4, 4]
    {
        {0, 0, 0, 2},
        {2, 2, 0, 2},
        {0, 2, 2, 2},
        {0, 0, 2, 0}
    };

    private int[,] block93 = new int[4, 4]
    {
        {0, 0, 2, 0},
        {0, 2, 2, 0},
        {2, 2, 0, 0},
        {0, 2, 2, 2}
    };

    /**
     * 落下ブロック一覧
     */
    public int[,] SetFallBlock(int blockNum, int rot)
    {
        int[,] fallBlock = new int[4, 4];
        switch (blockNum)
        {
            case 0:
                switch (rot)
                {
                    case 0:
                        fallBlock = block00;
                        break;
                    case 1:
                        fallBlock = block01;
                        break;
                    case 2:
                        fallBlock = block02;
                        break;
                    case 3:
                        fallBlock = block03;
                        break;
                }
                break;
            case 1:
                switch (rot)
                {
                    case 0:
                        fallBlock = block10;
                        break;
                    case 1:
                        fallBlock = block11;
                        break;
                    case 2:
                        fallBlock = block12;
                        break;
                    case 3:
                        fallBlock = block13;
                        break;
                }
                break;
            case 2:
                switch (rot)
                {
                    case 0:
                        fallBlock = block20;
                        break;
                    case 1:
                        fallBlock = block21;
                        break;
                    case 2:
                        fallBlock = block22;
                        break;
                    case 3:
                        fallBlock = block23;
                        break;
                }
                break;
            case 3:
                switch (rot)
                {
                    case 0:
                        fallBlock = block30;
                        break;
                    case 1:
                        fallBlock = block31;
                        break;
                }
                break;
            case 4:
                switch (rot)
                {
                    case 0:
                        fallBlock = block40;
                        break;
                    case 1:
                        fallBlock = block41;
                        break;
                }
                break;
            case 5:
                if (rot == 0) fallBlock = block50;
                else if (rot == 1) fallBlock = block51;
                break;
            case 6:
                fallBlock = block60;
                break;
            case 7:
                switch (rot)
                {
                    case 0:
                        fallBlock = block70;
                        break;
                    case 1:
                        fallBlock = block71;
                        break;
                    case 2:
                        fallBlock = block72;
                        break;
                    case 3:
                        fallBlock = block73;
                        break;
                }
                break;
            case 8:
                fallBlock = block80;
                break;
            case 9:
                switch (rot)
                {
                    case 0:
                        fallBlock = block90;
                        break;
                    case 1:
                        fallBlock = block91;
                        break;
                    case 2:
                        fallBlock = block92;
                        break;
                    case 3:
                        fallBlock = block93;
                        break;
                }
                break;
        }
        return fallBlock;
    }
}

