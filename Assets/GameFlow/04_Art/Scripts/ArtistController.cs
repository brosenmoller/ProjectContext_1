using UnityEngine;

public class ArtistController : MonoBehaviour
{
    public void OnArtistTurnEnd()
    {
        Color[,] pixelGrid = PixelBrushController.pixelGrid;
        int canvasSize = pixelGrid.GetLength(0);

        Texture2D texture = new(canvasSize, canvasSize, TextureFormat.RGBA64, false);

        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                texture.SetPixel(x, y, pixelGrid[x, y]);
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Point;

        Rect rect = new(0, 0, canvasSize, canvasSize);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), canvasSize);

        GameManager.Instance.SetPlayerSprite(sprite);
        GameManager.Instance.NextTurn();
    }
}

