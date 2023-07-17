/*
 * GameManager.cs
 * --------------
 * Made by Lucas Jeong
 * Contains main game logic and singleton instance.
 * Also contains screen control method.
 */

using System.Collections;
using System.Collections.Generic;
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
		gameOver = false;

		mainCamera                     = Camera.main;
		mainCamera!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		rotatorTr                      = GameObject.Find("Rotator").GetComponent<Transform>();
		rotatorRt                      = rotatorTr.rotation;
		mementoRotation                = Quaternion.identity;
		Dir                            = false;
		checkDir                       = false;

		Grid       = new GameGrid(gridSizeX, gridSizeY, gridSizeZ, blockSize);
		blockQueue = new BlockQueue();

		func = new List<Coroutine>
		{
			StartCoroutine(BlockDown()),
		};
	}

	private void Update()
	{
		if (gameOver)
		{
			GameOver();
		}
		else
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
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
#endif
		}
	}

	private static bool       gameOver;
	private        Camera     mainCamera;
	private        Transform  rotatorTr;
	private        Quaternion rotatorRt;
	private        Quaternion mementoRotation;
	private        bool       checkDir;
	private        bool       _dir;
	private bool Dir
	{
		get => _dir;
		set
		{
			if (value != _dir)
				checkDir = true;
			_dir = value;
		}
	}
	private                  Vector2         clickPos;
	[SerializeField] private float           initialCameraRotationX    = 15f;
	[SerializeField] private float           cameraRotationConstraintX = 55f;
	[SerializeField] private float           cameraSpeed               = 2000f;
	[SerializeField] private int             gridSizeX                 = 10;
	[SerializeField] private int             gridSizeY                 = 22;
	[SerializeField] private int             gridSizeZ                 = 10;
	public static            GameGrid        Grid;
	private static           BlockQueue      blockQueue;
	[SerializeField] private float           blockSize = 1.0f;
	[SerializeField] private float           interval  = 1.0f;
	private                  List<Coroutine> func;

	private IEnumerator BlockDown()
	{
		while (true)
		{
			if (!Grid.IsPlaneEmpty(0))
			{
				gameOver = true;

				break;
			}

			blockQueue.Current.Move(-Vector3.down);

			yield return new WaitForSeconds(interval);
		}
	}

	private void GameOver()
	{
		foreach (Coroutine coroutine in func)
		{
			StopCoroutine(coroutine);
		}
	}
}