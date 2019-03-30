using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlock : Tile
{

    public override void OnSpawnTile(TileManager tm)
    {
        m_tileManager = tm;
        tileData.AcceptClick = false;
        tileData.tileType = Enums.TileType.Block;
    }

}
