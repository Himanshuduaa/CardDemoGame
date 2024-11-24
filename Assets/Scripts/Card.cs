using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [Header("Card Components")]
    public Image cardImage;
    public Button cardButton;

    [Header("Card Sprites")]
    public Sprite FrontSprite; 
    public Sprite BackSprite;

    [Header("Card Properties")]
    private GameManager gameManager; 
    public int CardID { get; set; } 
    public bool IsMatched { get; private set; }
    public bool IsFlipped { get; private set; }

    public void SetupCard(Sprite frontSprite, Sprite backSprite, GameManager manager)
    {
        Debug.Log($"Setting up CardID {CardID} with frontSprite: {frontSprite.name} and backSprite: {backSprite.name}");
        FrontSprite = frontSprite;
        BackSprite = backSprite;
        cardImage.sprite = backSprite;
        gameManager = manager;
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnCardClicked);
    }
    public void SetState(bool isFlipped, bool isMatched)
    {
        Debug.Log($"Restoring CardID {CardID} | isFlipped: {isFlipped}, isMatched: {isMatched}");

        IsMatched = isMatched;
        IsFlipped = isFlipped;

        if (IsMatched)
        {
            gameObject.SetActive(false);
        }
        else
        {
            cardImage.sprite = IsFlipped ? FrontSprite : BackSprite;
            gameObject.SetActive(true);
        }
    }

    private void OnCardClicked()
    {
        if (!IsMatched && !IsFlipped)
        {
            gameManager.OnCardSelected(this);
        }
    }

    public void FlipCard(bool showFront)
    {
        Debug.Log($"Flipping CardID {CardID} to {(showFront ? "front" : "back")} side");
        if (IsFlipped == showFront) return;

        float flipDuration = 0.5f;

        transform.DORotate(new Vector3(0, 180f, 0), flipDuration, RotateMode.FastBeyond360)
            .OnUpdate(() =>
            {
                if (Mathf.Abs(transform.rotation.eulerAngles.y - 90f) < 10f)
                {
                    cardImage.sprite = showFront ? FrontSprite : BackSprite;
                }
            })
            .OnComplete(() =>
            {
                IsFlipped = showFront;
            });
    }
    public void SetMatched(bool matched)
    {
        IsMatched = matched;
        if (matched)
        {
            DisableCard();
        }
    }
    public void ResetCard()
    {
        IsFlipped = false;
        IsMatched = false;
        cardImage.sprite = BackSprite;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public bool IsMatch(Card otherCard)
    {
        return otherCard != null && FrontSprite.name == otherCard.FrontSprite.name;
    }
    private void DisableCard()
    {
        cardButton.interactable = false;
        transform.DOScale(Vector3.zero, 0.5f)
                 .OnComplete(() => Destroy(gameObject));
    }

    public void DestroyCard()
    {
        IsMatched = true;
        gameObject.SetActive(false);
    }
}
