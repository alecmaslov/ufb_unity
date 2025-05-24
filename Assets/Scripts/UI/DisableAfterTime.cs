using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public float delay;
    private float currentTime;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= delay)
        {
            currentTime = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        currentTime = 0;
    }
}
