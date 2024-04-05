using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Character;
using Unity.VisualScripting;
using UFB.Core;
using UFB.Map;

[CustomEditor(typeof(UFB.Character.CharacterController))]
public class CharacterControllerEditor : Editor
{
    Vector2Int _destination;

    float _raiseTileHeight = 10f;
    float _raiseTileSpeed = 0.5f;

    // bool _isRaised;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UFB.Character.CharacterController characterController =
            (UFB.Character.CharacterController)target;

        if (EditorApplication.isPlaying)
        {
            _destination = EditorGUILayout.Vector2IntField("Destination", _destination);

            if (GUILayout.Button("Print State"))
            {
                Debug.Log(characterController.State.Serialize());
            }

            if (GUILayout.Button("Play Entrance Animation"))
            {
                characterController.PlayEntranceAnimation();
            }

            if (GUILayout.Button("Force Move to Tile"))
            {
                var gameBoard = ServiceLocator.Current.Get<GameBoard>();
                characterController.ForceMoveToTile(
                    gameBoard.GetTileByCoordinates(Coordinates.FromVector2Int(_destination))
                );
            }

            // if (playerEntity.TileAttachable != null && playerEntity.TileAttachable.CurrentTile != null)
            // {
            //     EditorGUILayout.Vector2IntField("Current Position", playerEntity.TileAttachable.CurrentTile.Coordinates.ToVector2Int());
            // }

            // if (GUILayout.Button("Force To Tile"))
            // {
            //     var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            //     var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(coords);
            //     Debug.Log("Forcing to Tile: " + tile);
            //     playerEntity.ForceMoveToTile(tile);
            // }

            // if (GUILayout.Button("Force To Tile With Animation"))
            // {
            //     var coords = UFB.Map.Coordinates.FromVector2Int(_destination);
            //     var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(coords);
            //     Debug.Log("Forcing to Tile: " + tile);
            //     playerEntity.ForceMoveToTileAnimate(tile, 1.0f);
            // }

            if (GUILayout.Button("Preview Pathfind"))
            {
                //characterController.PreviewRoute(UFB.Map.Coordinates.FromVector2Int(_destination));
            }

            // if (GUILayout.Button("Toggle Hop"))
            // {
            //     playerEntity.ToggleHop();
            // }

            // if (GUILayout.Button("Focus Camera"))
            // {
            //     playerEntity.FocusCamera();
            // }

            // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // _raiseTileHeight = EditorGUILayout.FloatField("Raise Tile Height", _raiseTileHeight);
            // _raiseTileSpeed = EditorGUILayout.FloatField("Raise Tile Speed", _raiseTileSpeed);

            // if (GUILayout.Button("Raise/Lower Attached Tile"))
            // {
            //     if (playerEntity.CurrentTile != null)
            //     {
            //         float tileHeight = _raiseTileHeight;
            //         if (playerEntity.CurrentTile.IsStretched)
            //         {
            //             playerEntity.CurrentTile.ResetStretch();
            //         }
            //         else
            //         {
            //             playerEntity.CurrentTile.Stretch(tileHeight, _raiseTileSpeed);
            //         }

            //     }
            // }

            // if (playerEntity.CurrentTile != null && playerEntity.CurrentTile.IsStretched)
            // {
            //     if (GUILayout.Button("Slam!"))
            //     {
            //         playerEntity.CurrentTile.SlamDown();
            //     }
            // }
        }
    }
}
