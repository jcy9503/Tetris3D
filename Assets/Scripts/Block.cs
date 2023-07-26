/*
 * Block.cs
 * --------
 * Made by Lucas Jeong
 * Contains basic Block class.
 */

using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public class Block : IBlock
{
	private readonly int     id;
	public const     int     Type = 7;
	public readonly  int     Size;
	public           Coord   Pos;
	private readonly Coord[] tile;

	protected Block(int id, int size, Coord[] tile)
	{
		this.id   = id;
		this.Size = size;
		this.tile = tile;
	}

	public int GetId() => id;

	public void Reset()
	{
		Random randValue  = new();
		Coord  randRotate = new(randValue.Next(0, 4), randValue.Next(0, 4), randValue.Next(0, 4));

		Pos = new Coord(randValue.Next(0, GameManager.Grid.SizeX - Size), 0,
		                randValue.Next(0, GameManager.Grid.SizeZ - Size));

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

		Block tp = new(id, Size, tpTile)
		{
			Pos = Pos
		};

		return tp;
	}

	public void Move(Coord move)
	{
		Pos += move;
	}

	public IEnumerable<Coord> TilePositions()
	{
		return tile.Select(pos => new Coord(pos + Pos));
	}

	public void RotateXClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.Y = Size - 1 - tp.Z;
			coord.Z = tp.Y;
		}
	}

	public void RotateXCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.Y = tp.Z;
			coord.Z = Size - 1 - tp.Y;
		}
	}

	public void RotateYClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = Size - 1 - tp.Z;
			coord.Z = tp.X;
		}
	}

	public void RotateYCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = tp.Z;
			coord.Z = Size - 1 - tp.X;
		}
	}

	public void RotateZClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = Size - 1 - tp.Y;
			coord.Y = tp.X;
		}
	}

	public void RotateZCounterClockWise()
	{
		foreach (Coord coord in tile)
		{
			Coord tp = new(coord);
			coord.X = tp.Y;
			coord.Y = Size - 1 - tp.X;
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
		"Materials/BlockS",
		"Materials/BlockGrid"
	};
}