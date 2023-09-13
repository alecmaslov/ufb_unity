using UnityEngine;
using System.Collections;


public class OrbitCamera : MonoBehaviour, ICameraController
{
    public Transform target;
    public float radius = 10f;
    public float azimuthSpeed = 0.01f;
    public float elevationSpeed = 0.01f;
    [SerializeField] private float focusLerpSpeed = 0.1f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float lerpSpeed = 0.1f; // Lerp speed for position and rotation

    [SerializeField] private float azimuth;
    [SerializeField] private float elevation = 3f;
    [SerializeField] private float initAzimuth = 180f;

    [SerializeField] private float minRadius = 1f;
    [SerializeField] private float maxRadius = 20f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float arrowSpeed = 5f;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        if (target != null)
            radius = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {
        if (target == null) return;

        azimuth += azimuthSpeed * Time.deltaTime;
        // azimuth += Input.GetAxis("Horizontal") * azimuthSpeed * Time.deltaTime; 
        // elevation += Input.GetAxis("Vertical") * elevationSpeed * Time.deltaTime; 
        // Check for mouse scroll input

        #if UNITY_EDITOR
        PcControls();
        #endif

        var rotation = Quaternion.Euler(elevation, azimuth, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -radius) + target.position;

        transform.position = Vector3.Lerp(transform.position, position, lerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lerpSpeed);
        transform.LookAt(new Vector3(target.position.x, target.position.y + yOffset, target.position.z));
    }


    public void Control(float horizontal, float vertical)
    {
        azimuth += horizontal;
        elevation += vertical;
    }

    public void FocusOn(Transform t)
    {
        target = t;
        azimuth = initAzimuth;
    }

    public float DistanceFromTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private IEnumerator FocusOnRoutine(Transform t)
    {
        while (Vector3.Distance(target.position, t.position) > 0.01f)
        {
            target.position = Vector3.Lerp(target.position, t.position, focusLerpSpeed * Time.deltaTime);
            yield return null;
        }
        target.position = t.position;
    }

    
    private void PcControls()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            radius += (scrollInput * -1) * zoomSpeed; // Adjust the zoom speed as needed
            radius = Mathf.Clamp(radius, minRadius, maxRadius); // Optional: Clamp the zoom distance
        }


        float lrInput = Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        if (Mathf.Abs(lrInput) > 0.01f)
        {
            azimuth += lrInput * arrowSpeed;
        }        
    }
}
