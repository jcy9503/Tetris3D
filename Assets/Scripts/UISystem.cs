using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{
#region MainScreen

	private CanvasGroup mainCanvas;
	private GameObject  quitPanel;
	private Button      startBtn;
	private Button      optionBtn;
	private Button      leaderBoardBtn;
	private Button      quitBtn;
	private Button      quitYes;
	private Button      quitNo;

#endregion

#region PlayScreen

	private CanvasGroup playCanvas;
	private GameObject  pauseScreen;
	private GameObject  controlScreen;
	private GameObject  rotateBtns;
	private GameObject  moveBtns;
	private Button      pauseBtn;
	private Button      pauseHomeBtn;
	private Button      pauseResumeBtn;
	private Button      moveUpBtn;
	private Button      moveDownBtn;
	private Button      moveLeftBtn;
	private Button      moveRightBtn;
	private Button      rotateXBtn;
	private Button      rotateYBtn;
	private Button      rotateZBtn;
	private Button      rotateXInverseBtn;
	private Button      rotateYInverseBtn;
	private Button      rotateZInverseBtn;
	private TMP_Text    inGameScoreText;

#endregion

#region OptionScreen

	private CanvasGroup optionCanvas;
	private GameObject  soundPanel;
	private GameObject  graphicPanel;
	private GameObject  controlPanel;

#endregion

#region LeaderBoardScreen

	private CanvasGroup leaderBoardCanvas;

#endregion

#region GameOverScreen

	private CanvasGroup gameOverCanvas;
	private Button      retryBtn;
	private Button      gameOverHomeBtn;
	private TMP_Text    gameOverScoreText;

#endregion

	private          TMP_Text        finalScore;
	private          TextMeshProUGUI optionTitle;
	private readonly string[]        optionTitles = { "Sound", "Graphics", "Controls" };
	private          Button          soundTab;
	private          Slider          bgmSlider;
	private          Slider          sfxSlider;
	private          Button          graphicTab;
	private          TextMeshProUGUI blkOption;
	private const    string          blkOptionColor = "Color";
	private          Button          infoBtn;
	private          Button          controlTab;
	private          Button          btnMode;
	private          Button          destroyOnOff;
	private          Image           destroyCheck;
	private          Button          rotationOnOff;
	private          Image           rotationCheck;
	private          Button          optionBack;
	private          TextMeshProUGUI firstG;
	private          TextMeshProUGUI secondG;
	private          TextMeshProUGUI thirdG;
	private          Image           cloneG;
	private          Button          leaderBack;

	public UISystem()
	{
		InitUI();
	}

	private void InitUI()
	{
		UIInitMain();
		UIInitGameOver();
		UIInitPlay();
		UIInitOption();
		UIInitLeaderBoard();
	}

	private void UIInitPlay()
	{
		pauseScreen   = GameObject.Find("PauseScreen");
		controlScreen = GameObject.Find("ControlScreen");

		rotateBtns = GameObject.Find("Rotate_Buttons");
		moveBtns   = GameObject.Find("Move_Buttons");

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		rotateBtns.SetActive(false);
		moveBtns.SetActive(false);
#endif

		pauseBtn       = GameObject.Find("Pause").GetComponent<Button>();
		pauseHomeBtn   = GameObject.Find("Pause_Home").GetComponent<Button>();
		pauseResumeBtn = GameObject.Find("Pause_Resume").GetComponent<Button>();

		inGameScoreText      = GameObject.Find("Text_Score").GetComponent<TMP_Text>();
		inGameScoreText.text = "0";

		pauseBtn.onClick.AddListener(UIGamePauseOnClick);
		pauseHomeBtn.onClick.AddListener(UIPauseHomeOnClick);
		pauseResumeBtn.onClick.AddListener(UIGameResumeOnClick);

		playCanvas.gameObject.SetActive(false);
		pauseScreen.gameObject.SetActive(false);
	}

	private void UIInitGameOver()
	{
		gameOverCanvas = GameObject.Find("GameOverScreen").GetComponent<CanvasGroup>();

		retryBtn        = GameObject.Find("Retry_Button").GetComponent<Button>();
		gameOverHomeBtn = GameObject.Find("Home_Button").GetComponent<Button>();

		gameOverScoreText = GameObject.Find("GameOver_Score").GetComponent<TMP_Text>();

		retryBtn.onClick.AddListener(UIInitPlay);

		gameOverCanvas.gameObject.SetActive(false);
	}

	private void UIInitMain()
	{
		playCanvas        = GameObject.Find("PlayScreen").GetComponent<CanvasGroup>();
		mainCanvas        = GameObject.Find("MainScreen").GetComponent<CanvasGroup>();
		optionCanvas      = GameObject.Find("OptionScreen").GetComponent<CanvasGroup>();
		leaderBoardCanvas = GameObject.Find("LeaderBoard").GetComponent<CanvasGroup>();

		startBtn       = GameObject.Find("Game_Start").GetComponent<Button>();
		optionBtn      = GameObject.Find("Option").GetComponent<Button>();
		leaderBoardBtn = GameObject.Find("Leader_Board").GetComponent<Button>();
		quitBtn        = GameObject.Find("Quit").GetComponent<Button>();
		quitYes        = GameObject.Find("Quit_Yes").GetComponent<Button>();
		quitNo         = GameObject.Find("Quit_No").GetComponent<Button>();
		quitPanel      = GameObject.Find("Quit_Panel");

		startBtn.onClick.AddListener(UIGameStartOnClick);
		quitBtn.onClick.AddListener(delegate { quitPanel.gameObject.SetActive(true); });

		quitYes.onClick.AddListener(delegate { Application.Quit(); });
		quitNo.onClick.AddListener(delegate { quitPanel.gameObject.SetActive(false); });

		quitPanel.gameObject.SetActive(false);
	}

	private void UIInitOption()
	{
		soundPanel   = GameObject.Find("SoundBtns");
		graphicPanel = GameObject.Find("GraphicBtns");
		controlPanel = GameObject.Find("ControlBtns");

		optionTitle = GameObject.Find("Option_Title").GetComponent<TextMeshProUGUI>();
		soundTab    = GameObject.Find("Sound_Tab").GetComponent<Button>();
		soundTab.onClick.AddListener(OpenSoundTab);

		bgmSlider       = GameObject.Find("BGM_Slider").GetComponent<Slider>();
		bgmSlider.value = audioSourceBGM.volume;

		sfxSlider       = GameObject.Find("SFX_Slider").GetComponent<Slider>();
		sfxSlider.value = sfxVolume;

		graphicTab = GameObject.Find("Graphic_Tab").GetComponent<Button>();
		graphicTab.onClick.AddListener(OpenGraphicTab);
		destroyCheck  = GameObject.Find("Destroy_Check").GetComponent<Image>();
		rotationCheck = GameObject.Find("Rotation_Check").GetComponent<Image>();
		destroyOnOff  = GameObject.Find("Destroy_Effect_Box").GetComponent<Button>();
		destroyOnOff.onClick.AddListener(() => ImageOnOff(destroyCheck));
		rotationOnOff = GameObject.Find("Rotation_Effect_Box").GetComponent<Button>();
		rotationOnOff.onClick.AddListener(() => ImageOnOff(rotationCheck));

		blkOption      = GameObject.Find("ColorChange_Text").GetComponent<TextMeshProUGUI>();
		blkOption.text = blkOptionColor;
		GameObject.Find("ColorChange_Handle").GetComponent<Button>();

		GameObject.Find("ColorChange_Image").GetComponent<Image>();

		controlTab = GameObject.Find("Control_Tab").GetComponent<Button>();
		controlTab.onClick.AddListener(OpenControlTab);

		optionBack = GameObject.Find("Option_Back").GetComponent<Button>();
		//optionBack.onClick.AddListener(() => MoveScreen(curOption, curMain));

		soundPanel.gameObject.SetActive(true);
		graphicPanel.gameObject.SetActive(false);
		controlPanel.gameObject.SetActive(false);
		optionCanvas.gameObject.SetActive(false);
	}

	private void UIInitLeaderBoard()
	{
		//leaderBoard.onClick.AddListener(() => MoveScreen(curMain, curLeader));
		cloneG     = GameObject.Find("Boards").GetComponent<Image>();
		leaderBack = GameObject.Find("Leader_Back").GetComponent<Button>();
		//leaderBack.onClick.AddListener(() => MoveScreen(curLeader, curMain));

		leaderBoardCanvas.gameObject.SetActive(false);

		CloneGrades(cloneG);
	}

	private IEnumerator BtnClick(Image image)
	{
		float   interval    = 0.1f;
		Color   originColor = image.color;
		Color   pressed     = new(255, 255, 255, 0.2f);
		Vector3 trans       = new(0.01f, 0.01f);
		Vector3 originSize  = image.transform.localScale;

		while (true)
		{
			Vector3 localScale = image.transform.localScale;
			Vector3.Slerp(localScale, trans, interval);
			image.color = Color.Lerp(image.color, pressed, interval);

			yield return new WaitForSeconds(interval);

			localScale                 = originSize;
			image.transform.localScale = localScale;
			image.color                = originColor;

			yield break;
		}
	}

	public IEnumerator LerpImage(Image image)
	{
		Color originAlpha = new Color(255f, 255f, 255f, 1f);
		Color lowAlpha    = new Color(255f, 255f, 255f, 0f);
		float interval    = 0.1f;

		while (true)
		{
			image.color = Color.Lerp(image.color, lowAlpha, (interval + 0.4f) * Time.deltaTime);

			yield return new WaitForSeconds(interval);

			image.color = Color.Lerp(image.color, originAlpha, (interval + 0.4f) * Time.deltaTime);

			yield break;
		}
	}

	private static IEnumerator UIFadeInOut(CanvasGroup fadeOut, CanvasGroup fadeIn, float acc)
	{
		const float alphaUnit = 0.02f;
		float       alphaSet  = 1f;

		while (alphaSet >= 0f)
		{
			alphaSet      -= alphaUnit * acc;
			fadeOut.alpha =  alphaSet;

			yield return new WaitForSeconds(0.01f);
		}

		fadeOut.alpha = 0f;
		fadeIn.gameObject.SetActive(true);
		fadeIn.alpha = 0f;

		alphaSet = 0f;

		while (alphaSet <= 1f)
		{
			alphaSet     += alphaUnit * acc;
			fadeIn.alpha =  alphaSet;

			yield return new WaitForSeconds(0.01f);
		}
	}

	private void VolumeControl(float value, Slider slider)
	{
		slider.value = value;
	}

	private void OpenSoundTab()
	{
		soundPanel.gameObject.SetActive(true);
		graphicPanel.gameObject.SetActive(false);
		controlPanel.gameObject.SetActive(false);
		optionTitle.text = optionTitles[0];
	}

	private void OpenGraphicTab()
	{
		soundPanel.gameObject.SetActive(false);
		graphicPanel.gameObject.SetActive(true);
		controlPanel.gameObject.SetActive(false);
		optionTitle.text = optionTitles[1];
	}

	private void OpenControlTab()
	{
		soundPanel.gameObject.SetActive(false);
		graphicPanel.gameObject.SetActive(false);
		controlPanel.gameObject.SetActive(true);
		optionTitle.text = optionTitles[2];
	}

	private static void CloneGrades(Image boards)
	{
		Vector3 pos = new(0, -10f);

		for (int i = 0; i < 7; i++)
		{
			Instantiate(boards, pos, Quaternion.identity);
		}
	}

	private static void ImageOnOff(Behaviour blink)
	{
		blink.enabled = !blink.enabled;
	}

	private void UIGameStartOnClick()
	{
		startBtn.interactable = false;
		StartCoroutine(UIFadeInOut(mainCanvas, playCanvas, 1f));
		startBtn.interactable = true;
		mainCanvas.gameObject.SetActive(false);

		StartCoroutine(GameStart());
	}

	private void UIGamePauseOnClick()
	{
		controlScreen.SetActive(false);
		pauseScreen.SetActive(true);

		GamePause();
	}

	private void UIGameResumeOnClick()
	{
		pauseScreen.SetActive(false);
		controlScreen.SetActive(true);

		GameResume();
	}

	private void UIPauseHomeOnClick()
	{
		pauseHomeBtn.interactable = false;
		StartCoroutine(UIFadeInOut(playCanvas, mainCanvas, 1f));
		pauseHomeBtn.interactable = true;
		pauseScreen.SetActive(false);
		controlScreen.SetActive(true);
		playCanvas.gameObject.SetActive(false);

		StartCoroutine(GameHome());
	}

	private void UIReplayOnClick()
	{
	}
}