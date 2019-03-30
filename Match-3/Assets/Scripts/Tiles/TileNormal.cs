using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNormal : Tile
{

    public override void OnSpawnTile(TileManager tm)
    {
        base.OnSpawnTile(tm);
        tileData.tileType = Enums.TileType.Normal;
    }

}
