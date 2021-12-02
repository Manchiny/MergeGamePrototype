using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private int _screenWidth;
    private int _screenHeight;

    private float _speed = 6f;

    private Camera _camera;

    private Vector3 targetPosition;

    private CameraMover _cameraMover;
    private CameraItemDragMove _cameraItemDragMove;
    private CameraSwipeMove _cameraSwipeMove;

    private bool _isItemDrag;
    public bool IsItemDrag 
    {
        get => _isItemDrag;
        set
        {  
            if(value == true)
            {
                _cameraMover = _cameraItemDragMove;
            }
            else
            {
                _cameraMover = _cameraSwipeMove;
            }
        }
    } 

    private void Awake()
    {
        if(Instance == null)
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
        targetPosition = transform.position;

        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        _cameraItemDragMove = new CameraItemDragMove(_camera, _screenHeight, _screenWidth);
        _cameraSwipeMove = new CameraSwipeMove(_camera);

        IsItemDrag = false;
    }

    private void Update()
    {
        targetPosition =_cameraMover.UpdateTargetPosition();       
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, _speed * Time.deltaTime);
    } 
}
