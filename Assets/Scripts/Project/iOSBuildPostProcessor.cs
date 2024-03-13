// using System.Diagnostics;
// using System.IO;
// using UnityEditor;
// using UnityEditor.Build.Reporting;
// using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;

// public class iOSBuildPostProcessor
// {
//     [PostProcessBuild]
//     public static void OnPostprocessBuild(BuildReport report)
//     {
//         if (report.summary.platform != BuildTarget.iOS)
//             return;

//         UnityEngine.Debug.Log("Running postprocess build for Xcode!");


//         var projectPath = Path.Combine(
//             report.summary.outputPath,
//             "Builds/iOS/Unity-iPhone.xcodeproj/project.pbxproj"
//         );

//         var pbxProject = new PBXProject();
//         pbxProject.ReadFromFile(projectPath);

//         // Unity Framework
//         string target = pbxProject.GetUnityFrameworkTargetGuid();

//         pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-ld_classic");
//         pbxProject.WriteToFile(projectPath);
//     }
// }
