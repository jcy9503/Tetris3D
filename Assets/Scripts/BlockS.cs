public class BlockI : Block
{
    public BlockI() : base(1, 4, new[]
    {
        new[]
        {
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false }
        },
        new[]
        {
            new[] { false, true, false, false },
            new[] { false, true, false, false },
            new[] { false, true, false, false },
            new[] { false, true, false, false }
        },
        new[]
        {
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false }
        },
        new[]
        {
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false },
            new[] { false, false, false, false }
        }
    }, "Materials/BlockI")
    {
    }
}

public class BlockL : Block
{
    public BlockL() : base(2, 3, new[]
    {
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        },
        new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, true }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        }
    }, "Materials/BlockL")
    {
    }
}

public class BlockT : Block
{
    public BlockT() : base(3, 3, new[]
    {
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        },
        new[]
        {
            new[] { false, true, false },
            new[] { false, true, true },
            new[] { false, true, false }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        }
    }, "Materials/BlockT")
    {
    }
}

public class BlockO : Block
{
    public BlockO() : base(4, 2, new[]
    {
        new[]
        {
            new[] { true, true },
            new[] { true, true }
        },
        new[]
        {
            new[] { true, true },
            new[] { true, true }
        }
    }, "Materials/BlockO")
    {
    }
}

public class BlockJ : Block
{
    public BlockJ() : base(5, 3, new[]
    {
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        },
        new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { true, true, false }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        }
    }, "Materials/BlockJ")
    {
    }
}

public class BlockZ : Block
{
    public BlockZ() : base(6, 3, new[]
    {
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { true, true, false },
            new[] { false, true, true }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        }
    }, "Materials/BlockZ")
    {
    }
}

public class BlockS : Block
{
    public BlockS() : base(7, 3, new[]
    {
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, true, true },
            new[] { true, true, false }
        },
        new[]
        {
            new[] { false, false, false },
            new[] { false, false, false },
            new[] { false, false, false }
        }
    }, "Materials/BlockS")
    {
    }
}