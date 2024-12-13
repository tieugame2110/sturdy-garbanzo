using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchCards
{
	[CreateAssetMenu(fileName = "Card New", menuName = "Create New Card/CardSO", order = 1)]
	public class CardSO : ScriptableObject
	{
		public string CardID;

	}
}
