using System;
using System.Collections.Generic;

public class Block : IBlock
{
	private readonly uint       id;
	private readonly uint       size;
	private          Position   curPos;
	private          bool[][][] tile;
	private          string     matPath;

	protected Block(uint id, uint size, bool[][][] tile, string matPath)
	{
		this.id      = id;
		this.size    = size;
		this.tile    = tile;
		this.matPath = matPath;
	}

	public uint GetId() => id;

	public void Reset()
	{
		Random randValue = new();
		curPos = new Position(randValue.Next(0, GameManager.Grid.SizeX), 0, randValue.Next(0, GameManager.Grid.SizeZ));
	}

	public void Move(Position move)
	{
		curPos += move;
	}

	public IEnumerable<bool> TilePositions()
	{
		foreach (bool[][] info in tile)
		{
			
		}
	}

	public void RotateXClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[i][size - 1 - k][j];
				}
			}
		}

		tile = tp;
	}

	public void RotateXCounterClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[i][k][size - 1 - j];
				}
			}
		}

		tile = tp;
	}

	public void RotateYClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[size - 1 - k][j][i];
				}
			}
		}

		tile = tp;
	}

	public void RotateYCounterClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[k][j][size - 1 - i];
				}
			}
		}

		tile = tp;
	}

	public void RotateZClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[size - 1 - j][i][k];
				}
			}
		}

		tile = tp;
	}

	public void RotateZCounterClockWise()
	{
		bool[][][] tp = tile;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				for (int k = 0; k < size; ++k)
				{
					tp[i][j][k] = tile[j][size - 1 - i][k];
				}
			}
		}

		tile = tp;
	}
}