using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace UFB.Project
{
    #if UNITY_EDITOR
    public class Build
    {
        [MenuItem("Build/Build WebGL")]
        public static void BuildWebGL()
        {
            string customPath = System.Environment.GetEnvironmentVariable("UNITY_BUILD_PATH");
            if (customPath == null) {
                Debug.Log($"UNITY_BUILD_PATH not set, using default path: Builds/WebGL");
                customPath = "Builds/WebGL";
            } else {
                Debug.Log($"Building to: {customPath}");
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Main.unity" };
            buildPlayerOptions.locationPathName = "Build/WebGL";
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Run Project")]
        public static void RunProject() 
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
    #endif
}
