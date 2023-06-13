using UnityEngine;

public interface IBlock
{
    uint GetId();
    void Reset();
    void Move(int x, int y, int z);
    void RotateXClockWise();
    void RotateXCounterClockWise();
    void RotateYClockWise();
    void RotateYCounterClockWise();
    void RotateZClockWise();
    void RotateZCounterClockWise();
}