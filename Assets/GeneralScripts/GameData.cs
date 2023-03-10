using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public GameTheme gameTheme;

    public string namePlayer1;
    public string namePlayer2;
    public string namePlayer3;

    public Sprite playerSprite;
    public Sprite programmableEnemySprite;
    public Sprite programmableObject1Sprite; 
    public Sprite programmableObject2Sprite;

    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> programmableEnemyEventsActions;
    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> programmableObject1EventsActions;
    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> programmableObject2EventsActions;
}

public enum GameTheme
{
    Forest,
    Castle,
    SciFi,
}

