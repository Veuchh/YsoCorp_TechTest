using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TileRaycaster))]
public class GameInputHandler : MonoBehaviour
{
    TileRaycaster tileRaycaster;

    private void Awake()
    {
        tileRaycaster = GetComponent<TileRaycaster>();
    }

    public void OnTouch(InputValue value)
    {
        tileRaycaster.UpdateIsTouchDown(value.Get<float>() > .5f);
    }

    public void OnUpdateTouchPosition(InputValue value)
    {
        tileRaycaster.UpdateTouchScreenPosition(value.Get<Vector2>());
    }
}
