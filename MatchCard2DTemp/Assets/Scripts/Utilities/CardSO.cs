using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Card New", menuName = "Create New Card/CardSO", order = 1)]
public class CardSO : ScriptableObject
{
	public int CardID;
	public Sprite CardSprite;
}