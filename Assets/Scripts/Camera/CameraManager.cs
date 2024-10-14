using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    public Camera cam;
    public CinemachineInputProvider _inputProvider;
    public CinemachineFreeLook _cinemachineFreeLook;

    public Transform cameraTarget;

    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoom = 15f; // Minimum zoom level
    public float maxZoom = 60f; // Maximum zoom level

    private float targetZoom; // Target zoom level

    // FOR MOVEMENT
    public float speed = 0.1f; // Speed of the object movement
    public bool isMoving = false; // To track if the object should move

    public bool isCameraMove = true;

    // FOR ROTATE
    public bool isRotate = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    public void OnActiveInputAction()
    {
        _inputProvider.enabled = true;
    }

    public void OnInactiveInputAction() 
    {
        _inputProvider.enabled = false; 
    }

    public void SetTarget(Transform target)
    {
        _cinemachineFreeLook.LookAt = target;
        _cinemachineFreeLook.Follow = target;
        cameraTarget.position = target.position;
    }

    public void SetEnemyTarget(Transform target)
    {
        _cinemachineFreeLook.LookAt = target;
        cameraTarget.position = target.position;
    }

    private void Update()
    {
        _cinemachineFreeLook.enabled = UIGameManager.instance.IsCharacterCameraControl();

        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            handleMobileControl();
        } 
        else
        {
            handlePCControl();
        }
    }

    void handlePCControl()
    {
        // Check if the right mouse button is held down
        if (Input.GetMouseButton(0)) // 1 is the right mouse button
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (Input.GetMouseButton(2))
        {
            isMoving = true;
            isRotate = true;
        }
        else
        {
            isRotate = false;
        }


        if (isMoving && !isRotate && isCameraMove)
        {
            handleMove();
        }

        if (isRotate && isMoving)
        {
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 300;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 2;
            handleRoate();
        }
        else
        {
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0;
        }
        handleZoom();
    }

    void handleMobileControl()
    {
        // Check if the right mouse button is held down
        if (Input.touchCount == 1) // 1 is the right mouse button
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (Input.touchCount == 2)
        {
            isMoving = true;
            isRotate = true;
        }
        else
        {
            isRotate = false;
        }


        if (isMoving && !isRotate && isCameraMove)
        {
            handleMove();
        }

        handleRoate();


        handleZoom();
    }

    void handleZoom()
    {
        float scroll = 0f;

        // Mobile pinch to zoom
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            scroll = prevMagnitude - currentMagnitude;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0) // PC scroll wheel
        {
            scroll = -Input.GetAxis("Mouse ScrollWheel");
        }

        targetZoom += scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);


        // Smoothly interpolate the camera's field of view towards the target zoom
        _cinemachineFreeLook.m_Lens.FieldOfView = Mathf.Lerp(_cinemachineFreeLook.m_Lens.FieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }

    void handleMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(touch.position);
        }

        Debug.DrawRay(ray.origin, ray.direction, new Color(1, 0, 0));

        // Check if the ray hits an object in the scene
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Calculate the direction to the mouse position
            Vector3 targetPosition = hit.point;
            Vector3 direction = (targetPosition - cameraTarget.position).normalized;
            Debug.Log(direction + ", " + targetPosition);

            // Move the object towards the mouse position
            cameraTarget.position += direction * speed * Time.deltaTime;
        }
    }

    void handleRoate()
    {
/*        if (isRotate && isMoving)
        {
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 300;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 2;
        }
        else
        {*/
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0;
/*        }*/
    }

}
