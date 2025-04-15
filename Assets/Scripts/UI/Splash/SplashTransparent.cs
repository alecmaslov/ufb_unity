using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashTransparent : MonoBehaviour
{
    public float duration;
    public float fadeOutTime;
    
    public Image backgroundColor;
    public Image splashColor;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fadeOutTime += Time.deltaTime;
        float alpha = (duration - fadeOutTime) / duration;
        if (alpha < 0)
        {
            gameObject.SetActive(false);
        }
        backgroundColor.color = new Color(0, 0, 0, alpha);
        splashColor.color = new Color(1, 1, 1, alpha);
    }
}
