/*
 * GameManager.cs
 * --------------
 * Made by Lucas Jeong
 * Contains main game logic and singleton instance.
 * Also contains screen control method.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static readonly object      locker = new();
	private static          bool        shuttingDown;
	private static          GameManager instance;
	public static GameManager Instance
	{
		get
		{
			if (shuttingDown)
			{
				Debug.Log("[Singleton] instance GameManager already choked. Returning null.");

				return null;
			}

			lock (locker)
			{
				if (instance != null) return instance;

				instance = FindObjectOfType<GameManager>();

				if (instance == null)
				{
					instance = new GameObject("GameManager").AddComponent<GameManager>();
				}

				DontDestroyOnLoad(instance);
			}

			return instance;
		}
	}

	private void OnApplicationQuit()
	{
		shuttingDown = true;
	}

	private void OnDestroy()
	{
		shuttingDown = true;
	}

	private void Awake()
	{
		GameOver = false;

		mainCamera                     = Camera.main;
		mainCamera!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		rotatorTr                      = GameObject.Find("Rotator").GetComponent<Transform>();
		mementoRotation                = Quaternion.identity;
		Dir                            = false;
		checkDir                       = false;
		viewAngle                      = 0;

		Grid       = new GameGrid(ref gridSize, blockSize);
		BlockQueue = new BlockQueue();
		startOffset = new Vector3(-Grid.SizeX / 2f + blockSize / 2,
		                          Grid.SizeY  / 2f - blockSize / 2,
		                          -Grid.SizeZ / 2f + blockSize / 2);
		CurrentBlock = BlockQueue.GetAndUpdateBlock();

		blockMeshList  = new List<PrefabMesh>();
		shadowMeshList = new List<PrefabMesh>();
		gridMeshList   = new List<PrefabMesh>();

		RenderShadowBlock();

		keyList = new List<bool>()
		{
			false, false, false, false, false, false,
			false, false, false, false, false
		};

		func = new List<Coroutine>
		{
			StartCoroutine(BlockDown()),
			StartCoroutine(AngleCalculate()),
		};
	}

	private void Update()
	{
		if (GameOver)
		{
			Terminate();
		}
		else
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

			#region ScreenControl

			if (Input.GetMouseButtonDown(0))
			{
				mementoRotation = rotatorTr.rotation;
				clickPos        = Vector2.zero;
			}

			if (Input.GetMouseButton(0))
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

			if (Input.GetMouseButtonUp(0))
			{
				mementoRotation = rotatorTr.rotation;
			}

			if (Input.GetKey(KeyCode.UpArrow))
			{
				Quaternion target = rotatorTr.rotation;

				target *= Quaternion.AngleAxis(-0.2f, Vector3.right);

				float angleX = target.eulerAngles.x;
				angleX = angleX > 180 ? angleX - 360 : angleX;
				target.eulerAngles = new Vector3(Mathf.Clamp(angleX, -cameraRotationConstraintX,
				                                             cameraRotationConstraintX),
				                                 target.eulerAngles.y, 0f);
				rotatorTr.rotation = Quaternion.RotateTowards(rotatorTr.rotation, target,
				                                              cameraSpeed * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.DownArrow))
			{
				Quaternion target = rotatorTr.rotation;

				target *= Quaternion.AngleAxis(0.2f, Vector3.right);

				float angleX = target.eulerAngles.x;
				angleX = angleX > 180 ? angleX - 360 : angleX;
				target.eulerAngles = new Vector3(Mathf.Clamp(angleX, -cameraRotationConstraintX,
				                                             cameraRotationConstraintX),
				                                 target.eulerAngles.y, 0f);
				rotatorTr.rotation = Quaternion.RotateTowards(rotatorTr.rotation, target,
				                                              cameraSpeed * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				Quaternion rotation = rotatorTr.rotation;
				Quaternion target   = rotation;

				target             *= Quaternion.AngleAxis(0.2f, Vector3.up);
				target.eulerAngles =  new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0f);

				rotation = Quaternion.RotateTowards(rotation, target,
				                                    cameraSpeed * Time.deltaTime);
				rotatorTr.rotation = rotation;
			}

			if (Input.GetKey(KeyCode.RightArrow))
			{
				Quaternion rotation = rotatorTr.rotation;
				Quaternion target   = rotation;

				target             *= Quaternion.AngleAxis(-0.2f, Vector3.up);
				target.eulerAngles =  new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0f);

				rotation = Quaternion.RotateTowards(rotation, target,
				                                    cameraSpeed * Time.deltaTime);
				rotatorTr.rotation = rotation;
			}

			#endregion

			#region KeyBinding

			if (Input.GetKey(KeyCode.A) && !keyList[0])
			{
				MoveBlockLeft();
				RenderCurrentBlock();
				keyList[0] = true;
				StartCoroutine(KeyRewind(0));
			}

			if (Input.GetKey(KeyCode.D) && !keyList[1])
			{
				MoveBlockRight();
				RenderCurrentBlock();
				keyList[1] = true;
				StartCoroutine(KeyRewind(1));
			}

			if (Input.GetKey(KeyCode.W) && !keyList[2])
			{
				MoveBlockForward();
				RenderCurrentBlock();
				keyList[2] = true;
				StartCoroutine(KeyRewind(2));
			}

			if (Input.GetKey(KeyCode.S) && !keyList[3])
			{
				MoveBlockBackward();
				RenderCurrentBlock();
				keyList[3] = true;
				StartCoroutine(KeyRewind(3));
			}

			if ((Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.Keypad7)) && !keyList[4])
			{
				RotateBlockXClockWise();
				RenderCurrentBlock();
				keyList[4] = true;
				StartCoroutine(KeyRewind(4));
			}

			if ((Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Keypad8)) && !keyList[5])
			{
				RotateBlockXCounterClockWise();
				RenderCurrentBlock();
				keyList[5] = true;
				StartCoroutine(KeyRewind(5));
			}

			if ((Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad4)) && !keyList[6])
			{
				RotateBlockYClockWise();
				RenderCurrentBlock();
				keyList[6] = true;
				StartCoroutine(KeyRewind(6));
			}

			if ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad5)) && !keyList[7])
			{
				RotateBlockYCounterClockWise();
				RenderCurrentBlock();
				keyList[7] = true;
				StartCoroutine(KeyRewind(7));
			}

			if ((Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Keypad1)) && !keyList[8])
			{
				RotateBlockZClockWise();
				RenderCurrentBlock();
				keyList[8] = true;
				StartCoroutine(KeyRewind(8));
			}

			if ((Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Keypad2)) && !keyList[9])
			{
				RotateBlockZCounterClockWise();
				RenderCurrentBlock();
				keyList[9] = true;
				StartCoroutine(KeyRewind(9));
			}

			if (Input.GetKey(KeyCode.Space) && !keyList[10])
			{
				MoveBlockDownWhole();
				Render();
				keyList[10] = true;
				StartCoroutine(KeyRewind(10));
			}

			#endregion

#endif
		}
	}

	private static bool       GameOver { get; set; }
	private        Camera     mainCamera;
	private        Transform  rotatorTr;
	private        Quaternion mementoRotation;
	private        bool       checkDir;
	private        bool       dir;
	private bool Dir
	{
		get => dir;
		set
		{
			if (value != dir)
				checkDir = true;
			dir = value;
		}
	}
	private                  Vector2    clickPos;
	[SerializeField] private float      initialCameraRotationX    = 15f;
	[SerializeField] private float      cameraRotationConstraintX = 55f;
	[SerializeField] private float      cameraSpeed               = 2000f;
	private                  int        viewAngle;
	[SerializeField] private int[]      gridSize = { 10, 22, 10 };
	public static            GameGrid   Grid;
	private                  Vector3    startOffset;
	private static           BlockQueue BlockQueue { get; set; }
	private                  Block      currentBlock;
	private                  Block      shadowBlock;
	private Block CurrentBlock
	{
		get => currentBlock;
		set
		{
			currentBlock = value;
			currentBlock.Reset();
		}
	}
	private                  List<PrefabMesh> blockMeshList;
	private                  List<PrefabMesh> shadowMeshList;
	private                  List<PrefabMesh> gridMeshList;
	[SerializeField] private float            blockSize    = 1.0f;
	[SerializeField] private float            downInterval = 1.0f;
	[SerializeField] private float            keyInterval  = 0.2f;
	private                  List<bool>       keyList;
	private                  List<Coroutine>  func;

	private IEnumerator BlockDown()
	{
		while (true)
		{
			Render();

			if (!Grid.IsPlaneEmpty(0))
			{
				GameOver = true;

				break;
			}

			MoveBlockDown();

			yield return new WaitForSeconds(downInterval);
		}
	}

	private IEnumerator KeyRewind(int id)
	{
		yield return new WaitForSeconds(keyInterval);

		keyList[id] = false;
	}

	private IEnumerator AngleCalculate()
	{
		while (true)
		{
			if (GameOver) break;

			viewAngle = rotatorTr.rotation.eulerAngles.y switch
			{
				<= 45f or > 315f   => 0,
				<= 135f and > 45f  => 1,
				<= 225f and > 135f => 2,
				_                  => 3
			};
			
			yield return new WaitForSeconds(keyInterval);
		}
	}

	private void Terminate()
	{
		foreach (Coroutine coroutine in func)
		{
			StopCoroutine(coroutine);
		}

		GameOver = false;
	}

	private static bool BlockFits(Block block)
	{
		return block.TilePositions().All(coord => Grid.IsEmpty(coord.X, coord.Y, coord.Z));
	}

	private void RotateBlockXClockWise()
	{
		CurrentBlock.RotateXClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateXCounterClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void RotateBlockXCounterClockWise()
	{
		CurrentBlock.RotateXCounterClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateXClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void RotateBlockYClockWise()
	{
		CurrentBlock.RotateYClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateYCounterClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void RotateBlockYCounterClockWise()
	{
		CurrentBlock.RotateYCounterClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateYClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void RotateBlockZClockWise()
	{
		CurrentBlock.RotateZClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateZCounterClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void RotateBlockZCounterClockWise()
	{
		CurrentBlock.RotateZCounterClockWise();

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.RotateZClockWise();
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void MoveBlockLeft()
	{
		CurrentBlock.Move(Coord.Left[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.Move(Coord.Right[viewAngle]);
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void MoveBlockRight()
	{
		CurrentBlock.Move(Coord.Right[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.Move(Coord.Left[viewAngle]);
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void MoveBlockForward()
	{
		CurrentBlock.Move(Coord.Forward[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.Move(Coord.Backward[viewAngle]);
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void MoveBlockBackward()
	{
		CurrentBlock.Move(Coord.Backward[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			CurrentBlock.Move(Coord.Forward[viewAngle]);
		}
		else
		{
			RenderShadowBlock();
		}
	}

	private void MoveBlockDown()
	{
		CurrentBlock.Move(Coord.Down);

		if (BlockFits(CurrentBlock)) return;

		CurrentBlock.Move(Coord.Up);
		PlaceBlock();
	}

	private void MoveBlockDownWhole()
	{
		do
		{
			CurrentBlock.Move(Coord.Down);
		} while (BlockFits(CurrentBlock));

		CurrentBlock.Move(Coord.Up);
		PlaceBlock();
	}

	private static bool IsGamerOver()
	{
		return !(Grid.IsPlaneEmpty(-1) && Grid.IsPlaneEmpty(0));
	}

	private void PlaceBlock()
	{
		foreach (Coord coord in CurrentBlock.TilePositions())
		{
			Grid[coord.X, coord.Y, coord.Z] = CurrentBlock.GetId();
		}

		Grid.ClearFullRows();

		if (IsGamerOver())
		{
			GameOver = true;
		}
		else
		{
			CurrentBlock = BlockQueue.GetAndUpdateBlock();
			RenderShadowBlock();
		}
	}

	private void Render()
	{
		RenderGrid();
		RenderCurrentBlock();
	}

	private void ClearCurrentBlock()
	{
		foreach (PrefabMesh mesh in blockMeshList)
		{
			Destroy(mesh.Obj);
		}

		blockMeshList.Clear();
	}

	private void ClearShadowBlock()
	{
		foreach (PrefabMesh mesh in shadowMeshList)
		{
			Destroy(mesh.Obj);
		}

		shadowMeshList.Clear();
	}

	private void RenderCurrentBlock()
	{
		ClearCurrentBlock();

		foreach (Coord coord in CurrentBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			blockMeshList.Add(new PrefabMesh("Prefabs/Block", startOffset + offset,
			                                 Block.MatPath[CurrentBlock.GetId()]));
		}
	}

	private void RenderShadowBlock()
	{
		ClearShadowBlock();
		shadowBlock = CurrentBlock.CopyBlock();

		do
		{
			shadowBlock.Move(Coord.Down);
		} while (BlockFits(shadowBlock));

		shadowBlock.Move(Coord.Up);

		foreach (Coord coord in shadowBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			shadowMeshList.Add(new PrefabMesh("Prefabs/Block", startOffset + offset,
			                                  Block.MatPath[0]));
		}
	}

	private void RenderGrid()
	{
		foreach (PrefabMesh mesh in gridMeshList)
		{
			Destroy(mesh.Obj);
		}

		gridMeshList.Clear();

		for (int i = 0; i < Grid.SizeX; ++i)
		{
			for (int j = 0; j < Grid.SizeY; ++j)
			{
				for (int k = 0; k < Grid.SizeZ; ++k)
				{
					if (Grid[i, j, k] != 0)
					{
						Vector3 offset = new(i, -j, k);
						gridMeshList.Add(new PrefabMesh("Prefabs/Block", startOffset + offset,
						                                Block.MatPath[Grid[i, j, k]]));
					}
				}
			}
		}
	}
}