using UnityEditor;
using UnityEngine;

public class ChangeTextureType : AssetPostprocessor
{
    // private void OnPostprocessTexture(Texture2D texture)
    // {
    //     TextureImporter textureImporter = assetImporter as TextureImporter;
    //     if (textureImporter != null)
    //     {
    //         textureImporter.textureType = TextureImporterType.Sprite;
    //         AssetDatabase.ImportAsset(assetPath);
    //     }
    // }

    // [MenuItem("Custom/Change Texture Type for All Textures")]
    // private static void ChangeTextureTypeForAll()
    // {
    //     string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] { "Assets/Maps/Kraken" }); // Adjust the folder path as per your project

    //     foreach (string guid in guids)
    //     {
    //         string assetPath = AssetDatabase.GUIDToAssetPath(guid);
    //         TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
    //         if (textureImporter != null)
    //         {
    //             textureImporter.textureType = TextureImporterType.Sprite;
    //             AssetDatabase.ImportAsset(assetPath);
    //         }
    //     }
    // }
}
