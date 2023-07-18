using UnityEngine;

[RequireComponent(typeof(OrbitCamera))]
[RequireComponent(typeof(ZoomController))]
public class CameraController : MonoBehaviour
{
    public static ICameraController Controller { get; private set; }
    public static ZoomController ZoomController;


    private void Start()
    {
        Controller = GetComponent<OrbitCamera>();
        ZoomController = GetComponent<ZoomController>();
    }
    
}