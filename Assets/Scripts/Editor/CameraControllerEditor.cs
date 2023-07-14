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

        // string[] effectNames = effectsController.GetEffectsNames();
        // if (effectNames != null) {
        //     _effectChoice = EditorGUILayout.Popup("Effect", _effectChoice, effectNames);
        //     if (GUILayout.Button($"Run {effectNames[_effectChoice]}")) {
        //         effectsController.RunEffect(effectNames[_effectChoice]);
        //     }
        // }
    }

}