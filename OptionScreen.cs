using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionScreen : MonoBehaviour
{
    public Button Home_Button;
    

    //Sound Resources
    public Button SoundTab { get; private set; }
    public Image SoundPanel { get; private set; }
    private Slider BGM_Slider;
    private Slider SFX_Slider;


    //GamePlay Resources
    public Button GameplayTab { get; private set; }
    public Image GameplayPanel { get; private set; }
    public enum Language
    {
        English, Korean
    }
    public Language lang;
    delegate void OnLanguageChange(Language language);
    
    private Button Language_Select;
    private Text Language_Current;
    private string Language_Default_Text = "English";
    private string Language_Korean_Text = "ÇÑ±¹¾î";
    private Image Language_List_Arrow;

    private Image Language_List;
    private Button Language_English;
    private Button Language_Korean;


    //Key Setting Resources
    public Button KeySettingTab { get; private set; }
    public Image KeySettingPanel { get; private set; }
    public Button Help_Button;
    public Image Help_Window;






    

    void Start()
    {
        
        Home_Button = GameObject.Find("Back").AddComponent<Button>();
        Home_Button.onClick.AddListener(BackToMainScreen);


        //Tab Button
        SoundTab = GameObject.Find("Tab_Sound").AddComponent<Button>();
        SoundTab.onClick.AddListener(Open_SoundTab);
        GameplayTab = GameObject.Find("Tab_GamePlay").AddComponent<Button>();
        GameplayTab.onClick.AddListener(Open_GamePlayTab);
        KeySettingTab = GameObject.Find("Tab_Key").AddComponent<Button>();
        KeySettingTab.onClick.AddListener(Open_KeySettingTab);


        //Sound Bars
        BGM_Slider = GameObject.Find("Frame_BGM").AddComponent<Slider>();
        BGM_Slider.fillRect = GameObject.Find("Gauge_BGM").GetComponent<RectTransform>();
        BGM_Slider.minValue = 0; BGM_Slider.maxValue = 100; BGM_Slider.value = 100;
        BGM_Slider.direction = Slider.Direction.LeftToRight;

        SFX_Slider = GameObject.Find("Frame_SFX").AddComponent<Slider>();
        SFX_Slider.fillRect = GameObject.Find("Gauge_SFX").GetComponent<RectTransform>();
        SFX_Slider.minValue = 0; SFX_Slider.maxValue = 100; SFX_Slider.value = 100;
        SFX_Slider.direction = Slider.Direction.LeftToRight;


        //Gameplay Settings
        Language_Select = GameObject.Find("ComboBox_GamePlay").AddComponent<Button>();
        Language_Select.onClick.AddListener(Open_LanguageList);
        Language_List_Arrow = GameObject.Find("Arrow").GetComponent<Image>();
        Language_Current = GameObject.Find("Language_Current").GetComponent<Text>();
        Language_Current.text = Language_Default_Text;

        Language_List = GameObject.Find("Language_List").GetComponent<Image>();
        
        Language_English = GameObject.Find("Language_English").AddComponent<Button>();
        Language_English.onClick.AddListener(delegate { Debug.Log("English"); });

        Language_Korean = GameObject.Find("Language_Korean").AddComponent<Button>();
        Language_Korean.onClick.AddListener(delegate { Debug.Log("Korean"); });

        Language_List.gameObject.SetActive(false);


        //Key Setting
        Help_Button = GameObject.Find("Help").AddComponent<Button>();
        Help_Button.onClick.AddListener(Open_Help_Info);
        Help_Window = GameObject.Find("Help_Info").GetComponent<Image>(); Help_Window.gameObject.SetActive(false);


        //Tabs
        SoundPanel = GameObject.Find("Panel_Sound").GetComponent<Image>(); SoundPanel.gameObject.SetActive(true);
        GameplayPanel = GameObject.Find("Panel_GamePlay").GetComponent<Image>(); GameplayPanel.gameObject.SetActive(false);
        KeySettingPanel = GameObject.Find("Panel_Key").GetComponent<Image>(); KeySettingPanel.gameObject.SetActive(false);
    }


    private void BackToMainScreen()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Open_SoundTab()
    {
        SoundPanel.gameObject.SetActive(true);
        GameplayPanel.gameObject.SetActive(false);
        KeySettingPanel.gameObject.SetActive(false);
    }
    public void Open_GamePlayTab()
    {
        SoundPanel.gameObject.SetActive(false);
        GameplayPanel.gameObject.SetActive(true);
        KeySettingPanel.gameObject.SetActive(false);
    }
    public void Open_KeySettingTab()
    {
        SoundPanel.gameObject.SetActive(false);
        GameplayPanel.gameObject.SetActive(false);
        KeySettingPanel.gameObject.SetActive(true);

    }
    public void Open_LanguageList()
    {
        if (!Language_List.gameObject.activeSelf)
        {
            Language_List.gameObject.SetActive(true);
            Language_List_Arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            
        }
        else
        {
            Language_List.gameObject.SetActive(false);
            Language_List_Arrow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    public void Open_Help_Info()
    {
        if (!Help_Window.gameObject.activeSelf)
        {
            Help_Window.gameObject.SetActive(true);
        }
        else
        {
            Help_Window.gameObject.SetActive(false);
        }
    }

    public void Change_Language(Language lang)
    {
        switch (lang)
        {
            case Language.English:
                
                Language_List.gameObject.SetActive(false);
                return;
            case Language.Korean:
                
                Language_List.gameObject.SetActive(false);
                return;

            default: Debug.Log("Error : OptionScreen/Change_Language");
                break;
        }
    }

    

    void Update()
    {
        

    }
}
