using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : SerializedMonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    public List<Card> cardInBoard = new();
    public GameObject cardPrefab;
    public Transform boardParent;

    Queue<Card> flippedCardsQueue = new Queue<Card>();


    public Vector2 startPos;
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
        startPos = new Vector2(100, 420);
        offset = new Vector2(150, 150);
        SpawnCardMesh(rows, columns, startPos, offset, 1);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 0;
    }

    public void NormalDif()
    {
        var rows = 4;
        var columns = 3;
        startPos = new Vector2(50, 520);
        offset = new Vector2(125, 150);
        SpawnCardMesh(rows, columns, startPos, offset, 0.9f);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 1;
    }

    public void HardDif()
    {
        var rows = 5;
        var columns = 4;
        startPos = new Vector2(50, 520);
        offset = new Vector2(85, 110);
        SpawnCardMesh(rows, columns, startPos, offset, 0.8f);
        EventManager.EventRevealCard();
        GameManager.Instance.curDif = 2;
    }

    void SpawnCardMesh(int rows, int columns, Vector2 Pos, Vector2 offset, float scaleDownValue)
    {
        // Duyệt qua số hàng và số cột để tạo các thẻ
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Tính toán vị trí của mỗi thẻ
                Vector2 cardPosition = Pos + new Vector2(col * offset.x, -row * offset.y);

                // Tạo thẻ tại vị trí đã tính toán
                GameObject cardObject = Instantiate(cardPrefab, cardPosition, Quaternion.identity, boardParent);
                Card card = cardObject.GetComponent<Card>();
                cardInBoard.Add(card);

                // Nếu muốn scale down thẻ (giảm kích thước)
                if (scaleDownValue < 1)
                {
                    cardObject.transform.localScale = new Vector3(scaleDownValue, scaleDownValue, 1f); // Ví dụ giảm kích thước xuống 80%
                }

                // Đăng ký sự kiện OnCardFlipped
                card.OnCardFlipped += OnCardFlipped;
            }
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
        yield return new WaitForSeconds(1f);
        // Lấy 2 thẻ từ hàng đợi
        Card card1 = flippedCardsQueue.Dequeue();
        Card card2 = flippedCardsQueue.Dequeue();

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
