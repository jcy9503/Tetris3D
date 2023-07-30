/*
 * GameManager.cs
 * --------------
 * Made by Lucas Jeong / kimble
 * Contains main game logic and singleton instance.
 * Also contains screen control method.
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "IteratorNeverReturns")]
public class GameManager : MonoBehaviour
{
#region Singleton

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

#endregion

#region Variables

	[Header("Test Mode")] private static bool       gameOver;
	private static                       bool       isPause;
	private const                        int        baseScore  = 100;
	private static readonly              int[]      scoreValue = { 1, 2, 4, 8 };
	public static                        int        TotalScore;
	public static                        bool       TestGrid;
	[SerializeField] private             bool       testGrid;
	public static                        int        TestHeight;
	[SerializeField] private             int        testHeight = 4;
	public static                        bool       Regeneration;
	[SerializeField] private             bool       regeneration;
	[SerializeField] private             int        testFieldSize = 6;
	public static                        bool       TestModeBlock;
	[SerializeField] private             bool       testModeBlock;
	public static                        int        TestBlock;
	[SerializeField] private             int        testBlock = 3;
	private                              GameObject mainCameraObj;
	private                              Camera     mainCamera;
	private                              Transform  rotatorTr;
	private                              Quaternion mementoRotation;
	private                              bool       checkDir;
	private                              bool       dir;
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
	private Vector2 clickPos;

	[Space(20)] [Header("Camera Control")] [SerializeField]
	private float initialCameraRotationX = 15f;

	[SerializeField] private float cameraRotationConstraintX = 55f;
	[SerializeField] private float cameraSpeed               = 2000f;
	[SerializeField] private float cameraShakeAmount         = 0.5f;
	[SerializeField] private float cameraShakeTime           = 0.2f;
	private                  bool  isCameraShaking;
	private                  int   viewAngle;

	[Space(20)] [Header("Grid/Block")] [SerializeField]
	private int[] gridSize = { 10, 22, 10 };

	public static            GameGrid         Grid;
	private                  Vector3          startOffset;
	private static           BlockQueue       BlockQueue { get; set; }
	private                  Block            currentBlock;
	private                  Block            shadowBlock;
	private                  bool             canSaveBlock;
	private                  List<PrefabMesh> blockMeshList;
	private                  List<PrefabMesh> shadowMeshList;
	private                  List<PrefabMesh> gridMeshList;
	private                  List<LineMesh>   lineMeshList;
	[SerializeField] private float            blockSize    = 1.0f;
	[SerializeField] private float            downInterval = 1.0f;
	private                  List<bool>       keyUsing;
	private                  List<float>      keyIntervals;
	private const            float            defaultKeyInterval = 0.2f;
	private                  ParticleRender   rotationParticle;
	private const            string           vfxRotation = "Prefabs/VFX_Rotation";
	private                  float            lineGlowPower;

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

	private enum SFX_VALUE
	{
		BOOL = 0,
		CLICK,
		CLOSE,
		DROP1,
		DROP2,
		GAME_OVER,
		SHIFT,
		ITEM,
		ROTATE1,
		ROTATE2,
		SWITCH,
		UNAVAILABLE,
		HARD_DROP1,
		HARD_DROP2,
		HARD_DROP3,
		HARD_DROP4,
		HARD_DROP5,
		MOVE,
		PAUSE,
		CLEAR,
		TETRIS1,
		TETRIS2,
	}

	private readonly string[] BGM_PATH =
	{
		"BGM/BGM01",
		"BGM/BGM02",
		"BGM/BGM03",
		"BGM/BGM04",
		"BGM/BGM05",
		"BGM/BGM06",
		"BGM/BGM07",
		"BGM/BGM08",
		"BGM/BGM09",
		"BGM/BGM10",
		"BGM/BGM11",
		"BGM/BGM12",
	};

	private readonly string[] SFX_PATH =
	{
		"SFX/Bool",
		"SFX/Click",
		"SFX/Close",
		"SFX/Drop1",
		"SFX/Drop2",
		"SFX/GameOver",
		"SFX/Shift",
		"SFX/Item",
		"SFX/Rotate1",
		"SFX/Rotate2",
		"SFX/Switch",
		"SFX/Unavailable",
		"SFX/HardDrop01",
		"SFX/HardDrop02",
		"SFX/HardDrop03",
		"SFX/HardDrop04",
		"SFX/HardDrop05",
		"SFX/Move",
		"SFX/Pause",
		"SFX/Clear",
		"SFX/Tetris1",
		"SFX/Tetris2",
	};

	private                 List<Coroutine> logicList;
	private static          GameObject      blockObj;
	private static          GameObject      shadowObj;
	public static           GameObject      GridObj;
	private static          GameObject      effectObj;
	private static readonly int             alpha         = Shader.PropertyToID("_Alpha");
	private static readonly int             clear         = Shader.PropertyToID("_Clear");
	private static readonly int             emission      = Shader.PropertyToID("_Emission");
	private static readonly int             over          = Shader.PropertyToID("_GameOver");
	private static readonly int             smoothness    = Shader.PropertyToID("_Smoothness");
	private static readonly int             power         = Shader.PropertyToID("_Power");
	private static readonly int             color         = Shader.PropertyToID("_Color");
	private static readonly int             speed         = Shader.PropertyToID("_Speed");
	private static readonly int             gradientColor = Shader.PropertyToID("_GradientColor");
	private                 AudioSource     audioSourceBGM;
	private                 AudioSource[]   audioSourcesSFX;
	private                 List<AudioClip> bgmSource;
	private                 List<AudioClip> sfxSource;
	private                 int             sfxIdx;
	private const           float           bgmVolumeOrigin = 0.2f;
	private                 GameObject      cubeMeshes;
	private                 List<Transform> cubeTrs;
	private                 Animator[]      cubeAnimators;
	private                 Renderer[]      cubeRenderers;
	private const           int             totalAnim = 4;
	private static readonly int             phase     = Animator.StringToHash("Phase");
	private                 Coroutine       animFunc;

#endregion

#region MonoFunction

	private void Awake()
	{
		Init();

		SoundInit();

		BackgroundInit();
	}

	private void Init()
	{
		gameOver   = false;
		isPause    = false;
		TotalScore = 0;

		TestGrid      = testGrid;
		TestHeight    = testHeight;
		Regeneration  = regeneration;
		TestModeBlock = testModeBlock;
		TestBlock     = testBlock % Block.Type;

		GridObj   = GameObject.Find("Grid");
		blockObj  = GameObject.Find("Blocks");
		shadowObj = GameObject.Find("Shadow");
		effectObj = GameObject.Find("Effect");

		mainCameraObj                     = GameObject.Find("Main Camera");
		mainCameraObj!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		mainCamera                        = mainCameraObj.GetComponent<Camera>();
		rotatorTr                         = GameObject.Find("Rotator").GetComponent<Transform>();
		mementoRotation                   = Quaternion.identity;
		Dir                               = false;
		checkDir                          = false;
		viewAngle                         = 0;
		isCameraShaking                   = false;

		blockMeshList  = new List<PrefabMesh>();
		shadowMeshList = new List<PrefabMesh>();
		gridMeshList   = new List<PrefabMesh>();

		if (TestGrid)
		{
			gridSize[0] = testFieldSize;
			gridSize[2] = testFieldSize;
			Grid        = new GameGrid(ref gridSize, blockSize);
		}
		else
		{
			Grid = new GameGrid(ref gridSize, blockSize);
		}

		startOffset = new Vector3(-Grid.SizeX / 2f + blockSize / 2,
		                          Grid.SizeY  / 2f - blockSize / 2,
		                          -Grid.SizeZ / 2f + blockSize / 2);

		BlockQueue   = new BlockQueue();
		currentBlock = BlockQueue.GetAndUpdateBlock();
		canSaveBlock = true;

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
			2f,
		};

		logicList = new List<Coroutine>
		{
			StartCoroutine(BlockDown()),
			StartCoroutine(AngleCalculate()),
		};

		if (TestGrid) RenderGrid();

		rotationParticle = null;

		lineMeshList = new List<LineMesh>();
		RenderLine();
		lineGlowPower = lineMeshList[0].Renderer.material.GetFloat(power);
	}

	private void SoundInit()
	{
		audioSourceBGM             = mainCameraObj.AddComponent<AudioSource>();
		audioSourceBGM.playOnAwake = true;
		audioSourceBGM.loop        = false;
		audioSourceBGM.volume      = bgmVolumeOrigin;
		bgmSource                  = new List<AudioClip>();

		foreach (string path in BGM_PATH)
		{
			bgmSource.Add(Resources.Load<AudioClip>(path));
		}
		
		RandomPlayBGM();
		
		audioSourcesSFX = GridObj.GetComponentsInChildren<AudioSource>();
		sfxIdx          = -1;

		sfxSource = new List<AudioClip>();

		foreach (string path in SFX_PATH)
		{
			sfxSource.Add(Resources.Load<AudioClip>(path));
		}
	}

	private void BackgroundInit()
	{
		cubeMeshes    = GameObject.Find("Meshes");
		cubeTrs       = new List<Transform>();
		cubeAnimators = cubeMeshes.GetComponentsInChildren<Animator>();
		cubeRenderers = cubeMeshes.GetComponentsInChildren<Renderer>();

		Transform[] trs = cubeMeshes.GetComponentsInChildren<Transform>();

		foreach (Transform tr in trs)
		{
			if (tr.parent != cubeMeshes.transform) continue;

			cubeTrs.Add(tr);
		}

		foreach (Transform tr in cubeTrs)
		{
			float randFloat = Random.Range(0f, 360f);

			tr.rotation = Quaternion.Euler(0f, randFloat, 0f);

			randFloat = Random.Range(0.3f, 1.5f);

			tr.localScale = Vector3.one * randFloat;
		}

		foreach (Animator animator in cubeAnimators)
		{
			float randFloat = Random.Range(0.3f, 1f);
			int   randInt   = Mathf.Clamp(Random.Range(-2, totalAnim), 0, totalAnim - 1);

			animator.speed = randFloat;
			animator.SetInteger(phase, randInt);

			if (randInt != 0)
			{
				animator.gameObject.transform.rotation *= Quaternion.Euler(Random.Range(0f, 360f), 0f,
				                                                           Random.Range(0f, 360f));
			}
		}

		foreach (Renderer rd in cubeRenderers)
		{
			rd.material.SetFloat(speed, Random.Range(0.15f, 0.45f));
		}
		
		animFunc = StartCoroutine(AnimChange());
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
				isPause = true;
				PauseBGM(3f);
				GamePause();
				keyUsing[(int)KEY_VALUE.ESC] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
			}

		#endregion

#endif
			if (!audioSourceBGM.isPlaying && !gameOver)
			{
				RandomPlayBGM();
			}
		}

		else if (isPause)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

			if (Input.GetKey(KeyCode.Escape) && !keyUsing[(int)KEY_VALUE.ESC])
			{
				GameResume();
				ResumeBGM(3f);
				isPause                      = false;
				keyUsing[(int)KEY_VALUE.ESC] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
			}

#endif
		}
	}

#endregion

#region GameControl

	private void GamePause()
	{
		StartCoroutine(PlaySfx(SFX_VALUE.PAUSE));

		foreach (Coroutine coroutine in logicList)
		{
			StopCoroutine(coroutine);
		}
	}

	private void GameResume()
	{
		logicList.Add(StartCoroutine(BlockDown()));
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

			if (rotationParticle != null)
				rotationParticle.Obj.transform.position -= Vector3.up;

			yield return new WaitForSeconds(downInterval);
		}
	}

	private IEnumerator KeyRewind(int id)
	{
		yield return new WaitForSeconds(keyIntervals[id]);

		keyUsing[id] = false;
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

	private static bool IsGameOver()
	{
		return !Grid.IsPlaneEmpty(0);
	}

	private void PlaceBlock()
	{
		foreach (Coord coord in currentBlock.TilePositions())
		{
			Grid[coord.X, coord.Y, coord.Z] = currentBlock.GetId();
		}

		List<int> cleared = Grid.ClearFullRows();
		ScoreCalc(cleared.Count);

		StartCoroutine(ClearEffect(cleared));

		if (cleared.Count == 4)
		{
			StartCoroutine(PlaySfx(SFX_VALUE.TETRIS1));
			StartCoroutine(PlaySfx(SFX_VALUE.TETRIS2));

			StartCoroutine(CameraFOVEffect());
		}
		else if (cleared.Count > 0)
		{
			StartCoroutine(PlaySfx(SFX_VALUE.CLEAR));
		}

		RenderGrid();

		cleared.Clear();

		if (IsGameOver())
		{
			StartCoroutine(PlaySfx(SFX_VALUE.GAME_OVER));

			isPause  = true;
			gameOver = true;
			
			StopCoroutine(animFunc);
			StartCoroutine(PitchDownBGM(0.2f));
			StartCoroutine(GameOverEffect());
			StartCoroutine(AnimStop());
		}
		else
		{
			canSaveBlock = true;
			currentBlock = BlockQueue.GetAndUpdateBlock();
			RefreshCurrentBlock();
		}
	}

	private void ScoreCalc(int cleared)
	{
		if (cleared == 0) return;

		TotalScore += baseScore * scoreValue[cleared - 1];
	}

#endregion

#region CameraControl

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
		isCameraShaking = true;

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

		isCameraShaking = false;
	}

#endregion

#region BlockRotation

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
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

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
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                              = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                              = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                              = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                              = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

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
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

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
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                              = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                              = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                              = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                              = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYClockWise()
	{
		currentBlock.RotateYClockWise();

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.RotateYCounterClockWise();
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);
			Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYCounterClockWise()
	{
		currentBlock.RotateYCounterClockWise();

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.RotateYClockWise();
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);
			Quaternion rotation = Quaternion.identity;

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

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
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

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
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                              = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                              = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                              = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                              = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

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
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

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
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                              = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                              = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                              = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                              = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle                      = new ParticleRender(vfxRotation, offset, rotation);
					rotationParticle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

			RefreshCurrentBlock();
		}
	}

#endregion

#region BlockMove

	private void MoveBlockLeft()
	{
		currentBlock.Move(Coord.Left[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.Move(Coord.Right[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockRight()
	{
		currentBlock.Move(Coord.Right[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.Move(Coord.Left[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockForward()
	{
		currentBlock.Move(Coord.Forward[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.Move(Coord.Backward[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockBackward()
	{
		currentBlock.Move(Coord.Backward[viewAngle]);

		if (!BlockFits(currentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			currentBlock.Move(Coord.Forward[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

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

		PlayRandomSfx(SFX_VALUE.DROP1, SFX_VALUE.DROP2);

		currentBlock.Move(Coord.Up);
		PlaceBlock();
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
		{
			PlayRandomSfx(SFX_VALUE.HARD_DROP1, SFX_VALUE.HARD_DROP5);

			StartCoroutine(CameraShake());
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.DROP1, SFX_VALUE.DROP2);
		}

		currentBlock.Move(Coord.Up);
		PlaceBlock();
	}

#endregion

#region Effect

	private IEnumerator CameraFOVEffect()
	{
		const float target      = 120f;
		float       originFOV   = mainCamera.fieldOfView;
		float       originSpeed = Grid.Mesh.MRenderer.material.GetFloat(speed);

		isPause = true;
		Grid.Mesh.MRenderer.material.SetFloat(speed, 10f);

		while (mainCamera.fieldOfView < target - 1f)
		{
			mainCamera.fieldOfView =  Mathf.Lerp(mainCamera.fieldOfView, target, 0.1f);
			audioSourceBGM.pitch   -= 0.02f;

			yield return new WaitForSeconds(0.01f);
		}

		while (mainCamera.fieldOfView > originFOV + 1f)
		{
			mainCamera.fieldOfView =  Mathf.Lerp(mainCamera.fieldOfView, originFOV, 0.2f);
			audioSourceBGM.pitch   += 0.02f;

			yield return new WaitForSeconds(0.01f);
		}

		isPause = false;

		mainCamera.fieldOfView = originFOV;
		Grid.Mesh.MRenderer.material.SetFloat(speed, originSpeed);
		audioSourceBGM.pitch = 1f;
	}

	private IEnumerator GridEffect()
	{
		const float   alphaUnit = 0.01f;
		float         alphaSet  = Grid.Mesh.MRenderer.material.GetFloat(alpha) + alphaUnit;
		Vector3       targetLoc = Grid.Mesh.Obj.transform.position             - Vector3.up    * 5f;
		float         glowSet   = lineGlowPower                                + lineGlowPower * 0.01f;
		const float   range     = 0.15f;
		List<Vector3> listRd    = new();

		for (int i = 0; i < 24; ++i)
		{
			listRd.Add(new Vector3(Random.Range(-range, range),
			                       Random.Range(-range, range),
			                       Random.Range(-range, range)));
		}

		while ((Grid.Mesh.Obj.transform.position - targetLoc).magnitude > 0.001f)
		{
			alphaSet -= 0.01f;
			glowSet  -= lineGlowPower * 0.01f;

			Grid.Mesh.Obj.transform.position = Vector3.Lerp(Grid.Mesh.Obj.transform.position, targetLoc, 0.02f);
			Grid.Mesh.MRenderer.material.SetFloat(alpha, Mathf.Max(alphaSet, 0f));

			for (int i = 0; i < lineMeshList.Count; ++i)
			{
				lineMeshList[i].Renderer.material.SetFloat(power, glowSet);
				lineMeshList[i].Renderer.material.SetFloat(alpha, alphaSet);
				lineMeshList[i].Renderer.SetPosition(0, lineMeshList[i].Renderer.GetPosition(0) +
				                                        listRd[i * 2]);
				lineMeshList[i].Renderer.SetPosition(1, lineMeshList[i].Renderer.GetPosition(1) +
				                                        listRd[i * 2 + 1]);
			}

			yield return new WaitForSeconds(0.02f);
		}

		Destroy(Grid.Mesh.Obj);

		foreach (LineMesh mesh in lineMeshList)
		{
			Destroy(mesh.Obj);
		}
	}

	private IEnumerator GameOverEffect()
	{
		ClearCurrentBlock();
		ClearShadowBlock();

		const float explosionForce  = 200f;
		float       explosionRadius = Grid.SizeY;
		const float torque          = 50f;

		foreach (PrefabMesh mesh in gridMeshList)
		{
			Rigidbody rb = mesh.Obj.AddComponent<Rigidbody>();

			rb.AddExplosionForce(explosionForce,
			                     new Vector3(0f, startOffset.y - mesh.Pos.Y - blockSize / 2f, 0f),
			                     explosionRadius);

			Vector3 rdVec = new(Random.Range(-torque, torque), Random.Range(-torque, torque),
			                    Random.Range(-torque, torque));
			rb.AddTorque(rdVec);
			rb.angularDrag = Random.Range(0.5f, 2f);

			mesh.Renderer.material.SetFloat(over,       1f);
			mesh.Renderer.material.SetFloat(smoothness, 0f);
		}

		StartCoroutine(GridEffect());

		float alphaSet = 1.01f;

		while (alphaSet > 0f)
		{
			alphaSet -= 0.01f;

			foreach (PrefabMesh mesh in gridMeshList)
			{
				mesh.Renderer.material.SetFloat(alpha, alphaSet);
			}

			yield return new WaitForSeconds(0.02f);
		}

		foreach (PrefabMesh mesh in gridMeshList)
		{
			Destroy(mesh.Obj);
		}
	}

	private IEnumerator ClearEffect(List<int> cleared)
	{
		List<PrefabMesh> clearMeshList   = new();
		const float      explosionForce  = 900f;
		float            explosionRadius = Grid.SizeX + Grid.SizeZ;
		const float      explosionUp     = 5f;
		const float      torque          = 100f;

		foreach (int height in cleared)
		{
			for (int i = 0; i < Grid.SizeX; ++i)
			{
				for (int j = 0; j < Grid.SizeZ; ++j)
				{
					Vector3 offset = new(i, -height, j);
					PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset, Block.MatPath[^1],
					                      new Coord(i, height, j), ShadowCastingMode.Off);
					mesh.Renderer.material.SetFloat(clear,    1f);
					mesh.Renderer.material.SetFloat(color,    Random.Range(0f, 1f));
					mesh.Renderer.material.SetFloat(emission, 10f);

					Rigidbody rb = mesh.Obj.AddComponent<Rigidbody>();


					rb.AddForce(Physics.gravity * 40f, ForceMode.Acceleration);
					rb.AddForce(new Vector3(0f, Random.Range(-explosionUp, explosionUp), 0f),
					            ForceMode.Impulse);
					rb.AddExplosionForce(explosionForce,
					                     new Vector3(0f, startOffset.y - height, 0f),
					                     explosionRadius);

					Vector3 rdVec = new(Random.Range(-torque, torque), Random.Range(-torque, torque),
					                    Random.Range(-torque, torque));
					rb.AddTorque(rdVec);
					rb.angularDrag = Random.Range(0.5f, 2f);

					clearMeshList.Add(mesh);
					mesh.Obj.transform.parent = effectObj.transform;
				}
			}
		}

		float alphaSet    = 1.02f;
		float emissionSet = 2.1f;

		while (alphaSet > 0)
		{
			alphaSet    -= 0.02f;
			emissionSet =  emissionSet > 0 ? emissionSet - 0.1f : 0f;

			foreach (PrefabMesh mesh in clearMeshList)
			{
				mesh.Obj.transform.localScale *= 1.02f;
				mesh.Renderer.material.SetFloat(alpha,    alphaSet);
				mesh.Renderer.material.SetFloat(emission, emissionSet);
			}

			yield return new WaitForSeconds(0.02f);
		}

		foreach (PrefabMesh mesh in clearMeshList)
		{
			Destroy(mesh.Obj);
		}
	}

#endregion

#region Render

	private void RenderLine()
	{
		const float width = 0.05f;

		LineMesh mesh01 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX  / 2f, -Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh01);

		LineMesh mesh02 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX  / 2f, Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh02);

		LineMesh mesh03 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY  / 2f, Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh03);

		LineMesh mesh04 = new("Line", GridObj,
		                      new Vector3(Grid.SizeX / 2f, -Grid.SizeY / 2f, Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX / 2f, Grid.SizeY  / 2f, Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh04);

		LineMesh mesh05 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX  / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh05);

		LineMesh mesh06 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX  / 2f, Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh06);

		LineMesh mesh07 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY  / 2f, -Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh07);

		LineMesh mesh08 = new("Line", GridObj,
		                      new Vector3(Grid.SizeX / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX / 2f, Grid.SizeY  / 2f, -Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh08);

		LineMesh mesh09 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(-Grid.SizeX / 2f, -Grid.SizeY / 2f, Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh09);

		LineMesh mesh10 = new("Line", GridObj,
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(-Grid.SizeX / 2f, Grid.SizeY / 2f, Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh10);

		LineMesh mesh11 = new("Line", GridObj,
		                      new Vector3(Grid.SizeX / 2f, -Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX / 2f, -Grid.SizeY / 2f, Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh11);

		LineMesh mesh12 = new("Line", GridObj,
		                      new Vector3(Grid.SizeX / 2f, Grid.SizeY / 2f, -Grid.SizeZ / 2f),
		                      new Vector3(Grid.SizeX / 2f, Grid.SizeY / 2f, Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh12);
	}

	private void RenderCurrentBlock()
	{
		ClearCurrentBlock();

		foreach (Coord coord in currentBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset,
			                      Block.MatPath[currentBlock.GetId()], coord, ShadowCastingMode.Off);

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
			PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset,
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
						PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset, Block.MatPath[^1],
						                      new Coord(j, i, k), ShadowCastingMode.Off);
						mesh.Renderer.material.SetFloat(gradientColor, (float)i / (Grid.SizeY - 1));

						gridMeshList.Add(mesh);
						mesh.Obj.transform.parent = GridObj.transform;
					}
				}
			}
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

#endregion

#region UI
	
	
	private void InitUI()
	{
		
	}

#endregion

#region Sound

	private void RandomPlayBGM()
	{
		audioSourceBGM.clip = bgmSource[Random.Range(1, bgmSource.Count)];
		audioSourceBGM.Play();
	}

	private IEnumerator PitchDownBGM(float acc)
	{
		const float volDown   = 0.01f;
		const float pitchDown = 0.01f;

		while (audioSourceBGM.volume > 0f)
		{
			audioSourceBGM.volume -= volDown   * acc;
			audioSourceBGM.pitch  -= pitchDown * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.Stop();
		audioSourceBGM.volume = bgmVolumeOrigin;
		audioSourceBGM.pitch  = 1f;
	}

	private IEnumerator FadeOutBGM(float acc)
	{
		const float volDown = 0.01f;

		while (audioSourceBGM.volume > 0f)
		{
			audioSourceBGM.volume -= volDown * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.volume = 0f;
		audioSourceBGM.Pause();
	}

	private IEnumerator FadeInBGM(float acc)
	{
		const float volUp = 0.01f;

		audioSourceBGM.Play();

		while (audioSourceBGM.volume < bgmVolumeOrigin)
		{
			audioSourceBGM.volume += volUp * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.volume = bgmVolumeOrigin;
	}

	private void PauseBGM(float acc)
	{
		StartCoroutine(FadeOutBGM(acc));
	}

	private void ResumeBGM(float acc)
	{
		StartCoroutine(FadeInBGM(acc));
	}

	private IEnumerator PlaySfx(SFX_VALUE value)
	{
		sfxIdx = Mathf.Clamp(sfxIdx + 1, 0, audioSourcesSFX.Length - 1);
		audioSourcesSFX[sfxIdx].PlayOneShot(sfxSource[(int)value]);

		yield return new WaitForSeconds(1f);

		--sfxIdx;
	}

	private void PlayRandomSfx(SFX_VALUE start, SFX_VALUE end)
	{
		int rand = Random.Range((int)start, (int)end + 1);

		StartCoroutine(PlaySfx((SFX_VALUE)rand));
	}

#endregion

#region Background

	private IEnumerator AnimChange()
	{
		while (true)
		{
			int   randObj   = Random.Range(0,    cubeAnimators.Length);
			int   randInt   = Random.Range(0,    totalAnim);
			float randFloat = Random.Range(0.5f, 1f);

			cubeAnimators[randObj].SetInteger(phase, randInt);
			cubeAnimators[randObj].speed = randFloat;

			yield return new WaitForSeconds(1.0f);
		}
	}

	private IEnumerator AnimStop()
	{
		const float slowDown = 0.01f;

		while (cubeAnimators[0].speed > 0.01f)
		{
			foreach (Animator anim in cubeAnimators)
			{
				anim.speed = Mathf.Clamp(anim.speed - slowDown, 0f, 1f);
			}

			yield return new WaitForSeconds(0.1f);
		}

		foreach (Animator anim in cubeAnimators)
		{
			anim.speed = 0f;
		}
	}

#endregion
}