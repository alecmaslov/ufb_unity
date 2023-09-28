using UnityEditor;
using UnityEngine;
using UFB.Map;
using UFB.Core;
using UFB.Network;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UFB.Network.ApiTypes;
using UnityEditor.Search;

[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    private float _wallHeight = 0.5f;
    private float _stretchAmount = 0.5f;
    private bool _isGlowing = false;

    private string _mapName = "kraken";
    private string _entityName = "chest";
    private int _numSpawns = 10;
    private Vector2Int _tileCoords = new Vector2Int(0, 0);
    private string _mapId = "cln14g2b10g1wpevn2208gypy";

    private UFB.Network.ApiTypes.MapTile[] _mapTiles;

    [SerializeField]
    private UFB.Network.ApiTypes.Map _mapInfo;

    private async Task<UFB.Network.ApiTypes.MapTile[]> GetMapTiles(string mapId)
    {
        var apiClient = new ApiClient("api.thig.io", 8080);
        var mapTiles = await apiClient.Get<UFB.Network.ApiTypes.MapTile[]>(
            $"/maps/tiles?mapId={mapId}"
        );
        return mapTiles;
    }

    private async Task<UFB.Network.ApiTypes.Map> GetMapInfo(string mapId)
    {
        var apiClient = new ApiClient("api.thig.io", 8080);
        var mapInfo = await apiClient.Get<UFB.Network.ApiTypes.Map>($"/maps/{mapId}");
        return mapInfo;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameBoard gameBoard = (GameBoard)target;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Editor Properties", EditorStyles.boldLabel);

        _mapId = EditorGUILayout.TextField("Map ID", _mapId);

        if (GUILayout.Button("Get Map Info (API)"))
        {
            GetMapInfo(_mapId)
                .ContinueWith(map =>
                {
                    _mapInfo = map.Result;
                    Debug.Log($"Got map info! {map.Result.Serialize()}");
                });
        }

        if (GUILayout.Button("Spawn Board (API)"))
        {
            GetMapTiles(_mapId)
                .ContinueWith(tiles =>
                {
                    Debug.Log($"Got map tiles! {tiles.Result.Serialize()}");
                    try
                    {
                        var mapState = ApiConversions.MapStateFromApiResponse(
                            _mapInfo,
                            tiles.Result
                        );
                        Dispatcher.Enqueue(() => gameBoard.SpawnBoard(mapState));
                    }
                    catch (System.Exception e) { 
                        Debug.Log($"Exception: {e}");
                        Debug.Log("Failed to convert map state");
                    }
                });
        }

        // apiClient.Get<UFB.Network.ApiTypes.Map>("/maps/cln14fuec080ypevnq6a4ar6k"); // kraken

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.TextField("Map Name", _mapName);

        // if (GUILayout.Button("Spawn Board"))
        // {
        //     gameBoard.ClearBoard();
        //     gameBoard.SpawnBoard(_mapName);
        // }


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Spawn Entities", EditorStyles.boldLabel);

        _entityName = EditorGUILayout.TextField("Entity Name", _entityName);
        _tileCoords = EditorGUILayout.Vector2IntField("Tile Coords", _tileCoords);
        _numSpawns = EditorGUILayout.IntField("Num Spawns", _numSpawns);

        if (GUILayout.Button("Spawn Entity"))
        {
            // gameBoard.SpawnEntity(_entityName, Coordinates.FromVector2Int(_tileCoords));
            gameBoard.SpawnEntitiesRandom(_entityName, _numSpawns);
        }

        if (GUILayout.Button("Toggle Coordinates"))
        {
            gameBoard.IterateTiles(
                (tile) => {
                    // tile.ToggleCoordinateText();
                }
            );
        }

        // if (GUILayout.Button("Raise Tile"))
        // {
        //     gameBoard.GetComponent
        // }


        // think about making a class called EntitySpawner, that can randomly spawn entities
        // throughout the map

        // raising
        // Z_16 (25, 15)
        // actually raises
        // J_1 (9, 0)
    }
}
