using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UFB.Core;
using UFB.Events;
using UFB.Network;
using UnityEngine;

namespace UFB.UI
{
    public class RoomQRGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _qrPanel;

        [SerializeField]
        private Image _qrImage;

        private bool _isQRShowing = false;
        private Texture2D _qrTexture;

        public async void OnShowQRPressed()
        {
            if (!_isQRShowing)
            {
                if (_qrTexture == null)
                {
                    _qrTexture = await GenerateQRTexure();
                    // _qrPanel.GetComponentInChildren<Renderer>().material.mainTexture = _qrTexture;
                    // set qr image texture

                    _qrImage.sprite = Sprite.Create(
                        _qrTexture,
                        new Rect(0, 0, _qrTexture.width, _qrTexture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }

                _qrPanel.SetActive(true);
            }
            else
            {
                _qrPanel.SetActive(false);
            }

            _isQRShowing = !_isQRShowing;
        }

        private async Task<Texture2D> GenerateQRTexure()
        {
            var roomId = ServiceLocator.Current.Get<GameService>().Room.RoomId;
            var encodeController = GetComponent<QRCodeEncodeController>();

            var tcs = new TaskCompletionSource<Texture2D>();

            QRCodeEncodeController.QREncodeFinished handler = null;

            handler = (tex) =>
            {
                tcs.SetResult(tex);
                encodeController.onQREncodeFinished -= handler;
            };
            encodeController.onQREncodeFinished += handler;

            int errorlog = encodeController.Encode(roomId, QRCodeEncodeController.CodeMode.QR_CODE);
            if (errorlog != 0)
            {
                EventBus.Publish(
                    new ToastMessageEvent(
                        "Error scanning QR code: " + errorlog,
                        UIToast.ToastType.Error
                    )
                );
                tcs.SetException(new Exception("QR encoding failed with error: " + errorlog));
                return await tcs.Task;
            }

            return await tcs.Task;
        }
    }
}
