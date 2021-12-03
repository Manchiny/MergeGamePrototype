
using UnityEngine;

public class CameraItemDragMove : CameraMover
{
    private int _screenWidth;
    private int _screenHeight;

    private float _borderTouchOffset = 200f;
    private float _dragDelta = 1.5f;
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
            targetPosition.x -= GetDelta(Input.mousePosition.x, 0);
            // targetPosition.x -= _dragDelta;
        }
        else if (Input.mousePosition.x >= _screenWidth - _borderTouchOffset)
        {
            targetPosition.x += GetDelta(Input.mousePosition.x, _screenWidth);          
        }
        if (Input.mousePosition.y <= _borderTouchOffset)
        {
            targetPosition.y -= GetDelta(Input.mousePosition.y, 0);
        }
        else if (Input.mousePosition.y >= _screenHeight - _borderTouchOffset)
        {
            targetPosition.y += GetDelta(Input.mousePosition.y, _screenHeight);
        }

        return targetPosition;
    }
        
    private float GetDelta(float currentCoordinateValue, float targetSideBorderCoor)
    {
        float delta = Mathf.Abs(targetSideBorderCoor - currentCoordinateValue);
        float percent = Mathf.Abs(delta * 100f / (_borderTouchOffset));
        return _dragDelta-_dragDelta * percent / 100f;
    }
}
