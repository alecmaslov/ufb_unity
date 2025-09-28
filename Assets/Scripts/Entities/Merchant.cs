using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using GameDevWare.Serialization;
using UFB.Camera;
using UFB.Events;

namespace UFB.Entities
{
    public class MerchantSpawnEntityParameters : SpawnEntityParameters
    {
        public int seedId;
        public int merchantIndex;
        public string merchantName;
        public string[] inventory;
    }

    public class Merchant : MonoBehaviour, ISpawnableEntity, ICameraFocusable, IClickable
    {
        public SpawnEntity SpawnEntity { get; set; }
        public SpawnEntityParameters Parameters
        {
            get => _parameters;
        }
        private MerchantSpawnEntityParameters _parameters;

        private void OnEnable()
        {
            EventBus.Subscribe<SpawnItemEvent>(OnGetItemEvent);

        }
        private void OnDisable()
        {
            EventBus.Unsubscribe<SpawnItemEvent>(OnGetItemEvent);
        }

        public void Initialize(SpawnEntity spawnEntity)
        {
            SpawnEntity = spawnEntity;
            _parameters = JsonConvert.DeserializeObject<MerchantSpawnEntityParameters>(
                spawnEntity.parameters
            );
            Debug.Log($"Merchant initialized with parameters {_parameters.ToDetailedString()}");
        }

        private void OnGetItemEvent(SpawnItemEvent e)
        {
            if (e.tileId != SpawnEntity.tileId) return;
            Debug.Log($"merchant ==>> {e.tileId}, {e.targetTileId}");

            SpawnEntity.tileId = e.targetTileId;

            if (e.target != null) 
            {
                e.tile.AttachGameObject(gameObject, true);
            }
            //Destroy(gameObject);

        }

        public void OnFocus()
        {
            Debug.Log("Chest focused");
            EventBus.Publish(new CameraOrbitAroundEvent(transform, 0.5f));
        }

        public void OnUnfocus()
        {
            //throw new System.NotImplementedException();
        }

        public void OnClick()
        {
            // have some global way of determining click behavior of items,
            // which is informed by the ui manager and its current state
            OnFocus();
        }
    }
}
