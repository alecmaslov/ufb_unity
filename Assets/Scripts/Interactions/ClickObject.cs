using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// global raycast to any object with RaycastSelectable
public class ClickObject : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Get reference to the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        // If left mouse button is clicked
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Convert mouse position to ray
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log("Raycast Hit: " + hit.transform.name);

                IRaycastSelectable selectable = hit.transform.GetComponent<IRaycastSelectable>();

                if (selectable != null)
                {
                    selectable.OnClick();
                }

                // Or if you want to do something with the game object
                GameObject selectedObject = hit.transform.gameObject;
                // Now you can do something with selectedObject...
            }
        }
    }
}
