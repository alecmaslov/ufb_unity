using UnityEditor;
using UnityEngine;
using UFB.Effects;
using UFB.Utilities;

[CustomEditor(typeof(SubdivideMesh))]
public class SubdivideMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SubdivideMesh subdivideMesh = (SubdivideMesh)target;

        if (GUILayout.Button("Subdivide")) {
            subdivideMesh.Subdivide();
        }
    }
}
