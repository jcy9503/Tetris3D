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
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

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

	public static			 bool gameOver;
	private static           bool isPause;
	public static            bool TestMode;
	[SerializeField] private bool testMode;

	private GameObject mainCamera;
	private Transform  rotatorTr;
	private Quaternion mementoRotation;
	private bool       checkDir;
	private bool       dir;
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
	[SerializeField] private float      cameraShakeAmount         = 0.5f;
	[SerializeField] private float      cameraShakeTime           = 0.2f;
	private                  bool       cameraShake;
	private                  int        viewAngle;
	[SerializeField] private int[]      gridSize = { 10, 22, 10 };
	public static            GameGrid   Grid;
	private                  Vector3    startOffset;
	private static           BlockQueue BlockQueue { get; set; }
	private                  Block      currentBlock;
	private                  Block      shadowBlock;
	private static readonly  int        alpha = Shader.PropertyToID("_Alpha");

	private                  bool             canSaveBlock;
	private                  List<PrefabMesh> blockMeshList;
	private                  List<PrefabMesh> shadowMeshList;
	private                  List<PrefabMesh> gridMeshList;
	[SerializeField] private float            blockSize    = 1.0f;
	[SerializeField] private float            downInterval = 1.0f;
	private                  List<bool>       keyUsing;
	private                  List<float>      keyIntervals;
	private const            float            defaultKeyInterval = 0.2f;

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

	private delegate IEnumerator LogicFunc();

	private        LogicFunc       logicMethods;
	private        List<Coroutine> logicList;
	private static GameObject      blockObj;
	private static GameObject      shadowObj;
	public static  GameObject      GridObj;

	private void Awake()
	{
		gameOver = false;
		isPause  = false;
		TestMode = testMode;

		GridObj   = GameObject.Find("Grid");
		blockObj  = GameObject.Find("Blocks");
		shadowObj = GameObject.Find("Shadow");

		mainCamera                     = GameObject.Find("Main Camera");
		mainCamera!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		rotatorTr                      = GameObject.Find("Rotator").GetComponent<Transform>();
		mementoRotation                = Quaternion.identity;
		Dir                            = false;
		checkDir                       = false;
		viewAngle                      = 0;
		cameraShake                    = false;

		if (TestMode)
		{
			gridSize[0] = 4;
			gridSize[2] = 4;
			Grid        = new GameGrid(ref gridSize, blockSize);
		}
		else
		{
			Grid = new GameGrid(ref gridSize, blockSize);
		}

		BlockQueue = new BlockQueue();
		startOffset = new Vector3(-Grid.SizeX / 2f + blockSize / 2,
		                          Grid.SizeY  / 2f - blockSize / 2,
		                          -Grid.SizeZ / 2f + blockSize / 2);
		currentBlock = BlockQueue.GetAndUpdateBlock();
		canSaveBlock = true;

		blockMeshList  = new List<PrefabMesh>();
		shadowMeshList = new List<PrefabMesh>();
		gridMeshList   = new List<PrefabMesh>();

		RenderCurrentBlock();
		RenderShadowBlock();

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
			defaultKeyInterval,
		};

		logicMethods = BlockDown;

		logicList = new List<Coroutine>
		{
			StartCoroutine(logicMethods()),
			StartCoroutine(AngleCalculate()),
		};
	}

	public bool isOver()
	{
		return gameOver;
	}

	private void Update()
	{
		if (gameOver)
		{
			Terminate();
		}
		else if (!isPause)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

			#region ScreenControl

			if (Input.GetMouseButtonDown(0) && !cameraShake)
			{
				mementoRotation = rotatorTr.rotation;
				clickPos        = Vector2.zero;
			}

			if (Input.GetMouseButton(0) && !cameraShake)
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

			if (Input.GetMouseButtonUp(0) && !cameraShake)
			{
				mementoRotation = rotatorTr.rotation;
			}

			if (Input.GetKey(KeyCode.UpArrow) && !cameraShake)
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

			if (Input.GetKey(KeyCode.DownArrow) && !cameraShake)
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

			if (Input.GetKey(KeyCode.LeftArrow) && !cameraShake)
			{
				Quaternion rotation = rotatorTr.rotation;
				Quaternion target   = rotation;

				target             *= Quaternion.AngleAxis(1f, Vector3.up);
				target.eulerAngles =  new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0f);

				rotation = Quaternion.RotateTowards(rotation, target,
				                                    cameraSpeed * Time.deltaTime);
				rotatorTr.rotation = rotation;
			}

			if (Input.GetKey(KeyCode.RightArrow) && !cameraShake)
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
				isPause = true;
				GamePause();
				keyUsing[(int)KEY_VALUE.ESC] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
			}

			#endregion

#elif UNITY_ANDROID
		#region ScreenControl

			if (Input.touchCount == 1 && !cameraShake)
			{
				Touch touch = Input.GetTouch(0);

				if (touch.phase == TouchPhase.Began)
				{
					mementoRotation = rotatorTr.rotation;
					clickPos = Vector2.zero;
				}
			}

		#endregion

		#region BlockControl

		#endregion

#endif

			#region Effect

			if (shadowMeshList.Count > 0)
			{
				foreach (PrefabMesh mesh in shadowMeshList)
				{
					mesh.Renderer.sharedMaterial.SetFloat(alpha, Mathf.PingPong(Time.time, 0.85f) + 0.15f);
				}
			}

			#endregion
		}

		else if (isPause)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

			if (Input.GetKey(KeyCode.Escape) && !keyUsing[(int)KEY_VALUE.ESC])
			{
				GameResume();
				isPause                      = false;
				keyUsing[(int)KEY_VALUE.ESC] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
			}

#endif
		}
	}

	private void GamePause()
	{
		foreach (Coroutine coroutine in logicList)
		{
			StopCoroutine(coroutine);
		}
	}

	private void GameResume()
	{
		logicList.Add(StartCoroutine(logicMethods()));
		logicList.Add(StartCoroutine(AngleCalculate()));
	}

	private IEnumerator BlockDown()
	{
		while (true)
		{
			RenderCurrentBlock();

			if (!Grid.IsPlaneEmpty(0))
			{
				gameOver = true;

				break;
			}

			MoveBlockDown();

			yield return new WaitForSeconds(downInterval);
		}
	}

	private IEnumerator KeyRewind(int id)
	{
		yield return new WaitForSeconds(keyIntervals[id]);

		keyUsing[id] = false;
	}

	private IEnumerator AngleCalculate()
	{
		while (true)
		{
			if (gameOver) break;

			viewAngle = rotatorTr.rotation.eulerAngles.y switch
			{
				<= 45f or > 315f   => 0,
				<= 135f and > 45f  => 1,
				<= 225f and > 135f => 2,
				_                  => 3
			};

			yield return new WaitForSeconds(defaultKeyInterval);
		}
	}

	private IEnumerator CameraShake()
	{
		cameraShake = true;

		yield return StartCoroutine(RotatorShake());

		RotatorPositionClear();
	}

	private IEnumerator RotatorShake()
	{
		float timer = 0;

		while (timer <= cameraShakeTime)
		{
			rotatorTr.position =  (Vector3)Random.insideUnitCircle * cameraShakeAmount;
			timer              += Time.deltaTime;

			yield return null;
		}
	}

	private void RotatorPositionClear()
	{
		rotatorTr.position = Vector3.zero;

		cameraShake = false;
	}

	private void Terminate()
	{
		foreach (Coroutine coroutine in logicList)
		{
			StopCoroutine(coroutine);
		}

		gameOver = false;
	}

	private static bool BlockFits(Block block)
	{
		return block.TilePositions().All(coord => Grid.IsEmpty(coord.X, coord.Y, coord.Z));
	}

	private void RotateBlockXClockWise()
	{
		switch (viewAngle)
		{
			case 0:
				currentBlock.RotateXClockWise();

				break;

			case 1:
				currentBlock.RotateZCounterClockWise();

				break;

			case 2:
				currentBlock.RotateXCounterClockWise();

				break;

			case 3:
				currentBlock.RotateZClockWise();

				break;
		}

		if (!BlockFits(currentBlock))
		{
			switch (viewAngle)
			{
				case 0:
					currentBlock.RotateXCounterClockWise();

					break;

				case 1:
					currentBlock.RotateZClockWise();

					break;

				case 2:
					currentBlock.RotateXClockWise();

					break;

				case 3:
					currentBlock.RotateZCounterClockWise();

					break;
			}
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void RotateBlockXCounterClockWise()
	{
		switch (viewAngle)
		{
			case 0:
				currentBlock.RotateXCounterClockWise();

				break;

			case 1:
				currentBlock.RotateZClockWise();

				break;

			case 2:
				currentBlock.RotateXClockWise();

				break;

			case 3:
				currentBlock.RotateZCounterClockWise();

				break;
		}

		if (!BlockFits(currentBlock))
		{
			switch (viewAngle)
			{
				case 0:
					currentBlock.RotateXClockWise();

					break;

				case 1:
					currentBlock.RotateZCounterClockWise();

					break;

				case 2:
					currentBlock.RotateXCounterClockWise();

					break;

				case 3:
					currentBlock.RotateZClockWise();

					break;
			}
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYClockWise()
	{
		currentBlock.RotateYClockWise();

		if (!BlockFits(currentBlock))
		{
			currentBlock.RotateYCounterClockWise();
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYCounterClockWise()
	{
		currentBlock.RotateYCounterClockWise();

		if (!BlockFits(currentBlock))
		{
			currentBlock.RotateYClockWise();
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void RotateBlockZClockWise()
	{
		switch (viewAngle)
		{
			case 0:
				currentBlock.RotateZClockWise();

				break;

			case 1:
				currentBlock.RotateXClockWise();

				break;

			case 2:
				currentBlock.RotateZCounterClockWise();

				break;

			case 3:
				currentBlock.RotateXCounterClockWise();

				break;
		}

		if (!BlockFits(currentBlock))
		{
			switch (viewAngle)
			{
				case 0:
					currentBlock.RotateZCounterClockWise();

					break;

				case 1:
					currentBlock.RotateXCounterClockWise();

					break;

				case 2:
					currentBlock.RotateZClockWise();

					break;

				case 3:
					currentBlock.RotateXClockWise();

					break;
			}
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void RotateBlockZCounterClockWise()
	{
		switch (viewAngle)
		{
			case 0:
				currentBlock.RotateZCounterClockWise();

				break;

			case 1:
				currentBlock.RotateXCounterClockWise();

				break;

			case 2:
				currentBlock.RotateZClockWise();

				break;

			case 3:
				currentBlock.RotateXClockWise();

				break;
		}

		if (!BlockFits(currentBlock))
		{
			switch (viewAngle)
			{
				case 0:
					currentBlock.RotateZClockWise();

					break;

				case 1:
					currentBlock.RotateXClockWise();

					break;

				case 2:
					currentBlock.RotateZCounterClockWise();

					break;

				case 3:
					currentBlock.RotateXCounterClockWise();

					break;
			}
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void MoveBlockLeft()
	{
		currentBlock.Move(Coord.Left[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			currentBlock.Move(Coord.Right[viewAngle]);
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void MoveBlockRight()
	{
		currentBlock.Move(Coord.Right[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			currentBlock.Move(Coord.Left[viewAngle]);
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void MoveBlockForward()
	{
		currentBlock.Move(Coord.Forward[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			currentBlock.Move(Coord.Backward[viewAngle]);
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void MoveBlockBackward()
	{
		currentBlock.Move(Coord.Backward[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			currentBlock.Move(Coord.Forward[viewAngle]);
		}
		else
		{
			RefreshCurrentBlock();
		}
	}

	private void MoveBlockDown()
	{
		currentBlock.Move(Coord.Down);

		if (BlockFits(currentBlock))
		{
			RenderCurrentBlock();

			return;
		}

		currentBlock.Move(Coord.Up);
		StartCoroutine(PlaceBlock());
	}

	private void MoveBlockDownWhole()
	{
		int num = 0;

		do
		{
			currentBlock.Move(Coord.Down);
			++num;
		} while (BlockFits(currentBlock));

		if (num > 2)
			StartCoroutine(CameraShake());

		currentBlock.Move(Coord.Up);
		StartCoroutine(PlaceBlock());
	}

	private static bool IsGamerOver()
	{
		return !(Grid.IsPlaneEmpty(-1) && Grid.IsPlaneEmpty(0));
	}

	private IEnumerator ClearMeshEffect(int start, int end)
	{
		while (gridMeshList[end].Obj.transform.localScale.magnitude < 3f)
		{
			float alphaSet = 1.0f;
			for (int i = start; i < end; ++i)
			{
				gridMeshList[i].Obj.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
				gridMeshList[i].Renderer.sharedMaterial.SetFloat(alpha, alphaSet);

				yield return new WaitForSeconds(0.05f);

				alphaSet -= 0.05f;
			}
		}
	}

	private IEnumerator ClearEffect(List<int> cleared)
	{
		if (cleared.Count == 0) yield break;

		isPause = true;

		int idx   = 1;
		int start = -1;

		for (int i = 0; i < gridMeshList.Count; ++i)
		{
			if (start == -1 && cleared[^idx] == gridMeshList[i].Pos.Y)
			{
				start = i;
			}
			else if (start != -1 && gridMeshList[i].Pos.Y > cleared[^idx])
			{
				yield return StartCoroutine(ClearMeshEffect(start, i));

				start = -1;
				++idx;
			}
		}

		isPause = false;
	}

	private IEnumerator PlaceBlock()
	{
		foreach (Coord coord in currentBlock.TilePositions())
		{
			Grid[coord.X, coord.Y, coord.Z] = currentBlock.GetId();
		}

		List<int> cleared = Grid.ClearFullRows();

		yield return StartCoroutine(ClearEffect(cleared));

		RenderGrid();

		if (IsGamerOver())
		{
			gameOver = true;
		}
		else
		{
			canSaveBlock = true;
			currentBlock = BlockQueue.GetAndUpdateBlock();
			RefreshCurrentBlock();
		}
	}

	private void RefreshCurrentBlock()
	{
		RenderCurrentBlock();
		RenderShadowBlock();
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

	private void ClearGrid()
	{
		foreach (PrefabMesh mesh in gridMeshList)
		{
			Destroy(mesh.Obj);
		}

		gridMeshList.Clear();
	}

	private void RenderCurrentBlock()
	{
		ClearCurrentBlock();

		foreach (Coord coord in currentBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Block", startOffset + offset,
			                      Block.MatPath[currentBlock.GetId()], coord, ShadowCastingMode.On);

			blockMeshList.Add(mesh);
			mesh.Obj.transform.parent = blockObj.transform;
		}
	}

	private void RenderShadowBlock()
	{
		ClearShadowBlock();
		shadowBlock = currentBlock.CopyBlock();

		do
		{
			shadowBlock.Move(Coord.Down);
		} while (BlockFits(shadowBlock));

		shadowBlock.Move(Coord.Up);

		foreach (Coord coord in shadowBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Block", startOffset + offset,
			                      Block.MatPath[0], coord, ShadowCastingMode.Off);

			shadowMeshList.Add(mesh);
			mesh.Obj.transform.parent = shadowObj.transform;
		}
	}

	private void RenderGrid()
	{
		ClearGrid();

		for (int i = 0; i < Grid.SizeY; ++i)
		{
			for (int j = 0; j < Grid.SizeX; ++j)
			{
				for (int k = 0; k < Grid.SizeZ; ++k)
				{
					if (Grid[j, i, k] != 0)
					{
						Vector3 offset = new(j, -i, k);
						PrefabMesh mesh = new("Prefabs/Block", startOffset + offset, Block.MatPath[^1],
						                      new(j, i, k), ShadowCastingMode.On);

						gridMeshList.Add(mesh);
						mesh.Obj.transform.parent = GridObj.transform;
					}
				}
			}
		}
	}
}

