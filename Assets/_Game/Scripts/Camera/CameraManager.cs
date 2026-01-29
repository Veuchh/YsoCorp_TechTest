using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;

    Camera cam;

    private async void Awake()
    {
        cam = GetComponent<Camera>(); 
        
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnGridGenerated.AddListener(OnGridGenerated);
    }

    async void  OnGridGenerated()
    {
        await UniTask.NextFrame();
        SetCameraPosition(LevelHandler.Instance.OngoingLevelData.Tiles);
    }

    public async void SetCameraPosition(Tile[,] tiles)
    {
        await UniTask.NextFrame();
        var (center, size) = CalculateOrthoSize(tiles);
        cam.transform.localPosition = center;
        cam.orthographicSize = size;
    }

    private (Vector3 center, float size) CalculateOrthoSize(Tile[,] tiles)
    {
        Bounds bounds = new Bounds();

        foreach (var tile in tiles)
        {
            bounds.Encapsulate(tile.GetCollider().bounds);
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

        // Asymmetric padding (camera space)
        minX -= paddingLeft;
        maxX += paddingRight;
        minY -= paddingBottom;
        maxY += paddingTop;

        float width = maxX - minX;
        float height = maxY - minY;

        float size = Mathf.Max(
            height * 0.5f,
            width * 0.5f * cam.pixelHeight / cam.pixelWidth
        );

        // Center shift caused by asymmetric padding
        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;

        Vector3 localCenter = new Vector3(centerX, centerY, 0f);
        Vector3 worldCenter = cam.transform.TransformPoint(localCenter);

        Vector3 finalCenter =
            worldCenter - cam.transform.forward * 10f;

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
