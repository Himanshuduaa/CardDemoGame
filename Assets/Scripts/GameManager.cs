using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardGrid;
    public TextMeshProUGUI statsText;
    public Sprite cardBackSprite;
    public List<Sprite> cardFrontSprites;
    public GameStateManager gameStateManager;

    [SerializeField] private int noOfRows;
    [SerializeField] private int noOfColumns;

    private List<Card> cards = new List<Card>();
    private Card firstSelectedCard;
    private Card secondSelectedCard;

    [SerializeField] private int matchCount = 0;
    [SerializeField] private int turnsCount = 0;

    private bool canClick = true;

    private void Start()
    {
        StartGame(noOfRows, noOfColumns);
        UpdateUI();
    }

    public void StartGame(int rows, int cols)
    {
        ClearGrid();
        CreateGrid(rows, cols);
    }

    private void ClearGrid()
    {
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();
    }

    public void CreateGrid(int rows, int cols)
    {
        int totalCards = rows * cols;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Grid size must be even for pairing.");
            return;
        }

        int numberOfPairs = totalCards / 2;
        List<Sprite> pairedSprites = new List<Sprite>();

        for (int i = 0; i < numberOfPairs; i++)
        {
            Sprite sprite = cardFrontSprites[i % cardFrontSprites.Count];
            pairedSprites.Add(sprite);
            pairedSprites.Add(sprite);
        }

        pairedSprites.Shuffle();

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();

            card.CardID = i;
            card.SetupCard(pairedSprites[i], cardBackSprite, this);
            cards.Add(card);
        }
    }

    public void OnCardSelected(Card selectedCard)
    {
        if (!canClick || selectedCard == firstSelectedCard || selectedCard.IsMatched) return;

        if (firstSelectedCard == null || !firstSelectedCard.IsFlipped)
        {
            firstSelectedCard = selectedCard;
            firstSelectedCard.FlipCard(true);
        }
        else
        {
            secondSelectedCard = selectedCard;
            secondSelectedCard.FlipCard(true);
            turnsCount++;
            UpdateUI();
            CheckMatch();
        }
    }


    private void CheckMatch()
    {
        if (firstSelectedCard == null || secondSelectedCard == null) return;

        canClick = false;

        if (firstSelectedCard.IsMatch(secondSelectedCard))
        {
            matchCount++;
            UpdateUI();
            DestroyMatchedCards();
        }
        else
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                firstSelectedCard.ResetCard();
                secondSelectedCard.ResetCard();
                ResetSelection();
                canClick = true;
            });
        }
    }



    private void DestroyMatchedCards()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            firstSelectedCard.DestroyCard();
            secondSelectedCard.DestroyCard();
            ResetSelection();
            canClick = true;
        });
    }


    private void ResetSelection()
    {
        firstSelectedCard = null;
        secondSelectedCard = null;
    }


    private void UpdateUI()
    {
        statsText.text = $"Matches: {matchCount}  |  Turns: {turnsCount}";
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            matchCount = matchCount,
            turnsCount = turnsCount,
            cards = new List<CardData>()
        };

        foreach (Card card in cards)
        {
            saveData.cards.Add(new CardData
            {
                cardID = card.CardID,
                spriteIndex = cardFrontSprites.IndexOf(card.FrontSprite),
                isMatched = card.IsMatched,
                isFlipped = card.IsFlipped
            });
        }

        gameStateManager.SaveGame(saveData);
    }

    public void LoadGame()
    {
        SaveData saveData = gameStateManager.LoadGame();
        if (saveData == null)
        {
            Debug.Log("No save data found. Starting a new game.");
            StartGame(noOfRows, noOfColumns);
            return;
        }

        if (saveData.cards.Count != noOfRows * noOfColumns)
        {
            Debug.LogError("Save data does not match the current grid size. Starting a new game.");
            StartGame(noOfRows, noOfColumns);
            return;
        }

        matchCount = saveData.matchCount;
        turnsCount = saveData.turnsCount;

        ClearGrid();
        firstSelectedCard = null;

        foreach (var cardData in saveData.cards)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();

            Sprite frontSprite = cardFrontSprites[cardData.spriteIndex];
            card.CardID = cardData.cardID;
            card.SetupCard(frontSprite, cardBackSprite, this);
            cards.Add(card);
            card.SetState(cardData.isFlipped, cardData.isMatched);

            if (cardData.isFlipped && !cardData.isMatched && firstSelectedCard == null)
            {
                firstSelectedCard = card;
            }
        }

        UpdateUI();
    }


    public void OnSaveButtonClicked()
    {
        SaveGame();
    }

    public void OnLoadButtonClicked()
    {
        LoadGame();
    }
}
