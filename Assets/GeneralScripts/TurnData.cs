
public enum DeveloperTabs
{
    ProgrammableObject1,
    ProgrammableEnemy,
    ProgrammableObject2,
}

public enum ArtDrawTabs
{
    Player,
    Enemy,
    Finish,
}

public enum ArtChoseTabs 
{
    ProgrammableObject1,
    ProgrammableObject2,
}

public enum RoomType
{
    Other,
    Development,
    Art,
    Design,
    PlayTest,
}

public enum Player
{
    Unassigned,
    Player1,
    Player2,
    Player3,
}

public struct TurnData
{
    public int sceneIndex;
    public float timer;

    public bool programmableObject1Unlocked;
    public bool programmableObject2Unlocked;
    public bool programmableEnemyUnlocked;

    public bool playerDrawUnlocked;
    public bool finishDrawUnlocked;

    public DeveloperTabs startingDeveloperTab;
    public ArtDrawTabs startingArtDrawTab;
    public ArtChoseTabs startingArtChoseTab;

    public bool artChoseStart;
    public bool infinitePlayTest;

    public RoomType roomType;
    public Player player;

    public TurnData(int sceneIndex, float timer, RoomType roomType, Player player, bool programmableObject1Unlocked = false, bool programmableObject2Unlocked = false, bool programmableEnemyUnlocked = false, bool playerDrawUnlocked = false, bool finishDrawUnlocked = false, bool artChoseStart = true, DeveloperTabs startingDeveloperTab = DeveloperTabs.ProgrammableObject1, ArtDrawTabs startingArtDrawTab = ArtDrawTabs.Player, ArtChoseTabs startingArtChoseTab = ArtChoseTabs.ProgrammableObject1, bool infinitePlayTest = false)
    {
        this.sceneIndex = sceneIndex;
        this.timer = timer;
        this.roomType = roomType;
        this.player = player;
        this.programmableObject1Unlocked = programmableObject1Unlocked;
        this.programmableObject2Unlocked = programmableObject2Unlocked;
        this.programmableEnemyUnlocked = programmableEnemyUnlocked;
        this.playerDrawUnlocked = playerDrawUnlocked;
        this.finishDrawUnlocked = finishDrawUnlocked;
        this.artChoseStart = artChoseStart;
        this.startingDeveloperTab = startingDeveloperTab;
        this.startingArtDrawTab = startingArtDrawTab;
        this.startingArtChoseTab = startingArtChoseTab;
        this.infinitePlayTest = infinitePlayTest;
    }
}

