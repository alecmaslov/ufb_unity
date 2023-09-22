using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    public AnimationCurve zoomCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1));

    private Camera _camera;

    public float CurrentZoom => _currentZoom;
    private float _currentZoom;
    private Interpolator<float> _zoomInterpolator;

    public void Start()
    {
        _camera = GetComponent<Camera>();
        _zoomInterpolator = new Interpolator<float>(
            monoBehaviour: this,
            lerpFunction: Mathf.Lerp,
            onUpdate: OnZoom,
            getCurrent: () => _currentZoom,
            curve: zoomCurve
        );
    }

    private void OnZoom(float value)
    {
        _currentZoom = value;
        _camera.fieldOfView = value;
    }

    public void ZoomTo(float amount, float speed)
    {
        _zoomInterpolator.LerpTo(amount, speed);
    }
}
