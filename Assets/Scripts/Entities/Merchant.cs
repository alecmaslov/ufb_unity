using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using GameDevWare.Serialization;

namespace UFB.Entities
{
    public class MerchantSpawnEntityParameters : SpawnEntityParameters
    {
        public int seedId;
        public int merchantIndex;
        public string merchantName;
        public string[] inventory;
    }

    public class Merchant : MonoBehaviour, ISpawnableEntity
    {
        public SpawnEntity SpawnEntity { get; private set; }
        public SpawnEntityParameters Parameters
        {
            get => _parameters;
        }
        private MerchantSpawnEntityParameters _parameters;

        public void Initialize(SpawnEntity spawnEntity)
        {
            SpawnEntity = spawnEntity;
            _parameters = JsonConvert.DeserializeObject<MerchantSpawnEntityParameters>(
                spawnEntity.parameters
            );
            Debug.Log($"Merchant initialized with parameters {_parameters.ToDetailedString()}");
        }
    }
}
