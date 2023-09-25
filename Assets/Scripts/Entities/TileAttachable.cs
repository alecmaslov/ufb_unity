using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UFB.Entities;
using UFB.Map;

namespace UFB.Entities {

    public class TileAttachable : MonoBehaviour
    {
        private TileEntity _currentTile;
        public TileEntity CurrentTile { 
            get => _currentTile;
            private set {
                LastTile = _currentTile;
                _currentTile = value;
            }
        }

        public TileEntity LastTile { get; private set; }
        public Action<TileEntity> OnAttach;
        public Action<TileEntity> OnDetach;

        public void AttachToTile(TileEntity tile) {
            if (CurrentTile != null) {
                Debug.LogError("TileAttachable already attached to tile " + CurrentTile.name + " Make sure to detach first to avoid unwanted side-effects");
                CurrentTile.DetachEntity(this);
            }
            CurrentTile = tile;


            tile.AttachEntity(this);
            OnAttach?.Invoke(tile);
        }

        public void DetachFromTile() {
            if (CurrentTile == null) {
                return;
            }
            CurrentTile.DetachEntity(this);
            OnDetach?.Invoke(CurrentTile);
            CurrentTile = null;
        }

        public void OnTilePositionUpdated(Vector3 newPosition) {
            transform.position = newPosition;
        }
    }


}