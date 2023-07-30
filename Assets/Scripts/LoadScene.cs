using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
	private AsyncOperation async;
	private CanvasGroup    canvasGroup;
	private Image          progressBar;
	
	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		progressBar = GameObject.Find("Progress").GetComponent<Image>();
		
		StartCoroutine(Load());
	}

	private IEnumerator Load()
	{
		yield return StartCoroutine(FadeIn());
		
		async                      = SceneManager.LoadSceneAsync("MainScene");
		async.allowSceneActivation = false;

		while (async.progress < 0.9f)
		{
			progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, async.progress, 0.05f);

			yield return new WaitForSeconds(0.05f);
		}

		while (progressBar.fillAmount < 1.0f)
		{
			progressBar.fillAmount += 0.05f;

			yield return new WaitForSeconds(0.05f);
		}

		yield return StartCoroutine(FadeOut());

		async.allowSceneActivation = true;
	}

	private IEnumerator FadeIn()
	{
		while (canvasGroup.alpha < 0.99f)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, 0.1f);

			yield return new WaitForSeconds(0.05f);
		}

		canvasGroup.alpha = 1f;
	}

	private IEnumerator FadeOut()
	{
		while (canvasGroup.alpha > 0.01f)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, 0.1f);

			yield return new WaitForSeconds(0.05f);
		}

		canvasGroup.alpha = 0f;
	}
}
