using UnityEngine;
using System.Runtime.Serialization;
using System.Text;

public class SpriteSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Sprite sprite = (Sprite)obj;

        byte[] textureBytes = sprite.texture.EncodeToPNG();
        info.AddValue("textureBytes", Encoding.Default.GetString(textureBytes), typeof(string));

        info.AddValue("rectX", sprite.rect.x);
        info.AddValue("rectY", sprite.rect.y);
        info.AddValue("rectWidth", sprite.rect.width);
        info.AddValue("rectHeight", sprite.rect.height);

        info.AddValue("pivotX", sprite.pivot.x);
        info.AddValue("pivotY", sprite.pivot.y);

        info.AddValue("pixelPerUnit", sprite.pixelsPerUnit);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        int canvasSize = (int)info.GetValue("rectWidth", typeof(int));

        Texture2D texture = new(canvasSize, canvasSize, TextureFormat.RGBA64, false);
        Rect rect = new(
            (int)info.GetValue("rectX", typeof(int)),
            (int)info.GetValue("rectY", typeof(int)),
            (int)info.GetValue("rectWidth", typeof(int)),
            (int)info.GetValue("rectHeight", typeof(int))
        );

        Vector2 pivot = new(
            (float)info.GetValue("pivotX", typeof(float)),
            (float)info.GetValue("pivotY", typeof(float))
        );

        int pixelPerUnit = (int)info.GetValue("pixelPerUnit", typeof(int));

        byte[] textureBytes = Encoding.Default.GetBytes((string)info.GetValue("textureBytes", typeof(string)));
        texture.LoadImage(textureBytes);
        
        Sprite sprite = Sprite.Create(texture, rect, pivot, pixelPerUnit);
        
        obj = sprite;
        return obj;
    }
}

