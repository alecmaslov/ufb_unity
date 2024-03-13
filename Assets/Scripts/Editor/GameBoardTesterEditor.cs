using UnityEditor;
using UnityEngine;
using UFB.Map;
using UFB.Core;
using UFB.Network;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UFB.Network.ApiTypes;
using UnityEditor.Search;

[CustomEditor(typeof(GameBoardTester))]
public class GameBoardTesterEditor : Editor
{
    private string _mapId = "cln14g2b10g1wpevn2208gypy";

    private UFB.Network.ApiTypes.MapTile[] _mapTiles;

    [SerializeField]
    private UFB.Network.ApiTypes.Map _mapInfo;

    private int _sliderValue = 0;

    private int _tileSliderValue = 0;

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

        GameBoardTester gameBoardTester = (GameBoardTester)target;

        _mapId = EditorGUILayout.TextField("Map ID", _mapId);



        _tileSliderValue = EditorGUILayout.IntSlider("Tile Index", _tileSliderValue, 0, 26 * 26 - 1);

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
                        Dispatcher.Enqueue(() => gameBoardTester.SpawnBoard(mapState));
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Exception: {e}");
                        Debug.Log("Failed to convert map state");
                    }
                });
        }

        if (GUILayout.Button("Stretch Random Tile"))
        {
            gameBoardTester.StretchRandomTile();
        }

        if (GUILayout.Button("Stretch Selected Tile")) 
        {
            gameBoardTester.StretchTileAt(_tileSliderValue);
        }
    }
}
