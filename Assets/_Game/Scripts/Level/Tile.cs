using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Collider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<Collider>();
    }

    public Collider GetCollider()
    {
        return boxCollider;
    }
}
