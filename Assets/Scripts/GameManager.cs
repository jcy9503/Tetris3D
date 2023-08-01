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
using UnityEngine.Serialization;

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

	[Header("Test Mode")] public static bool  IsGameOver;
	private static                      bool  isPause;
	private const                       int   baseScore  = 100;
	private static readonly             int[] scoreValue = { 1, 2, 4, 8 };
	public static                       int   TotalScore;
	public static                       bool  TestGrid;
	[SerializeField] private            bool  testGrid;
	public static                       int   TestHeight;
	[SerializeField] private            int   testHeight = 4;
	public static                       bool  Regeneration;
	[SerializeField] private            bool  regeneration;
	[SerializeField] private            int   testFieldSize = 6;
	public static                       bool  TestModeBlock;
	[SerializeField] private            bool  testModeBlock;
	public static                       int   TestBlock;
	[SerializeField] private            int   testBlock = 3;
	[Space(20)] [Header("Grid/Block")] [SerializeField]
	private int[] gridSize = { 10, 22, 10 };
	public static  GameGrid        Grid;
	private static BlockQueue      BlockQueue { get; set; }
	public static  Block           CurrentBlock;
	public static  Block           ShadowBlock;
	public static  bool            CanSaveBlock;
	public const   float           BlockSize    = 1.0f;
	private        float           downInterval = 1.0f;
	private        List<Coroutine> logicList;

#endregion

#region MonoFunction

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		IsGameOver = false;
		isPause    = true;
		TotalScore = 0;

		TestGrid      = testGrid;
		TestHeight    = testHeight;
		Regeneration  = regeneration;
		TestModeBlock = testModeBlock;
		TestBlock     = testBlock % Block.Type;

		if (TestGrid)
		{
			gridSize[0] = testFieldSize;
			gridSize[2] = testFieldSize;
			Grid        = new GameGrid(ref gridSize, BlockSize);
		}
		else
		{
			Grid = new GameGrid(ref gridSize, BlockSize);
		}

		BlockQueue   = new BlockQueue();
		CurrentBlock = BlockQueue.GetAndUpdateBlock();
		CanSaveBlock = true;
	}

	private void Update()
	{
		if (IsGameOver)
		{
			Terminate();
		}
		else if (!isPause)
		{
#endif

			if (!audioSourceBGM.isPlaying && !IsGameOver)
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
				keyUsing[(int)KEY_VALUE.ESC] = true;
				StartCoroutine(KeyRewind((int)KEY_VALUE.ESC));
			}

#endif
		}
	}

#endregion

#region GameControl

	private IEnumerator GameStart()
	{
		StartCoroutine(PlaySfx(SFX_VALUE.CLICK));

		StartCoroutine(FadeOutBGM(2f));
		StopCoroutine(mainBGM);

		yield return StartCoroutine(CameraGameStart());

		RandomPlayBGM();

		isPause = false;

		logicList = new List<Coroutine>
		{
			StartCoroutine(BlockDown()),
			StartCoroutine(AngleCalculate()),
		};
	}

	private IEnumerator GameHome()
	{
		StartCoroutine(PlaySfx(SFX_VALUE.CLICK));

		yield return StartCoroutine(CameraGameHome());

		mainBGM = StartCoroutine(PlayMainBGM());

		isPause = true;

		Replay();
	}

	private void Replay()
	{
		if (TestGrid)
		{
			gridSize[0] = testFieldSize;
			gridSize[2] = testFieldSize;
			Grid        = new GameGrid(ref gridSize, BlockSize);
		}
		else
		{
			Grid = new GameGrid(ref gridSize, BlockSize);
		}

		CurrentBlock = BlockQueue.GetAndUpdateBlock();
		BlockQueue.SaveBlockReset();
		CanSaveBlock = true;

		RenderCurrentBlock();
		RenderShadowBlock();

		if (TestGrid) RenderGrid();

		RenderLine();
		lineGlowPower = lineMeshList[0].Renderer.material.GetFloat(power);
	}

	private void GamePause()
	{
		StartCoroutine(PlaySfx(SFX_VALUE.PAUSE));

		UIGamePauseOnClick();

		isPause = true;
		PauseBGM(3f);

		foreach (Coroutine coroutine in logicList)
		{
			StopCoroutine(coroutine);
		}
	}

	private void GameResume()
	{
		StartCoroutine(PlaySfx(SFX_VALUE.RESUME));

		UIGameResumeOnClick();

		isPause = false;
		ResumeBGM(3f);

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
				IsGameOver = true;

				break;
			}

			MoveBlockDown();

			if (rotationParticle != null)
				rotationParticle.Obj.transform.position -= Vector3.up;

			yield return new WaitForSeconds(downInterval);
		}
	}

	private void Terminate()
	{
		foreach (Coroutine coroutine in logicList)
		{
			StopCoroutine(coroutine);
		}

		IsGameOver = false;
	}

	private static bool BlockFits(Block block)
	{
		return block.TilePositions().All(coord => Grid.IsEmpty(coord.X, coord.Y, coord.Z));
	}

	private static bool CheckGameOver()
	{
		return !Grid.IsPlaneEmpty(0);
	}

	private void PlaceBlock()
	{
		foreach (Coord coord in CurrentBlock.TilePositions())
		{
			Grid[coord.X, coord.Y, coord.Z] = CurrentBlock.GetId();
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

		if (CheckGameOver())
		{
			StartCoroutine(PlaySfx(SFX_VALUE.GAME_OVER));

			isPause    = true;
			IsGameOver = true;

			gameOverScoreText.text = TotalScore.ToString();

			StopCoroutine(animFunc);
			StartCoroutine(PitchDownBGM(0.2f));
			StartCoroutine(GameOverEffect());
			StartCoroutine(AnimStop());
			StartCoroutine(UIFadeInOut(playCanvas, gameOverCanvas, 1f));
		}
		else
		{
			CanSaveBlock = true;
			CurrentBlock = BlockQueue.GetAndUpdateBlock();
			RefreshCurrentBlock();
		}
	}

	private void ScoreCalc(int cleared)
	{
		if (cleared == 0) return;

		TotalScore           += baseScore * scoreValue[cleared - 1];
		inGameScoreText.text =  TotalScore.ToString();
	}

#endregion

#region BlockRotation

	private void RotateBlockXClockWise()
	{
		switch (viewAngle)
		{
			case 0:
				CurrentBlock.RotateXClockWise();

				break;

			case 1:
				CurrentBlock.RotateZCounterClockWise();

				break;

			case 2:
				CurrentBlock.RotateXCounterClockWise();

				break;

			case 3:
				CurrentBlock.RotateZClockWise();

				break;
		}

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			switch (viewAngle)
			{
				case 0:
					CurrentBlock.RotateXCounterClockWise();

					break;

				case 1:
					CurrentBlock.RotateZClockWise();

					break;

				case 2:
					CurrentBlock.RotateXClockWise();

					break;

				case 3:
					CurrentBlock.RotateZCounterClockWise();

					break;
			}
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation         = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 1:
					rotation         = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 2:
					rotation         = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 3:
					rotation         = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

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
				CurrentBlock.RotateXCounterClockWise();

				break;

			case 1:
				CurrentBlock.RotateZClockWise();

				break;

			case 2:
				CurrentBlock.RotateXClockWise();

				break;

			case 3:
				CurrentBlock.RotateZCounterClockWise();

				break;
		}

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			switch (viewAngle)
			{
				case 0:
					CurrentBlock.RotateXClockWise();

					break;

				case 1:
					CurrentBlock.RotateZCounterClockWise();

					break;

				case 2:
					CurrentBlock.RotateXCounterClockWise();

					break;

				case 3:
					CurrentBlock.RotateZClockWise();

					break;
			}
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation         = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 1:
					rotation         = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 2:
					rotation         = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 3:
					rotation         = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYClockWise()
	{
		CurrentBlock.RotateYClockWise();

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.RotateYCounterClockWise();
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);
			Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;
			}

			Destroy(rotationParticle!.Obj, 0.3f);
			rotationParticle = null;

			RefreshCurrentBlock();
		}
	}

	private void RotateBlockYCounterClockWise()
	{
		CurrentBlock.RotateYCounterClockWise();

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.RotateYClockWise();
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);
			Quaternion rotation = Quaternion.identity;

			switch (viewAngle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

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
				CurrentBlock.RotateZClockWise();

				break;

			case 1:
				CurrentBlock.RotateXClockWise();

				break;

			case 2:
				CurrentBlock.RotateZCounterClockWise();

				break;

			case 3:
				CurrentBlock.RotateXCounterClockWise();

				break;
		}

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			switch (viewAngle)
			{
				case 0:
					CurrentBlock.RotateZCounterClockWise();

					break;

				case 1:
					CurrentBlock.RotateXCounterClockWise();

					break;

				case 2:
					CurrentBlock.RotateZClockWise();

					break;

				case 3:
					CurrentBlock.RotateXClockWise();

					break;
			}
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation         = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 1:
					rotation         = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 2:
					rotation         = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 3:
					rotation         = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

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
				CurrentBlock.RotateZCounterClockWise();

				break;

			case 1:
				CurrentBlock.RotateXCounterClockWise();

				break;

			case 2:
				CurrentBlock.RotateZClockWise();

				break;

			case 3:
				CurrentBlock.RotateXClockWise();

				break;
		}

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			switch (viewAngle)
			{
				case 0:
					CurrentBlock.RotateZClockWise();

					break;

				case 1:
					CurrentBlock.RotateXClockWise();

					break;

				case 2:
					CurrentBlock.RotateZCounterClockWise();

					break;

				case 3:
					CurrentBlock.RotateXCounterClockWise();

					break;
			}
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.ROTATE1, SFX_VALUE.ROTATE2);

			Vector3 offset = StartOffset + CurrentBlock.Pos.ToVector() + new Vector3(-0.5f, 0.5f, -0.5f) * BlockSize +
			                 new Vector3(1f, -1f, 1f) * (CurrentBlock.Size * BlockSize * 0.5f);

			Quaternion rotation;

			switch (viewAngle)
			{
				case 0:
					rotation         = Quaternion.Euler(90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 1:
					rotation         = Quaternion.Euler(0f, 0f, -90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 2:
					rotation         = Quaternion.Euler(-90f, 0f, 0f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

					break;

				case 3:
					rotation         = Quaternion.Euler(0f, 0f, 90f);
					rotationParticle = new ParticleRender(vfxRotation, offset, rotation);

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
		CurrentBlock.Move(Coord.Left[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.Move(Coord.Right[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockRight()
	{
		CurrentBlock.Move(Coord.Right[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.Move(Coord.Left[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockForward()
	{
		CurrentBlock.Move(Coord.Forward[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.Move(Coord.Backward[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockBackward()
	{
		CurrentBlock.Move(Coord.Backward[viewAngle]);

		if (!BlockFits(CurrentBlock))
		{
			StartCoroutine(PlaySfx(SFX_VALUE.UNAVAILABLE));

			CurrentBlock.Move(Coord.Forward[viewAngle]);
		}
		else
		{
			StartCoroutine(PlaySfx(SFX_VALUE.MOVE));

			RefreshCurrentBlock();
		}
	}

	private void MoveBlockDown()
	{
		CurrentBlock.Move(Coord.Down);

		if (BlockFits(CurrentBlock))
		{
			RenderCurrentBlock();

			return;
		}

		PlayRandomSfx(SFX_VALUE.DROP1, SFX_VALUE.DROP2);

		CurrentBlock.Move(Coord.Up);
		DropEffect();
		PlaceBlock();
	}

	private void MoveBlockDownWhole()
	{
		int num = 0;

		do
		{
			CurrentBlock.Move(Coord.Down);
			++num;
		} while (BlockFits(CurrentBlock));

		if (num > 2)
		{
			PlayRandomSfx(SFX_VALUE.HARD_DROP1, SFX_VALUE.HARD_DROP5);

			StartCoroutine(CameraShake());
		}
		else
		{
			PlayRandomSfx(SFX_VALUE.DROP1, SFX_VALUE.DROP2);
		}

		CurrentBlock.Move(Coord.Up);
		DropEffect();
		PlaceBlock();
	}

#endregion
}