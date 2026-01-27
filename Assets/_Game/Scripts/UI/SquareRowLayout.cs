using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class SquareRowLayout : MonoBehaviour
{
    GridLayoutGroup gridLayoutGroup;
    RectTransform rect;
    void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rect = transform as RectTransform;
    }
    void LateUpdate()
    {
        int count = transform.childCount;
        if (count == 0)
            return;

        float totalSpacing = gridLayoutGroup.spacing.x * (count - 1);
        float totalPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;

        float availableWidth = rect.rect.width - totalSpacing - totalPadding;
        float availableHeight = rect.rect.height - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

        float size = Mathf.Min(
            availableHeight,
            availableWidth / count
        );

        gridLayoutGroup.cellSize = new Vector2(size, size);
    }
}
