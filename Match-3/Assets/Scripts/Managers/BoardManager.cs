using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : Manager
{
    [SerializeField]
    private TileManager m_tileManager;
    private IBoardFillStrategy m_boardFillStrategy = new TextBasedBoardFillStrategy();

    private int[,] m_logicalBoard;
    private Tile[,] m_tilesOnBoard;
    List<Tile> m_matchedTiles;


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
        //m_matchedTiles = new List<Tile>();



        //Get the logical filled board data based on chosen fill method
        m_logicalBoard = m_boardFillStrategy.GetFilledBoard(width, height);

        //If we have a filled board from above step
        if (m_logicalBoard != null)
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


        

    }


    public void CheckAllBoardForMatch()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!m_tilesOnBoard[i, j].tileGraphics.tileHighlighted)
                {
                    FindMatch(new Vector2(i, j), new Vector2(i, j), false);
                }

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
       
        
        //Set tile coordinates
        tile_to_place.tileData.X = x;
        tile_to_place.tileData.Y = y;
       
        //Set name in coordinate style for easy debugging of game objects
        tile_to_place.gameObject.name = " Tile (" + x + "," + y + ")";

      
      

        tile_to_place.HighlightTile(false);

        //When clearing tiles, empty space has tilecode 0 which are de-activated tiles
        if (tileCode != 0)
        {
            //Clean up visuals
            tile_to_place.transform.parent = this.transform;
            tile_to_place.gameObject.SetActive(true);
        }

        else
        {
            tile_to_place.transform.position = new Vector2(x, y);
        }


        //Logically & graphically place the tiles on board
        m_logicalBoard[x, y] = tileCode;
        m_tilesOnBoard[x, y] = tile_to_place;

        
    }


    /// <summary>
    /// Swap the tiles
    /// </summary>
    /// <param name="startTileIndex">Destination Index / coordinate of 1st tile to swap/move to</param>
    /// <param name="targetTileIndex">Destination Index / coordinate of 2nd tile to swap/move to</param>
    public void SwapTilesOnBoard(Vector2 startTileIndex, Vector2 targetTileIndex)
    {
        int startTileCode = m_logicalBoard[(int)startTileIndex.x, (int)startTileIndex.y];
        int destTileCode = m_logicalBoard[(int)targetTileIndex.x, (int)targetTileIndex.y];

        //Get the graphical tiles to swap from the board
        Tile startTile = m_tilesOnBoard[(int)startTileIndex.x, (int)startTileIndex.y];
        Tile destTile = m_tilesOnBoard[(int)targetTileIndex.x, (int)targetTileIndex.y];

       

        PlaceTilesOnBoard(startTile, startTileCode, (int)targetTileIndex.x, (int)targetTileIndex.y);
        PlaceTilesOnBoard(destTile, destTileCode, (int)startTileIndex.x, (int)startTileIndex.y);


        
    }


    /// <summary>
    /// Finds match between two swapped tiles
    /// </summary>
    /// <param name="startTileIndex">1st swap tile</param>
    /// <param name="targetTileIndex">2nd swap tile</param>
    /// <returns></returns>
    public bool FindMatch(Vector2 startTileIndex, Vector2 targetTileIndex, bool swapCheck = false)
    {
  
        //1st Swap Tile Match check horizontal and vertical
        List<Tile> l1 = FindHorizontalMatches((int)targetTileIndex.x, (int)targetTileIndex.y);
        List<Tile> l2 = FindVerticalMatches((int)targetTileIndex.x, (int)targetTileIndex.y);


       

        //Temp list to store all found matches
        List<Tile> m_matchedTilesTarget = new List<Tile>();
        List<Tile> m_matchedTilesStart = new List<Tile>();
        bool matchFound = false;

        //Concatinate both horizontal and vertical match found list objects
        if (l1 == null)
        {
            l1 = new List<Tile>();
        }

        if (l2 == null)
        {
            l2 = new List<Tile>();
        }

        if (l1 != null && l2 != null)
        {
            m_matchedTilesTarget = l1.Union(l2).ToList();
        }

        if (m_matchedTilesTarget != null && m_matchedTilesTarget.Count > 0)
        {
            //Highlight the found matches
            //ShowFoundMatches(ref m_matchedTiles);
           
            matchFound = true;
        }


        //If we are checking for matches from all tiles only 1 above iteration is enough
        //This is special case for checking twice when tiles are swapped
        if(swapCheck)
        {
            //2nd Swap Tile Match check
            l1 = FindHorizontalMatches((int)startTileIndex.x, (int)startTileIndex.y);
            l2 = FindVerticalMatches((int)startTileIndex.x, (int)startTileIndex.y);




            if (l1 == null)
            {
                l1 = new List<Tile>();
            }

            if (l2 == null)
            {
                l2 = new List<Tile>();
            }

            if (l1 != null && l2 != null)
            {
                m_matchedTilesStart = l1.Union(l2).ToList();
            }

            if (m_matchedTilesStart != null && m_matchedTilesStart.Count > 0)
            {
               // ShowFoundMatches(ref m_matchedTiles);

                matchFound = true;
            }

        }


        if(m_matchedTilesTarget == null)
        {
            m_matchedTilesTarget = new List<Tile>();
        }

        if(m_matchedTiles == null)
        {
            m_matchedTiles = new List<Tile>();
        }

        if(swapCheck)
        {

            if (m_matchedTilesStart == null)
            {
                m_matchedTilesStart = new List<Tile>();
            }

            m_matchedTiles = m_matchedTiles.Union(m_matchedTilesTarget.Union(m_matchedTilesStart).ToList()).ToList();
        }

        else
        {
            m_matchedTiles = m_matchedTiles.Union(m_matchedTilesTarget).ToList();
        }

        

        if(m_matchedTiles != null)
        {
            ShowFoundMatches(ref m_matchedTiles);
        }
       


      

        return matchFound;
       


    }

  



    /// <summary>
    /// This function finds all the vertical matches in a column
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns a list with all tiles matched vertically in a column</returns>
    private List<Tile> FindVerticalMatches(int x, int y)
    {
        //Get the starting tile from which to start matching
        int startTileCode = m_logicalBoard[x, y];
        int matchCount = 1;

        //Temp list to store all matches found vertically
        List<Tile> verticalMatches = new List<Tile>();

        //Add the starting tile 
        verticalMatches.Add(m_tilesOnBoard[x, y]);

        //This search alogrithm will find matches by searching 2 neighbouring tiles above the starting tile
        // and 2 neighbouring tiles below the starting tile making a total of 5 max tiles that can be matched
        //We continue search if we have matching tile codes in up as well as down direction
        //We end search if next neighbouring tile has different tile code or we have reached the array boundary

        
            // UP (Start searching for 2 mathcing neighbours above starting tile)
            //TODO: Change the number 2 to a variable so we can set how many max matches we can find
            // 2 represents 2 above and 2 below so total 5 matches including the start tile
            for (int i = 1; i < height + width; i++)
            {
                //Check if we have 2 neighbours above
                if (!CheckBounds(x, y + i))
                {
                    //If not break and continue searching for 2 neighbours below the starting tile
                    break;
                }

                //Get the next tile code
                int nextTileCode = m_logicalBoard[x, y + i];

                //If the tile above has same tile code // then a match is found
                if (startTileCode == nextTileCode)
                {
                    //Increment match counter to keep track of how many matches
                    matchCount += 1;

                    //Add the tile to the matches list
                    if (!verticalMatches.Contains(m_tilesOnBoard[x, y + i]))
                    {
                        verticalMatches.Add(m_tilesOnBoard[x, y + i]);
                    }

                    //EXTEND: We can add special bonuses for more matches
                    if (matchCount >= 3)
                    {
                        //Utils.DebugUtils.Log("Matched Vertically " + matchCount);

                    }
                }

                //If the tile above has different tile code then end the up search
                else
                {

                    break;
                }

            }

        




        // Down search
        for (int i = 1; i < height + width; i++)
        {

            if (!CheckBounds(x, y - i))
            {
                break;
            }


            int nextTileCode = m_logicalBoard[x, y - i];


            if (startTileCode == nextTileCode)
            {
              
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
      

       
         //If we have minimum of 3 matches then return those matches
        if (matchCount >= 3)
        {
            return verticalMatches;
        }

       

        return null;


    }



    /// <summary>
    /// This function finds all the horizontal matches in a row
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns a list with all matches found horizontally in a row</returns>
    private List<Tile> FindHorizontalMatches(int x, int y)
    {   
        int startTileCode = m_logicalBoard[x, y];
        int matchCount = 1;

        List<Tile> horizontalMatches = new List<Tile>();
        horizontalMatches.Add(m_tilesOnBoard[x, y]);

         // Right 
            for (int i = 1; i < height + width; i++)
            {

                if (!CheckBounds(x + i, y))
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
        for (int i = 1; i < height + width; i++)
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
            return horizontalMatches;
        }
      
        return null;
      
    }



    /// <summary>
    /// Checks whether the index positons are within the boundaries of the board dimenstion
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns true if index position is within the boundaries of board arrray</returns>
    private bool CheckBounds(int x, int y)
    {
        if ((x >= 0 && x < width) && (y >= 0 && y < height))
        {
            return true;
        }

        return false;
    }



    /// <summary>
    /// Highlights the tiles which are matched
    /// </summary>
    /// <param name="matchedTiles"></param>
    public void ShowFoundMatches(ref List<Tile> matchedTiles)
    {
        

        for(int i = 0; i < matchedTiles.Count; i++)
        {
            matchedTiles[i].HighlightTile(true);
        }


        //StartCoroutine(ClearMatchedTiles(matchedTiles));
        //ClearMatchedTiles(matchedTiles);
      
    }




    //TODO: Clear tiles at index function for later bonus abilities
    public void ClearTiles()
    {
     
        if(m_matchedTiles == null)
        {
            return;
        }

        //Clear Tiles
        for (int i = 0; i < m_matchedTiles.Count; i++)
        {
            //yield return new WaitForSeconds(0.2f);

            int x = m_matchedTiles[i].tileData.X;
            int y = m_matchedTiles[i].tileData.Y;

                
            //Make it Empty tile
            m_logicalBoard[x, y] = 0;

            //Clear Tile
            m_tilesOnBoard[x, y].OnDeSpawnTile();


        }

        Collapse();
            

    }


    //Clears all tiles from col and row
    public void ClearTiles(int col, int row, bool clearRow = false, bool clearCol = false)
    {
        if(clearRow == false & clearCol == false)
        {
            ClearTiles(new Vector2(col, row));
            return;
        }

        if(clearRow)
        {
            //Clear row
            for (int i = 0; i < width; i++)
            {
                //yield return new WaitForSeconds(0.2f);

                //Make it Empty tile
                m_logicalBoard[i, row] = 0;

                //Clear Tile
                m_tilesOnBoard[i, row].OnDeSpawnTile();

                Collapse(i, row);
            }
        }

        if(clearCol)
        {

            //Clear col
            for (int i = 0; i < height; i++)
            {
                //yield return new WaitForSeconds(0.2f);

                //Make it Empty tile
                m_logicalBoard[col, i] = 0;

                //Clear Tile
                m_tilesOnBoard[col, i].OnDeSpawnTile();

                Collapse(col, i);
            }
        }






    }


    public void ClearTiles(Vector2 index)
    {
        int x = (int)index.x;
        int y = (int)index.y;

        //Make it Empty tile
        m_logicalBoard[x, y] = 0;

        //Clear Tile
        m_tilesOnBoard[x, y].OnDeSpawnTile();

        Collapse(x, y);
    }


   



    public void Collapse()
    {
        for (int i = 0; i < m_matchedTiles.Count; i++)
        {

            //We need to move all pieces above all of the cleared tiles by 1 row
            for (int y = m_matchedTiles[i].tileData.Y; y < height - 1; y++)
            {

                m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileGraphics.tileFalling = true;

                float fallSpeed = m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileGraphics.tileFallSpeed;

                m_tileManager.AnimateTile(m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1], new Vector2(m_matchedTiles[i].tileData.X, y), fallSpeed);
               
                SwapTilesOnBoard(new Vector2(m_matchedTiles[i].tileData.X, y + 1), new Vector2(m_matchedTiles[i].tileData.X, y));

                
               // yield return new WaitForEndOfFrame();
            }

          
        }

        
   
        print("End");
    }


    //Collapse at pos x, y
    public void Collapse(int X, int Y)
    {
        
         //We need to move all pieces above all of the cleared tiles by 1 row
        for (int y = Y; y < height - 1; y++)
        {
           m_tilesOnBoard[X, y + 1].tileGraphics.tileFalling = true;

           float fallSpeed = m_tilesOnBoard[X, y + 1].tileGraphics.tileFallSpeed;

           m_tileManager.AnimateTile(m_tilesOnBoard[X, y + 1], new Vector2(X, y), fallSpeed);

           SwapTilesOnBoard(new Vector2(X, y + 1), new Vector2(X, y));
               
        }
    
        print("End");
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
