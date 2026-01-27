using UnityEngine;

public class TileRaycaster : MonoBehaviour
{
    bool isTouchDown = false;
    Vector2 lastTouchScreenPos = Vector2.zero;

    Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (isTouchDown)
        {
            HoverRaycast();
        }
    }

    void HoverRaycast()
    {
        Tile bufferTile = LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile;
        if (Physics.Raycast(mainCam.ScreenPointToRay(lastTouchScreenPos), out RaycastHit hit)
            && hit.collider.GetComponent<Tile>() is Tile hitTile)
        {
            LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile = hitTile;
        }
        else
        {
            LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile = null;
        }

        if (bufferTile != LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile)
        {
            LevelHandler.Instance.OngoingLevelData.IsHighlightDirty = true;
        }
    }

    public void UpdateIsTouchDown(bool isTouchDown)
    {
        this.isTouchDown = isTouchDown;

        if (LevelHandler.Instance != null
            && !isTouchDown)
        {
            LevelHandler.Instance.OngoingLevelData.CurrentlyHoveredTile = null;
            LevelHandler.Instance.OngoingLevelData.IsHighlightDirty = true;
        }
    }

    public void UpdateTouchScreenPosition(Vector2 newTouchPos)
    {
        lastTouchScreenPos = newTouchPos;
    }
}
