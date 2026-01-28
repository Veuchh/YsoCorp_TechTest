using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    float currentJumpFadeProgress;
    Animator animator;
    Dictionary<Renderer, MaterialPropertyBlock> bodyParts;

    Sequence currentTween;

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
        TryKillCurrentTween();

        currentTween = DOTween.Sequence();

        float angle = Mathf.Atan2(targetPos.x - transform.position.x, targetPos.z - transform.position.z) * Mathf.Rad2Deg;

        angle += reverseRotation ? 180 : 0;

        currentTween.AppendCallback(() => animator.SetTrigger(START_JUMP_TRIGGER));
        currentTween.Append(rotationTarget.DOLocalRotate(new Vector3(0, angle, 0), jumpDuration/4));

        currentTween.Join(transform.DOMove(transform.position + Vector3.up * jumpHeight, jumpDuration/2).SetEase(Ease.Linear));

        currentTween.Join(
            DOTween.To(
                getter:() => currentJumpFadeProgress,
                setter:x => SetJumpFadeProgress(x),
                endValue:0,
                duration: jumpDuration / 3).SetEase(Ease.OutQuint));

        currentTween.AppendInterval(jumpDuration * .02f);
        currentTween.AppendCallback(() => transform.position = targetPos + Vector3.up * jumpHeight);
        currentTween.AppendInterval(jumpDuration * .02f);

        currentTween.AppendCallback(() => animator.SetTrigger(END_JUMP_TRIGGER));
        currentTween.Append(transform.DOMove(targetPos, jumpDuration/2).SetEase(Ease.Linear));

        currentTween.Join(
            DOTween.To(
                getter: () => currentJumpFadeProgress,
                setter: x => SetJumpFadeProgress(x),
                endValue: 1,
                duration: jumpDuration / 3).SetEase(Ease.InQuint));
    }

    void TryKillCurrentTween()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
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
}
