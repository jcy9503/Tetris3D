using UnityEngine;

public class CBlock : IBlock
{
    private readonly int id;
    private readonly uint size;
    private Coord curPos;
    private bool[][][] tile;

    public CBlock(int id, uint size, bool[][][] tile)
    {
        this.id = id;
        this.size = size;
        this.tile = tile;
    }

    public int GetId() => id;

    public void Reset()
    {
        curPos = new Coord(Random.Range(0, GameManager.Grid.SizeX), 0, GameManager.Grid.SizeZ);
    }

    public void Move(Coord move)
    {
        curPos += move;
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