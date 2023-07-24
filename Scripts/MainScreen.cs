using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : GameManager,IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Transform Block_Spin;
    public float Spinning_Speed = 0.05f;
    private Image SwitchImage;

    private Image Main_Castle_Glow;
    private float GlowingInterval = 5f;
    private Text Title;
    
    private AudioSource BGM;

    public Button GameStart;
    public Button Score;
    public Button Option;
    public Button Quit;

    public Image Quit_Message;
    public Collider Quit_BackScreen;
    public Button Quit_Yes;
    public Button Quit_No;

    public void Awake()
    {
        Block_Spin = GameObject.Find("Block_Spin").GetComponent<Transform>();
        SwitchImage = Resources.Load<Image>("Resources/Main_Button_Click");

        Main_Castle_Glow = GameObject.Find("Main_Castle_Glow").GetComponent<Image>();
        Title = GameObject.Find("Main_Title").GetComponent<Text>();
        BGM = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        BGM.loop = true; BGM.playOnAwake = true;

        GameStart = GameObject.Find("GameStart").AddComponent<Button>();
        Score = GameObject.Find("LeaderBoard").AddComponent<Button>();
        Option = GameObject.Find("Option").AddComponent<Button>();
        Quit = GameObject.Find("Quit").AddComponent<Button>();

        Quit_Message = GameObject.Find("Quit_YM_Message").GetComponent<Image>();
        Quit_Yes = GameObject.Find("Quit_Button_Y").AddComponent<Button>();
        Quit_No = GameObject.Find("Quit_Button_N").AddComponent<Button>();
        Quit_BackScreen = GameObject.Find("Quit_BackScreen").GetComponent<Collider>(); Quit_BackScreen.gameObject.SetActive(false);

        GameStart.onClick.AddListener(StartGame);
        Option.onClick.AddListener(ShowOption);
        Score.onClick.AddListener(ShowScore);
        Quit.onClick.AddListener(ShowQuit);
    }

    public IEnumerator RotateBlock()
    {
        Block_Spin.Rotate(new Vector3(Spinning_Speed * Time.deltaTime, 0), Space.World);
        yield return null;
    }
    public IEnumerator PressButton(Button btn, Vector3 newsize)
    {
        float timer = 0;
        Vector3 Original_Size = btn.transform.localScale;

        while (timer < Spinning_Speed)
        { 
            timer += Time.deltaTime;
            yield return null;
            btn.transform.localScale = Vector3.Lerp(Original_Size, newsize, timer / Spinning_Speed);
        }
    }
    public IEnumerator GlowingCastle()
    {
        float time = 0.0f;
        while (time < GlowingInterval)
        {
            Main_Castle_Glow.color = Color.LerpUnclamped(Color.clear, Color.white, time / GlowingInterval);
            time += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator TitleBooming()
    {
        float time = 0.0f;
        Vector3 original = Title.transform.localScale;
        Vector3 Boom = new Vector3(Title.transform.localScale.x + 0.01f, Title.transform.localScale.y + 0.01f);
        while (time < Spinning_Speed)
        {
            Title.transform.localScale = Vector3.Lerp(Title.transform.localScale, Boom, 3f * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForSeconds(1f);
            Title.transform.localScale = Vector3.Lerp(Title.transform.localScale, original, 3f * Time.deltaTime);
            yield return new WaitForSeconds(1f);
        }
    }
    

    public void StartGame()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }
    public void ShowScore()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
    public void ShowOption()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
    public void ShowQuit()
    {
        Quit_BackScreen.gameObject.SetActive(true);

        Quit_Yes.onClick.AddListener(Application.Quit);

        Quit_No.onClick.AddListener(delegate { Quit_BackScreen.gameObject.SetActive(false); });
    }
    public void Update()
    {
        StartCoroutine(RotateBlock()); //If Game Starts, off this
        StartCoroutine(GlowingCastle());
        StartCoroutine(TitleBooming());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(PressButton(GameStart, GameStart.transform.localScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}