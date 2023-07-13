using UnityEditor;
using UnityEngine;
using UFB.Effects;

[CustomEditor(typeof(EffectsController))]
public class EffectsControllerEditor : Editor
{

    private string _effectName = "tileStretch";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectsController effectsController = (EffectsController)target;

        _effectName = GUILayout.TextField(_effectName);

        // idea - enumerate all the effects to make a drop down list to run any of them

        if (GUILayout.Button("Run Effect")) {
            Debug.Log("Run Effect " + _effectName);
            effectsController.RunEffect(_effectName);
        }
    }

}