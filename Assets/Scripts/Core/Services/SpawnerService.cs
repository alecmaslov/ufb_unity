using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UFB.Network;

namespace UFB.Dependencies
{
    public interface IRequireNetworkService
    {
        NetworkService NetworkService { get; set; }
    }

    // public interface IRequires


    // public interface IRequire
}

namespace UFB.Core
{
    public class SpawnerService : MonoBehaviour, IService
    {
        private static SpawnerService _instance;

        // Static method to get the instance
        public static SpawnerService Instance => _instance;

        private void Awake()
        {
            // Ensure only one instance of SpawnerService exists
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task<T> Spawn<T>(AssetReference assetReference, Transform parent) where T : MonoBehaviour
        {
            var tcs = new TaskCompletionSource<T>();

            Addressables.InstantiateAsync(assetReference, parent).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    tcs.SetException(new Exception("Failed to instantiate from Addressables."));
                    return;
                }

                T component = obj.Result.GetComponent<T>();

                if (component == null)
                {
                    tcs.SetException(new Exception($"The instantiated object does not contain a component of type {typeof(T)}"));
                    return;
                }

                InjectDependencies(component);
                tcs.SetResult(component);
            };

            return await tcs.Task;
        }


        /// <summary>
        /// Spawns a prefab from Addressables into the gameobject of parentName. If parentName is not found,
        /// a new gameobject will be created with the given name.
        /// </summary>
        public async Task<T> Spawn<T>(AssetReference assetReference, string parentName) where T : MonoBehaviour
        {
            var parent = GameObject.Find(parentName).transform;
            if (parent == null)
            {
                parent = new GameObject(parentName).transform;
            }
            return await Spawn<T>(assetReference, parent);
        }


        public async Task<GameObject> Spawn(AssetReference assetReference) 
        {
            var tcs = new TaskCompletionSource<GameObject>();

            Addressables.InstantiateAsync(assetReference).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    tcs.SetException(new Exception("Failed to instantiate from Addressables."));
                    return;
                }

                tcs.SetResult(obj.Result);
            };

            return await tcs.Task;
        }


        // automatically inject dependencies depending on component requirements
        private void InjectDependencies<T>(T component) where T : MonoBehaviour
        {
            if (component is Dependencies.IRequireNetworkService networkedComponent)
            {
                networkedComponent.NetworkService = ServiceLocator.Current.Get<NetworkService>();
            }
        }
    }
}