using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    const string TEXTURE_ID = "_MainText";

    [SerializeField] ParticleSystem attackPS;
    [SerializeField] float attackEffectDuration = .25f;

    new Renderer renderer;
    MaterialPropertyBlock mpb;
    Collider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<Collider>();
        renderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
    }

    public async void PlayAttackPS()
    {
        attackPS.Play();
        await UniTask.Delay(Mathf.RoundToInt(attackEffectDuration * 1000));
        attackPS.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    public Collider GetCollider()
    {
        return boxCollider;
    }

    public void SetTexture(Texture newTexture)
    {
        mpb.SetTexture(TEXTURE_ID, newTexture);
        renderer.SetPropertyBlock(mpb);
    }
}
