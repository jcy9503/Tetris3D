public class GameGrid
{
    private readonly int[,,] Grid;

    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public int SizeZ { get; private set; }

    public int this[int x, int y, int z]
    {
        get => Grid[x, y, z];
        set => Grid[x, y, z] = value;
    }

    public GameGrid(int x, int y, int z)
    {
        SetGrid(x, y, z);
        Grid = new int[x, y, z];
    }

    public void SetGrid(int x, int y, int z)
    {
        SizeX = x;
        SizeY = y;
        SizeZ = z;
    }

    public bool IsInside(int x, int y, int z)
    {
        return x >= 0 && x < SizeX && y >= 0 && y < SizeY && z >= 0 && z < SizeZ;
    }

    public bool IsEmpty(int x, int y, int z)
    {
        return IsInside(x, y, z) && Grid[x, y, z] == 0;
    }

    public bool IsPlaneFull(int y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                if (Grid[x, y, z] == 0)
                    return false;
        }

        return true;
    }

    public bool IsPlaneEmpty(int y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                if (Grid[x, y, z] != 0)
                    return false;
        }

        return true;
    }

    private void ClearPlane(int y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                Grid[x, y, z] = 0;
        }
    }

    private void MovePlaneDown(int y, int numPlanes)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                Grid[x, y + numPlanes, z] = Grid[x, y, z];
                Grid[x, y, z] = 0;
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