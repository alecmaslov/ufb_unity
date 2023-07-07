using UnityEditor;
using UnityEngine;
using UFB.Entities;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Wall wall = (Wall)target;

        if (GUILayout.Button("Activate")) {
            wall.Activate();
        }

        if (GUILayout.Button("Deactivate")) {
            wall.Deactivate();
        }
    }

}