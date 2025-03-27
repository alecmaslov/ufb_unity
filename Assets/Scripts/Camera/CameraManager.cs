using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    public GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;

    [HideInInspector]
    public Camera cam;
    public CinemachineInputProvider _inputProvider;
    public CinemachineFreeLook _cinemachineFreeLook;

    public Transform cameraTarget;

    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoom = 15f; // Minimum zoom level
    public float maxZoom = 60f; // Maximum zoom level

    private float targetZoom = 60; // Target zoom level

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
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
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
        //_cinemachineFreeLook.LookAt = target;
        //_cinemachineFreeLook.Follow = target;
        //cameraTarget.position = target.position;
    }

    public void SetEnemyTarget(Transform target)
    {
        //_cinemachineFreeLook.LookAt = target;
        //cameraTarget.position = target.position;
    }

    public void setCameraTarget(Transform target)
    {
        cameraTarget.parent = target;
        if (target != null)
        {
            cameraTarget.localPosition = Vector3.zero;
        }
    }
    
    public bool IsUIClicked = false;

    private void Update()
    {
        _cinemachineFreeLook.enabled = UIGameManager.instance.IsCharacterCameraControl();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            click_data.position = touch.position;
            click_results.Clear();

            ui_raycaster.Raycast(click_data, click_results);

            foreach (RaycastResult result in click_results)
            {
                Debug.Log($"UI element: {result.gameObject.name}");
            }

            if(click_results.Count > 0)
            {
                IsUIClicked = true;
                return;
            }
            else
            {
                IsUIClicked = false;
            }
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
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
        _cinemachineFreeLook.m_Lens.FieldOfView = Mathf.Lerp(_cinemachineFreeLook.m_Lens.FieldOfView, targetZoom, Time.deltaTime * 6f);
    }

    private Vector2 touchStart;  // Start position of the touch
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 camStartPos;

    void handleMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(touch.position);
            click_data.position = touch.position;
            click_results.Clear();

            ui_raycaster.Raycast(click_data, click_results);

            /*foreach (RaycastResult result in click_results)
            {
                Debug.Log($"UI element: {result.gameObject.name}");                
            }*/
        }

        /*// Check if the ray hits an object in the scene
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Calculate the direction to the mouse position
            Vector3 targetPosition = hit.point;
            Vector3 direction = (targetPosition - cameraTarget.position).normalized;
            Debug.Log(direction + ", " + targetPosition);

            // Move the object towards the mouse position
            cameraTarget.position += direction * speed * Time.deltaTime;
        }*/


        // Handle zoom (pinch) and drag
        if (Input.touchCount == 1)
        {
            // One finger drag (move map)
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                camStartPos = cameraTarget.position;
                isMovingCamera = false;

                ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    startPos = hit.point;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    endPos = hit.point;
                    Vector3 direction = endPos - startPos;
                    isMovingCamera = true;
                    cameraTarget.transform.position = camStartPos + new Vector3(-direction.x, 0, -direction.z) * speed;
                }

            }
        }
    }

    public bool isMovingCamera = false;
    
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


    public void OnUIClicked()
    {
        Touch touch = Input.GetTouch(0);
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        click_data.position = touch.position;
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        if (click_results.Count > 0)
        {
            IsUIClicked = true;
            return;
        }
        else
        {
            IsUIClicked = false;
        }
        
    }

}
