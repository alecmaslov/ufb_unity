using UnityEngine;

public interface ICameraController {
    static ICameraController Controller { get; }
    void FocusOn(Transform transform);
    float DistanceFromTarget();
}