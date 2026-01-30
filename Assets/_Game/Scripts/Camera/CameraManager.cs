using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Padding (Camera Space Units)")]
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;

    [Header("Zoom")]
    [SerializeField] float zoomPaddingMultiplier = 3f;
    [SerializeField] float zoomDuration = .5f;
    [SerializeField] float zoomLingerDuration = .1f;

    [Header("Camera")]
    [SerializeField] float cameraDistance = 10f;

    Vector3 neutralPos;
    float neutralSize;

    Camera cam;
    Sequence currentTween;

    private async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        cam = GetComponent<Camera>();

        while (LevelHandler.Instance == null)
            await UniTask.NextFrame();

        LevelHandler.Instance.OnGridGenerated.AddListener(OnGridGenerated);
    }

    async void OnGridGenerated()
    {
        await UniTask.NextFrame();
        SetCameraPosition(LevelHandler.Instance.OngoingLevelData.Tiles);
    }

    async void SetCameraPosition(Tile[,] tiles)
    {
        await UniTask.NextFrame();

        List<Tile> tilesList = new List<Tile>();
        foreach (var tile in tiles)
            tilesList.Add(tile);

        var (center, size) = CalculateOrthoSize(tilesList);

        neutralPos = center;
        neutralSize = size;

        cam.transform.localPosition = center;
        cam.orthographicSize = size;
    }

    public void ZoomOnTile(Tile tile, bool zoomOut = true)
    {
        TryKillSequence();

        currentTween = DOTween.Sequence();

        var (zoomCenter, zoomSize) = CalculateOrthoSize(
            new List<Tile> { tile },
            zoomPaddingMultiplier);

        // Zoom in
        currentTween.Append(
            cam.transform.DOLocalMove(zoomCenter, zoomDuration * 0.5f)
                .SetEase(Ease.InQuad));

        currentTween.Join(
            DOTween.To(
                () => cam.orthographicSize,
                x => cam.orthographicSize = x,
                zoomSize,
                zoomDuration * 0.5f)
                .SetEase(Ease.InQuad));

        if (zoomOut)
        {
            currentTween.AppendInterval(zoomLingerDuration);

            // Zoom out
            currentTween.Append(
                cam.transform.DOLocalMove(neutralPos, zoomDuration * 0.5f)
                    .SetEase(Ease.OutQuad));

            currentTween.Join(
                DOTween.To(
                    () => cam.orthographicSize,
                    x => cam.orthographicSize = x,
                    neutralSize,
                    zoomDuration * 0.5f)
                    .SetEase(Ease.OutQuad));
        }
    }

    void TryKillSequence()
    {
        if (currentTween != null)
            currentTween.Kill();
    }

    private (Vector3 center, float size) CalculateOrthoSize(
        List<Tile> tiles,
        float paddingMultiplier = 1f)
    {
        Bounds bounds = new Bounds();
        bool initialized = false;

        foreach (var tile in tiles)
        {
            if (!initialized)
            {
                bounds = tile.GetCollider().bounds;
                initialized = true;
            }
            else
            {
                bounds.Encapsulate(tile.GetCollider().bounds);
            }
        }

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        foreach (var corner in GetBoundsCorners(bounds))
        {
            Vector3 local = cam.transform.InverseTransformPoint(corner);

            minX = Mathf.Min(minX, local.x);
            maxX = Mathf.Max(maxX, local.x);
            minY = Mathf.Min(minY, local.y);
            maxY = Mathf.Max(maxY, local.y);
        }

        // Apply asymmetric padding (camera space)
        minX -= paddingLeft * paddingMultiplier;
        maxX += paddingRight * paddingMultiplier;
        minY -= paddingBottom * paddingMultiplier;
        maxY += paddingTop * paddingMultiplier;

        float width = maxX - minX;
        float height = maxY - minY;

        float size = Mathf.Max(
            height * 0.5f,
            width * 0.5f * cam.pixelHeight / cam.pixelWidth
        );

        float centerX, centerY;

        // 🔑 SOLUTION 1:
        // Override center when focusing on a single tile
        if (tiles.Count == 1)
        {
            Vector3 localCenter = cam.transform.InverseTransformPoint(bounds.center);
            centerX = localCenter.x;
            centerY = localCenter.y;
        }
        else
        {
            centerX = (minX + maxX) * 0.5f;
            centerY = (minY + maxY) * 0.5f;
        }

        Vector3 localFinalCenter = new Vector3(centerX, centerY, 0f);
        Vector3 worldCenter = cam.transform.TransformPoint(localFinalCenter);

        Vector3 finalCenter =
            worldCenter - cam.transform.forward * cameraDistance;

        return (finalCenter, size);
    }

    Vector3[] GetBoundsCorners(Bounds b)
    {
        Vector3 c = b.center;
        Vector3 e = b.extents;

        return new Vector3[]
        {
            c + new Vector3( e.x,  e.y,  e.z),
            c + new Vector3( e.x,  e.y, -e.z),
            c + new Vector3( e.x, -e.y,  e.z),
            c + new Vector3( e.x, -e.y, -e.z),
            c + new Vector3(-e.x,  e.y,  e.z),
            c + new Vector3(-e.x,  e.y, -e.z),
            c + new Vector3(-e.x, -e.y,  e.z),
            c + new Vector3(-e.x, -e.y, -e.z),
        };
    }
}
