
using UnityEngine;

public class CameraItemDragMove : CameraMover
{
    private int _screenWidth;
    private int _screenHeight;

    private float _borderTouchOffset = 150f;
    private float _dragDelta = 1f;
    public CameraItemDragMove(Camera camera, int screenHeight, int screenWidth)
    {
        _camera = camera;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        
    }
    public override Vector3 UpdateTargetPosition()
    {
        targetPosition = _camera.transform.position;

        if (Input.mousePosition.x <= _borderTouchOffset)
        {
            targetPosition.x -= _dragDelta;
        }
        else if (Input.mousePosition.x >= _screenWidth - _borderTouchOffset)
        {
            targetPosition.x += _dragDelta;
        }

        if (Input.mousePosition.y <= _borderTouchOffset)
        {
            targetPosition.y -= _dragDelta;
        }
        if (Input.mousePosition.y >= _screenHeight - _borderTouchOffset)
        {
            targetPosition.y += _dragDelta;
        }
        return targetPosition;
    }
}
