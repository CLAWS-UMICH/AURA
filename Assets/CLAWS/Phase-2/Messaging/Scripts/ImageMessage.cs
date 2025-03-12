using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class ImageMessage
{
    public string imageData;  // Base64-encoded image

    public ImageMessage(Sprite sprite)
    {
        Texture2D texture = SpriteToTexture(sprite);
        imageData = ImageUtils.EncodeImageToBase64(texture);
    }

    private Texture2D SpriteToTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        texture.SetPixels(sprite.texture.GetPixels(
            (int)sprite.textureRect.x, 
            (int)sprite.textureRect.y, 
            (int)sprite.textureRect.width, 
            (int)sprite.textureRect.height));
        texture.Apply();
        return texture;
    }
}


public static class ImageUtils
{
    public static string EncodeImageToBase64(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }

    public static Texture2D DecodeBase64ToImage(string base64String)
    {
        byte[] imageBytes = Convert.FromBase64String(base64String);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageBytes))
        {
            return texture;
        }

        Debug.LogError("Failed to load image from Base64 string.");
        return null;
    }
}
