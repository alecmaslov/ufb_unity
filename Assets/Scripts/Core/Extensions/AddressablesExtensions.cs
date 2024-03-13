using UFB.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesExtensions
{
    public static AsyncOperationHandle<T> LoadAssetAsyncDownloadProgress<T>(
        string resourceAddress,
        string label = null
    )
    {
        var task = Addressables.LoadAssetAsync<T>(resourceAddress);
        EventBus.Publish(new DownloadProgressEvent(task, label));
        return task;
    }
}
