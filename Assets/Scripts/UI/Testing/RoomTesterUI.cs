using UnityEngine;
using UnityEngine.UI;
using UFB.Network;
using TMPro;
using UFB.UI;

namespace UFB.UITesting
{
    [RequireComponent(typeof(UfbRoomClient))]
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


        void Start()
        {
            _roomClient = GetComponent<UfbRoomClient>();

            createRoomButton.onClick.AddListener(CreateRoom);
            joinRoomButton.onClick.AddListener(JoinRoom);
            leaveRoomButton.onClick.AddListener(LeaveRoom);

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            leaveRoomButton.interactable = false;

            connectionStatusText.text = "Not Connected";
            connectionStatusText.color = Color.red;


            _roomClient.OnClientInitialized += () =>
            {
                connectionStatusText.text = "Connected";
                connectionStatusText.color = Color.green;

                createRoomButton.interactable = true;
                joinRoomButton.interactable = true;
            };

            roomNameText.text = "None";

            _roomClient.OnRoomJoined += (roomId) =>
            {
                roomNameText.text = roomId;
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
            _roomClient.CreateRoom();
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