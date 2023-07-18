using Random = System.Random;

public class BlockQueue
{
	public BlockQueue()
	{
		nextBlock = RandomBlock();
	}

	private static readonly BlockFactory blockCreateFunc = new();
	private const           int          blockTypeNum    = 7;
	private                 Block        nextBlock;

	private static Block RandomBlock()
	{
		Random randValue = new();

		return blockCreateFunc.BlockSpawn(randValue.Next(0, blockTypeNum));
	}

	public Block GetAndUpdateBlock()
	{
		Block block = nextBlock;

		do
		{
			nextBlock = RandomBlock();
		} while (block.GetId() == nextBlock.GetId());

		return block;
	}
}