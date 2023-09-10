using UnityEngine;
using UnityEngine.UI;
using UFB.Network;
using TMPro;
using UFB.UI;
using UFB.Gameplay;

namespace UFB.UITesting
{
    public class RoomTesterUI : MonoBehaviour
    {
        public GameObject inputDialogPrefab;
        public Button createRoomButton;
        public Button joinRoomButton;
        public Button leaveRoomButton;

        public RectTransform uiContainer;

        public TextMeshProUGUI connectionStatusText;
        public TextMeshProUGUI roomNameText;

        private UfbRoomClient _roomClient;


        void OnEnable()
        {
            // _roomClient = GetComponent<UfbRoomClient>();
            _roomClient = GameController.Instance.RoomClient;


            createRoomButton.onClick.AddListener(CreateRoom);
            joinRoomButton.onClick.AddListener(JoinRoom);
            leaveRoomButton.onClick.AddListener(LeaveRoom);

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            leaveRoomButton.interactable = false;

            connectionStatusText.text = "Not Connected";
            connectionStatusText.color = Color.red;


            // GameController.

            _roomClient.OnClientInitialized += () =>
            {
                connectionStatusText.text = "Connected";
                connectionStatusText.color = Color.green;

                createRoomButton.interactable = true;
                joinRoomButton.interactable = true;
            };

            roomNameText.text = "None";

            _roomClient.OnRoomJoined += (roomState) =>
            {
                // roomNameText.text = roomId;
                roomNameText.text = _roomClient.Room.RoomId;

                // here we can add info about how many players are in the room


                
                createRoomButton.interactable = false;
                joinRoomButton.interactable = false;
                leaveRoomButton.interactable = true;
            };

            _roomClient.OnRoomLeft += () =>
            {
                roomNameText.text = "None";
                leaveRoomButton.interactable = false;
                createRoomButton.interactable = true;
                joinRoomButton.interactable = true;
            };
        }


        void CreateRoom()
        {
            // _roomClient.CreateRoom();
        }

        void JoinRoom()
        {
            // pop up something here to get user input
            var dialog = Instantiate(inputDialogPrefab, uiContainer).GetComponent<TextInputDialog>();
            dialog.Initialize("Enter Room ID");
            dialog.OnSubmit += (input) =>
            {
                _roomClient.JoinRoom(input);
            };
        }

        void LeaveRoom()
        {
            _roomClient.LeaveRoom();
        }
    }
}