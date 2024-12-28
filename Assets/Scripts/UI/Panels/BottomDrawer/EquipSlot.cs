using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

namespace UFB.UI
{
    [ExecuteAlways]
    public class EquipSlot : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        public int slotIdx = 0;
        // have a property of an equippable

        private Item _item;

        private PowerMove[] powerMoves;

        public void Init(Item power, PowerMove[] _powerMoves)
        {
            powerMoves = _powerMoves;
            _item = power;
            _icon.sprite = GlobalResources.instance.powers[power.id];
            _icon.gameObject.SetActive(true);
        }

        public void ResetImage()
        {
            _item = null;
            _icon.gameObject.SetActive(false);
        }

        public void OnClickSlot()
        {
            if(_item != null)
            {
                if (UIGameManager.instance.bottomAttackPanel.gameObject.activeSelf) 
                {
                    
                    UIGameManager.instance.bottomAttackPanel.InitPowermove(_item, this, powerMoves);
                }
                else if(UIGameManager.instance.tapSelfPanel.gameObject.activeSelf)
                {
                    UIGameManager.instance.tapSelfPanel.InitPowermove(_item, this, powerMoves);
                }
                else
                {
                    UIGameManager.instance.powerMovePanel.Init(_item, this, powerMoves);
                }
            
            } 
            else
            {
                UIGameManager.instance.equipPanel.OnInitEquipView(slotIdx);
            }
        }
    }
}
