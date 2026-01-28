using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Canvas))]
public class UICard : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent<CardData> OnCardClicked;

    public CardData CardData => cardData;

    [Header("Setup")]
    [SerializeField] Image icon;

    [Header("Tween - Draw")]
    [SerializeField] float cardDrawTweenDuration = .2f;

    [Header("Tween - Select")]
    [SerializeField] float cardSelectTweenDuration = .1f;
    [SerializeField] float cardSelectTweenYOffset = 50f;
    [SerializeField] float cardSelectScaleMultiplier = 1.2f;
    [SerializeField] int canvasSortingOrderOnSelect= 11;

    [Header("Tween - Deselect")]
    [SerializeField] float cardDeselectTweenDuration = .1f;
    [SerializeField] int canvasSortingOrderOnDeselect= 10;

    [Header("Tween - Played")]
    [SerializeField] float cardPlayedTweenDuration = .1f;

    CardData cardData;
    Sequence currentTween;
    CanvasGroup canvasGroup;
    Canvas canvas;
    bool isCardPlayed = false;

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;

        icon.sprite = cardData.CardImage;

        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }

    public void PlayDrawCardTween()
    {
        if (isCardPlayed)
            return;

        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            transform.DOLocalMove(Vector3.zero, cardDrawTweenDuration).SetEase(Ease.OutQuad));
    }

    public void PlaySelectCardTween()
    {
        if (isCardPlayed)
            return;

        TryKillTween();

        currentTween = DOTween.Sequence();
        currentTween.AppendCallback(
            () => canvas.sortingOrder = canvasSortingOrderOnSelect);

        currentTween.Append(
            transform.DOLocalMove(Vector3.up * cardSelectTweenYOffset, cardSelectTweenDuration).SetEase(Ease.OutQuad));

        currentTween.Join(
            transform.DOScale(Vector3.one * cardSelectScaleMultiplier, cardSelectTweenDuration).SetEase(Ease.OutQuad));
    }

    public void PlayDeselectCardTween()
    {
        if (isCardPlayed)
            return;

        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            transform.DOLocalMove(Vector3.zero, cardDeselectTweenDuration).SetEase(Ease.OutQuad));

        currentTween.Join(
            transform.DOScale(Vector3.one, cardDeselectTweenDuration).SetEase(Ease.OutQuad));

        currentTween.AppendCallback(
            () => canvas.sortingOrder = canvasSortingOrderOnDeselect);
    }

    public void PlayPlayedCardTween()
    {
        if (isCardPlayed)
            return;

        isCardPlayed = true;

        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            canvasGroup.DOFade(0, cardPlayedTweenDuration).SetEase(Ease.OutQuad));

        currentTween.AppendCallback(
            () => Destroy(gameObject));
    }

    private void TryKillTween()
    {
        currentTween?.Kill();
        currentTween = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCardPlayed)
            return;

        OnCardClicked?.Invoke(cardData);
    }
}
