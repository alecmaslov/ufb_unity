using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UFB.Interactions
{
    public class ClickObject : MonoBehaviour
    {
        private UnityEngine.Camera _mainCamera;

        void Start()
        {
            // Get reference to the main camera
            _mainCamera = UnityEngine.Camera.main;
        }

        void Update()
        {
            // If left mouse button is clicked
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Convert mouse position to ray
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Raycast Hit: " + hit.transform.name);

                    if (hit.transform.TryGetComponent<IRaycastSelectable>(out var selectable))
                    {
                        selectable.OnClick();
                    }
                }
            }
        }
    }
}
