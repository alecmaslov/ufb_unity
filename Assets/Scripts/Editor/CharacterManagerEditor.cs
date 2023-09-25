using UnityEditor;
using UnityEngine;
using UFB.Entities;
using UFB.Character;
using UFB.Core;

[CustomEditor(typeof(CharacterManager))]
public class CharacterManagerEditor : Editor
{

    private string _saveConfigName = "PlayerConfiguration";
    private int _loadConfigIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CharacterManager playerManager = (CharacterManager)target;

        _saveConfigName = EditorGUILayout.TextField("Save State Name", _saveConfigName);


        if (GUILayout.Button("Spawn New Character")) 
        {
            
        }

        /**
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

            */

    }

}