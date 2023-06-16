using System.Collections.Generic;

public class BlockFactory
{
    private delegate CBlock CreateBlocks();

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

    public CBlock BlockSpawn(int id)
    {
        CreateBlocks func = createFactory[id - 1];
        
        return func();
    }

    private static CBlock CreateBlockI()
    {
        CBlock block = new CBlockI();
        return block;
    }

    private static CBlock CreateBlockL()
    {
        CBlock block = new CBlockL();
        return block;
    }

    private static CBlock CreateBlockT()
    {
        CBlock block = new CBlockT();
        return block;
    }

    private static CBlock CreateBlockO()
    {
        CBlock block = new CBlockO();
        return block;
    }

    private static CBlock CreateBlockJ()
    {
        CBlock block = new CBlockJ();
        return block;
    }

    private static CBlock CreateBlockZ()
    {
        CBlock block = new CBlockZ();
        return block;
    }

    private static CBlock CreateBlockS()
    {
        CBlock block = new CBlockS();
        return block;
    }
}