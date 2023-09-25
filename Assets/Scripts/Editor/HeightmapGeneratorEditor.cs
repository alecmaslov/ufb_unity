using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
[CustomEditor(typeof(HeightmapGenerator))]
public class HeightmapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();


        HeightmapGenerator heightmapGenerator = (HeightmapGenerator)target;


        if (GUILayout.Button("Generate Heightmap"))
        {
            heightmapGenerator.ApplyHeightmap();
        }

    }

}