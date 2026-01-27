using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent<CardData> OnCardClicked;

    [Header("Setup")]
    [SerializeField] Image icon;

    [Header("Tween - Draw")]
    [SerializeField] float cardDrawTweenDuration = .2f;

    [Header("Tween - Select")]
    [SerializeField] float cardSelectTweenDuration = .1f;
    [SerializeField] float cardSelectTweenYOffset = 50f;
    [SerializeField] float cardSelectScaleMultiplier = 1.2f;

    [Header("Tween - Deselect")]
    [SerializeField] float cardDeselectTweenDuration = .1f;

    CardData cardData;
    Sequence currentTween;

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;

        icon.sprite = cardData.CardImage;
    }

    public void PlayDrawCardTween()
    {
        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            transform.DOLocalMove(Vector3.zero, cardDrawTweenDuration).SetEase(Ease.OutQuad));
    }

    public void PlaySelectCardTween()
    {
        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            transform.DOLocalMove(Vector3.up * cardSelectTweenYOffset, cardSelectTweenDuration).SetEase(Ease.OutQuad));

        currentTween.Join(
            transform.DOScale(Vector3.one * cardSelectScaleMultiplier, cardSelectTweenDuration).SetEase(Ease.OutQuad));
    }

    public void PlayDeselectCardTween()
    {
        TryKillTween();

        currentTween = DOTween.Sequence();

        currentTween.Append(
            transform.DOLocalMove(Vector3.zero, cardDeselectTweenDuration).SetEase(Ease.OutQuad));

        currentTween.Join(
            transform.DOScale(Vector3.one, cardDeselectTweenDuration).SetEase(Ease.OutQuad));
    }

    private void TryKillTween()
    {
        currentTween?.Kill();
        currentTween = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnCardClicked?.Invoke(cardData);
    }
}
