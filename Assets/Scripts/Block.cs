public class Position
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Position(int X, int Y, int Z)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }
}

public abstract class Block
{
    protected abstract Position[][] Tiles { get; }
}
