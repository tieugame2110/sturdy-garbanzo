using MatchCards;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : SerializedMonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    public List<Card> cardInBoard = new();
    public GameObject cardPrefab;
    public RectTransform boardParent; // Sửa thành RectTransform

    Queue<Card> flippedCardsQueue = new Queue<Card>();

    public RectTransform spawnPos; // Sửa thành RectTransform
    public Vector2 offset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Khởi tạo board
        //CreateBoard();
    }

    public void ClearAllCard()
    {
        if (cardInBoard.Count <= 0) return;
        // Duyệt qua tất cả các thẻ trong danh sách cardInBoard
        foreach (Card card in cardInBoard)
        {
            // Hủy GameObject của thẻ khỏi scene
            Destroy(card.gameObject);
        }

        // Xóa tất cả các thẻ trong danh sách
        cardInBoard.Clear();
    }

    public void EasyDif()
    {
        var rows = 3;
        var columns = 2;

        offset = new Vector2(420, 420);
        var _starPos = spawnPos.anchoredPosition + new Vector2(30, -30);
        SpawnCardMesh(rows, columns, _starPos, offset, 1);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 0;
    }

    public void NormalDif()
    {
        var rows = 4;
        var columns = 3;

        offset = new Vector2(420*0.6f, 420*0.7f);
        var _starPos = spawnPos.anchoredPosition; 
        SpawnCardMesh(rows, columns, _starPos, offset, 0.9f);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 1;
    }

    public void HardDif()
    {
        var rows = 5;
        var columns = 4;

        offset = new Vector2(420 * 0.4f, 420 * 0.5f);
        var _starPos = spawnPos.anchoredPosition;
        SpawnCardMesh(rows, columns, _starPos, offset, 0.8f);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 2;
    }

    void SpawnCardMesh(int rows, int columns, Vector2 Pos, Vector2 offset, float scaleDownValue)
    {
        // Clear tất cả card trước khi spawn
        ClearAllCard();
        var cardSOs = GameManager.Instance.cardSOs;

        // Kiểm tra số lượng thẻ cần spawn
        int totalCards = rows * columns;

        // Tạo danh sách các cặp thẻ
        List<CardSO> pairedCardSOs = new List<CardSO>();
        for (int i = 0; i < totalCards / 2; i++)
        {
            // Lặp lại CardSO nếu cần để tạo đủ số cặp
            pairedCardSOs.Add(cardSOs[i % cardSOs.Count]);
        }

        // Nhân đôi danh sách để tạo cặp
        pairedCardSOs.AddRange(pairedCardSOs);

        // Trộn ngẫu nhiên danh sách
        ShuffleList(pairedCardSOs);

        // Duyệt qua số hàng và số cột để tạo các thẻ
        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (cardIndex >= totalCards) return; // Đảm bảo không vượt quá số lượng cần thiết

                // Tính toán vị trí của mỗi thẻ
                Vector2 cardPosition = Pos + new Vector2(col * offset.x, -row * offset.y);

                // Chuyển đổi từ không gian Canvas (anchoredPosition) sang không gian thế giới (world space)
                Vector2 worldPosition = boardParent.TransformPoint(cardPosition);

				// Tạo thẻ tại vị trí đã tính toán
				//GameObject cardObject = Instantiate(cardPrefab, cardPosition, Quaternion.identity, boardParent);
				GameObject cardObject = PoolManager.Spawn(cardPrefab, worldPosition, Quaternion.identity);
				Card card = cardObject.GetComponent<Card>();

                // Gán CardSO cho thẻ
                card.Initialize(pairedCardSOs[cardIndex]);
                cardInBoard.Add(card);

                // Nếu muốn scale down thẻ (giảm kích thước)
                if (scaleDownValue < 1)
                {
                    cardObject.transform.localScale = new Vector3(scaleDownValue, scaleDownValue, 1f);
                }

                // Đăng ký sự kiện OnCardFlipped
                card.OnCardFlipped -= OnCardFlipped;
                card.OnCardFlipped += OnCardFlipped;

                cardIndex++;
            }
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    void OnCardFlipped(Card card)
    {
        // Nếu thẻ đã lật hoặc đã matched thì bỏ qua
        if (card.State == CardState.Flipped || card.State == CardState.Matched)
            return;

        flippedCardsQueue.Enqueue(card);

        if (flippedCardsQueue.Count % 2 == 0)
        {
            StartCoroutine(CheckMatch());
        }
    }




    IEnumerator CheckMatch()
    {

        // Lấy 2 thẻ từ hàng đợi
        Card card1 = flippedCardsQueue.Dequeue();
        Card card2 = flippedCardsQueue.Dequeue();
        yield return new WaitForSeconds(1f);
        // Kiểm tra match
        if (card1.Matches(card2))
        {
            card1.SetMatched();
            card2.SetMatched();
            cardInBoard.Remove(card1);
            cardInBoard.Remove(card2);
            SoundManager.Instance.PlaySound(SoundManager.Instance.matchedSound);
            GameManager.Instance.AddScore(5);
            GameManager.Instance.combo++;
            GameManager.Instance.gameTurn++;
            CancelInvoke(nameof(CheckEndGame));
            Invoke(nameof(CheckEndGame), 1f);
        }
        else
        {
            yield return new WaitForSeconds(0.25f); // Thời gian chờ trước khi úp lại
            card1.FlipBack();
            card2.FlipBack();
            SoundManager.Instance.PlaySound(SoundManager.Instance.mismatchSound);
            GameManager.Instance.combo = 0;
            GameManager.Instance.AddScore(0);
            GameManager.Instance.gameTurn++;
        }
    }

    private void CheckEndGame()
    {
        if (cardInBoard.Count <= 0)
        {
            GameManager.Instance.EndGame();
        }
    }
}