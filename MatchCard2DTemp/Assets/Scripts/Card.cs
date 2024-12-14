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
    [OnValueChanged("Initialize")]public CardSO cardSO;
    public int CardID;
    public CardState State { get; private set; } = CardState.Hidden;

    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;
    private Button cardBtn;
    [SerializeField] private Image cardImage;

    public Action<Card> OnCardFlipped;
    private Vector3 scaleMemory;

	private void Awake()
	{
        EventManager.EventRevealCard += RevealCard;
        cardBtn = GetComponent<Button>();
	}
	// Start is called before the first frame update
	void Start()
    {
        scaleMemory = transform.localScale; 
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
            OnCardFlipped?.Invoke(this);
            SetState(CardState.Flipped);
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
    void TestReveal()
	{
        State = CardState.Flipped;
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

                    transform.DOScaleX(scaleMemory.x, 0.5f);
                    frontFace.SetActive(true);
                    backFace.SetActive(false);
                });
                break;
            case CardState.Matched:
                frontFace.SetActive(true);
                backFace.SetActive(false);
                transform.DOShakeScale(0.5f).OnComplete(() => {
                    //gameObject.SetActive(false);
                    Destroy(gameObject);
                });

                break;
        }
    }
    public void RevealCard()
	{
        State = CardState.Flipped;
        frontFace.SetActive(true);
        backFace.SetActive(false);
        Invoke(nameof(FlipBack),3);
    }
	private void OnDisable()
	{
        EventManager.EventRevealCard -= RevealCard;
    }
}
