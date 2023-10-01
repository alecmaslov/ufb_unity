using UnityEngine;
using System.Collections.Generic;

namespace UFB.Camera
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CameraState", menuName = "UFB/CameraState", order = 1)]
    public class CameraState : ScriptableObject
    {
        public float posX,
            posY,
            posZ;
        public float rotX,
            rotY,
            rotZ,
            rotW;
        public float zoom;
        public float transitionDuration;

        public CameraState(
            Vector3 position,
            Quaternion rotation,
            float zoom,
            float transitionDuration = 1f
        )
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

        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(rotX, rotY, rotZ, rotW);
        }
    }
}
