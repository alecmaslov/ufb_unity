using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Core;

[CustomEditor(typeof(RotationAnimator))]
public class RotationAnimatorEditor : Editor
{

    private Quaternion _targetRotation = Quaternion.identity;
    private Vector4 _randomRange = Vector4.one;
    private float _duration = 1.5f;
    private string _key = "test";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RotationAnimator animatorTest = (RotationAnimator)target;

        Vector4 rotationVec = EditorGUILayout.Vector4Field("Target Scale", new Vector4(_targetRotation.x, _targetRotation.y, _targetRotation.z, _targetRotation.w));
        _randomRange = EditorGUILayout.Vector4Field("Random Rotation", _randomRange);
        _duration = EditorGUILayout.FloatField("Duration", _duration);
        _key = EditorGUILayout.TextField("Key", _key);

        if (GUILayout.Button("Rotate To")) {
            animatorTest.AnimateTo(new Quaternion(rotationVec.x, rotationVec.y, rotationVec.z, rotationVec.w), _duration);
        }

        if (GUILayout.Button("Random Rotation")) {
            _targetRotation = new Quaternion(
                Random.Range(-_randomRange.x, _randomRange.x), 
                Random.Range(-_randomRange.y, _randomRange.y), 
                Random.Range(-_randomRange.z, _randomRange.z),
                Random.Range(-_randomRange.w, _randomRange.w)  
            );
            animatorTest.AnimateTo(_targetRotation, _duration);
        }

        if (GUILayout.Button("Set Snapshot")) {
            animatorTest.SetSnapshot(_targetRotation, _key);
        }

        if (GUILayout.Button("Rotate To Snapshot")) {
            animatorTest.AnimateToSnapshot(_key, _duration);
        }

        // if (GUILayout.Button("Activate")) {
        //     animatorTest.Activate();
        // }

        // if (GUILayout.Button("Deactivate")) {
        //     wall.Deactivate();
        // }
    }

}