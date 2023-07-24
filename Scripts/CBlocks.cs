public class CBlockI : CBlock
{
    public CBlockI() : base(1, 4, new[]
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
    })
    {
    }
}

public class CBlockL : CBlock
{
    public CBlockL() : base(2, 3, new[]
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
    })
    {
    }
}

public class CBlockT : CBlock
{
    public CBlockT() : base(3, 3, new[]
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
    })
    {
    }
}

public class CBlockO : CBlock
{
    public CBlockO() : base(4, 2, new[]
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
    })
    {
    }
}

public class CBlockJ : CBlock
{
    public CBlockJ() : base(5, 3, new[]
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
    })
    {
    }
}

public class CBlockZ : CBlock
{
    public CBlockZ() : base(6, 3, new[]
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
    })
    {
    }
}

public class CBlockS : CBlock
{
    public CBlockS() : base(7, 3, new[]
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
    })
    {
    }
}