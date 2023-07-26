public class BlockI : Block
{
	public BlockI() : base(1, 4, new Coord[]
	{
		new(0, 1, 1),
		new(1, 1, 1),
		new(2, 1, 1),
		new(3, 1, 1)
	})
	{
	}
}

public class BlockL : Block
{
	public BlockL() : base(2, 3, new Coord[]
	{
		new(1, 0, 1),
		new(1, 1, 1),
		new(1, 2, 1),
		new(2, 2, 1)
	})
	{
	}
}

public class BlockT : Block
{
	public BlockT() : base(3, 3, new Coord[]
	{
		new(0, 1, 1),
		new(1, 1, 1),
		new(2, 1, 1),
		new(1, 2, 1)
	})
	{
	}
}

public class BlockO : Block
{
	public BlockO() : base(4, 2, new Coord[]
	{
		new(0, 0, 0),
		new(1, 0, 0),
		new(0, 0, 1),
		new(1, 0, 1),
		new(0, 1, 0),
		new(1, 1, 0),
		new(0, 1, 1),
		new(1, 1, 1)
	})
	{
	}
}

public class BlockJ : Block
{
	public BlockJ() : base(5, 3, new Coord[]
	{
		new(1, 0, 1),
		new(1, 1, 1),
		new(1, 2, 1),
		new(0, 2, 1)
	})
	{
	}
}

public class BlockZ : Block
{
	public BlockZ() : base(6, 3, new Coord[]
	{
		new(0, 0, 1),
		new(1, 0, 1),
		new(1, 1, 1),
		new(2, 1, 1)
	})
	{
	}
}

public class BlockS : Block
{
	public BlockS() : base(7, 3, new Coord[]
	{
		new(0, 1, 1),
		new(1, 1, 1),
		new(1, 0, 1),
		new(2, 0, 1)
	})
	{
	}
}