using UnityEditor;
using UnityEngine;
using UFB.Map;

[CustomEditor(typeof(MeshTileMapGenerator))]
public class MeshMapGeneratorEditor : Editor
{
    private float _tileHeight = 1.0f;
    private Vector2Int _tileSelector = new Vector2Int(0, 0);
    private Vector3 _offset = new Vector3(0.0f, 0.0f, 0.0f);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        MeshTileMapGenerator script = (MeshTileMapGenerator)target;

        // vector2 to set the tile selection
        _tileSelector = EditorGUILayout.Vector2IntField("Tile Selector", _tileSelector);

        // slider to set the tile height
        _tileHeight = EditorGUILayout.Slider("Tile Height", _tileHeight, 0.0f, 10.0f);

        _offset = EditorGUILayout.Vector3Field("Offset", _offset);

        if (GUILayout.Button("Set Tile Height"))
        {
            script.SetTileHeight(_tileSelector.x, _tileSelector.y, _tileHeight, _offset);
        }
    }
}