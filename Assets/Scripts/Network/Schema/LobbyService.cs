using System;
using Colyseus;
using UnityEngine;
using System.Threading.Tasks;
using UFB.Core;
using UFB.Network;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;

public class LobbyService : MonoBehaviour
{
    public static LobbyService Instance;
    private ColyseusRoom<LobbyState> lobby;

    async void Awake()
    {
        Instance = this;
        //await ConnectLobby();
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    public async void OnConnectLobby()
    {
        await ConnectLobby();
        HandleMessage();
    }
    
    public async Task ConnectLobby()
    {
        
        lobby = await ServiceLocator.Current
            .Get<NetworkService>().ColyseusClient.JoinOrCreate<LobbyState>("lobby");
        lobby.OnStateChange += OnLobbyUpdate;
    }

    private void HandleMessage()
    {
        lobby.OnMessage<UIRoomData>("room-by-id", GetRoomById);
        lobby.OnMessage<string>("create-room", ShowWaitingRoom);
    }

    private async void ShowWaitingRoom(string roomId)
    {
        Debug.Log("ShowWaitingRoom : " +  roomId);
        await WaitingRoomManager.instance.Join(roomId);
    }
    private void GetRoomById(UIRoomData message)
    {
        MainScene.instance.createRoomPanel.InitData(message);
    }

    void OnLobbyUpdate(LobbyState state, bool first)
    {
        foreach (LobbyRoomInfo room in state.rooms.Values)
        {
            Debug.Log($"Room: {room.name} ({room.playerCount}/{room.maxPlayers})");
        }
    }

    public async Task GetRoomById(string roomId)
    {
        if (lobby == null)
        {
            await ConnectLobby();
        }
        lobby.Send("room-by-id", roomId);
    }
    
    public void CreateRoom(UfbRoomCreateOptions createOptions, UfbRoomJoinOptions  joinOptions)
    {
        var name = createOptions.mapName;
        var isPrivate = createOptions.isPrivate;
        var maxPlayers = createOptions.rules.maxPlayers;
        var ownerId = createOptions.ownerId;
        lobby.Send("create_room", new
        {
            name,
            ownerId,
            maxPlayers,
            isPrivate
        });
    }

    public void DeleteRoom(string roomId)
    {
        lobby.Send("delete", roomId);
    }
}