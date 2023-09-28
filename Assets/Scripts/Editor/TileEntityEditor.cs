using UnityEditor;
using UnityEngine;
using UFB.Entities;

[CustomEditor(typeof(TileEntity))]
public class TileEntityEditor : Editor
{
    private float _wallHeight = 0.5f;
    private float _stretchAmount = 10f;
    private float _stretchDuration = 1.0f;
    private bool _isGlowing = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileEntity tileEntity = (TileEntity)target;
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Editor Properties", EditorStyles.boldLabel);

        _wallHeight = EditorGUILayout.FloatField("Wall Height", _wallHeight);


        if (GUILayout.Button("Set Wall Height")) {
            tileEntity.SetWallHeight(_wallHeight);
        }

        if (GUILayout.Button("Toggle Glow"))
        {
            if (_isGlowing) {
                tileEntity.Glow(0, 0.5f);
            } else {
                tileEntity.Glow(1, 0.5f); 
            }
            _isGlowing = !_isGlowing;
        }

        if (GUILayout.Button("Print Info"))
        {
            Debug.Log(tileEntity.GameTile.ToString());
        }

        if (GUILayout.Button("Rotate"))
        {
            tileEntity.gameObject.transform.Rotate(0, 90, 0);
        }


        _stretchAmount = EditorGUILayout.FloatField("Stretch Amount", _stretchAmount);
        _stretchDuration = EditorGUILayout.FloatField("Stretch Duration", _stretchDuration);
        
        if (GUILayout.Button("Stretch")) {
            tileEntity.Stretch(_stretchAmount, _stretchDuration);
        }


        if (GUILayout.Button("Toggle Coordinates")) {
            tileEntity.ToggleCoordinateText();
        }

        // if (GUILayout.Button("Ripple Effect")) {
        //     GameManager.Instance.GameBoard.RunRippleEffect(tileEntity);
        // }

        if (GUILayout.Button("Slam!")) {
            tileEntity.SlamDown();
        }

    }

}