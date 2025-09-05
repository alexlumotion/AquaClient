using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSender : MonoBehaviour
{

    public TexturePainter texturePainter; // Посилання на компонент TexturePainter
    public TextureSenderClient textureSenderClient; // Посилання на компонент TextureSenderClient

    public bool sendCustomTexture = false; // Прапорець для відправки текстури


    // Update is called once per frame
    void Update()
    {
        if (sendCustomTexture)
        {
            sendCustomTexture = false; // Скидаємо прапорець після відправки
            SendTexture();
        }
    }
    
    public void SendTexture()
    {
        if (texturePainter == null || textureSenderClient == null)
        {
            Debug.LogError("🚫 TextureSender: Не вказані компоненти TexturePainter або TextureSenderClient.");
            return;
        }

        Texture2D toSend = texturePainter.GetPaintedTextureCopy();
        textureSenderClient.SendTexture(toSend);
    }

}
