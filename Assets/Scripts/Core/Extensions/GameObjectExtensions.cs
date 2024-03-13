using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject gameObject)
        where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    public static void DestroyAllChildren(this GameObject gameObject)
    {
        // Check if the application is playing
        if (Application.isPlaying)
        {
            // Iterate backwards to safely destroy all children
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            // Use DestroyImmediate in the editor
            var children = new List<GameObject>();
            foreach (Transform child in gameObject.transform)
            {
                children.Add(child.gameObject);
            }

            foreach (GameObject child in children)
            {
                GameObject.DestroyImmediate(child);
            }
        }
    }
}
