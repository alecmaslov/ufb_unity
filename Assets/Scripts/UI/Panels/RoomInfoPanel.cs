using UnityEngine;
using TMPro;
using UFB.Core;

namespace UFB.UI
{
    public class RoomInfoPanel : MonoBehaviour
    {
        public TextMeshProUGUI roomIdText;

        public void Awake()
        {
            roomIdText.text = ServiceLocator.Current.Get<GameService>().Room.RoomId;
        }
    }
}
