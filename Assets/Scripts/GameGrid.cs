/*
 * GameGrid.cs
 * -----------
 * Made by Lucas Jeong
 * Contains class related to game grid data.
 */

using UnityEngine;

public class GameGrid
{
	private CubeMesh mesh;
	private int[,,]  grid;
	private float    unit;
	public  int      SizeX { get; private set; }
	public  int      SizeY { get; private set; }
	public  int      SizeZ { get; private set; }
	public int this[int x, int y, int z]
	{
		get => grid[x + 1, y + 1, z + 1];
		set => grid[x + 1, y + 1, z + 1] = value;
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
		grid = new int[SizeX + 2, SizeY + 2, SizeZ + 2];

		for (int i = 0; i < SizeX + 2; ++i)
		{
			for (int j = 0; j < SizeY + 2; ++j)
			{
				for (int k = 0; k < SizeZ + 2; ++k)
				{
					if (i == 0 || i == SizeX + 1 || j == SizeY + 1 || k == 0 || k == SizeZ + 1)
						grid[i, j, k] = -1;
					else
						grid[i, j, k] = 0;
				}
			}
		}

		mesh = new CubeMesh("GridMesh", SizeX, SizeY, SizeZ, unit, true, 
		                    "Materials/Grid", false, null);
		mesh.Obj.transform.parent = GameManager.GridObj.transform;
	}

	private void SetGrid(int x, int y, int z, float gridUnit)
	{
		SizeX = x;
		SizeY = y;
		SizeZ = z;
		unit  = gridUnit;
	}

	private bool IsInside(int x, int y, int z)
	{
		return x >= 0 && x < SizeX && y >= 0 && y < SizeY && z >= 0 && z < SizeZ;
	}

	public bool IsEmpty(int x, int y, int z)
	{
		return IsInside(x, y, z) && grid[x + 1, y + 1, z + 1] == 0;
	}

	private bool IsPlaneFull(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				if (grid[x + 1, y + 1, z + 1] == 0)
					return false;
		}

		return true;
	}

	public bool IsPlaneEmpty(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				if (grid[x + 1, y + 1, z + 1] != 0)
					return false;
		}

		return true;
	}

	private void ClearPlane(int y)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
				grid[x + 1, y + 1, z + 1] = 0;
		}
	}

	private void MovePlaneDown(int y, int numPlanes)
	{
		for (int x = 0; x < SizeX; ++x)
		{
			for (int z = 0; z < SizeZ; ++z)
			{
				grid[x + 1, y + 1    + numPlanes, z + 1] = grid[x + 1, y + 1, z + 1];
				grid[x + 1, y + 1, z + 1]                = 0;
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