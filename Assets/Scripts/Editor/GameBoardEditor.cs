using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;

[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    private float _wallHeight = 0.5f;
    private float _stretchAmount = 0.5f;
    private bool _isGlowing = false;

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

        // if (GUILayout.Button("Toggle Glow"))
        // {
        //     if (_isGlowing) {
        //         gameBoard.Glow(0, 0.5f);
        //     } else {
        //         gameBoard.Glow(1, 0.5f); 
        //     }
        //     _isGlowing = !_isGlowing;
        // }

        // if (GUILayout.Button("Print Info"))
        // {
        //     Debug.Log(gameBoard.GameTile.ToString());
        // }

        // if (GUILayout.Button("Rotate"))
        // {
        //     gameBoard.gameObject.transform.Rotate(0, 90, 0);
        // }

    }

}