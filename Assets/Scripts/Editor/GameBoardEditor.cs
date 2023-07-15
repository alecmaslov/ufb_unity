using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
using UFB.Map;

[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    private float _wallHeight = 0.5f;
    private float _stretchAmount = 0.5f;
    private bool _isGlowing = false;

    private string _mapName = "kraken";
    private string _entityName = "chest";
    private int _numSpawns = 10;
    private Vector2Int _tileCoords = new Vector2Int(0, 0);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameBoard gameBoard = (GameBoard)target;
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Editor Properties", EditorStyles.boldLabel);

        // _wallHeight = EditorGUILayout.FloatField("Wall Height", _wallHeight);


        if (GUILayout.Button("Rotate Tiles")) {
            gameBoard.IterateTiles((tile) => {
                tile.wallContainer.Rotate(0, 90, 0);
            });
        }


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.TextField("Map Name", _mapName);
        
        if (GUILayout.Button("Spawn Board"))
        {
            gameBoard.ClearBoard();
            gameBoard.SpawnBoard(_mapName);
        }

        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Spawn Entities", EditorStyles.boldLabel);

        _entityName = EditorGUILayout.TextField("Entity Name", _entityName);
        _tileCoords = EditorGUILayout.Vector2IntField("Spawn Coords", _tileCoords);
        _numSpawns = EditorGUILayout.IntField("Num Spawns", _numSpawns);
        
        if (GUILayout.Button("Spawn Entity"))
        {
            // gameBoard.SpawnEntity(_entityName, Coordinates.FromVector2Int(_tileCoords));
            gameBoard.SpawnEntitiesRandom(_entityName, _numSpawns);
        }


        // think about making a class called EntitySpawner, that can randomly spawn entities
        // throughout the map
    }

}