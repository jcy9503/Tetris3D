/*
 * IBlock.cs
 * ---------
 * Made by Lucas Jeong
 * Contains Coord class used for expressing integer-based 3D coordinate
 * Contains Block interface.
 */

using UnityEngine;

public class Coord
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }

	public Coord(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Coord(Coord param)
	{
		X = param.X;
		Y = param.Y;
		Z = param.Z;
	}

	public override string ToString()
	{
		return "X: " + X + " Y: " + Y + " Z: " + Z;
	}

	public Vector3 ToVector()
	{
		Vector3 vector = new(X, -Y, Z);

		return vector;
	}

	public static Coord operator +(Coord param) => param;
	public static Coord operator -(Coord param) => new(-param.X, -param.Y, -param.Z);

	public static Coord operator +(Coord param1, Coord param2)
		=> new(param1.X + param2.X, param1.Y + param2.Y, param1.Z + param2.Z);

	public static Coord operator -(Coord param1, Coord param2)
		=> param1 + (-param2);

	public static readonly Coord[] Right =
	{
		new(1, 0, 0),
		new(0, 0, -1),
		new(-1, 0, 0),
		new(0, 0, 1)
	};
	public static readonly Coord[] Left     =
	{
		new(-1, 0, 0),
		new(0, 0, 1),
		new(1, 0, 0),
		new(0, 0, -1)
	};
	public static readonly Coord[] Forward  =
	{
		new(0, 0, 1),
		new(1, 0, 0),
		new(0, 0, -1),
		new(-1, 0, 0)
	};
	public static readonly Coord[] Backward =
	{
		new(0, 0, -1),
		new(-1, 0, 0),
		new(0, 0, 1),
		new(1, 0, 0)
	};
	public static readonly Coord   Up       = new(0, -1, 0);
	public static readonly Coord   Down     = new(0, 1, 0);
}

public interface IBlock
{
	int  GetId();
	void Reset();
	void Move(Coord move);
	void RotateXClockWise();
	void RotateXCounterClockWise();
	void RotateYClockWise();
	void RotateYCounterClockWise();
	void RotateZClockWise();
	void RotateZCounterClockWise();
}