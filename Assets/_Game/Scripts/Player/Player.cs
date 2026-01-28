using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const string OPACITY_KEY = "_Opacity";
    const string COLOR_KEY = "_ColorMultiplier";
    const string START_JUMP_TRIGGER = "JumpStart";
    const string END_JUMP_TRIGGER = "JumpEnd";

    [SerializeField] Transform rotationTarget;

    [Header("Tween - Jump")]
    [SerializeField] float jumpDuration = .3f;
    [SerializeField] float jumpHeight = 5;

    [Header("Tween - Rotate")]
    [SerializeField] float rotateDuration = .35f;

    float currentJumpFadeProgress;
    Animator animator;
    Dictionary<Renderer, MaterialPropertyBlock> bodyParts;

    Sequence currentMoveTween;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        bodyParts = new Dictionary<Renderer, MaterialPropertyBlock>();

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bodyParts.Add(renderer, new MaterialPropertyBlock());
            renderer.GetPropertyBlock(bodyParts[renderer]);
        }
    }

    public void MoveToPosition(Vector3 targetPos, bool reverseRotation)
    {
        TryKillCurrentMoveTween();

        currentMoveTween = DOTween.Sequence();

        currentMoveTween.AppendCallback(() => animator.SetTrigger(START_JUMP_TRIGGER));
        currentMoveTween.Append(RotateTowardsPositionTween(targetPos, jumpDuration / 4, reverseRotation));

        currentMoveTween.Join(transform.DOMove(transform.position + Vector3.up * jumpHeight, jumpDuration / 2).SetEase(Ease.Linear));

        currentMoveTween.Join(
            DOTween.To(
                getter: () => currentJumpFadeProgress,
                setter: x => SetJumpFadeProgress(x),
                endValue: 0,
                duration: jumpDuration / 3).SetEase(Ease.OutQuint));

        currentMoveTween.AppendInterval(jumpDuration * .02f);
        currentMoveTween.AppendCallback(() => transform.position = targetPos + Vector3.up * jumpHeight);
        currentMoveTween.AppendInterval(jumpDuration * .02f);

        currentMoveTween.AppendCallback(() => animator.SetTrigger(END_JUMP_TRIGGER));
        currentMoveTween.Append(transform.DOMove(targetPos, jumpDuration / 2).SetEase(Ease.Linear));

        currentMoveTween.Join(
            DOTween.To(
                getter: () => currentJumpFadeProgress,
                setter: x => SetJumpFadeProgress(x),
                endValue: 1,
                duration: jumpDuration / 3).SetEase(Ease.InQuint));
    }

    public void RotateTowardsPosition(Vector3 targetPosition, bool reverseRotation = false)
    {
        TryKillCurrentMoveTween();

        currentMoveTween = DOTween.Sequence();
        currentMoveTween.Append(RotateTowardsPositionTween(targetPosition, rotateDuration, reverseRotation));
    }

    Tween RotateTowardsPositionTween(Vector3 targetPosition, float duration, bool reverseRotation = false)
    {
        float angle = Mathf.Atan2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z) * Mathf.Rad2Deg;

        angle += reverseRotation ? 180 : 0;
        return rotationTarget.DOLocalRotate(new Vector3(0, angle, 0), duration);
    }

    void TryKillCurrentMoveTween()
    {
        if (currentMoveTween != null)
        {
            currentMoveTween.Kill();
        }
    }

    public void SetJumpFadeProgress(float jumpFadeProgress)
    {
        currentJumpFadeProgress = jumpFadeProgress;

        foreach (Renderer renderer in bodyParts.Keys)
        {
            bodyParts[renderer].SetFloat(OPACITY_KEY, jumpFadeProgress);
            bodyParts[renderer].SetFloat(COLOR_KEY, jumpFadeProgress);
            renderer.SetPropertyBlock(bodyParts[renderer]);
        }
    }

    public void TriggerAnimation(PlayerAnimations animationID)
    {
        animator.SetTrigger(animationID.ToString());
    }
}
