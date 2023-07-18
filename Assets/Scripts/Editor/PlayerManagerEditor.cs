using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Player;
using UFB.Core;

[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor : Editor
{

    private string _saveConfigName = "PlayerConfiguration";
    private int _loadConfigIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerManager playerManager = (PlayerManager)target;

        _saveConfigName = EditorGUILayout.TextField("Save State Name", _saveConfigName);

        if (GUILayout.Button("Save Player Configuration")) {
            playerManager.SavePlayerConfiguration(_saveConfigName);
        }

        if (GUILayout.Button("Load Player Configuration")) {
            playerManager.LoadPlayerConfiguration(_saveConfigName);
        }

        string[] configNames = ApplicationData.GetFiles("gamestate/player-config", true);
        if (configNames != null) {
            _loadConfigIndex = EditorGUILayout.Popup("Player Configurations", _loadConfigIndex, configNames);

            if (GUILayout.Button($"Load {configNames[_loadConfigIndex]}")) {
                playerManager.LoadPlayerConfiguration(configNames[_loadConfigIndex]);
            }
        }

    }

}