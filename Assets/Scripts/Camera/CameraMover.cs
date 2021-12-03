using UnityEngine;

public abstract class CameraMover
{
    protected Camera _camera;
    public abstract Vector3 UpdateTargetPosition();
    protected Vector3 targetPosition;

    public void ResetTarget(Vector3 target)
    {
        targetPosition = target;
    }
}
