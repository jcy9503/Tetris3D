using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    public Button Home_Button;
    public Image goldCrown; public Gradient goldGradient;
    public Image silverCrown; public Gradient silverGradient;
    public Image bronzeCrown; public Gradient bronzeGradient;

    public float colorKey;

    delegate void ShinyImage();
    void Start()
    {
        Home_Button = GameObject.Find("Back").AddComponent<Button>();
        Home_Button.onClick.AddListener(BackToMainScreen);

        goldCrown = GameObject.Find("Crown 1").GetComponent<Image>();
        silverCrown = GameObject.Find("Crown 2").GetComponent<Image>();
        bronzeCrown = GameObject.Find("Crown 3").GetComponent<Image>();
    }

    private void BackToMainScreen()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }


    public void ShinyCrown()
    { 
        
    }

    void Update()
    {
        
    }
}
