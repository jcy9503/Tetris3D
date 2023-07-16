using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour, IBeginDragHandler, IDragHandler
{
	private static readonly object locker = new();

	private static bool shuttingDown;

	private static GameManager instance;

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

	private static bool gameOver;

	private Camera     mainCamera;
	private Transform  cameraRotator;
	private Quaternion cameraRotation;
	private Vector2    _angleUpdate;
	private Vector2 AngleUpdate
	{
		get => _angleUpdate;
		set
		{
			_angleUpdate.y = value.y;
			if (value.x > initialCameraRotationX + cameraRotationConstraintX)
				_angleUpdate.x = initialCameraRotationX + cameraRotationConstraintX;
			else if (value.x < initialCameraRotationX - cameraRotationConstraintX)
				_angleUpdate.x = initialCameraRotationX - cameraRotationConstraintX;
		}
	}
	private                  Vector2 angleTemp;
	private                  Vector2 clickPos;
	[SerializeField] private float   initialCameraRotationX    = 15f;
	[SerializeField] private float   cameraRotationConstraintX = 55f;
	[SerializeField] private float   cameraSpeed               = 10f;

	[SerializeField] private int gridSizeX = 10;
	[SerializeField] private int gridSizeY = 22;
	[SerializeField] private int gridSizeZ = 10;

	public static  GameGrid   Grid;
	private static BlockQueue blockQueue;

	[SerializeField] private float blockSize = 1.0f;
	[SerializeField] private float interval  = 1.0f;

	private List<Coroutine> func;

	private void Awake()
	{
		gameOver = false;

		mainCamera                     = Camera.main;
		mainCamera!.transform.rotation = Quaternion.Euler(initialCameraRotationX, 0f, 0f);
		cameraRotator                  = GameObject.Find("Camera Rotator").GetComponent<Transform>();
		cameraRotation                 = cameraRotator.rotation;
		AngleUpdate                    = new Vector2(cameraRotation.eulerAngles.x, cameraRotation.eulerAngles.y);

		Grid       = new GameGrid(gridSizeX, gridSizeY, gridSizeZ, blockSize);
		blockQueue = new BlockQueue();

		func = new List<Coroutine>
		{
			StartCoroutine(BlockDown())
		};
	}

	private void Update()
	{
		if (gameOver)
		{
			GameOver();
		}
	}

	private IEnumerator BlockDown()
	{
		while (true)
		{
			if (!Grid.IsPlaneEmpty(0))
				gameOver = true;

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

	public void OnBeginDrag(PointerEventData eventData)
	{
		BeginDrag(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnDrag(eventData.position);
	}

	private void BeginDrag(Vector2 pos)
	{
		clickPos  = pos;
		angleTemp = AngleUpdate;
		Debug.Log(pos.ToString());
	}

	private void OnDrag(Vector2 pos)
	{
		AngleUpdate = new Vector2(angleTemp.x + (pos.x - clickPos.x) * cameraSpeed / Screen.width,
		                          angleTemp.y - (pos.y - clickPos.y) * cameraSpeed / Screen.height);
		Debug.Log(AngleUpdate.ToString());

		cameraRotation = Quaternion.Euler(AngleUpdate);
	}
}