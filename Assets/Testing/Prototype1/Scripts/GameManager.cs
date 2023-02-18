using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private StateMachine<GameManager> stateMachine;

    public Controls controls;

    public SpriteRenderer spriteRenderer;

    public void SetPlayerSprite(Sprite playerSprite)
    {
        spriteRenderer.sprite = playerSprite;
    }

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

        Rect rect = new(0, 0, canvasSize, canvasSize);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero, canvasSize);

        SetPlayerSprite(sprite);
    }

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void Start()
    {
        stateMachine = new StateMachine<GameManager>(
            this,
            new ArtistState(),
            new DeveloperState(),
            new DesignerState(),
            new PlayTestState()
        );

        stateMachine.ChangeState(typeof(ArtistState));
    }
}

