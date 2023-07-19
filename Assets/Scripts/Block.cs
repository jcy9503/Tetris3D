using System;
using System.Collections.Generic;
using System.Linq;

public class Block : IBlock
{
	private readonly int     id;
	private readonly int     size;
	private          Coord   curPos;
	private readonly Coord[] tile;

	protected Block(int id, int size, Coord[] tile)
	{
		this.id   = id;
		this.size = size;
		this.tile = tile;
	}

	public int GetId() => id;

	public void Reset()
	{
		Random randValue  = new();
		Coord  randRotate = new(randValue.Next(0, 4), randValue.Next(0, 4), randValue.Next(0, 4));

		curPos = new Coord(randValue.Next(0, GameManager.Grid.SizeX - size), 0,
		                   randValue.Next(0, GameManager.Grid.SizeZ - size));

		for (int i = 0; i < randRotate.X; ++i)
			RotateXClockWise();
		for (int i = 0; i < randRotate.Y; ++i)
			RotateYClockWise();
		for (int i = 0; i < randRotate.Z; ++i)
			RotateZClockWise();
	}

	public Block CopyBlock()
	{
		Coord[] tpTile = new Coord[tile.Length];

		for (int i = 0; i < tile.Length; ++i)
		{
			tpTile[i] = tile[i];
		}

		Block tp = new Block(id, size, tpTile)
		{
			curPos = curPos
		};

		return tp;
	}

	public void Move(Coord move)
	{
		curPos += move;
	}

	public IEnumerable<Coord> TilePositions()
	{
		return tile.Select(pos => new Coord(pos + curPos));
	}

	public void RotateXClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.Y = size - 1 - tp.Z;
			coord.Z = tp.Y;
		}
	}

	public void RotateXCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.Y = tp.Z;
			coord.Z = size - 1 - tp.Y;
		}
	}

	public void RotateYClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = size - 1 - tp.Z;
			coord.Z = tp.X;
		}
	}

	public void RotateYCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = tp.Z;
			coord.Z = size - 1 - tp.X;
		}
	}

	public void RotateZClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = size - 1 - tp.Y;
			coord.Y = tp.X;
		}
	}

	public void RotateZCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = tp.Y;
			coord.Y = size - 1 - tp.X;
		}
	}
	
	public static readonly string[] MatPath =
	{
		"Materials/BlockShadow",
		"Materials/BlockI",
		"Materials/BlockL",
		"Materials/BlockT",
		"Materials/BlockO",
		"Materials/BlockJ",
		"Materials/BlockZ",
		"Materials/BlockS"
	};
}