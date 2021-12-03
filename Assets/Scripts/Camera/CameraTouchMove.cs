using UnityEngine;

public class CameraTouchMove : CameraMover
{
    private Vector2 _startPosition;
    public CameraTouchMove(Camera camera)
    {
        _camera = camera;
        targetPosition = _camera.transform.position;
    }
    public override Vector3 UpdateTargetPosition()
    {     
        if (Input.GetMouseButtonDown(0))
        {
            _startPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = _camera.transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 position = new Vector2(_camera.ScreenToWorldPoint(Input.mousePosition).x, _camera.ScreenToWorldPoint(Input.mousePosition).y) - _startPosition;
            targetPosition = new Vector3(_camera.transform.position.x - position.x, _camera.transform.position.y - position.y, _camera.transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            
        }
        return targetPosition;
    }
}
