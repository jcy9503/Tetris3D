using UnityEngine;

public class CBlockQueue
{
    public CBlockQueue()
    {
        NextBlock = RandomBlock();
    }
    
    private static readonly BlockFactory BlockCreate = new();

    private const int maxId = 7;

    public CBlock NextBlock { get; private set; }

    private CBlock curBlock;
    public CBlock Cur
    {
        get => curBlock;
        private set
        {
            curBlock = value;
            curBlock.Reset();
        }
    }

    private CBlock RandomBlock()
    {
        return BlockCreate.BlockSpawn(Random.Range(1, maxId + 1));
    }

    public CBlock GetAndUpdateBlock()
    {
        CBlock block = NextBlock;

        do
        {
            NextBlock = RandomBlock();
        } while (block.GetId() == NextBlock.GetId());

        return block;
    }
}