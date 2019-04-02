using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBomb : Tile
{
  
    public Enums.BombType bombType;
    public SpriteRenderer spriteRenderer;


    public override void OnSpawnTile(TileManager tm)
    {
        base.OnSpawnTile(tm);
        tileData.tileType = Enums.TileType.Bomb;

        int rand = Random.Range(1, 5);

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

      
        SwitchBombType(rand);
        
    }


    public override void OnDeSpawnTile()
    {
        tileData.TILE_CODE = Constants.BOMB_TILE_CODE;
        base.OnDeSpawnTile();
    }  


    public void SwitchBombColor(int code)
    {
        

        switch(code)
        {
            case Constants.RED_TILE_CODE:
                spriteRenderer.color = Color.red;
                
                break;

            
            case Constants.GREEN_TILE_CODE:
                spriteRenderer.color = Color.green;
               
                break;

            //Purple
            case Constants.PURPLE_TILE_CODE:
                spriteRenderer.color  = new Color(143, 0 , 255);
                
                break;

            //Aqua
            case Constants.AQUA_TILE_CODE:
                spriteRenderer.color = new Color(0, 255, 252);
                
                break;


            case Constants.YELLOW_TILE_CODE:
                spriteRenderer.color = Color.yellow;
                
                break;


            case Constants.BLUE_TILE_CODE:
                spriteRenderer.color = Color.blue;
               
                break;

            //Pink
            case Constants.PINK_TILE_CODE:
                spriteRenderer.color = new Color(255, 0, 215);
   
                break;

            case Constants.ORANGE_TILE_CODE:
                spriteRenderer.color = new Color(255, 100, 0);
                break;


            default:
                spriteRenderer.color = Color.white;
                   break;

        }
    }


    public void SwitchBombType(int rand)
    {
        switch(rand)
        {
            case 1: bombType = Enums.BombType.Adjacent;
                    break;

            case 2:
                bombType = Enums.BombType.Row;
                break;


            case 3:
                bombType = Enums.BombType.RowCol;
                break;


            case 4:
                bombType = Enums.BombType.Column;
                break;
        }
    }
}
