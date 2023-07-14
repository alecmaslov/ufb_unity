using UnityEngine;


public class CameraController : MonoBehaviour
{
    public static ICameraController Controller { get; private set; }


    private void Start()
    {
        Controller = GetComponent<OrbitCamera>();
    }
    
}