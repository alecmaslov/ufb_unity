using UnityEditor;
using UnityEngine;
using UFB.Entities;

[CustomEditor(typeof(EffectsManager))]
public class EffectsManagerEditor : Editor
{

    private string _effectName = "tileStretch";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectsManager effectsManager = (EffectsManager)target;

        _effectName = GUILayout.TextField(_effectName);

        if (GUILayout.Button("Run Effect")) {
            Debug.Log("Run Effect " + _effectName);
            effectsManager.RunEffect(_effectName);
        }
    }

}