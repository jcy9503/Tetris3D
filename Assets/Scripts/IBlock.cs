public interface IBlock
{
	uint GetId();
	void Reset();
	void Move(Position move);
	void RotateXClockWise();
	void RotateXCounterClockWise();
	void RotateYClockWise();
	void RotateYCounterClockWise();
	void RotateZClockWise();
	void RotateZCounterClockWise();
}