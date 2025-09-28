using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using GameDevWare.Serialization;

namespace UFB.Entities
{
    public class SpawnEntityParameters { }

    public class PortalSpawnEntityParameters : SpawnEntityParameters
    {
        public int seedId;
        public int portalGroup;
        public int portalIndex;
    }

    public interface ISpawnableEntity
    {
        SpawnEntity SpawnEntity { get; }
        SpawnEntityParameters Parameters { get; }

        void Initialize(SpawnEntity spawnEntity);
    }

    public class Portal : MonoBehaviour, ISpawnableEntity
    {
        public SpawnEntity SpawnEntity { get; private set; }
        public SpawnEntityParameters Parameters { get => _portalParameters; }
        public PortalSpawnEntityParameters _portalParameters;

        [SerializeField]
        private GameObject _bluePortalPrefab;

        [SerializeField]
        private GameObject _greenPortalPrefab;

        private Renderer _renderer;

        public void Initialize(SpawnEntity spawnEntity)
        {
            SpawnEntity = spawnEntity;
            // Parameters = ParseSpawnParameters(spawnEntity.parameters);
            _portalParameters = JsonConvert.DeserializeObject<PortalSpawnEntityParameters>(spawnEntity.parameters);
            _renderer = GetComponent<Renderer>();
            if (_portalParameters.portalIndex == 0) {
                Instantiate(_bluePortalPrefab, transform);
            } else {
                Instantiate(_greenPortalPrefab, transform);
            }
            Debug.Log($"Portal initialized with parameters {_portalParameters.ToDetailedString()}");
            // Addressables.InstantiateAsync(spawnEntity.resourceAddress).Completed += OnPortalLoaded;

            UIGameManager.instance.portals.Add(this);
        }

        public PortalSpawnEntityParameters ParseSpawnParameters(string parameters)
        {
            return JsonConvert.DeserializeObject<PortalSpawnEntityParameters>(parameters);
        }
    }
}
