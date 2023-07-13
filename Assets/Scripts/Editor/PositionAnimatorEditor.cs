using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Core;

[CustomEditor(typeof(PositionAnimator))]
public class PositionAnimatorEditor : Editor
{

    private Vector3 _targetPosition;
    private Vector3 _randomRange;
    private float _duration = 1.5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PositionAnimator animatorTest = (PositionAnimator)target;

        _targetPosition = EditorGUILayout.Vector3Field("Target Position", _targetPosition);
        _randomRange = EditorGUILayout.Vector3Field("Random Range", _randomRange);
        _duration = EditorGUILayout.FloatField("Duration", _duration);

        if (GUILayout.Button("MoveTo")) {
            animatorTest.AnimateTo(_targetPosition, _duration);
        }

        if (GUILayout.Button("Random Position")) {
            _targetPosition = new Vector3(
                Random.Range(-_randomRange.x, _randomRange.x), 
                Random.Range(-_randomRange.y, _randomRange.y), 
                Random.Range(-_randomRange.z, _randomRange.z));
            animatorTest.AnimateTo(_targetPosition, _duration);
        }

        // if (GUILayout.Button("Activate")) {
        //     animatorTest.Activate();
        // }

        // if (GUILayout.Button("Deactivate")) {
        //     wall.Deactivate();
        // }
    }

}