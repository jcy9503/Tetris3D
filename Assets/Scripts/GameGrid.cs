public class GameGrid
{
    private readonly int[,,] grid;

    private int SizeX { get; }
    private int SizeY { get; }
    private int SizeZ { get; }

    public int this[int X, int Y, int Z]
    {
        get => grid[X, Y, Z];
        set => grid[X, Y, Z] = value;
    }

    public GameGrid(int X, int Y, int Z)
    {
        SizeX = X;
        SizeY = Y;
        SizeZ = Z;
        grid = new int[X, Y, Z];
    }

    public bool IsInside(int X, int Y, int Z)
    {
        return X >= 0 && X < SizeX && Y >= 0 && Y < SizeY && Z >= 0 && Z < SizeZ;
    }

    public bool IsEmpty(int X, int Y, int Z)
    {
        return IsInside(X, Y, Z) && grid[X, Y, Z] == 0;
    }

    public bool IsPlaneFull(int Y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                if (grid[x, Y, z] == 0)
                    return false;
        }

        return true;
    }

    public bool IsPlaneEmpty(int Y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                if (grid[x, Y, z] != 0)
                    return false;
        }

        return true;
    }

    private void ClearPlane(int Y)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
                grid[x, Y, z] = 0;
        }
    }

    private void MovePlaneDown(int Y, int NumPlanes)
    {
        for (int x = 0; x < SizeX; ++x)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                grid[x, Y + NumPlanes, z] = grid[x, Y, z];
                grid[x, Y, z] = 0;
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