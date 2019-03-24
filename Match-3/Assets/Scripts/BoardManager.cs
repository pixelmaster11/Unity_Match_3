using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Manager
{
    [SerializeField]
    private TileManager m_tileManager;

    private int[,] m_logicalBoard;
    private Tile[,] m_tilesOnBoard;


    public int width;
    public int height;
    public float cameraOffset;

    
   

    private void Start()
    {
        SetupCamera();    
       
    }


    public override void ManagedUpdate()
    {
        
    }




    /// <summary>
    /// Initialize the game board with random fills
    /// </summary>
    public void InitializeBoard()
    {
        //Init the board arrays
        m_logicalBoard = new int[width, height];
        m_tilesOnBoard = new Tile[width, height];

       
        Random.InitState((int)System.DateTime.Now.Ticks);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                //TODO: ADD Random/Manual Strategy pattern
                int tileCode = Random.Range(1, 9); 
                
                Tile tile_to_place = m_tileManager.GetTileFromFactoryByCode(tileCode);
                tile_to_place.transform.position = new Vector2(i, j);

                PlaceTilesOnBoard(tile_to_place, tileCode, i, j);
            


            }
        }

      
    }





    /// <summary>
    /// This functions places all the tiles on board logically & graphically in both arrays
    /// </summary>
    /// <param name="tile_to_place">The tile to place on the board</param>
    /// <param name="tileCode">Tile code of the tile to place</param>
    /// <param name="x">X index of board</param>
    /// <param name="y">Y index of board</param>
    public void PlaceTilesOnBoard(Tile tile_to_place, int tileCode, int x, int y)
    {
        ///////
        //NOTE: Cannot Set position here as we need to animate movement
        //      Initialize position outside this function avoid that problem
        //      tile_to_place.transform.position = new Vector2(i, j);
        ///////
        
        //Set tile coordinates
        tile_to_place.tileData.X = x;
        tile_to_place.tileData.Y = y;
       
        //Set name in coordinate style for easy debugging of game objects
        tile_to_place.gameObject.name = " Tile (" + x + "," + y + ")";

        //Clean up visuals
        tile_to_place.transform.parent = this.transform;
        tile_to_place.gameObject.SetActive(true);
        tile_to_place.HighlightTile(false);

        //Logically & graphically place the tiles on board
        m_logicalBoard[x, y] = tileCode;
        m_tilesOnBoard[x, y] = tile_to_place;
    }


    /// <summary>
    /// Swap the tiles
    /// </summary>
    /// <param name="startTileIndex">Index / coordinate of 1st tile to swap</param>
    /// <param name="targetTileIndex">Index / coordinate of 2nd tile to swap with</param>
    public void SwapTilesOnBoard(Vector2 startTileIndex, Vector2 targetTileIndex)
    {   
        //Get the graphical tiles to swap from the board
        Tile startTile = m_tilesOnBoard[(int)startTileIndex.x, (int)startTileIndex.y];
        Tile destTile = m_tilesOnBoard[(int)targetTileIndex.x, (int)targetTileIndex.y];

        //Place both the tiles on board logically and graphically
        PlaceTilesOnBoard(startTile, startTile.tileData.TILE_CODE, (int)targetTileIndex.x, (int)targetTileIndex.y);
        PlaceTilesOnBoard(destTile, destTile.tileData.TILE_CODE, (int)startTileIndex.x, (int)startTileIndex.y);
    }


    public void SetupCamera()
    {


        Camera.main.transform.position = new Vector3((float)(width - 1) / 2, (float)(height - 1) / 2, -10);

        float aspectRatio = ((float)Screen.width / (float)Screen.height);

        float cameraVertical = (float)height / 2 + (float)cameraOffset;

        float cameraHorizontal = ((float)width / 2 + (float)cameraOffset) / (float)aspectRatio;

        Camera.main.orthographicSize = cameraVertical > cameraHorizontal ? cameraVertical : cameraHorizontal;
    }

    

  

}
