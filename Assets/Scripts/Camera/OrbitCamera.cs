using UnityEngine;
using System.Collections;

namespace UFB.Camera
{
    public class OrbitCamera : MonoBehaviour, ICameraPositioner
    {
        public Transform target;
        public float radius = 10f;
        public float azimuthSpeed = 0.01f;
        public float elevationSpeed = 0.01f;

        [SerializeField]
        private float focusLerpSpeed = 0.1f;

        [SerializeField]
        private float yOffset = 1f;

        [SerializeField]
        private float lerpSpeed = 0.1f; // Lerp speed for position and rotation

        [SerializeField]
        private float azimuth;

        [SerializeField]
        private float elevation = 3f;

        [SerializeField]
        private float initAzimuth = 180f;

        [SerializeField]
        private float minRadius = 1f;

        [SerializeField]
        private float maxRadius = 20f;

        [SerializeField]
        private float zoomSpeed = 5f;

        [SerializeField]
        private float arrowSpeed = 5f;

        private void Start()
        {
            if (target != null)
                radius = Vector3.Distance(transform.position, target.position);
        }

        public void ApplyTransform(Transform t)
        {
            if (target == null)
            {
                Debug.LogWarning("No target set for OrbitCamera");
                return;
            }

            azimuth += azimuthSpeed * Time.deltaTime;

            // #if UNITY_EDITOR
            //             PcControls();
            // #endif

            var rotation = Quaternion.Euler(elevation, azimuth, 0);
            var position = rotation * new Vector3(0.0f, 0.0f, -radius) + target.position;

            transform.position = Vector3.Lerp(transform.position, position, lerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lerpSpeed);

            // t.SetPositionAndRotation(position, rotation);
            t.LookAt(
                new Vector3(target.position.x, target.position.y + yOffset, target.position.z)
            );
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

        public void LookAtTarget(Transform t)
        {
            t.LookAt(target);
        }

        /// <summary>
        /// Solves for the azimuth and elevation of the camera based on a new position
        /// </summary>
        /// <param name="pos"></param>
        public void SolvePosition(Vector3 pos)
        {
            Debug.Log(
                $"[OrbitCamera] Solving position: {pos} | Current => radius: {radius}, azimuth: {azimuth}, elevation: {elevation}"
            );
            // Calculate relative position to the target
            Vector3 relativePos = pos - target.position;
            // Calculate the radius
            radius = Mathf.Sqrt(
                relativePos.x * relativePos.x
                    + relativePos.y * relativePos.y
                    + relativePos.z * relativePos.z
            );
            // Calculate the azimuth (in degrees)
            azimuth = Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg;
            // Calculate the elevation (in degrees)
            elevation =
                Mathf.Atan2(
                    relativePos.y,
                    Mathf.Sqrt(relativePos.x * relativePos.x + relativePos.z * relativePos.z)
                ) * Mathf.Rad2Deg;

            Debug.Log(
                $"[OrbitCamera] Solved position: {pos} | New => radius: {radius}, azimuth: {azimuth}, elevation: {elevation}"
            );
        }

        public float DistanceFromTarget()
        {
            return Vector3.Distance(transform.position, target.position);
        }

        private void PcControls()
        {
            float scrollInput = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                radius += (scrollInput * -1) * zoomSpeed; // Adjust the zoom speed as needed
                radius = Mathf.Clamp(radius, minRadius, maxRadius); // Optional: Clamp the zoom distance
            }

            float lrInput = UnityEngine.Input.GetKey(KeyCode.LeftArrow)
                ? -1
                : (UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
            if (Mathf.Abs(lrInput) > 0.01f)
            {
                azimuth += lrInput * arrowSpeed;
            }
        }
    }
}
// private IEnumerator FocusOnRoutine(Transform t)
// {
//     while (Vector3.Distance(target.position, t.position) > 0.01f)
//     {
//         target.position = Vector3.Lerp(target.position, t.position, focusLerpSpeed * Time.deltaTime);
//         yield return null;
//     }
//     target.position = t.position;
// }
