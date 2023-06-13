using UnityEngine;

public class CBlock : IBlock
{
    private uint id;
    private Vector3 curPos;

    public uint GetId() => id;

    public void Reset()
    {
        curPos = new Vector3(Random.Range())
    }

    public void Move(int x, int y, int z)
    {
        
    }

    public void RotateXClockWise()
    {
        
    }

    public void RotateXCounterClockWise()
    {
        
    }

    public void RotateYClockWise()
    {
        
    }

    public void RotateYCounterClockWise()
    {
        
    }

    public void RotateZClockWise()
    {
        
    }

    public void RotateZCounterClockWise()
    {
        
    }
}