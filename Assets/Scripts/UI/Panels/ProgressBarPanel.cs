using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace UFB.UI
{
    public class ProgressBarPanel : MonoBehaviour
    {
        public Slider progressBar;
        private Coroutine _asyncOpCoroutine;


        public void LoadAsyncOperation(AsyncOperation operation)
        {
            // operation.completed += (op) => gameObject.SetActive(false);
            if (_asyncOpCoroutine != null)
            {
                StopCoroutine(_asyncOpCoroutine);
            }
            _asyncOpCoroutine = StartCoroutine(AsyncOperationUpdate(operation));
        }

        private IEnumerator AsyncOperationUpdate(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                progressBar.value = progress;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}