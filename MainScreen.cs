using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    public Transform Block_Spin;
    public float Spinning_Speed = 0.05f;
    
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

        GameStart = GameObject.Find("GameStart").AddComponent<Button>();
        Score = GameObject.Find("LeaderBoard").AddComponent<Button>();
        Option = GameObject.Find("Option").AddComponent<Button>();
        Quit = GameObject.Find("Quit").AddComponent<Button>();

        Quit_Message = GameObject.Find("Quit_YM_Message").GetComponent<Image>();
        Quit_Yes = GameObject.Find("Quit_Button_Y").AddComponent<Button>();
        Quit_No = GameObject.Find("Quit_Button_N").AddComponent<Button>();
        Quit_BackScreen = GameObject.Find("Quit_BackScreen").GetComponent<Collider>(); Quit_BackScreen.gameObject.SetActive(false);

        GameStart.onClick.AddListener(StartGame);
        Score.onClick.AddListener(ShowScore);
        Option.onClick.AddListener(ShowOption);
        Quit.onClick.AddListener(ShowQuit);


    }

    public IEnumerator RotateBlock()
    {
        Block_Spin.Rotate(new Vector3( Spinning_Speed * Time.deltaTime, 0), Space.World);
        yield return null;
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
    }
}