using UnityEngine;
using System.Collections;

public class CameraOrbitSimple : MonoBehaviour
{
    public Transform target; // The target point around which the camera will rotate
    public float radius = 10f; // The distance from the camera to the target
    public float azimuthSpeed = 0.01f; // The speed of rotation along the azimuth (horizontal plane)
    public float elevationSpeed = 0.01f; // The speed of rotation along the elevation (vertical plane)
    [SerializeField] private float focusLerpSpeed = 0.1f; // The speed of lerp when focusing on a new target
    [SerializeField] private float azimuth = 0f; // Azimuth angle
    [SerializeField] private float elevation = 0f; // Elevation angle

    // Update is called once per frame
    void Update()
    {
        azimuth += azimuthSpeed * Time.deltaTime;
        Vector3 newPosition = new Vector3(
            target.position.x + radius * Mathf.Cos(elevation) * Mathf.Sin(azimuth),
            target.position.y + elevation,
            target.position.z + radius * Mathf.Cos(elevation) * Mathf.Cos(azimuth)
        );

        transform.position = newPosition;
        transform.LookAt(target);
    }

    public void FocusOn(Transform t)
    {
        StartCoroutine(FocusOnRoutine(t));
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
}
