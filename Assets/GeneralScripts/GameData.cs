using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public GameTheme gameTheme;

    public string namePlayer1;
    public string namePlayer2;
    public string namePlayer3;

    public Sprite playerSprite;
    public Sprite enemySprite;
    public Sprite programmableObject1Sprite; 
    public Sprite programmableObject2Sprite;

    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> programmableObject1EventActionDictionary;
    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> programmableObject2EventActionDictionary;
    public Dictionary<ProgrammableEventType, ProgrammableActionType[]> enemyEventActionDictionary;
}

public enum GameTheme
{
    Forest,
    Castle,
    SciFi,
}

