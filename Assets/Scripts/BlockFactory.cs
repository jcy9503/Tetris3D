using System.Collections.Generic;

public class BlockFactory
{
    private delegate Block CreateBlocks();

    private readonly List<CreateBlocks> createFactory;

    public BlockFactory()
    {
        createFactory = new List<CreateBlocks>
        {
            CreateBlockI,
            CreateBlockL,
            CreateBlockT,
            CreateBlockO,
            CreateBlockJ,
            CreateBlockZ,
            CreateBlockS
        };
    }

    public Block BlockSpawn(int id)
    {
        CreateBlocks func = createFactory[id];

        return func();
    }

    private static Block CreateBlockI()
    {
        Block block = new BlockI();
        return block;
    }

    private static Block CreateBlockL()
    {
        Block block = new BlockL();
        return block;
    }

    private static Block CreateBlockT()
    {
        Block block = new BlockT();
        return block;
    }

    private static Block CreateBlockO()
    {
        Block block = new BlockO();
        return block;
    }

    private static Block CreateBlockJ()
    {
        Block block = new BlockJ();
        return block;
    }

    private static Block CreateBlockZ()
    {
        Block block = new BlockZ();
        return block;
    }

    private static Block CreateBlockS()
    {
        Block block = new BlockS();
        return block;
    }
}