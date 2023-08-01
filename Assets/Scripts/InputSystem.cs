using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
	private enum KEY_VALUE
	{
		LEFT = 0,
		RIGHT,
		UP,
		DOWN,
		ROTATE_X,
		ROTATE_X_INV,
		ROTATE_Y,
		ROTATE_Y_INV,
		ROTATE_Z,
		ROTATE_Z_INV,
		SPACE,
		LEFT_ALT,
		ESC
	}

	private       Vector2     clickPos;
	private       List<bool>  keyUsing;
	private       List<float> keyIntervals;
	private const float       defaultKeyInterval = 0.2f;

	public InputSystem()
	{
		Init();
	}

	private void Init()
	{
		clickPos = Vector2.zero;
		
		keyUsing = new List<bool>
		{
			false, false, false, false, false, false,
			false, false, false, false, false, false,
			false,
		};
		keyIntervals = new List<float>()
		{
			defaultKeyInterval, defaultKeyInterval, defaultKeyInterval,
			defaultKeyInterval, defaultKeyInterval, defaultKeyInterval,
			defaultKeyInterval, defaultKeyInterval, defaultKeyInterval,
			defaultKeyInterval, defaultKeyInterval, 0.1f,
			2f,
		};
	}

	public void InputWindows()
	{
	#region ScreenControl

		if (Input.GetMouseButtonDown(0) && !isCameraShaking)
		{
			mementoRotation = rotatorTr.rotation;
			clickPos        = Vector2.zero;
		}

		if (Input.GetMouseButton(0) && !isCameraShaking)
		{
			Vector2 angle = new(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

			clickPos += angle;
			Dir      =  Mathf.Abs(clickPos.x) > Mathf.Abs(clickPos.y);

			if (checkDir)
			{
				rotatorTr.rotation = mementoRotation;
				checkDir           = false;
				clickPos           = Vector2.zero;
			}

			Quaternion target = rotatorTr.rotation;

			if (Dir)
			{
				target *= Quaternion.AngleAxis(angle.x, Vector3.right);
			}
			else
			{
				target *= Quaternion.AngleAxis(angle.y, Vector3.up);
			}

			float angleX = target.eulerAngles.x;
			angleX = angleX > 180 ? angleX - 360 : angleX;
			target.eulerAngles = new Vector3(Mathf.Clamp(angleX, -cameraRotationConstraintX,
			                                             cameraRotationConstraintX),
			                                 target.eulerAngles.y, 0f);
			rotatorTr.rotation = Quaternion.RotateTowards(rotatorTr.rotation, target,
			                                              cameraSpeed * Time.deltaTime);
		}

		if (Input.GetMouseButtonUp(0) && !isCameraShaking)
		{
			mementoRotation = rotatorTr.rotation;
		}

		if (Input.GetKey(KeyCode.UpArrow) && !isCameraShaking)
		{
			Quaternion target = rotatorTr.rotation;

			target *= Quaternion.AngleAxis(-1f, Vector3.right);

			float angleX = target.eulerAngles.x;
			angleX = angleX > 180 ? angleX - 360 : angleX;
			target.eulerAngles = new Vector3(Mathf.Clamp(angleX, -cameraRotationConstraintX,
			                                             cameraRotationConstraintX),
			                                 target.eulerAngles.y, 0f);
			rotatorTr.rotation = Quaternion.RotateTowards(rotatorTr.rotation, target,
			                                              cameraSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.DownArrow) && !isCameraShaking)
		{
			Quaternion target = rotatorTr.rotation;

			target *= Quaternion.AngleAxis(1f, Vector3.right);

			float angleX = target.eulerAngles.x;
			angleX = angleX > 180 ? angleX - 360 : angleX;
			target.eulerAngles = new Vector3(Mathf.Clamp(angleX, -cameraRotationConstraintX,
			                                             cameraRotationConstraintX),
			                                 target.eulerAngles.y, 0f);
			rotatorTr.rotation = Quaternion.RotateTowards(rotatorTr.rotation, target,
			                                              cameraSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.LeftArrow) && !isCameraShaking)
		{
			Quaternion rotation = rotatorTr.rotation;
			Quaternion target   = rotation;

			target             *= Quaternion.AngleAxis(1f, Vector3.up);
			target.eulerAngles =  new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0f);

			rotation = Quaternion.RotateTowards(rotation, target,
			                                    cameraSpeed * Time.deltaTime);
			rotatorTr.rotation = rotation;
		}

		if (Input.GetKey(KeyCode.RightArrow) && !isCameraShaking)
		{
			Quaternion rotation = rotatorTr.rotation;
			Quaternion target   = rotation;

			target             *= Quaternion.AngleAxis(-1f, Vector3.up);
			target.eulerAngles =  new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0f);

			rotation = Quaternion.RotateTowards(rotation, target,
			                                    cameraSpeed * Time.deltaTime);
			rotatorTr.rotation = rotation;
		}

		if (Input.GetKey(KeyCode.LeftShift) && canSaveBlock)
		{
			StartCoroutine(PlaySfx(SFX_VALUE.SHIFT));

			currentBlock = BlockQueue.SaveAndUpdateBlock(currentBlock);
			canSaveBlock = false;
			RefreshCurrentBlock();
		}

	#endregion

	#region BlockControl

		if (Input.GetKey(KeyCode.A) && !keyUsing[(int)KEY_VALUE.LEFT])
		{
			MoveBlockLeft();
			keyUsing[(int)KEY_VALUE.LEFT] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.LEFT));
		}

		if (Input.GetKey(KeyCode.D) && !keyUsing[(int)KEY_VALUE.RIGHT])
		{
			MoveBlockRight();
			keyUsing[(int)KEY_VALUE.RIGHT] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.RIGHT));
		}

		if (Input.GetKey(KeyCode.W) && !keyUsing[(int)KEY_VALUE.UP])
		{
			MoveBlockForward();
			keyUsing[(int)KEY_VALUE.UP] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.UP));
		}

		if (Input.GetKey(KeyCode.S) && !keyUsing[(int)KEY_VALUE.DOWN])
		{
			MoveBlockBackward();
			keyUsing[(int)KEY_VALUE.DOWN] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.DOWN));
		}

		if ((Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.Keypad7)) && !keyUsing[(int)KEY_VALUE.ROTATE_X])
		{
			RotateBlockXCounterClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_X] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_X));
		}

		if ((Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Keypad8)) && !keyUsing[(int)KEY_VALUE.ROTATE_X_INV])
		{
			RotateBlockXClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_X_INV] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_X_INV));
		}

		if ((Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad4)) && !keyUsing[(int)KEY_VALUE.ROTATE_Y])
		{
			RotateBlockYClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_Y] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Y));
		}

		if ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad5)) && !keyUsing[(int)KEY_VALUE.ROTATE_Y_INV])
		{
			RotateBlockYCounterClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_Y_INV] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Y_INV));
		}

		if ((Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Keypad1)) && !keyUsing[(int)KEY_VALUE.ROTATE_Z])
		{
			RotateBlockZCounterClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_Z] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Z));
		}

		if ((Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Keypad2)) &&
		    !keyUsing[(int)KEY_VALUE.ROTATE_Z_INV])
		{
			RotateBlockZClockWise();
			keyUsing[(int)KEY_VALUE.ROTATE_Z_INV] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Z_INV));
		}

		if (Input.GetKey(KeyCode.Space) && !keyUsing[(int)KEY_VALUE.SPACE])
		{
			MoveBlockDownWhole();
			keyUsing[(int)KEY_VALUE.SPACE] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.SPACE));
		}

		if (Input.GetKey(KeyCode.LeftAlt) && !keyUsing[(int)KEY_VALUE.LEFT_ALT])
		{
			MoveBlockDown();
			keyUsing[(int)KEY_VALUE.LEFT_ALT] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.LEFT_ALT));
		}

	#endregion

	#region GameManagement

		if (Input.GetKey(KeyCode.Escape) && !keyUsing[(int)KEY_VALUE.ESC])
		{
			GamePause();
			keyUsing[(int)KEY_VALUE.ESC] = true;
			StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
		}

	#endregion
	}

	private IEnumerator KeyRewind(int id)
	{
		yield return new WaitForSeconds(keyIntervals[id]);

		keyUsing[id] = false;
	}
}