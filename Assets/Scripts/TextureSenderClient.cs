using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

public class TextureSenderClient : MonoBehaviour
{
    [Header("Налаштування")]
    public string serverIP = "192.168.0.100";
    public int serverPort = 5005;
    public Texture2D textureToSend;
    public string clientId = "client1";
    public bool StartSendTexture = false;

    void Update()
    {
        if (StartSendTexture)
        {
            StartSendTexture = false;
            SendTexture(textureToSend);
        }
    }

    [ContextMenu("▶️ Надіслати текстуру")]
    public void SendTexture(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("[CLIENT] ⚠️ Текстура не задана");
            return;
        }

        try
        {
            Debug.Log($"[CLIENT] 🌐 Підключення до {serverIP}:{serverPort}...");
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            using (NetworkStream stream = client.GetStream())
            {
                client.ReceiveTimeout = 5000;

                byte[] idBytes = Encoding.UTF8.GetBytes(clientId);
                byte[] idLength = BitConverter.GetBytes(idBytes.Length);

                byte[] imageBytes = texture.EncodeToJPG(75); // менше навантаження на мережу
                byte[] imageLength = BitConverter.GetBytes(imageBytes.Length);

                stream.Write(idLength, 0, 4);
                stream.Write(idBytes, 0, idBytes.Length);
                stream.Write(imageLength, 0, 4);
                stream.Write(imageBytes, 0, imageBytes.Length);

                Debug.Log("✅ [CLIENT] Текстура успішно надіслана");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ [CLIENT] Помилка відправки: {ex.Message}");
        }
    }

    public void SetClientStr(string clientName)
    {
        clientId = clientName;
    }

}
