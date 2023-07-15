using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UFB.Entities;

namespace UFB.Entities {

    public class TileAttachable : MonoBehaviour
    {
        public TileEntity CurrentTile { get; private set; }
        public Action<TileEntity> OnAttach;
        public Action<TileEntity> OnDetach;

        public void AttachToTile(TileEntity tile) {
            CurrentTile = tile;
            tile.AttachEntity(this);
            OnAttach?.Invoke(tile);
        }

        public void DetachFromTile() {
            CurrentTile.DetachEntity(this);
            OnDetach?.Invoke(CurrentTile);
            CurrentTile = null;
        }

        public void OnTilePositionUpdated(Vector3 newPosition) {
            transform.position = newPosition;
        }
    }


}