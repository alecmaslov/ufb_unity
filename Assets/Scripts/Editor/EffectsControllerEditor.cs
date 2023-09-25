using UnityEditor;
using UnityEngine;
using UFB.Effects;

[CustomEditor(typeof(EffectsController))]
public class EffectsControllerEditor : Editor
{

    private int _effectChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectsController effectsController = (EffectsController)target;

        string[] effectNames = effectsController.GetEffectsNames();
        if (effectNames != null && effectNames.Length > 0) {
            _effectChoice = EditorGUILayout.Popup("Effect", _effectChoice, effectNames);
            if (GUILayout.Button($"Run {effectNames[_effectChoice]}")) {
                effectsController.RunEffect(effectNames[_effectChoice]);
            }
        }
    }

}