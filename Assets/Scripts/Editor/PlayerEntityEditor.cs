using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
[CustomEditor(typeof(PlayerEntity))]
public class PlayerEntityEditor : Editor
{
    Vector2Int _destination;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerEntity playerEntity = (PlayerEntity)target;

        _destination = EditorGUILayout.Vector2IntField("Destination", _destination);

        if (GUILayout.Button("Force To Tile")) {
            var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(coords);
            Debug.Log("Forcing to Tile: " + tile);
            playerEntity.ForceMoveToTile(tile);
        }

        if (GUILayout.Button("Preview Pathfind"))
        {
            playerEntity.PreviewRoute(UFB.Map.Coordinates.FromVector2Int(_destination));
        }

        if (GUILayout.Button("Pathfind To Tile"))
        {
            var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(coords);
            playerEntity.TryMoveToTile(tile);
        }

        if (GUILayout.Button("Toggle Hop")) {
            playerEntity.Hop();
        }

        if (GUILayout.Button("Focus Camera")) {
            playerEntity.FocusCamera();
        }
    }

}