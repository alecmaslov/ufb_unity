using UnityEditor;
using UnityEngine;
using UFB.UI;

[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIManager uiManager = (UIManager)target;

        if (GUILayout.Button("Show Toast"))
        {
            uiManager.ShowToast("This is a toast message");
        }

    }

}