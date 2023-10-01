using UnityEditor;
using UnityEngine;
using UFB.Core;
using UFB.Camera;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    private bool _useOrbit = true;
    private string _saveCameraStateName = "CameraState";
    private int _loadStateIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraController cameraController = (CameraController)target;

        _saveCameraStateName = EditorGUILayout.TextField("Save State Name", _saveCameraStateName);
        if (GUILayout.Button("Save Camera State"))
        {
            cameraController.SaveCameraState(_saveCameraStateName);
        }

        if (GUILayout.Button("Load Camera State"))
        {
            cameraController.LoadCameraState(_saveCameraStateName);
        }

        string[] stateNames = ApplicationData.GetFiles("presets/camera", true);
        if (stateNames != null)
        {
            _loadStateIndex = EditorGUILayout.Popup("Camera States", _loadStateIndex, stateNames);

            if (GUILayout.Button($"Load {stateNames[_loadStateIndex]}"))
            {
                cameraController.LoadCameraState(stateNames[_loadStateIndex]);
            }
        }

        // if (GUILayout.Button("Move to Overhead"))
        // {
        //     cameraController.MoveToOverhead();
        // }

        if (GUILayout.Button("Random Transform"))
        {
            cameraController.TransformTo(
                new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)),
                Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)),
                1f
            );
        }


        if (GUILayout.Button("Stop Repositioning"))
        {
            cameraController.StopRepositioning();
        }


        if (GUILayout.Button("Toggle Orbit"))
        {
            _useOrbit = !_useOrbit;
            cameraController.UseOrbit = _useOrbit;
        }
    }

}