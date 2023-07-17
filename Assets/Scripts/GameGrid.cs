/*
 * GameGrid.cs
 * -----------
 * Made by Lucas Jeong
 * Contains class related to game grid data.
 */

using UnityEngine;

public class GameGrid
{
	private int[,,]  grid;
	public  CubeMesh GridMesh { get; private set; }
	public  float    GridUnit { get; private set; }
	public  int      SizeX    { get; private set; }
	public  int      SizeY    { get; private set; }
	public  int      SizeZ    { get; private set; }
	public int this[int x, int y, int z]
	{
		get => grid[x, y, z];
		set => grid[x, y, z] = value;
	}

	public GameGrid(ref int[] size, float blockSize)
	{
		if (size.Length != 3)
		{
			Debug.LogError("Grid initialization failed.");
		}

		SetGrid(size[0], size[1], size[2], blockSize);
		Init();
	}

	private void Init()
	{
		grid = new int[SizeX, SizeY, SizeZ];

		GridMesh = new CubeMesh("Grid", SizeX, SizeY, SizeZ, GridUnit, true, "Materials/Grid", false, null);
	}

	private void SetGrid(int x, int y, int z, float unit)
	{
		SizeX    = x;
		SizeY    = y;
		SizeZ    = z;
		GridUnit = unit;
	}

	public bool IsInside(int x, int y, int z)
	{
		return x >= 0 && x < SizeX && y >= 0 && y < SizeY && z >= 0 && z < SizeZ;
	}

	public bool IsEmpty(int x, int y, int z)
	{
		return IsInside(x, y, z) && grid[x, y, z] == 0;
	}

	public bool IsPlaneFull(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				if (grid[x, y, z] == 0)
					return false;
		}

		return true;
	}

	public bool IsPlaneEmpty(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				if (grid[x, y, z] != 0)
					return false;
		}

		return true;
	}

	private void ClearPlane(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				grid[x, y, z] = 0;
		}
	}

	private void MovePlaneDown(int y, int numPlanes)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
			{
				grid[x, y + numPlanes, z] = grid[x, y, z];
				grid[x, y, z]             = 0;
			}
		}
	}

	public int ClearFullRows()
	{
		int cleared = 0;

		for (int y = SizeY - 1; y >= 0; --y)
		{
			if (IsPlaneFull(y))
			{
				ClearPlane(y);
				++cleared;
			}
			else if (cleared > 0)
			{
				MovePlaneDown(y, cleared);
			}
		}

		return cleared;
	}
}