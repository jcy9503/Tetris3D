using UnityEngine;

public class BlockQueue
{
    public BlockQueue()
    {
        NextBlock = RandomBlock();
    }
    
    private static readonly BlockFactory blockCreate = new();

    private const int maxId = 7;

    public Block NextBlock { get; private set; }

    private Block curBlock;
    public Block Cur
    {
        get => curBlock;
        private set
        {
            curBlock = value;
            curBlock.Reset();
        }
    }

    private static Block RandomBlock()
    {
        return blockCreate.BlockSpawn(Random.Range(0, maxId));
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