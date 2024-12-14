using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum CardState
{
    Hidden,
    Flipped,
    Matched
}
public class Card : MonoBehaviour
{
    public CardSO cardSO;
    public int CardID { get; private set; }
    public CardState State { get; private set; } = CardState.Hidden;

    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;
    private Button cardBtn;
    [SerializeField] private Image cardImage;

    public Action<Card> OnCardFlipped;

	private void Awake()
	{
        cardBtn = GetComponent<Button>();
	}
	// Start is called before the first frame update
	void Start()
    {
        AddEventCardBtn();
    }
    void AddEventCardBtn()
	{
        cardBtn.onClick.AddListener(() => {
            Flip();
        });

    }
    public void Initialize(CardSO _card)
    {
        cardSO = _card;
        CardID = cardSO.CardID;
        cardImage.sprite = cardSO.CardSprite;
        SetState(CardState.Hidden);
    }

    public void Flip()
    {
        if (State == CardState.Hidden)
        {
            SetState(CardState.Flipped);
            OnCardFlipped?.Invoke(this);
            SoundManager.Instance.PlaySound(SoundManager.Instance.revealCard);
        }
    }

    public void FlipBack()
    {
        if (State == CardState.Flipped)
        {
            SetState(CardState.Hidden);
        }
    }
    public void SetMatched()
    {
        SetState(CardState.Matched);
    }


    public bool Matches(Card otherCard)
    {
        return CardID == otherCard.CardID;
    }
    private void SetState(CardState newState)
    {
        State = newState;
        UpdateVisuals();
    }
    [Button]
    void TestMatched()
	{
        State = CardState.Matched;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        switch (State)
        {
            case CardState.Hidden:
                frontFace.SetActive(false);
                backFace.SetActive(true);
                break;
            case CardState.Flipped:
                transform.DOScaleX(0, 0.5f).OnComplete(() => {

                    transform.DOScaleX(1, 0.5f);
                    frontFace.SetActive(true);
                    backFace.SetActive(false);
                });
                break;
            case CardState.Matched:
                frontFace.SetActive(true);
                backFace.SetActive(false);
                transform.DOShakeScale(0.5f).OnComplete(() => {
                    gameObject.SetActive(false);
                });

                break;
        }
    }
}
