using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayScreen : MonoBehaviour
{
    private RectTransform Control_Panel;


    private Image GameOver_Panel;
    private Transform GameOver_Title;
    private Transform GameOver_Description;

    private Button GameOver_Home_Button;
    private Text GameOver_Home_Text;
    private Image GameOver_Home_Icon;

    private Button GameOver_Retry_Button;
    private Text GameOver_Retry_Text;
    private Image GameOver_Retry_Icon;

    private Text Final_Score;

    void Start()
    {
        Control_Panel = GameObject.Find("Controls").GetComponent<RectTransform>();



        GameOver_Panel = GameObject.Find("GameOver").GetComponent<Image>();
        GameOver_Title = GameObject.Find("Text_GameOver").GetComponent<Transform>();
        GameOver_Description = GameObject.Find("FinalScore").GetComponent<Transform>();
        Final_Score = GameObject.Find("Score").GetComponent<Text>();

        GameOver_Home_Button = GameObject.Find("Home_Button").AddComponent<Button>();
        GameOver_Home_Text = GameObject.Find("Text_Home").GetComponent<Text>();
        GameOver_Home_Icon = GameObject.Find("Home").GetComponent<Image>();
        GameOver_Home_Button.onClick.AddListener(onClickGoHome);

        GameOver_Retry_Button = GameObject.Find("Retry_Button").AddComponent<Button>();
        GameOver_Retry_Text = GameObject.Find("Text_Retry").GetComponent<Text>();
        GameOver_Retry_Icon = GameObject.Find("Retry").GetComponent<Image>();
        GameOver_Retry_Button.onClick.AddListener(onClickDoRetry);

        GameOver_Panel.gameObject.SetActive(false);
    }


    private IEnumerator GameOverTitle()
    {
        Vector3 OriginPos = GameOver_Title.transform.position;
        Vector3 TitleTargetPos = new Vector3(0, 160);

        GameOver_Title.transform.position = Vector3.Lerp(OriginPos, TitleTargetPos, 5f);

        if (GameOver_Title.transform.position == TitleTargetPos) yield break;

    }
    private IEnumerator GameOverButtons()
    {
        Vector3 OriginPos1 = GameOver_Home_Button.transform.position;
        Vector3 OriginPos2 = GameOver_Retry_Button.transform.position;

        Vector3 ButtonTargetPos1 = new Vector3(-165, 340);
        Vector3 ButtonTargetPos2 = new Vector3(165, 340);

        GameOver_Home_Button.transform.position = Vector3.Lerp(OriginPos1, ButtonTargetPos1, 5f);
        GameOver_Retry_Button.transform.position = Vector3.Lerp(OriginPos2, ButtonTargetPos2, 5f);

        if (OriginPos1 == ButtonTargetPos1 | OriginPos1 == ButtonTargetPos2)
            yield break;

    }
    private bool GameIsOver()
    {
        if (GameManager.gameOver)
        {
            Control_Panel.gameObject.SetActive(false);
            GameOver_Panel.gameObject.SetActive(true);
            StartCoroutine(GameOverTitle());
            StartCoroutine(GameOverButtons());

            return GameManager.gameOver;
        }
        return false;
    }
    private void onClickGoHome()
    {
        GameOver_Home_Button.image.color = new Color(255, 80, 0);
        GameOver_Home_Text.color = new Color(255, 255, 255);
        GameOver_Home_Icon.color = new Color(255, 255, 255);

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    private void onClickDoRetry()
    {
        GameOver_Retry_Button.image.color = new Color(255, 80, 0);
        GameOver_Retry_Text.color = new Color(255, 255, 255);
        GameOver_Retry_Icon.color = new Color(255, 255, 255);

        SceneManager.LoadScene(4, LoadSceneMode.Single);
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }

    void Update()
    {
        GameIsOver();
    }
}

