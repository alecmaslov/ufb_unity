using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;

public class QRCodeGenerator : MonoBehaviour
{
    public RawImage qrImage;
    public int size = 512;

    void Start()
    {
        GenerateQRCode("Hello from QRCodeWriter!");
    }

    public void GenerateQRCode(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("QR text is empty");
            return;
        }

        QRCodeWriter qrCodeWriter = new QRCodeWriter();

        BitMatrix bitMatrix = qrCodeWriter.encode(
            text,
            BarcodeFormat.QR_CODE,
            size,
            size
        );

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color32[] pixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isBlack = bitMatrix[x, y];
                pixels[y * size + x] = isBlack ? Color.black : Color.white;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        qrImage.texture = texture;
    }
}