using UnityEngine;

public class CameraSwipeMove : CameraMover
{
    private Vector2 _startPosition;

    public CameraSwipeMove(Camera camera)
    {
        _camera = camera;
    }
    public override Vector3 UpdateTargetPosition()
    {
        Vector3 targetPosition;

        if (Input.GetMouseButtonDown(0))
        {
            _startPosition = _camera.ScreenToWorldPoint(Input.mousePosition);   
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 position = new Vector2(_camera.ScreenToWorldPoint(Input.mousePosition).x, _camera.ScreenToWorldPoint(Input.mousePosition).y) - _startPosition;
            targetPosition = new Vector3(_camera.transform.position.x - position.x, _camera.transform.position.y - position.y, _camera.transform.position.z);
            
            return targetPosition;
        }

        return _camera.transform.position;
    }
}
