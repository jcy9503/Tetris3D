using Random = System.Random;

public class BlockQueue
{
	public BlockQueue()
	{
		currentBlock = RandomBlock();
		nextBlock    = RandomBlock();
	}

	private static readonly BlockFactory blockCreateFunc = new();
	private const           int          blockType       = 7;
	private                 Block        nextBlock;
	private                 Block        currentBlock;
	public Block Current
	{
		get => currentBlock;
		private set
		{
			currentBlock = value;
			currentBlock.Reset();
		}
	}

	private static Block RandomBlock()
	{
		Random randValue = new();

		return blockCreateFunc.BlockSpawn(randValue.Next(0, blockType));
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