using System.Collections.Generic;
using UnityEngine;

public class HeightmapGenerator : MonoBehaviour
{
    public int width = 26; // x dimension
    public int height = 26; // y dimension
    public Texture2D generatedTexture;

    // public Renderer renderer;

    private Dictionary<Vector2Int, float> _heightMap = new Dictionary<Vector2Int, float>();

    private void Start()
    {
        ApplyHeightmap();
    }

    public void GenerateRandomHeightmap()
    {
        for (int y = 0; y < generatedTexture.height; y++)
        {
            for (int x = 0; x < generatedTexture.width; x++)
            {
                // var color = generatedTexture.GetPixel(x, y);
                // var height = color.r;
                var height = Random.Range(0f, 1f);
                _heightMap.Add(new Vector2Int(x, y), height);
            }
        }
    }

    public void ApplyHeightmap()
    {
        generatedTexture = new Texture2D(width, height);

        for (int y = 0; y < generatedTexture.height; y++)
        {
            for (int x = 0; x < generatedTexture.width; x++)
            {
                // For this example, we're just using random values for height.
                // Replace this with your logic for determining height values.
                float heightValue = Random.Range(0f, 1f);

                Color color = new Color(heightValue, 0, 0); // Only red channel is set
                generatedTexture.SetPixel(x, y, color);
            }
        }

        // Apply changes to the texture and set it as non-mipmap (for simplicity).
        generatedTexture.Apply(false);

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_HeightMap", generatedTexture);

        // Optionally save the texture as an asset (useful for debugging).
        // Uncomment this if you want to save:
        System.IO.File.WriteAllBytes(
            "Assets/Images/GeneratedHeightmap.png",
            generatedTexture.EncodeToPNG()
        );
    }
}
