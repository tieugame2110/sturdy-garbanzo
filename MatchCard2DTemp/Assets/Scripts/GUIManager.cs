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
    public Button playBtn;
    public Button homeBtn;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        AddEventPlayBtn();
        AddEventHomeBtn();
    }
    void AddEventPlayBtn()
	{
        playBtn.onClick.AddListener(() => {
            homeScreen.SetActive(false);
            //start game
        });
	}
    void AddEventHomeBtn()
    {
        homeBtn.onClick.AddListener(() => {
            homeScreen.SetActive(true);
            //start game
        });
    }
    public void UpdateUI()
	{

	}
}
