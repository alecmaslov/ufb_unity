using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace UFB.UI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ScreenspacePopupMenu : PopupMenu
    {
        [SerializeField]
        private ScreenspaceMeshTriangle _meshTriangle;

        private GameObject _entity;

        private void OnEnable()
        {
            _meshTriangle.gameObject.SetActive(true);
            _meshTriangle.SetAnchor(_panel);
        }

        public void Initialize(GameObject entity)
        {
            SetTitle(entity.name);
            _entity = entity;
            _meshTriangle.SetTarget(entity.transform);
        }
    }
}
