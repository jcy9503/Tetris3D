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
using UnityEngine.UI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using System.Xml.Serialization;
using TMPro;
using System.Linq.Expressions;
using System.Reflection;
using System;
using Unity.VisualScripting;

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
	private const                        int        baseScore = 100;
	public static                        int        Score;
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
	private                              GameObject mainCamera;
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
	private static readonly  int              alpha        = Shader.PropertyToID("_Alpha");
	private static readonly  int              clear        = Shader.PropertyToID("_Clear");
	private static readonly  int              color        = Shader.PropertyToID("_Color");
	private static readonly  int              emission     = Shader.PropertyToID("_Emission");
	private static readonly  int              over         = Shader.PropertyToID("_GameOver");
	private static readonly  int              smoothness   = Shader.PropertyToID("_Smoothness");
	private static readonly  int              sortingOrder = Shader.PropertyToID("_QueueOffset");
	private                  bool             canSaveBlock;
	private                  List<PrefabMesh> blockMeshList;
	private                  List<PrefabMesh> shadowMeshList;
	private                  List<PrefabMesh> gridMeshList;
	[SerializeField] private float            blockSize    = 1.0f;
	[SerializeField] private float            downInterval = 1.0f;
	private                  List<bool>       keyUsing;
	private                  List<float>      keyIntervals;
	private const            float            defaultKeyInterval = 0.2f;
	private                  ParticleRender   particle;
	private const            string           vfxRotation = "Prefabs/VFX_Rotation";
	private                  Renderer         renderTopCloud;
	private                  Renderer         renderBottomCloud;

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
	private static GameObject      effectObj;



    private Button pauseHomeBtn;
	private RectTransform curPlay;
	private RectTransform curMain;
	private RectTransform curOption;
	private RectTransform curLeader;


    private			Button pauseBtn;
	private			Image pauseScreen;
	private			Button resumePlay;
	private			Image arrowUp;
	private			Image arrowDown;
	private			Image arrowLeft;
	private			Image arrowRight;
	private			Image xRotation;
	private			Image xIRotation;
	private			Image yRotation;
	private			Image yIRotation;
	private			Image zRotation;
	private			Image zIRotation;
    private			Text curScore;

	private			Image gameoverScreen;
	private Text gameoverText;
	private RectTransform Anchor1;
    private RectTransform Anchor2;
    private			Button retryButton;
	private			Button homeButton;
	
	private			Text finalScore;


	private			Image mainTitle;
	private			Image mainImage;
	private			Button gameStart;
	private			Button optionBtn;
	private			Button leaderBoard;
	private			Button quitBtn;
	private			Image quitMsg;
	private			Button quitYes;
	private			Button quitNo;


	private RectTransform soundPanel;
    private RectTransform graphicPanel;
    private RectTransform controlPanel;
    private			TextMeshProUGUI optionTitle;
	private string[] optionTitles = { "Sound", "Graphics", "Controls"  };
    private			Button soundTab;
	private			Slider bgmSlider;
	private			Slider sfxSlider;
	private			float bgmVolume = 1.0f;
	private			float sfxVolume = 1.0f;

	private			Button graphicTab;
    private			TextMeshProUGUI blkOption;
	private			string blkOption_Mono = "Mono";
	private			string blkOption_Color = "Color";
    private			Image btnOptionImg;
	private			Button blkOptionBtn;
	private			Button infoBtn;

    private			Button controlTab;
	private			Button btnMode;
	private			Button destroyOnoff;
	private Image destroyCheck;
	private			Button rotationOnoff;
    private Image rotationCheck;


    private			TextMeshProUGUI firstG;
	private			TextMeshProUGUI secondG;
	private			TextMeshProUGUI thirdG;
	private			Image cloneG;
	

#endregion

#region MonoFunction

	private void Awake()
	{
		gameOver = false;
		isPause  = false;
		Score    = 0;

		TestGrid      = testGrid;
		TestHeight    = testHeight;
		Regeneration  = regeneration;
		TestModeBlock = testModeBlock;
		TestBlock     = testBlock % Block.Type;

		GridObj   = GameObject.Find("Grid");
		blockObj  = GameObject.Find("Blocks");
		shadowObj = GameObject.Find("Shadow");
		effectObj = GameObject.Find("Effect");

		mainCamera                     = GameObject.Find("Main Camera");
		mainCamera!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		rotatorTr                      = GameObject.Find("Rotator").GetComponent<Transform>();
		mementoRotation                = Quaternion.identity;
		Dir                            = false;
		checkDir                       = false;
		viewAngle                      = 0;
		isCameraShaking                = false;

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
			defaultKeyInterval,
		};

		logicMethods = BlockDown;

		logicList = new List<Coroutine>
		{
			StartCoroutine(logicMethods()),
			StartCoroutine(AngleCalculate()),
		};

		if (TestGrid) RenderGrid();

		particle          = null;
		renderTopCloud    = GameObject.Find("CloudTop").GetComponent<Renderer>();
		renderBottomCloud = GameObject.Find("CloudBottom").GetComponent<Renderer>();

       
        #region MainScreen
        curPlay = GameObject.Find("PlayScreen").GetComponent<RectTransform>();
        curMain = GameObject.Find("MainScreen").GetComponent<RectTransform>();
        curOption = GameObject.Find("OptionScreen").GetComponent<RectTransform>();
        curLeader = GameObject.Find("LeaderBoard").GetComponent<RectTransform>();

        mainTitle = GameObject.Find("Title").GetComponent<Image>(); 
		mainImage = GameObject.Find("Main_Castle_Glow").GetComponent<Image>();
		gameStart = GameObject.Find("Game_Start").AddComponent<Button>();
        optionBtn = GameObject.Find("Option").AddComponent<Button>(); 
        leaderBoard = GameObject.Find("Leader_Board").AddComponent<Button>();
        quitBtn = GameObject.Find("Quit").AddComponent<Button>(); 
		quitYes = GameObject.Find("Quit_Yes").AddComponent<Button>();
        quitNo = GameObject.Find("Quit_No").AddComponent<Button>();
        quitMsg = GameObject.Find("Quit_Panel").GetComponent<Image>();
        #endregion
		#region PlayScreen
        pauseBtn = GameObject.Find("Pause").GetComponent<Button>();
        pauseScreen = GameObject.Find("PauseScreen").GetComponent<Image>();
        pauseHomeBtn = GameObject.Find("Pause_Home").GetComponent<Button>();
        resumePlay = GameObject.Find("Pause_Resume").GetComponent<Button>();
        

        arrowUp = GameObject.Find("Arrow_Up").GetComponent<Image>(); new UIElement(arrowUp);
        arrowDown = GameObject.Find("Arrow_Down").GetComponent<Image>(); new UIElement(arrowDown);
        arrowRight = GameObject.Find("Arrow_Right").GetComponent<Image>(); new UIElement(arrowRight);
        arrowLeft = GameObject.Find("Arrow_Left").GetComponent<Image>(); new UIElement(arrowLeft);

        xRotation = GameObject.Find("X_ClockWise").GetComponent<Image>(); new UIElement(xRotation);
        xIRotation = GameObject.Find("X_R_ClockWise").GetComponent<Image>(); new UIElement(xIRotation);
        yRotation = GameObject.Find("Y_ClockWise").GetComponent<Image>(); new UIElement(yRotation);
        yIRotation = GameObject.Find("Y_R_ClockWise").GetComponent<Image>(); new UIElement(yIRotation);
        zRotation = GameObject.Find("Z_ClockWise").GetComponent<Image>(); new UIElement(zRotation);
        zIRotation = GameObject.Find("Z_R_ClockWise").GetComponent<Image>(); new UIElement(zIRotation);

		gameoverScreen = GameObject.Find("GameOver").GetComponent<Image>();
		Anchor1 = GameObject.Find("Anchor1").GetComponent<RectTransform>();
        Anchor2 = GameObject.Find("Anchor2").GetComponent<RectTransform>();
        retryButton = GameObject.Find("Retry").AddComponent<Button>();
        homeButton = GameObject.Find("Home").GetComponent<Button>();
		#endregion

		soundPanel = GameObject.Find("SoundBtns").GetComponent<RectTransform>();
        graphicPanel = GameObject.Find("GraphicBtns").GetComponent<RectTransform>();
        controlPanel = GameObject.Find("ControlBtns").GetComponent<RectTransform>();

        optionTitle = GameObject.Find("Option_Title").GetComponent<TextMeshProUGUI>();
		soundTab = GameObject.Find("Sound_Tab").GetComponent<Button>();
		soundTab.onClick.AddListener(openSoundTab);
		
        bgmSlider = GameObject.Find("BGM_Slider").GetComponent<Slider>(); bgmSlider.value = bgmVolume;
		
        sfxSlider = GameObject.Find("SFX_Slider").GetComponent<Slider>(); sfxSlider.value = sfxVolume;


        graphicTab = GameObject.Find("Graphic_Tab").GetComponent<Button>();
		graphicTab.onClick.AddListener(openGraphicTab);
        destroyCheck = GameObject.Find("Destroy_Check").GetComponent<Image>();
        rotationCheck = GameObject.Find("Rotation_Check").GetComponent<Image>();
		destroyOnoff = GameObject.Find("Destroy_Effect_Box").GetComponent<Button>();
		destroyOnoff.onClick.AddListener(()=> imageOnOff(destroyCheck));
		rotationOnoff = GameObject.Find("Rotation_Effect_Box").GetComponent<Button>();
        rotationOnoff.onClick.AddListener(() => imageOnOff(rotationCheck));

        blkOption = GameObject.Find("ColorChange_Text").GetComponent<TextMeshProUGUI>(); blkOption.text = blkOption_Color;
		blkOptionBtn = GameObject.Find("ColorChange_Handle").GetComponent<Button>();
		
		btnOptionImg = GameObject.Find("ColorChange").GetComponent<Image>();

        controlTab = GameObject.Find("Control_Tab").GetComponent<Button>();
		controlTab.onClick.AddListener(openControlTab);

		leaderBoard.onClick.AddListener(() => moveScreen(curMain, curLeader));
		cloneG = GameObject.Find("Boards").GetComponent<Image>();

		initializeMain();
		initializePlay();
		initializeOption();
		initializeLeader();
		initializeUI();
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
				StartCoroutine(BtnClick(arrowLeft));
			}

			if (Input.GetKey(KeyCode.D) && !keyUsing[(int)KEY_VALUE.RIGHT])
			{
				MoveBlockRight();
				keyUsing[(int)KEY_VALUE.RIGHT] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.RIGHT));
                StartCoroutine(BtnClick(arrowRight));
            }

			if (Input.GetKey(KeyCode.W) && !keyUsing[(int)KEY_VALUE.UP])
			{
				MoveBlockForward();
				keyUsing[(int)KEY_VALUE.UP] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.UP));
                StartCoroutine(BtnClick(arrowUp));
            }

			if (Input.GetKey(KeyCode.S) && !keyUsing[(int)KEY_VALUE.DOWN])
			{
				MoveBlockBackward();
				keyUsing[(int)KEY_VALUE.DOWN] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.DOWN));
                StartCoroutine(BtnClick(arrowDown));
            }

			if ((Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.Keypad7)) && !keyUsing[(int)KEY_VALUE.ROTATE_X])
			{
				RotateBlockXCounterClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_X] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_X));
                StartCoroutine(BtnClick(xRotation));
            }

			if ((Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Keypad8)) && !keyUsing[(int)KEY_VALUE.ROTATE_X_INV])
			{
				RotateBlockXClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_X_INV] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_X_INV));
                StartCoroutine(BtnClick(xIRotation));
            }

			if ((Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad4)) && !keyUsing[(int)KEY_VALUE.ROTATE_Y])
			{
				RotateBlockYClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_Y] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Y));
                StartCoroutine(BtnClick(yRotation));
            }

			if ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad5)) && !keyUsing[(int)KEY_VALUE.ROTATE_Y_INV])
			{
				RotateBlockYCounterClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_Y_INV] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Y_INV));
                StartCoroutine(BtnClick(yIRotation));
            }

			if ((Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Keypad1)) && !keyUsing[(int)KEY_VALUE.ROTATE_Z])
			{
				RotateBlockZCounterClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_Z] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Z));
                StartCoroutine(BtnClick(zRotation));
            }

			if ((Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Keypad2)) &&
			    !keyUsing[(int)KEY_VALUE.ROTATE_Z_INV])
			{
				RotateBlockZClockWise();
				keyUsing[(int)KEY_VALUE.ROTATE_Z_INV] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ROTATE_Z_INV));
                StartCoroutine(BtnClick(zIRotation));
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
				StartCoroutine(BtnClick(pauseBtn.image));
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

	#region Effect

		if (shadowMeshList.Count > 0)
		{
			foreach (PrefabMesh mesh in shadowMeshList)
			{
				mesh.Renderer.material.SetFloat(alpha, Mathf.PingPong(Time.time, 0.7f) + 0.15f);
			}
		}

		float col = Mathf.PingPong(Time.time * 0.1f, 2f);

		renderTopCloud.material.SetFloat(color, col);
		renderBottomCloud.material.SetFloat(color, col);
		Grid.Mesh.MRenderer.material.SetFloat(color, col);

	#endregion
	}

#endregion

#region GameControl

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

			if (particle != null)
				particle.Obj.transform.position -= Vector3.up;

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

		StartCoroutine(ClearEffect(cleared));

		RenderGrid();

		cleared.Clear();

		if (IsGameOver())
		{
			isPause  = true;
			gameOver = true;

			StartCoroutine(GameOverEffect());
		}
		else
		{
			canSaveBlock = true;
			currentBlock = BlockQueue.GetAndUpdateBlock();
			RefreshCurrentBlock();
		}
	}

    private IEnumerator BtnClick(Image image)
    {
        float interval = 0.1f;
        Color originColor = image.color;
        Color pressed = new Color(255, 255, 255, 0.2f);
		Vector3 trans = new Vector3(0.01f, 0.01f);
		Vector3 originSize = image.transform.localScale;
		while (true)
		{
            image.transform.localScale = Vector3.Slerp(image.transform.localScale, trans, interval);
            image.color = Color.Lerp(image.color, pressed, interval);

            yield return new WaitForSeconds(interval);

			image.transform.localScale = originSize;
            image.color = originColor;

            yield break;
        }
    }
	private IEnumerator ImgMove(Transform image, Vector3 targetPos)
	{
		float interval = 0.1f;
		Vector3 originPos = image.transform.localPosition;

		float comparePos = (image.transform.localPosition.x + targetPos.x) - targetPos.x;

		if (comparePos >= targetPos.x)
		{
			while (true)
			{
				image.transform.localPosition -= Vector3.Slerp(image.transform.localPosition, originPos, interval);

				yield break;
			}
		}
		else
		{
            while (true)
            {
                image.transform.localPosition += Vector3.Slerp(originPos, targetPos, interval);

                yield break;
            }
        }
	}
	public IEnumerator LerpImage(Image image)
	{
		Color originAlpha = new Color(255f, 255f, 255f, 1f);
		Color lowAlpha = new Color (255f, 255f, 255f, 0f);
		float interval = 0.1f;

		while (true)
		{
			image.color = Color.Lerp(image.color, lowAlpha, (interval + 0.4f) * Time.deltaTime);

			yield return new WaitForSeconds(interval);

			image.color = Color.Lerp(image.color, originAlpha, (interval + 0.4f) * Time.deltaTime);
			
			yield break;
		}
	}
	public void FadeinOut(RectTransform fadeOut, RectTransform fadeIn)
	{
		while (true)
		{
            Screen.brightness -= 0.1f * Time.deltaTime;
            if (Screen.brightness == 0f) break;
        }
        fadeOut.gameObject.SetActive(false);
        while (true)
		{
			fadeIn.gameObject.SetActive(true);
			Screen.brightness += 0.1f * Time.deltaTime;
			if (Screen.brightness == 1f) break;
        }
	}
	//public IEnumerator MoveScreen(RectTransform curScreen, RectTransform nextScreen)
	//{
 //       float interval = 0.1f;
 //       float loading = 0.3f;
 //       while (true)
	//	{
 //           Screen.brightness -= loading * Time.deltaTime;
	//		if (Screen.brightness == 0f)
	//		{
	//			curScreen.gameObject.SetActive(false);
	//			break;
	//		}
 //       }

	//	nextScreen.gameObject.SetActive(true);

	//	yield return new WaitForSeconds(interval);
		
	//	while (true)
	//	{
	//		Screen.brightness += loading * Time.deltaTime;
	//		if (Screen.brightness == 1f) yield break;
	//	}
	//}
	public void moveScreen(RectTransform curScreen, RectTransform nextScreen)
	{
		curScreen.gameObject.SetActive(false);
		nextScreen.gameObject.SetActive(true);
	}

	private void volumeControl(float value, Slider slider)
	{
		slider.value = value;
	}
	private void openSoundTab()
	{
		soundPanel.gameObject.SetActive(true);
		graphicPanel.gameObject.SetActive(false);
        controlPanel.gameObject.SetActive(false);
    }
	private void openGraphicTab()
	{
        soundPanel.gameObject.SetActive(false);
        graphicPanel.gameObject.SetActive(true);
        controlPanel.gameObject.SetActive(false);
    }
	private void openControlTab()
	{
        soundPanel.gameObject.SetActive(false);
        graphicPanel.gameObject.SetActive(false);
        controlPanel.gameObject.SetActive(true);
    }
	private void cloneGrades(Image Boards)
	{
		Vector3 pos = new Vector3(0, -10f);
		for (int i = 0; i < 7; i++)
		{
            Instantiate(Boards, pos, Quaternion.identity);
        }
		
	}
	private void imageOnOff(Image blink)
	{
		blink.enabled = !blink.enabled;
	}
    public void initializeUI()
    {
		initializeMain();
        curPlay.gameObject.SetActive(false);
        curOption.gameObject.SetActive(false);
        curLeader.gameObject.SetActive(false);
    }

    public void initializePlay()
	{
		pauseBtn.onClick.AddListener(delegate { pauseScreen.gameObject.SetActive(true); });
		pauseBtn.onClick.AddListener(delegate { GamePause(); });
		pauseHomeBtn.onClick.AddListener(delegate { moveScreen(curPlay, curMain); });
		resumePlay.onClick.AddListener(delegate { pauseScreen.gameObject.SetActive(false); });
		retryButton.onClick.AddListener(delegate { initializePlay(); });
		homeButton.onClick.AddListener(delegate { moveScreen(curPlay, curMain); });

		if (gameOver)
		{ 
			gameoverScreen.gameObject.SetActive(true);
			ImgMove(gameoverText.transform, Anchor1.transform.position);
			ImgMove(homeButton.transform, Anchor2.transform.position);
			ImgMove(retryButton.transform, Anchor2.transform.position);
		}

		pauseScreen.gameObject.SetActive(false);
		gameoverScreen.gameObject.SetActive(false);
	}
	public void initializeMain()
	{
		gameStart.onClick.AddListener(delegate { moveScreen(curMain, curPlay); });
		optionBtn.onClick.AddListener(delegate { moveScreen(curMain, curOption); });
		leaderBoard.onClick.AddListener(delegate { moveScreen(curMain, curPlay); });
        quitBtn.onClick.AddListener(delegate { quitMsg.gameObject.SetActive(true); });

        quitYes.onClick.AddListener(delegate { Application.Quit(); });
        quitNo.onClick.AddListener(delegate { quitMsg.gameObject.SetActive(false); });

        quitMsg.gameObject.SetActive(false);
    }
	public void initializeOption()
	{
		soundPanel.gameObject.SetActive(true);
		graphicPanel.gameObject.SetActive(false);
		controlPanel.gameObject.SetActive(false);
	}
	public void initializeLeader()
	{
		cloneGrades(cloneG);
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

#region BlockControl

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                      = Quaternion.Euler(0f, 0f, 90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                      = Quaternion.Euler(90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                      = Quaternion.Euler(0f, 0f, -90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                      = Quaternion.Euler(-90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                      = Quaternion.Euler(0f, 0f, -90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                      = Quaternion.Euler(-90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                      = Quaternion.Euler(0f, 0f, 90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                      = Quaternion.Euler(90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);
			Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);
			Quaternion rotation = Quaternion.identity;

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                      = Quaternion.Euler(-90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                      = Quaternion.Euler(0f, 0f, 90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                      = Quaternion.Euler(90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                      = Quaternion.Euler(0f, 0f, -90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * blockSize +
			                 new Vector3(1f, -1f, 1f) * (currentBlock.Size * blockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation                      = Quaternion.Euler(90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 1:
					rotation                      = Quaternion.Euler(0f, 0f, -90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 2:
					rotation                      = Quaternion.Euler(-90f, 0f, 0f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;

				case 3:
					rotation                      = Quaternion.Euler(0f, 0f, 90f);
					particle                      = new ParticleRender(vfxRotation, offset, rotation);
					particle.Obj.transform.parent = effectObj.transform;

					break;
			}

			Destroy(particle!.Obj, 0.3f);
			particle = null;

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
			StartCoroutine(CameraShake());

		currentBlock.Move(Coord.Up);
		PlaceBlock();
	}

#endregion

#region Effect

	private static IEnumerator GridEffect()
	{
		const float alphaUnit = 0.01f;
		float       alphaSet  = Grid.Mesh.MRenderer.material.GetFloat(alpha) + alphaUnit;
		Vector3     targetLoc = Grid.Mesh.Obj.transform.position             - Vector3.up * 5f;

		while ((Grid.Mesh.Obj.transform.position - targetLoc).magnitude > 0.001f)
		{
			alphaSet -= 0.01f;

			Grid.Mesh.Obj.transform.position = Vector3.Lerp(Grid.Mesh.Obj.transform.position, targetLoc, 0.02f);
			Grid.Mesh.MRenderer.material.SetFloat(alpha, Mathf.Max(alphaSet, 0f));

			yield return new WaitForSeconds(0.02f);
		}

		Destroy(Grid.Mesh.Obj);
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

	private void RenderCurrentBlock()
	{
		ClearCurrentBlock();

		foreach (Coord coord in currentBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset,
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
						                      new Coord(j, i, k), ShadowCastingMode.On);

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

    public class UIElement
    {
        private GameObject obj;
        private Image img;
        private Button btn;
        private Button.ButtonClickedEvent click;
        private RectTransform rect;
		GameManager manager;

        public UIElement()
        {
            CreateBasic();
        }
		public UIElement(Image image)
		{
			CreateBasic();
			CreateButton(image);
		}
        public UIElement(Image image, string str)
		{
            CreateBasic();
            FindCreateButton(image, str);
        }

        private void CreateBasic()
        {
            obj = new GameObject();
            rect = obj.AddComponent<RectTransform>();
        }
		private void CreateButton(Image image)
		{
			this.img = image;
			btn = img.gameObject.AddComponent<Button>();
		}
		private void FindCreateButton(Image image, string str)
        {
			image = GameObject.Find(str).GetComponent<Image>();
            btn = image.gameObject.AddComponent<Button>();
        }

    }
    #endregion
}