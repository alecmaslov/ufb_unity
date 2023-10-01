using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Events;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace UFB.Events
{
    public class DownloadProgressEvent
    {
        public AsyncOperationHandle handle;
        public string label;

        public Action<DownloadStatus> onProgress;
        public Action onComplete;

        public DownloadProgressEvent(AsyncOperationHandle handle, string label)
        {
            this.handle = handle;
            this.label = label;

            ProgressHandler(handle);

            handle.Completed += (obj) =>
            {
                onComplete?.Invoke();
            };
        }

        private async void ProgressHandler(AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.GetDownloadStatus());
                await Task.Yield();
            }
        }
    }
}

namespace UFB.UI
{
    public class DownloadStatusPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _progressBarPrefab;

        [SerializeField]
        private Transform _progressBarContainer;

        private void OnEnable()
        {
            EventBus.Subscribe<DownloadProgressEvent>(OnDownloadProgressEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DownloadProgressEvent>(OnDownloadProgressEvent);
        }

        private void OnDownloadProgressEvent(DownloadProgressEvent e)
        {
            if (e.handle.IsDone)
                return;

            var progressBarObj = Instantiate(_progressBarPrefab, _progressBarContainer);
            progressBarObj.name = $"{e.label})";

            ProgressBar progressBar = progressBarObj.GetComponent<ProgressBar>();

            e.onProgress = (downloadStatus) =>
            {
                progressBar.SetProgress(downloadStatus.Percent);
                progressBar.SetLabel(
                    $"{e.label} | {Math.Round(downloadStatus.Percent * 100, 1)}% | {Math.Round(downloadStatus.DownloadedBytes * 1e-6, 2)}/{Math.Round(downloadStatus.TotalBytes * 1e-6, 2)} (Mb)"
                );
            };

            e.onComplete = () =>
            {
                Destroy(progressBarObj);
            };
        }
    }
}
