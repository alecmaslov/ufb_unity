using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{

    private int _effectChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraController cameraController = (CameraController)target;

    }

}