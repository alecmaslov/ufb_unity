using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UFB.Events;

namespace UFB.UI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WorldspacePopupMenu : PopupMenu
    {
        [SerializeField]
        private float lookAtCameraSpeed = 0.1f;

        private WorldspaceMeshTriangle _meshTriangle;

        [SerializeField]
        private Material _meshTriangleMaterial;

        private UnityEngine.Camera _camera;

        public override void Initialize(PopupMenuEvent popupMenuEvent)
        {
            base.Initialize(popupMenuEvent);
            _meshTriangle.SetTarget(popupMenuEvent.target);
        }

        private void OnEnable()
        {
            _meshTriangle = WorldspaceMeshTriangle.Create(_meshTriangleMaterial);
            _meshTriangle.SetAnchor(_panel);
            _camera = UnityEngine.Camera.main;
            transform.LookAt(_camera.transform);
        }

        private void OnDisable()
        {
            Destroy(_meshTriangle.gameObject);
        }

        private void Update()
        {
            var lookRotation = Quaternion.LookRotation(
                _camera.transform.forward,
                _camera.transform.up
            );
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                lookAtCameraSpeed
            );
        }
    }
}
