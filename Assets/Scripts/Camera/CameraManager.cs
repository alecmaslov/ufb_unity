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
    }

}
