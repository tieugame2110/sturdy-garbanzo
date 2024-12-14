using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance { get; private set; }
    public TMP_Text scoreTxt;
    public TMP_Text comboTxt;
    public TMP_Text turnTxt;
    public GameObject homeScreen;
    public Button EasyBtn;
    public Button NorBtn;
    public Button HardBtn;
    public Button homeBtn;
    public Button muteBtn;
    private TMP_Text muteText;

    public TMP_Text highScoreTxt;
    public TMP_Text lastScoreTxt;
    public GameObject nextScreen;
    public Button nextBtn;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        muteText = muteBtn.GetComponentInChildren<TMP_Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadGameScore();
        LoadSoundStatus();
        AddEventEasyBtn();
        AddEventNorBtn();
        AddEventHardBtn();
        AddEventHomeBtn();
        AddEventMuteBtn();
        AddEventNextBtn();
    }

	private void AddEventNextBtn()
	{
        nextBtn.onClick.AddListener(() => {
            nextScreen.SetActive(false);
            GameManager.Instance.NextGame();
        });
    }

	void LoadGameScore()
	{
        highScoreTxt.text = $"Highest Score: {ConstantManager.userData.highScore}";
        lastScoreTxt.text = $"Last Score: {ConstantManager.userData.lastScore}";
    }

    private void AddEventMuteBtn()
    {
        muteBtn.onClick.AddListener(() => ToggleSound());
    }

    private void ToggleSound()
    {
        ConstantManager.userData.gameSound = 1 - ConstantManager.userData.gameSound;  
        UpdateMuteText();
        SoundManager.Instance.SetVolume(ConstantManager.userData.gameSound);
        ConstantManager.userData.SaveData();
    }

    void LoadSoundStatus()
    {
        var SoundM = SoundManager.Instance;
        UpdateMuteText();
        SoundM.SetVolume(ConstantManager.userData.gameSound);
    }

    private void UpdateMuteText()
    {
        muteText.text = ConstantManager.userData.gameSound == 1 ? "Sound: ON" : "Sound: OFF";
    }

    void AddEventEasyBtn()
	{
        EasyBtn.onClick.AddListener(() => {
            homeScreen.SetActive(false);
            BoardManager.Instance.EasyDif();
        });
	}
    void AddEventNorBtn()
    {
        NorBtn.onClick.AddListener(() => {
            homeScreen.SetActive(false);
            BoardManager.Instance.NormalDif();
        });
    }
    void AddEventHardBtn()
    {
        HardBtn.onClick.AddListener(() => {
            homeScreen.SetActive(false);
            BoardManager.Instance.HardDif();
        });
    }
    void AddEventHomeBtn()
    {
        homeBtn.onClick.AddListener(() => {
            homeScreen.SetActive(true);
            BoardManager.Instance.ClearAllCard();
            UpdateHomeScreen();
            GameManager.Instance.ResetGame();
            UpdateGameUI();
        });
    }
    public void UpdateHomeScreen()
	{
        LoadGameScore();
    }
    public void UpdateGameUI()
    {
        scoreTxt.text = $"{GameManager.Instance.playerScore}";
        comboTxt.text = $"{GameManager.Instance.combo}";
        turnTxt.text = $"{GameManager.Instance.gameTurn}";
    }
}
