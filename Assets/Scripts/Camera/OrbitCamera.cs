using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    private float _rotateEase = 1f;
    private Transform _focusedTransform;
    private float _elevation;
    private float _azimuth;
    private float _radius;

    private Vector3 _targetPosition;
    private Vector3 _targetNorm;
    private Quaternion _targetRotation;
    

    private Vector2 _currentAxis = new Vector2(0f,0f);
    
    void Update()
    {
        _targetPosition = _focusedTransform.forward * -_radius;

        // 2 * Mathf.PI * _distanc
    }




    // private IEnumerator Mo

    public void FocusOnTransform(Transform t) {
        _focusedTransform = t;
    }

    public void ApplyAxis(Vector2 axis) {
        // Vector2 normalizedAxis

        // apply a given control axis to the orbit camera
        // _targetNorm = VectorUtils.NormalizedSphericalPosition()
    }

    
}
