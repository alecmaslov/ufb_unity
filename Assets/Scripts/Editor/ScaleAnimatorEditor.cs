using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Core;

[CustomEditor(typeof(ScaleAnimator))]
public class ScaleAnimatorEditor : Editor
{

    private Vector3 _tagetScale = Vector3.one;
    private Vector3 _randomRange = Vector3.one;
    private float _duration = 1.5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScaleAnimator animatorTest = (ScaleAnimator)target;

        _tagetScale = EditorGUILayout.Vector3Field("Target Scale", _tagetScale);
        _randomRange = EditorGUILayout.Vector3Field("Random Scale", _randomRange);
        _duration = EditorGUILayout.FloatField("Duration", _duration);

        if (GUILayout.Button("Scale To")) {
            animatorTest.AnimateTo(_tagetScale, _duration, () => {
                Debug.Log("[EDITOR] Animation Complete");
            });
        }

        if (GUILayout.Button("Random Scale")) {
            _tagetScale = new Vector3(
                Random.Range(0, _randomRange.x), 
                Random.Range(0, _randomRange.y), 
                Random.Range(0, _randomRange.z));
            animatorTest.AnimateTo(_tagetScale, _duration, () => {
                Debug.Log("[EDITOR] Animation Complete");
            });
        }
    }

}