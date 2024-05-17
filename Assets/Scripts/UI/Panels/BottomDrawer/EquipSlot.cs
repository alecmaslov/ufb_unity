using System.Collections;
using System.Collections.Generic;
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

        // have a property of an equippable

        private Item _item;

        public void Init(Item power)
        {
            _item = power;
            _icon.sprite = GlobalResources.instance.powers[power.id];
            _icon.gameObject.SetActive(true);
        }

        public void ResetImage()
        {
            _item = null;
            _icon.gameObject.SetActive(false);
        }
    }
}
