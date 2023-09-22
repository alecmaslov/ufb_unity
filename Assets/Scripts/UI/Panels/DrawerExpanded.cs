using System.Collections;
using System.Collections.Generic;
using UFB.Gameplay;
using UnityEngine;

namespace UFB.UI
{
    public class DrawerExpanded : MonoBehaviour
    {
        public void OnLeaveGameButtonClicked()
        {
            // pop-up "are you sure"
            Debug.Log("Leave Game button clicked");

            GameManager.Instance.LeaveGame();
        }

        public void OnSettingsButtonClicked()
        {
            Debug.Log("Settings button clicked");
        }
    }
}
