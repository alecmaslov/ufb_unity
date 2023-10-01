using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UFB.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public Slider slider;
        public TextMeshProUGUI labelText;

        public void SetProgress(float progress)
        {
            slider.value = progress;
        }

        public void SetLabel(string label)
        {
            labelText.text = label;
        }
    }
}



// public class ProgressBarEvent<T>
// {
//     public Action<T> onProgress;
//     public string label;
//     public string id;

//     public ProgressBarEvent(Action<T> onProgress, string label, string id)
//     {
//         this.onProgress = onProgress;
//         this.label = label;
//     }
// }
