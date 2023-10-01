using UnityEditor;
using UnityEngine;
using UFB.Camera;

[CustomEditor(typeof(ZoomController))]
public class ZoomControllerEditor : Editor
{
    private float _zoomAmount = 40f;
    private float _zoomSpeed = 0.5f;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ZoomController zoomController = (ZoomController)target;

        // add a slider for zoom
        _zoomAmount = EditorGUILayout.Slider("Zoom Amount", _zoomAmount, 3f, 110.0f);
        _zoomSpeed = EditorGUILayout.Slider("Zoom Speed", _zoomSpeed, 0.01f, 10f);

        if (GUILayout.Button("Zoom"))
        {
            zoomController.ZoomTo(_zoomAmount, _zoomSpeed);
        }
    }
}