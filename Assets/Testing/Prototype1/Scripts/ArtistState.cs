using System.Collections.Generic;
using UnityEngine;

public class ArtistState : State<GameManager>
{
    public override void OnEnter()
    {
        new Timer(20, OnArtistTurnEnd);
    }

    private void OnArtistTurnEnd()
    {

    }
}
