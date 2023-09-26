using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "CameraState", menuName = "UFB/CameraState", order = 1)]
public class CameraState : ScriptableObject
{
    // Individual float components for position and rotation
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float zoom;
    public float transitionDuration;

    // Constructor using Vector3 and Quaternion
    public CameraState(Vector3 position, Quaternion rotation, float zoom, float transitionDuration = 1f)
    {
        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.rotX = rotation.x;
        this.rotY = rotation.y;
        this.rotZ = rotation.z;
        this.rotW = rotation.w;
        this.zoom = zoom;
        this.transitionDuration = transitionDuration;
    }

    // Constructor using CameraController
    public CameraState(CameraController cameraController, float transitionDuration = 1f)
    {
        Vector3 position = cameraController.transform.position;
        Quaternion rotation = cameraController.transform.rotation;

        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.rotX = rotation.x;
        this.rotY = rotation.y;
        this.rotZ = rotation.z;
        this.rotW = rotation.w;
        this.zoom = cameraController.Zoom.CurrentZoom;
        this.transitionDuration = transitionDuration;
    }

    // Convenience methods for getting the Vector3 position and Quaternion rotation
    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }
}