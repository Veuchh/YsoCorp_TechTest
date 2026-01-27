using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    const string TEXTURE_ID = "_MainText";

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
