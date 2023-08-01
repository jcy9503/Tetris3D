using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	private GameObject mainCameraObj;
	private Camera     mainCamera;
	private Transform  rotatorTr;
	private Quaternion mementoRotation;
	private Quaternion gameCameraRotation        = Quaternion.Euler(15f, 0f, 0f);
	private float      cameraRotationConstraintX = 55f;
	private float      cameraSpeed               = 2000f;
	private float      cameraShakeAmount         = 0.5f;
	private float      cameraShakeTime           = 0.2f;
	private bool       isCameraShaking;
	private int        viewAngle;
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

	public CameraSystem()
	{
		Init();
	}

	private void Init()
	{
		mainCameraObj                     = GameObject.Find("Main Camera");
		mainCameraObj!.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		mainCamera                        = mainCameraObj.GetComponent<CameraSystem>();
		rotatorTr                         = GameObject.Find("Rotator").GetComponent<Transform>();
		mementoRotation                   = Quaternion.identity;
		Dir                               = false;
		checkDir                          = false;
		viewAngle                         = 0;
		isCameraShaking                   = false;
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
	
	private IEnumerator CameraGameStart()
	{
		float pastTime = 0f;

		while (pastTime < 2f)
		{
			mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation,
			                                                 Quaternion.LookRotation(new Vector3(0f, -6.35f, 23.7f)),
			                                                 0.05f);

			yield return new WaitForSeconds(0.01f);

			pastTime += 0.01f;
		}

		mainCamera.transform.rotation = gameCameraRotation;
	}

	private IEnumerator CameraGameHome()
	{
		float pastTime = 0f;

		while (pastTime < 2f)
		{
			mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation,
			                                                 Quaternion.LookRotation(Vector3.right),
			                                                 0.05f);

			yield return new WaitForSeconds(0.01f);

			pastTime += 0.01f;
		}

		mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
	}
	
	private void RotatorPositionClear()
	{
		rotatorTr.position = Vector3.zero;

		isCameraShaking = false;
	}
}