using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Button SoundTab;
    public Button GamePlayTab;
    public Button KeySetting;

    public Button Gameplay_Slider; private int slider = 150;
    public Button Gameplay_Language;

    public Image SoundPanel;
    public Image GamePlayPanel;
    public Image KeyPanel;

    void Start()
    {
        //Initialize Components
        SoundTab = GameObject.Find("Tab_Sound").GetComponent<Button>();
        GamePlayTab = GameObject.Find("Tab_GamePlay").GetComponent<Button>();
        KeySetting = GameObject.Find("Tab_Key").GetComponent<Button>();

        Gameplay_Slider = GameObject.Find("Slder_Handle_GamePlay").GetComponent<Button>();
        Gameplay_Language = GameObject.Find("ComboBox_Button_GamePlay").GetComponent<Button>();

        SoundTab.onClick.AddListener(OpenSoundTab);
        GamePlayTab.onClick.AddListener(OpenGamePlayTab);
        KeySetting.onClick.AddListener(OpenKeySettingTab);

        Gameplay_Slider.onClick.AddListener(ChangeBlockColor);
        Gameplay_Language.onClick.AddListener(ChangeBlockColor);

        SoundPanel = GameObject.Find("Panel_Sound").GetComponent<Image>(); SoundPanel.gameObject.SetActive(true);
        GamePlayPanel = GameObject.Find("Panel_GamePlay").GetComponent<Image>(); GamePlayPanel.gameObject.SetActive(false);
        KeyPanel = GameObject.Find("Panel_Key").GetComponent<Image>(); KeyPanel.gameObject.SetActive(false);


    }
    public void ChangeBlockColor()
    {
        //change block color, combobox scroll
        Gameplay_Slider.transform.position =
            new Vector2(Gameplay_Slider.gameObject.transform.position.x + slider, Gameplay_Slider.gameObject.transform.position.y);
    }
    public void OpenSoundTab()
    {
        SoundPanel.gameObject.SetActive(true);
        GamePlayPanel.gameObject.SetActive(false);
        KeyPanel.gameObject.SetActive(false);
    }
    public void OpenGamePlayTab()
    {
        SoundPanel.gameObject.SetActive(false);
        GamePlayPanel.gameObject.SetActive(true);
        KeyPanel.gameObject.SetActive(false);
    }
    public void OpenKeySettingTab()
    {
        SoundPanel.gameObject.SetActive(false);
        GamePlayPanel.gameObject.SetActive(false);
        KeyPanel.gameObject.SetActive(true);
    }

    void Update()
    {
        
    }
}
