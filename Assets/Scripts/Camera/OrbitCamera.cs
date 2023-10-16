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
        public float minElevation = 1f;

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
            var rotation = Quaternion.Euler(elevation, azimuth, 0);
            var position = rotation * new Vector3(0.0f, 0.0f, -radius) + target.position;
            t.SetPositionAndRotation(
                Vector3.Lerp(transform.position, position, lerpSpeed),
                Quaternion.Slerp(transform.rotation, rotation, lerpSpeed)
            );

            t.LookAt(
                new Vector3(target.position.x, target.position.y + yOffset, target.position.z)
            );
        }

        private Coroutine _restorePositionRoutine;
        private Vector2 _restorePosition;
        private float _restoreRadius;

        public void CancelRestorePosition()
        {
            if (_restorePositionRoutine != null)
            {
                StopCoroutine(_restorePositionRoutine);
            }
        }

        // temporarily allows camera to be panned to a position, but restores after given timeout
        public void RotateTemporary(Vector2 axis, float timeout)
        {
            Debug.Log($"[OrbitCamera] RotateTemporary: {axis} | {timeout}");
            Vector2 restorePosition = new(azimuth, elevation);
            azimuth += axis.x;
            elevation += axis.y;
            CancelRestorePosition();
            _restorePositionRoutine = CoroutineHelpers.DelayedAction(
                () =>
                {
                    azimuth = restorePosition.x;
                    elevation = restorePosition.y;
                },
                timeout,
                this
            );
        }

        public void Rotate(Vector2 axis)
        {
            Rotate(axis.x, axis.y);
        }

        public void Rotate(float horizontal, float vertical)
        {
            // make sure we cancel any existing restore position routine
            CancelRestorePosition();
            azimuth += horizontal;
            elevation += vertical;
            elevation = Mathf.Clamp(elevation, minElevation, 90f);
        }

        public void ChangeRadius(float delta)
        {
            radius = Mathf.Clamp(radius + delta * zoomSpeed, minRadius, maxRadius);
        }

        public void ChangeRadiusTemporary(float delta, float timeout)
        {
            float restoreRadius = radius;
            radius = Mathf.Clamp(radius + delta * zoomSpeed, minRadius, maxRadius);
            CancelRestorePosition();
            _restorePositionRoutine = CoroutineHelpers.DelayedAction(
                () =>
                {
                    radius = restoreRadius;
                },
                timeout,
                this
            );
        }

        public void FocusOn(Transform t)
        {
            CancelRestorePosition();
            target = t;
            azimuth = initAzimuth;
        }

        // public void FocusOn

        public void LookAtTarget(Transform t)
        {
            CancelRestorePosition();
            // we need to also figure out the calculation
            // t.LookAt(target);

            Vector3 relativePos = t.position - target.position;

            azimuth = Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg;
            elevation =
                Mathf.Atan2(
                    relativePos.y,
                    Mathf.Sqrt(relativePos.x * relativePos.x + relativePos.z * relativePos.z)
                ) * Mathf.Rad2Deg;

        }

        /// <summary>
        /// Solves for the azimuth and elevation of the camera based on a new position
        /// </summary>
        /// <param name="pos"></param>
        public void SolvePosition(Vector3 pos)
        {
            CancelRestorePosition();
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
            
            elevation = Mathf.Clamp(elevation, minElevation, 90f);

            Debug.Log(
                $"[OrbitCamera] Solved position: {pos} | New => radius: {radius}, azimuth: {azimuth}, elevation: {elevation}"
            );
        }

        public float DistanceFromTarget()
        {
            return Vector3.Distance(transform.position, target.position);
        }
    }
}
