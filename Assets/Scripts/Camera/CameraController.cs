using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    [SerializeField] private float _speed = 10f;

    private Camera _camera;

    private int _screenWidth, _screenHeight;
    private Vector3 _targetPosition;
    private CameraMover _cameraMover;
    private CameraItemDragMove _cameraItemDragMove;
    private CameraTouchMove _cameraTouchMove;

    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifire;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;

    float zoomModifireSpeed = 0.02f;

    private bool _isItemDrag;
    public bool IsItemDrag
    {
        get => _isItemDrag;
        set
        {
            if (value == true)
            {
                _cameraItemDragMove.ResetTarget(_targetPosition);
                _cameraMover = _cameraItemDragMove;
            }
            else
            {
                _cameraTouchMove.ResetTarget(_targetPosition);
                _cameraMover = _cameraTouchMove;
            }
        }
    }

    public bool IsZoomig { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _camera = GetComponent<Camera>();

            return;
        }
        Destroy(gameObject);
    }
    private void Start()
    {
        _targetPosition = transform.position;

        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        _cameraItemDragMove = new CameraItemDragMove(_camera, _screenHeight, _screenWidth);
        _cameraTouchMove = new CameraTouchMove(_camera);

        IsItemDrag = false;
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            IsZoomig = true;
            ZoomPinch();
            return;
        }
        else
        {
            IsZoomig = false;
            _targetPosition = _cameraMover.UpdateTargetPosition();
        }
        
    }
    private void FixedUpdate()
    {
        if (IsZoomig)
            return;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, _speed * Time.deltaTime);
    }

    private void ZoomPinch()
    {

        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
        secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

        touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
        touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

        zoomModifire = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifireSpeed;

        if (touchesPrevPosDifference > touchesCurPosDifference)
            _camera.orthographicSize += zoomModifire;
        if (touchesPrevPosDifference < touchesCurPosDifference)
            _camera.orthographicSize -= zoomModifire;
    }
}
