using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
[CustomEditor(typeof(PlayerEntity))]
public class PlayerEntityEditor : Editor
{

    [SerializeField] private int _x = 0;
    [SerializeField] private int _y = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerEntity playerEntity = (PlayerEntity)target;

        _x = EditorGUILayout.IntField("X", _x);
        _y = EditorGUILayout.IntField("Y", _y);

        if (GUILayout.Button("Move To Tile")) {
            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(new UFB.Map.Coordinates(_x, _y));
            playerEntity.MoveToTile(tile);
            // GameController.Instance.GameBoard.MoveEntityToTile(playerEntity, _x, _y);
            // Debug.Log("Run Effect " + _effectName);
            // effectsManager.RunEffect(_effectName);
        }
    }

}