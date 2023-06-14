using UnityEngine;

public interface IBlock
{
    uint GetId();
    void Reset();
    void Move(Vector3 move);
    void RotateXClockWise();
    void RotateXCounterClockWise();
    void RotateYClockWise();
    void RotateYCounterClockWise();
    void RotateZClockWise();
    void RotateZCounterClockWise();
}