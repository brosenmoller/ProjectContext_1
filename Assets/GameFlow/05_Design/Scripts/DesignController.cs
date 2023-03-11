using UnityEngine;

public enum GridCellContent
{
    Player = 0,
    Enemy = 1,
    ProgrammableObject1 = 2,
    ProgrammableObject2 = 3,
    GroundTileVariation1 = 4,
    GroundTileVariation2 = 5,
}

public class DesignController : MonoBehaviour
{
    [Header("Settings")]
    public Rect gameArea;

    [Header("References")]
    [SerializeField] private LineRenderer borderLine;

    private void Awake()
    {
        SetupBorder();
    }

    private void SetupBorder()
    {
        borderLine.positionCount = 4;

        borderLine.SetPositions(new Vector3[4]
        {
           new Vector3 (gameArea.x, gameArea.y, 0),
           new Vector3 (gameArea.x + gameArea.width, gameArea.y, 0),
           new Vector3 (gameArea.x + gameArea.width, gameArea.y + gameArea.height, 0),
           new Vector3 (gameArea.x, gameArea.y + gameArea.height, 0),
        });
    }

    public void OnDesignTurnEnd()
    {
        GameManager.Instance.NextTurn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(gameArea.center, gameArea.size);
        DrawRect(gameArea);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }
}

