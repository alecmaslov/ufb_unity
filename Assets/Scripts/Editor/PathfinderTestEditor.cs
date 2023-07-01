using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Development;
using UFB.Map;

[CustomEditor(typeof(PathfinderTest))]
public class PathfinderTestEditor : Editor
{

    [SerializeField] private int _startX = 0;
    [SerializeField] private int _startY = 0;

    
    [SerializeField] private int _endX = 0;
    [SerializeField] private int _endY = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathfinderTest pathfinderTest = (PathfinderTest)target;

        _startX = EditorGUILayout.IntField("Start X", _startX);
        _startY = EditorGUILayout.IntField("Start Y", _startY);
        _endX = EditorGUILayout.IntField("End X", _endX);
        _endY = EditorGUILayout.IntField("End Y", _endY);

        if (GUILayout.Button("Pathfind")) {
            pathfinderTest.PathfindBetween(new Coordinates(_startX, _startY), new Coordinates(_endX, _endY));
        }
    }

}