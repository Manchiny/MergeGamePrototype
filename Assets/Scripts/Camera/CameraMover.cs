using UnityEngine;

public abstract class CameraMover
{
    protected Camera _camera;
    public abstract Vector3 UpdateTargetPosition();
}
