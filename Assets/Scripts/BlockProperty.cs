using System;
using UnityEngine;

public class BlockProperty
{
	private int blockStatus;
	private Color blockColor;

    public int BlockStatus { get => blockStatus; set => blockStatus = value; }
	public Color BlockColor { get => blockColor; set => blockColor = value; }

	public void Copy(BlockProperty blockProperty)
    {
		blockStatus = blockProperty.blockStatus;
		blockColor = blockProperty.blockColor;
    }

}

