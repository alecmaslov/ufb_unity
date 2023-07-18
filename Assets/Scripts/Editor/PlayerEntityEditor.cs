using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
[CustomEditor(typeof(PlayerEntity))]
public class PlayerEntityEditor : Editor
{
    Vector2Int _destination;

    float _raiseTileHeight = 10f;
    float _raiseTileSpeed = 0.5f;
    // bool _isRaised;

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();


        PlayerEntity playerEntity = (PlayerEntity)target;

        if (playerEntity.TileAttachable.CurrentTile != null)
        {
            EditorGUILayout.Vector2IntField("Current Position", playerEntity.TileAttachable.CurrentTile.Coordinates.ToVector2Int());
        }
        _destination = EditorGUILayout.Vector2IntField("Destination", _destination);

        if (GUILayout.Button("Force To Tile"))
        {
            var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(coords);
            Debug.Log("Forcing to Tile: " + tile);
            playerEntity.ForceMoveToTile(tile);
        }

        if (GUILayout.Button("Force To Tile With Animation"))
        {
            var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(coords);
            Debug.Log("Forcing to Tile: " + tile);
            playerEntity.ForceMoveToTileAnimate(tile, 1.0f);
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

        if (GUILayout.Button("Toggle Hop"))
        {
            playerEntity.ToggleHop();
        }

        if (GUILayout.Button("Focus Camera"))
        {
            playerEntity.FocusCamera();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        _raiseTileHeight = EditorGUILayout.FloatField("Raise Tile Height", _raiseTileHeight);
        _raiseTileSpeed = EditorGUILayout.FloatField("Raise Tile Speed", _raiseTileSpeed);

        if (GUILayout.Button("Raise/Lower Attached Tile"))
        {
            if (playerEntity.CurrentTile != null)
            {
                float tileHeight = _raiseTileHeight;
                if (playerEntity.CurrentTile.IsStretched)
                {
                    playerEntity.CurrentTile.ResetStretch();
                }
                else
                {
                    playerEntity.CurrentTile.Stretch(tileHeight, _raiseTileSpeed);
                }

            }
        }

        if (playerEntity.CurrentTile != null && playerEntity.CurrentTile.IsStretched)
        {
            if (GUILayout.Button("Slam!"))
            {
                playerEntity.CurrentTile.SlamDown();
            }
        }
    }

}