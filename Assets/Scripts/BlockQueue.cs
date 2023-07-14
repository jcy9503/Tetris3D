using Random = System.Random;

public class BlockQueue
{
    public BlockQueue()
    {
        NextBlock = RandomBlock();
    }

    private static readonly BlockFactory blockCreate = new();

    private const int maxId = 7;

    public Block NextBlock { get; private set; }

    private Block currentBlock;

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
        return blockCreate.BlockSpawn(randValue.Next(0, maxId));
    }

    public Block GetAndUpdateBlock()
    {
        Block block = NextBlock;

        do
        {
            NextBlock = RandomBlock();
        } while (block.GetId() == NextBlock.GetId());

        return block;
    }
}