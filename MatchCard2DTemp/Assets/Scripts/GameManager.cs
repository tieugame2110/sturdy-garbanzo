using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public List<CardSO> cardSOs = new();
	public int curDif = 0;
	public int playerScore = 0;
	public int combo = 0;
	public int gameTurn = 0;

	private void Awake()
	{

		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		ConstantManager.userData.LoadData();
	}
	private void Start()
	{

	}

	public void AddScore(int _score)
	{
		playerScore = playerScore + (_score * (combo+1));
		GUIManager.Instance.UpdateGameUI();
	}
	public void ResetGame()
	{
		playerScore = 0;
		combo = 0;
		gameTurn = 0;
	}
	public void EndGame()
	{
		GUIManager.Instance.nextScreen.SetActive(true);
		ConstantManager.userData.lastScore = playerScore;
		if(ConstantManager.userData.highScore < playerScore)
		{
			ConstantManager.userData.highScore = playerScore;
		}
		ConstantManager.userData.SaveData();
	}
	public void NextGame()
	{
		BoardManager.Instance.ClearAllCard();
		if (curDif < 2)
		{
			curDif++;
		}
		switch (curDif)
		{
			case 0:
				BoardManager.Instance.EasyDif();
				break;
			case 1:
				BoardManager.Instance.NormalDif();
				break;
			case 2:
				BoardManager.Instance.HardDif();
				break;
		}
	}
}
