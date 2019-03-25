using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : Manager
{
    [SerializeField]
    private TileManager m_tileManager;
    private IBoardFillStrategy m_boardFillStrategy = new RandomBoardFillStrategy();

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
    /// Initialize the game board with chosen fill method
    /// </summary>
    public void InitializeBoard()
    {
        //Init the board arrays
        m_logicalBoard = new int[width, height];
        m_tilesOnBoard = new Tile[width, height];

        //Get the logical filled board data based on chosen fill method
        m_logicalBoard = m_boardFillStrategy.GetFilledBoard(width, height);

        //If we have a filled board from above step
        if(m_logicalBoard != null)
        {
            //Fill the graphical board using logical board data
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Get tile code
                    int tileCode = m_logicalBoard[i, j];

                    //Get the appropriate tile based on tile code
                    Tile tile_to_place = m_tileManager.GetTileFromFactoryByCode(tileCode);

                    //initialize the tile position
                    tile_to_place.transform.position = new Vector2(i, j);

                    //Place the tile on board
                    PlaceTilesOnBoard(tile_to_place, tileCode, i, j);



                }

            }
        }

        else
        {
            Utils.DebugUtils.LogError("Board Dimensions are: Width = " + width + " Height = " + height);
        }


       

        //Find Matches on board Initialization
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FindMatch(new Vector2(i , j), new Vector2(i , j));
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

        startTile.AnimateSwap(targetTileIndex, 0.3f);
        destTile.AnimateSwap(startTileIndex, 0.3f);


        //Place both the tiles on board logically and graphically
        PlaceTilesOnBoard(startTile, startTile.tileData.TILE_CODE, (int)targetTileIndex.x, (int)targetTileIndex.y);
        PlaceTilesOnBoard(destTile, destTile.tileData.TILE_CODE, (int)startTileIndex.x, (int)startTileIndex.y);

       

        //No Match Found
        if (!FindMatch(startTileIndex, targetTileIndex))
        {
            startTile.AnimateSwap(startTileIndex, 0.3f);
            destTile.AnimateSwap(targetTileIndex, 0.3f);

            PlaceTilesOnBoard(startTile, startTile.tileData.TILE_CODE, (int)startTileIndex.x, (int)startTileIndex.y);
            PlaceTilesOnBoard(destTile, destTile.tileData.TILE_CODE, (int)targetTileIndex.x, (int)targetTileIndex.y);
        }

    }


    /// <summary>
    /// Finds match between two swapped tiles
    /// </summary>
    /// <param name="startTileIndex">1st swap tile</param>
    /// <param name="targetTileIndex">2nd swap tile</param>
    /// <returns></returns>
    public bool FindMatch(Vector2 startTileIndex, Vector2 targetTileIndex)
    {
  

        //1st Swap Tile Match check horizontal and vertical
        List<Tile> l1 = FindHorizontalMatches((int)targetTileIndex.x, (int)targetTileIndex.y);
        List<Tile> l2 = FindVerticalMatches((int)targetTileIndex.x, (int)targetTileIndex.y);

        //Temp list to store all found matches
        List<Tile> tileMatches = new List<Tile>();
        bool matchFound = false;

        //Concatinate both horizontal and vertical match found list objects
        if (l1 == null)
        {
            tileMatches = l2;
        }

        else if (l2 == null)
        {
            tileMatches = l1;
        }

        else if (l1 != null && l2 != null)
        {
            tileMatches = l1.Union(l2).ToList();
        }

        if (tileMatches != null && tileMatches.Count > 0)
        {
            //Highlight the found matches
            ShowFoundMatches(ref tileMatches);
            tileMatches.Clear();
            matchFound = true;
        }



        //2nd Swap Tile Match check
        l1 = FindHorizontalMatches((int)startTileIndex.x, (int)startTileIndex.y);
        l2 = FindVerticalMatches((int)startTileIndex.x, (int)startTileIndex.y);





        if (l1 == null)
        {
            tileMatches = l2;
        }

        else if (l2 == null)
        {
            tileMatches = l1;
        }

        else if (l1 != null && l2 != null)
        {
            tileMatches = l1.Union(l2).ToList();
        }

        if (tileMatches != null && tileMatches.Count > 0)
        {
            ShowFoundMatches(ref tileMatches);
            tileMatches.Clear();
            matchFound = true;
        }


        return matchFound;
       


    }





    public bool CheckBounds(int x, int y)
    {
        if ((x >= 0 && x < width) && (y >= 0 && y < height))
        {
            return true;
        }

        return false;
    }


    

    public List<Tile> FindVerticalMatches(int x, int y)
    {
        //Utils.DebugUtils.Log("Row: " + y + " Col: " + x);

        int startTileCode = m_logicalBoard[x, y];
        int matchCount = 1;


        List<Tile> verticalMatches = new List<Tile>();
        verticalMatches.Add(m_tilesOnBoard[x, y]);

        // Up
        for (int i = 1; i <= 2; i++)
        {

            if (!CheckBounds(x, y + i))
            {
                break;
            }



            int nextTileCode = m_logicalBoard[x, y + i];

           
            if (startTileCode == nextTileCode)
            {
                
                matchCount += 1;

                if (!verticalMatches.Contains(m_tilesOnBoard[x, y + i]))
                {
                    verticalMatches.Add(m_tilesOnBoard[x, y + i]);
                }

                if (matchCount >= 3)
                {
                    //Utils.DebugUtils.Log("Matched Vertically " + matchCount);

                }
            }

            else
            {
                
                break;
            }



        }





        // Down
        for (int i = 1; i <= 2; i++)
        {

            if (!CheckBounds(x, y - i))
            {
                break;
            }


            int nextTileCode = m_logicalBoard[x, y - i];


            if (startTileCode == nextTileCode)
            {
                //TODO: Add matching tiles
                matchCount += 1;

                if (!verticalMatches.Contains(m_tilesOnBoard[x, y - i]))
                {
                    verticalMatches.Add(m_tilesOnBoard[x, y - i]);
                }

                if (matchCount >= 3)
                {
                    //Utils.DebugUtils.Log("Matched vertically " + matchCount);

                }
            }

            else
            {
                
                break;
            }



        }
      

       

        if (matchCount >= 3)
        {
            //ShowFoundMatches();
            return verticalMatches;
        }

       

        return null;


    }




    public List<Tile> FindHorizontalMatches(int x, int y)
    {
        //Utils.DebugUtils.Log("Row: " + y + " Col: " + x);

       
        int startTileCode = m_logicalBoard[x, y];
        int matchCount = 1;


        List<Tile> horizontalMatches = new List<Tile>();
        horizontalMatches.Add(m_tilesOnBoard[x, y]);

        // Right 
        for (int i = 1; i <= 2; i++)
        {

            if(!CheckBounds(x + i, y))
            {
                break;
            }


            
            int nextTileCode = m_logicalBoard[x + i, y];

           

            if (startTileCode == nextTileCode)
            {
                //TODO: Add matching tiles
                matchCount += 1;

                if (!horizontalMatches.Contains(m_tilesOnBoard[x + i, y]))
                {
                    horizontalMatches.Add(m_tilesOnBoard[x + i, y]);
                }

                if (matchCount >= 3)
                {
                   // Utils.DebugUtils.Log("Matched Horizontally " + matchCount);
                    
                }
            }

            else
            {
                
                break;
            }
            
           

        }


      


        // Left
        for (int i = 1; i <= 2; i++)
        {

            if (!CheckBounds(x - i, y))
            {
                break;
            }


            int nextTileCode = m_logicalBoard[x - i, y];


            if (startTileCode == nextTileCode)
            {
                //TODO: Add matching tiles
                matchCount += 1;

                if (!horizontalMatches.Contains(m_tilesOnBoard[x - i, y]))
                {
                    horizontalMatches.Add(m_tilesOnBoard[x - i, y]);
                }

                if (matchCount >= 3)
                {
                    //Utils.DebugUtils.Log("Matched Horizontally " + matchCount);

                }
            }

            else
            {
                
                break;
            }



        }

        

        if (matchCount >= 3)
        {
            //ShowFoundMatches();
            return horizontalMatches;
        }


        

        return null;
       



    }


    public void ShowFoundMatches(ref List<Tile> matchedTiles)
    {
        for(int i = 0; i < matchedTiles.Count; i++)
        {
            matchedTiles[i].HighlightTile(true);
        }

      
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
