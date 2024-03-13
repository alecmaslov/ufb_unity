// using System.IO;
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;
// using UnityEngine;

// public class AppleBuildPostProcessor : MonoBehaviour
// {
//     [PostProcessBuild(1)]
//     public static void OnPostProcessBuild(BuildTarget target, string path)
//     {
//         if (target == BuildTarget.iOS)
//         {
//             ModifyValues(path);
//         }
//     }

//     private static void ModifyValues(string path)
//     {
//         var projectPath = PBXProject.GetPBXProjectPath(path);
//         var project = new PBXProject();
//         project.ReadFromFile(projectPath);

//         var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
//         project.AddBuildProperty(frameworkTargetGuid, "OTHER_LDFLAGS", "-ld64");

//         project.WriteToFile(projectPath);
//     }
// }
