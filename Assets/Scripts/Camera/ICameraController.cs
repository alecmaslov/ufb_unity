using UnityEngine;

namespace UFB.Camera
{
    public interface ICameraPositioner
    {
        static ICameraPositioner Controller { get; }
        void ApplyTransform(Transform t);
        void FocusOn(Transform transform);
    }
    // void MoveTo(Vector3 position, Quaternion rotation, float duration);
    // float DistanceFromTarget();
}
