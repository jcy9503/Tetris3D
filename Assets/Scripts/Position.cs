public struct Position
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }

	public Position(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public static Position operator +(Position param) => param;
	public static Position operator -(Position param) => new Position(-param.X, -param.Y, -param.Z);

	public static Position operator +(Position param1, Position param2)
		=> new Position(param1.X + param2.X, param1.Y + param2.Y, param1.Z + param2.Z);

	public static Position operator -(Position param1, Position param2)
		=> param1 + (-param2);
}