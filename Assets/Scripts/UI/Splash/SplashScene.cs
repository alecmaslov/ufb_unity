using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    public float delay = 2;
    public float curTime;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= delay)
        {
            curTime = 0;
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
