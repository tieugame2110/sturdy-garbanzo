using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<CardSO> cardSOs = new();
    public int playerScore = 0;
    public int combo = 0;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }

    public void AddScore(int amount)
    {
        playerScore += amount;
        Debug.Log($"Score: {playerScore}");
        GUIManager.Instance.UpdateUI();
    }
    public void ResetGame()
	{
        playerScore = 0;
    }
}
