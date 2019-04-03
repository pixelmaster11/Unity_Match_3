using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is responsible for handling all board functionality and operation
/// Uses strategy pattern for filling board and shuffling the board
/// This pattern can be extended for all other board operations as well
/// </summary>
public class BoardManager : Manager
{
    #region VARIABLES AND REFERENCES

    [SerializeField]
    private TileManager m_tileManager;

    [SerializeField]
    private AudioManager m_audioManager;

    [SerializeField]
    private ScoreManager m_scoreManager;

    [SerializeField]
    private GameObject m_bgTile;
    private List<GameObject> m_bgTiles = new List<GameObject>();

    private IBoardFillStrategy m_boardFillStrategy;
    private IBoardShuffleStrategy m_boardShuffleStrategy;

    private int[,] m_logicalBoard;
    private Tile[,] m_tilesOnBoard;
    List<Tile> m_matchedTiles;
    List<Tile> m_bombPosition = new List<Tile>();

    public bool Shuffling = true;


    List<Vector2> possibleMoves = new List<Vector2>();
    List<Tile> highlightedMoves = new List<Tile>();

    public int width;
    public int height;
    public float cameraOffset;

    private int[,] m_storedLogicalBoard;

    #endregion // VARIABLES AND REFERENCES

    #region MONO METHODS

    private void Start()
    {      
        SetupCamera();         
    }


    public override void ManagedUpdate()
    {
        
    }

    #endregion //MONO METHODS

    //Initialize functions like fill the board / arrays
    #region INIT

    /// <summary>
    /// Just a helper / debug function to load background tiles
    /// </summary>
    public void InitBGTiles()
    {
        if(m_bgTiles.Count == width * height)
        {
            for (int i = 0; i < m_bgTiles.Count; i++)
            {
                m_bgTiles[i].gameObject.SetActive(true);
            }
        }

        else
        {
            if(m_bgTiles.Count > 0)
            {
                m_bgTiles.Clear();
            }
           

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject go = (GameObject)Instantiate(m_bgTile, new Vector2(i, j), Quaternion.identity);
                    go.transform.parent = m_tileManager.transform;
                    m_bgTiles.Add(go);
                }
            }
        }

       
    }


    /// <summary>
    /// Initialize the game board with chosen fill method
    /// </summary>
    public IEnumerator InitializeBoard()
    {
        InitBGTiles();

        //Init the board arrays
        m_logicalBoard = new int[width, height];
        m_tilesOnBoard = new Tile[width, height];    
        m_matchedTiles = new List<Tile>();
        m_bombPosition = new List<Tile>();
        possibleMoves = new List<Vector2>();
        highlightedMoves = new List<Tile>();


        Random.InitState((int)System.DateTime.Now.Ticks);

        m_boardFillStrategy = new RandomBoardFillStrategy();



        //Fill the graphical board using logical board data
        for (int i = 0; i < width; i++)
        {
           for (int j = 0; j < height; j++)
           {
               
                //Get the logical filled board data based on chosen fill method
                int tileCode = m_boardFillStrategy.FillBoard(width, height, i, j, Constants.MAX_TILE_CODES, 0, false);

 
                //Get the appropriate tile based on tile code
                Tile tile_to_place = m_tileManager.GetTileFromFactory(tileCode);
            
                //Place the tile on board
                PlaceTilesOnBoard(tile_to_place, tile_to_place.tileData.TILE_CODE, i, j);

                int x = 1;


                //while there are matches on fill
                //Get new tile
                while (CheckMatchOnFill(i, j))
                {

                    //If there is a match / remove previous place tile and get a new tile
                    m_tilesOnBoard[i, j].OnDeSpawnTile();

                    //Get new tile 
                    tileCode = m_boardFillStrategy.FillBoard(width, height, i, j, Constants.MAX_TILE_CODES, 0, true);

                    //Get the appropriate tile based on new tile code
                    tile_to_place = m_tileManager.GetTileFromFactory(tileCode);

                
                    //Place the new tile on board
                    PlaceTilesOnBoard(tile_to_place, tile_to_place.tileData.TILE_CODE, i, j);



                    x++;

                    //If max iterations passed
                    //TODO: RESTART LOADING NEW PIECES FROM 0
                    if (x >= 100)
                    {
                        break;
                    }

                }


                //Activate so we can animate tile
                tile_to_place.gameObject.SetActive(true);

                //Animate tile on start
                m_tileManager.AnimateTile(tile_to_place, new Vector2(i, j), 1f);




            }

        }

        //Cache the board for reloading purposes
        m_storedLogicalBoard = m_logicalBoard;

        yield return new WaitForSeconds(1);

        //Check if we have atleast 1 possible move 
        if(IsThereAPossibleMatch())
        {
            m_tileManager.canAcceptInputs = true;
        }

        //If not create a new level
        else
        {

            NewLevel();
        }
       


    }

    #endregion //INIT

    //Checks for any matches on board while filling or iterates through full board for any matches
    #region CHECK FOR MATCHES
    /// <summary>
    /// This function checks for matches as the board gets filled on initialize
    /// </summary>
    /// <param name="x">X position of tile</param>
    /// <param name="y">Y position of tile</param>
    /// <returns></returns>
    public bool CheckMatchOnFill(int x , int y)
    {
        bool matchFoundX = false;
        bool matchFoundY = false;

        //We fill the board from bottom-left so we only need to check 2-left & 2-down neighbours for matches
        //Check for 2-left neighbours of the current tile at x,y
        for (int i = x - 1; i >= (x-2); i--)
        {
            //If within board boundary
            if(CheckBounds(i , y))
            {
                //Break if tile is a block tile
                if(m_tilesOnBoard[x , y].tileData.tileType == Enums.TileType.Block || 
                    m_tilesOnBoard[i , y].tileData.tileType == Enums.TileType.Block)
                {
                    break;
                }

                //Check for match
                if (m_logicalBoard[x, y] == m_logicalBoard[i, y])
                {
                    matchFoundX = true;
                }

                else
                {
                    matchFoundX = false;
                }
            }

            //Break if outside boundary
            else
            {
                break;
            }
         
        }

        //Check for 2-down neighbours of the current tile at x,y
        for (int i = y - 1; i >= (y - 2); i--)
        {
            if(CheckBounds(x, i))
            {

                //Break if tile is a block tile
                if (m_tilesOnBoard[x, y].tileData.tileType == Enums.TileType.Block ||
                    m_tilesOnBoard[x, i].tileData.tileType == Enums.TileType.Block)
                {
                    break;
                }

                if (m_logicalBoard[x, y] == m_logicalBoard[x, i])
                {
                    matchFoundY = true;
                }

                else
                {
                    matchFoundY = false;
                }
            }

            else
            {
                break;
            }

        }

        //if either 2 left or 2 down are matching , then return match found
        return (matchFoundX || matchFoundY);


    }

    /// <summary>
    /// This function checks for all matches on the board 
    /// </summary>
    public void CheckAllBoardForMatch()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {                          
                FindMatch(new Vector2(i, j), new Vector2(i, j), false);
                
            }

        }
    }


    /// <summary>
    /// This functions checks if board has atleast a single match
    /// </summary>
    /// <returns></returns>
    public bool BoardHasMatch()
    {
        if(m_matchedTiles.Count < 3)
        {
            return false;
        }

        return true;
    }

    #endregion // CHECK FOR MATCHES


    //Checks for all possible moves a user can make / suggests moves
    #region CHECK FOR POSSIBLE MOVES
    /// <summary>
    /// This function is responsible for suggesting possible moves to the player
    /// </summary>
    public void SuggestMoves()
    {
        possibleMoves.Clear();

        //For all tiles
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j < height ; j++)
            {
                //Get neighbours and possible moves only if tile is not a block type
                if(m_tilesOnBoard[i , j].tileData.tileType != Enums.TileType.Block)
                {
                    //Get a 3x3 matrix of neighbours 
                    GetNeighbours(i, j);
                }
                          
            }

        }

        //GetRandomSuggestedMove(2);

       
    }


    /// <summary>
    /// This functions checks if there is atleast 1 possible move
    /// </summary>
    /// <returns></returns>
    public bool IsThereAPossibleMatch()
    {
        SuggestMoves();

        if(possibleMoves.Count < 3)
        {
            return false;
        }

        return true;
    }

    
   
    /// <summary>
    /// This function iterates through 3x3 matrix of adjacent neighbours of tile at x,y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void GetNeighbours(int x, int y)
    {
        Vector2 possibleMove = Vector2.zero;

      

        //Left
        if (CheckBounds(x - 1, y) && m_logicalBoard[x, y] == m_logicalBoard[x - 1, y])
        {
              

            possibleMove = GetPossibleMoveX(x, y, 1);

            if (possibleMove != Vector2.zero)
            {
          
               
                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x - 1, y));
                possibleMoves.Add(possibleMove);
            }

            possibleMove = GetPossibleMoveX(x - 1, y, -1);

            if (possibleMove != Vector2.zero)
            {
          

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x - 1, y));
                possibleMoves.Add(possibleMove);
            }


        }


        //Right
        if (CheckBounds(x + 1, y) && m_logicalBoard[x, y] == m_logicalBoard[x + 1, y])
        {
            possibleMove = GetPossibleMoveX(x, y, -1);

            if (possibleMove != Vector2.zero)
            {
             

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x + 1, y));
                possibleMoves.Add(possibleMove);
            }

            possibleMove = GetPossibleMoveX(x + 1, y, 1);

            if (possibleMove != Vector2.zero)
            {
             

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x + 1, y));
                possibleMoves.Add(possibleMove);
               
            }

            possibleMove = GetPossibleMoveXExtra(x, y, -1);

            if (possibleMove != Vector2.zero)
            {
               

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x + 1, y));
                possibleMoves.Add(possibleMove);

            }

            possibleMove = GetPossibleMoveXExtra(x + 1, y, 1);

            if (possibleMove != Vector2.zero)
            {
              

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x + 1, y));
                possibleMoves.Add(possibleMove);
            }

        }



        //Down
        if (CheckBounds(x, y - 1) && m_logicalBoard[x, y] == m_logicalBoard[x, y - 1])
        {
            possibleMove = GetPossibleMoveY(x, y, 1);

            if (possibleMove != Vector2.zero)
            {
               

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y - 1));
                possibleMoves.Add(possibleMove);
            }

            possibleMove = GetPossibleMoveY(x, y - 1, -1);

            if (possibleMove != Vector2.zero)
            {
              

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y - 1));
                possibleMoves.Add(possibleMove);
            }

        }



        //Up
        if (CheckBounds(x, y + 1) && m_logicalBoard[x, y] == m_logicalBoard[x, y + 1])
        {
            possibleMove = GetPossibleMoveY(x, y, -1);

            if (possibleMove != Vector2.zero)
            {
               
                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y + 1));
                possibleMoves.Add(possibleMove);
            }

            possibleMove = GetPossibleMoveY(x, y + 1, 1);

            if (possibleMove != Vector2.zero)
            {
               

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y + 1));
                possibleMoves.Add(possibleMove);
            }

            possibleMove = GetPossibleMoveYExtra(x, y, -1);

            if (possibleMove != Vector2.zero)
            {
                

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y + 1));
                possibleMoves.Add(possibleMove);

            }

            possibleMove = GetPossibleMoveYExtra(x, y + 1, 1);

            if (possibleMove != Vector2.zero)
            {
               

                possibleMoves.Add(new Vector2(x, y));
                possibleMoves.Add(new Vector2(x, y + 1));
                possibleMoves.Add(possibleMove);
            }

        }

            //Left Up
            if (CheckBounds(x - 1, y + 1) && m_logicalBoard[x, y] == m_logicalBoard[x - 1, y + 1] &&
                CheckBounds(x, y + 1) && m_tilesOnBoard[x, y + 1].tileData.tileType != Enums.TileType.Block &&
                CheckBounds(x - 1, y) && m_tilesOnBoard[x - 1, y].tileData.tileType != Enums.TileType.Block)
            {
                possibleMove = GetPossibleMoveDiag(x, y, 1);

                if (possibleMove != Vector2.zero)
                {
                    
                    
                    possibleMoves.Add(new Vector2(x, y));
                    possibleMoves.Add(new Vector2(x - 1, y + 1));
                    possibleMoves.Add(possibleMove);

                }

            }



            //Left Down
            if (CheckBounds(x - 1, y - 1) && m_logicalBoard[x, y] == m_logicalBoard[x - 1, y - 1] &&
                CheckBounds(x - 1, y) && m_tilesOnBoard[x - 1, y].tileData.tileType != Enums.TileType.Block &&
                CheckBounds(x , y - 1) && m_tilesOnBoard[x, y - 1].tileData.tileType != Enums.TileType.Block)
            {
                possibleMove = GetPossibleMoveDiag(x, y, -1);

                if (possibleMove != Vector2.zero)
                {
                   
                    
                    possibleMoves.Add(new Vector2(x, y));
                    possibleMoves.Add(new Vector2(x - 1, y - 1));
                    possibleMoves.Add(possibleMove);
                }

            }


            //RightDown 
            if (CheckBounds(x + 1, y - 1) && m_logicalBoard[x, y] == m_logicalBoard[x + 1, y - 1] &&
                CheckBounds(x, y - 1) && m_tilesOnBoard[x, y - 1].tileData.tileType != Enums.TileType.Block &&
                CheckBounds(x + 1, y) && m_tilesOnBoard[x + 1, y].tileData.tileType != Enums.TileType.Block)
            {
                possibleMove = GetPossibleMoveDiag(x, y, 1);

                if (possibleMove != Vector2.zero)
                {
                  
                    possibleMoves.Add(new Vector2(x, y));
                    possibleMoves.Add(new Vector2(x + 1, y - 1));
                    possibleMoves.Add(possibleMove);
                }

            }


            //Right Up
            if (CheckBounds(x + 1, y + 1) && m_logicalBoard[x, y] == m_logicalBoard[x + 1, y + 1] &&
                CheckBounds(x + 1, y) && m_tilesOnBoard[x + 1, y].tileData.tileType != Enums.TileType.Block &&
                CheckBounds(x, y + 1) && m_tilesOnBoard[x, y + 1].tileData.tileType != Enums.TileType.Block)
            {
                
                possibleMove = GetPossibleMoveDiag(x, y, -1);
                

                if (possibleMove != Vector2.zero)
                {
                   

                    possibleMoves.Add(new Vector2(x, y));
                    possibleMoves.Add(new Vector2(x + 1, y + 1));
                    possibleMoves.Add(possibleMove);
                }

            }

          
        
    }

    /// <summary>
    /// This function gets a random move to suggest
    /// </summary>
    /// <param name="numberOfMoves">Number of moves to suggest to user</param>
    public void GetRandomSuggestedMove(int numberOfMoves)
    {
        //Remove any duplicate entries
        //possibleMoves = possibleMoves.Distinct().ToList();

        if (possibleMoves.Count < 3)
        {
            return;
        }

        int prevRand = -1;
        int rand;
        int randIndex = 0;

        if(numberOfMoves >= (possibleMoves.Count / 3))
        {
            numberOfMoves = 1;
        }

        for(int i = 0; i < numberOfMoves; i++)
        {
                
            rand = Random.Range(0, (possibleMoves.Count / 3)); 
            
            while(rand == prevRand)
            {
                rand = Random.Range(0, (possibleMoves.Count / 3));
            }


            randIndex = rand * 3;

            //Utils.DebugUtils.Log("COunt: " + possibleMoves.Count + " Number of moves: "+ numberOfMoves);
            //Utils.DebugUtils.Log("Rand: " + rand);
            //Utils.DebugUtils.Log("RandIndex: " + randIndex);

            for (int k = randIndex; k < randIndex + 3; k++)
            {
                HighlightMove(possibleMoves[k]);
                highlightedMoves.Add(m_tilesOnBoard[(int)possibleMoves[k].x , (int)possibleMoves[k].y]);
            }

            randIndex = 0;
            prevRand = rand;
        }

      
    }


    /// <summary>
    /// Highlights a given tile
    /// </summary>
    /// <param name="suggestedMove"></param>
    public void HighlightMove(Vector2 suggestedMove)
    {
        m_tilesOnBoard[(int)suggestedMove.x, (int)suggestedMove.y].HighlightTile(true);
    }


    public void ClearHighlightedMoves()
    {
        for(int i = 0; i < highlightedMoves.Count; i++)
        {
            highlightedMoves[i].HighlightTile(false);
        }

        highlightedMoves.Clear();
    }

    //Row wise move check
    //     X     X
    //     0 X X 0 
    //     X     X
    //
    public Vector2 GetPossibleMoveX(int x, int y, int direction)
    {
        //Right/Left Up diagonal
        //And tile below it is not a block tile
        if (CheckBounds(x + 1 * direction, y + 1 ) && m_logicalBoard[x, y] == m_logicalBoard[x + 1 * direction, y + 1] &&
            CheckBounds(x + 1 * direction, y) && m_tilesOnBoard[x + 1 * direction, y].tileData.tileType != Enums.TileType.Block)
        {
           
            return new Vector2(x + 1 * direction, y + 1);
        }

        //Right/Left down diagonal
        else if (CheckBounds(x + 1 * direction, y - 1 ) && m_logicalBoard[x, y] == m_logicalBoard[x + 1 * direction , y - 1 ] &&
            CheckBounds(x + 1 * direction, y) && m_tilesOnBoard[x + 1 * direction, y].tileData.tileType != Enums.TileType.Block)
        {
            
            return new Vector2(x + 1 * direction, y - 1);
        }

       
         return Vector2.zero;
        

    }


    //Col wise move check
    //
    //  X 0 X 
    //    X
    //    X
    //  X 0 X
    //
    public Vector2 GetPossibleMoveY(int x, int y, int direction)
    {
        //Right/Left Up diagonal
        if (CheckBounds(x - 1 , y + 1 * direction) && m_logicalBoard[x, y] == m_logicalBoard[x - 1 , y + 1 * direction] &&
            CheckBounds(x, y + 1 * direction) && m_tilesOnBoard[x, y + 1 * direction].tileData.tileType != Enums.TileType.Block)
        {
            return new Vector2(x - 1, y + 1 * direction);
        }

        //Right/Left down diagonal
        else if (CheckBounds(x + 1 , y + 1 * direction) && m_logicalBoard[x, y] == m_logicalBoard[x + 1, y + 1 * direction] &&
            CheckBounds(x, y + 1 * direction) && m_tilesOnBoard[x, y + 1 * direction].tileData.tileType != Enums.TileType.Block)
        {
            return new Vector2(x + 1, y + 1 * direction);
        }


        return Vector2.zero;

    }


    //Special Col wise case check 
    // X    X
    // 0    X
    // X    0
    // X    X
    
    public Vector2 GetPossibleMoveYExtra(int x, int y, int direction)
    {
        //Right Up/down 2 spaces
        if (CheckBounds(x , y + 2 * direction) && m_logicalBoard[x, y] == m_logicalBoard[x , y + 2 * direction] &&
            CheckBounds(x, y + 1 * direction) && m_tilesOnBoard[x, y + 1 * direction].tileData.tileType != Enums.TileType.Block)
        {
            return new Vector2(x, y + 2 * direction);
        }
      
        return Vector2.zero;

    }


    //Special Row wise move check
    //          
    //     X X 0 X 
    //     X 0 X X    
    //
    public Vector2 GetPossibleMoveXExtra(int x, int y, int direction)
    {
        //Right Up/down 2 spaces
        if (CheckBounds(x + 2 * direction, y) && m_logicalBoard[x, y] == m_logicalBoard[x + 2 * direction, y] &&
            CheckBounds(x + 1 * direction, y) && m_tilesOnBoard[x + 1 * direction, y].tileData.tileType != Enums.TileType.Block)
        {
            return new Vector2(x + 2 * direction, y);
        }

        return Vector2.zero;

    }



    //Special case diagonal move check
    //  X 0 X
    //    X
    //
    //    X
    //  X 0 X
    public Vector2 GetPossibleMoveDiag(int x, int y, int direction)
    {
        
        if (CheckBounds(x - 1, y - 1 * direction) && m_logicalBoard[x, y] == m_logicalBoard[x - 1, y - 1 * direction])
        {
            return new Vector2(x - 1, y - 1 * direction);
        }


        else if (CheckBounds(x + 1 , y + 1 * direction) && m_logicalBoard[x, y] == m_logicalBoard[x + 1, y + 1 * direction]) 
        {
            return new Vector2(x + 1, y + 1 * direction);
        }

        return Vector2.zero;
    }

    #endregion // CHECK FOR POSSIBLE MOVES


    //Swapping tiles logically / placing them in board arrays and shuffling board
    #region BOARD SWAP / PLACEMENT / SHUFFLE
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

        //If tilecode = 0
        else
        {
            tile_to_place.OnDeSpawnTile();
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
    /// This function shuffles the board based on chosen shuffling strategy
    /// </summary>
    public void ShuffleBoard()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        m_boardShuffleStrategy = new SimpleBoardShuffle();

        m_boardShuffleStrategy.ShuffleBoard(m_logicalBoard, m_tilesOnBoard, this, m_tileManager);


    }

    #endregion //BOARD SWAP / PLACEMENT / SHUFFLE


    //Finding match algorithm 
    #region MATCH FINDING
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
            //ShowFoundMatches(ref m_matchedTiles);
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
                
                //Get next TileType
                Enums.TileType nextTileType = m_tilesOnBoard[x , y + i].tileData.tileType;

                //If its a block type do not match
                if(nextTileType == Enums.TileType.Block)
                {
                    break;
                }

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
                    //If we have match of 4 then place a bomb
                    if (matchCount > 3)
                    {
                        //Utils.DebugUtils.Log("Matched Vertically " + matchCount);
                         if(!m_bombPosition.Contains(m_tilesOnBoard[x,y]) && !m_matchedTiles.Contains(m_tilesOnBoard[x,y]))
                            m_bombPosition.Add(m_tilesOnBoard[x, y]);

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

            //Get next TileType
            Enums.TileType nextTileType = m_tilesOnBoard[x, y - i].tileData.tileType;

            //If its a block type do not match
            if (nextTileType == Enums.TileType.Block)
            {
                break;
            }

            if (startTileCode == nextTileCode)
            {
              
                matchCount += 1;

                if (!verticalMatches.Contains(m_tilesOnBoard[x, y - i]))
                {
                    verticalMatches.Add(m_tilesOnBoard[x, y - i]);
                }

                //If we have a match of 4 vertically then place a bomb
                if (matchCount > 3)
                {
                    //Utils.DebugUtils.Log("Matched vertically " + matchCount);
                    if (!m_bombPosition.Contains(m_tilesOnBoard[x, y]) && !m_matchedTiles.Contains(m_tilesOnBoard[x, y]))
                        m_bombPosition.Add(m_tilesOnBoard[x, y]);

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

                //Get next TileType
                Enums.TileType nextTileType = m_tilesOnBoard[x + i, y].tileData.tileType;

                //If its a block type do not match
                if (nextTileType == Enums.TileType.Block)
                {
                    break;
                }

                if (startTileCode == nextTileCode)
                {
                    
                    matchCount += 1;

                    if (!horizontalMatches.Contains(m_tilesOnBoard[x + i, y]))
                    {
                        horizontalMatches.Add(m_tilesOnBoard[x + i, y]);
                    }

                    //If we have match of 4 then place a bomb
                    if (matchCount > 3)
                    {
                        // Utils.DebugUtils.Log("Matched Horizontally " + matchCount);
                        if(!m_bombPosition.Contains(m_tilesOnBoard[x, y]) && !m_matchedTiles.Contains(m_tilesOnBoard[x, y]))
                            m_bombPosition.Add(m_tilesOnBoard[x, y]);

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

            //Get next TileType
            Enums.TileType nextTileType = m_tilesOnBoard[x - i, y].tileData.tileType;

            //If its a block type do not match
            if (nextTileType == Enums.TileType.Block)
            {
                break;
            }

            if (startTileCode == nextTileCode)
            {
                
                matchCount += 1;

                if (!horizontalMatches.Contains(m_tilesOnBoard[x - i, y]))
                {
                    horizontalMatches.Add(m_tilesOnBoard[x - i, y]);
                }

                //If we have a match of 4 // Place bomb
                //TODO: match of 5 for a special bomb
                if (matchCount > 3)
                {
                    if (!m_bombPosition.Contains(m_tilesOnBoard[x, y]) && !m_matchedTiles.Contains(m_tilesOnBoard[x, y]))
                        m_bombPosition.Add(m_tilesOnBoard[x, y]);
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
        
        if (matchedTiles.Count >= 3)
        {

            for (int i = 0; i < matchedTiles.Count; i++)
            {
                matchedTiles[i].HighlightTile(true);
            }
        }
      
                
    }

    #endregion //MATCH FINDING


    //Clearingg tiles --> collapsing tiles --> filling new tiles
    #region CLEAR / COLLAPSE / FILL NEW
    /// <summary>
    /// This function clears the matched tiles from the board
    /// </summary>
    public void ClearTiles()
    {
     
        if(m_matchedTiles == null)
        {
            return;
        }

        //Temp list to hold special cleared tiles by any bombs / bonus abilities
        List<Tile> specialClearedTile = new List<Tile>();

        //Clear Tiles
        for (int i = 0; i < m_matchedTiles.Count; i++)
        {

            int x = m_matchedTiles[i].tileData.X;
            int y = m_matchedTiles[i].tileData.Y;
    
            //Get bombed tiles if any
            if(m_tilesOnBoard[x, y].tileData.tileType == Enums.TileType.Bomb)
            {
                //Get corresponding tiles to clear based on bomb type
                switch(m_tilesOnBoard[x, y].GetComponent<TileBomb>().bombType)
                {
                    case Enums.BombType.Row:
                                              specialClearedTile = specialClearedTile.Union(GetRowTiles(x, y)).ToList();
                                              break;


                    case Enums.BombType.Column:
                                               specialClearedTile = specialClearedTile.Union(GetColTiles(x, y)).ToList();
                                               break;


                    case Enums.BombType.RowCol:
                                               specialClearedTile = specialClearedTile.Union(GetRowTiles(x, y).Union(GetColTiles(x,y)).ToList()).ToList();
                                               break;


                    case Enums.BombType.Adjacent:
                                               specialClearedTile = specialClearedTile.Union(Get3x3AdjacentTiles(x, y)).ToList();
                                               break;
                }
            }
                           
        }

        specialClearedTile = specialClearedTile.Distinct().ToList();

        //Add bombed tiles
        m_matchedTiles = m_matchedTiles.Union(specialClearedTile).ToList();
        m_matchedTiles = m_matchedTiles.Distinct().ToList();

        specialClearedTile.Clear();

        //Clear Tiles
        for (int i = 0; i < m_matchedTiles.Count; i++)
        {
            
            int x = m_matchedTiles[i].tileData.X;
            int y = m_matchedTiles[i].tileData.Y;


            if (m_scoreManager.goalTileCode == m_logicalBoard[x, y])
            {
                m_scoreManager.UpdateTilesCleared();
            }

            //Make it Empty tile
            m_logicalBoard[x, y] = 0;

            //Clear Tile
            m_tilesOnBoard[x, y].OnDeSpawnTile();

           
 
        }

        //Create bombs if any
        CreateBomb();
       


        //Collapse rows / cols
        Collapse();


    }


    #region BOMB
    /// <summary>
    /// This function creates and places a bomb on the board 
    /// </summary>
    public void CreateBomb()
    {
        //Create Bomb
        for (int i = 0; i < m_bombPosition.Count; i++)
        {
            //Get bomb tile from factory
            Tile bombTile = m_tileManager.GetTileFromFactory(Constants.BOMB_TILE_CODE);

            //Get x , y position to place bomb
            int x = (int)m_bombPosition[i].tileData.X;
            int y = (int)m_bombPosition[i].tileData.Y;

            //Set bomb position
            bombTile.transform.position = new Vector2(x, y);

            //Set bomb color and code
            TileBomb bombComponent = bombTile.GetComponent<TileBomb>();
            bombComponent.tileData.TILE_CODE = m_bombPosition[i].tileData.TILE_CODE;
            
            //place bomb on board
            PlaceTilesOnBoard(bombTile, m_bombPosition[i].tileData.TILE_CODE, x, y);

            bombComponent.SwitchBombColor(m_bombPosition[i].tileData.TILE_CODE);

            //Remove the bomb position to fill
            if (m_matchedTiles.Contains(m_bombPosition[i]))
            {
                m_matchedTiles.Remove(m_bombPosition[i]);
            }

        }

        if(m_bombPosition.Count > 0)
        {
            m_audioManager.PlaySFX("OnBomb");
        }

        else
        {
            m_audioManager.PlaySFX("OnMatch");
        }
       

        //Clear bomb positions after placing bombs
        m_bombPosition.Clear();

    }


    /// <summary>
    /// This function returns all the row tiles 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<Tile> GetRowTiles(int x, int y)
    {
        List<Tile> rowTiles = new List<Tile>();

        for(int i = 0; i < width; i++)
        {
            rowTiles.Add(m_tilesOnBoard[i, y]);
        }

        return rowTiles;
    }


    /// <summary>
    /// This function returns all the column tiles of column 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<Tile> GetColTiles(int x, int y)
    {
        List<Tile> colTiles = new List<Tile>();

        for (int i = 0; i < height; i++)
        {
            colTiles.Add(m_tilesOnBoard[x, i]);
        }

        return colTiles;
    }


    /// <summary>
    /// This function returns 3x3 adjacent tiles
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<Tile> Get3x3AdjacentTiles(int x, int y)
    {
        List<Tile> adjTiles = new List<Tile>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if(CheckBounds(x + i, y + j))
                {
                    adjTiles.Add(m_tilesOnBoard[x + i, y + j]);
                }
               
            }
        }

        return adjTiles;
    }

    #endregion //BOMB


    
    /// <summary>
    /// This function collapses all the tiles down into the cleared cells
    /// </summary>
    public void Collapse()
    {

        //m_matchedTiles = m_matchedTiles.OrderBy(x => x.tileData.Y).ToList();

        for (int i = 0; i < m_matchedTiles.Count; i++)
        {

            //We need to move all pieces above all of the cleared tiles by 1 row
            for (int y = m_matchedTiles[i].tileData.Y; y < height - 1; y++)
            {

                int prevY = y;

                //While there is a block tile..keep moving up
                while (m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileData.tileType == Enums.TileType.Block)
                {
                    if ((y + 1) < height - 1)
                    {
                        y++;
                    }

                    else
                    {
                        break;
                    }
                }

                //Check If there is a block tile at the very top of the board
                if(y == height - 2 && m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileData.tileType == Enums.TileType.Block)
                {
                    continue;
                }
               


                m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileGraphics.tileFalling = true;

                float fallSpeed = m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1].tileGraphics.tileFallSpeed;

                m_tileManager.AnimateTile(m_tilesOnBoard[m_matchedTiles[i].tileData.X, y + 1], new Vector2(m_matchedTiles[i].tileData.X, prevY), fallSpeed);
               
                SwapTilesOnBoard(new Vector2(m_matchedTiles[i].tileData.X, y + 1), new Vector2(m_matchedTiles[i].tileData.X, prevY));


                prevY = y;
                

                
            }


           
        }

        
        if(m_matchedTiles.Count > 3)
        {
            m_audioManager.PlaySFX("OnBomb");
        }
   
       
    }

    
    /// <summary>
    ///  This function fills in new tiles after all tiles have been cleared and collapsed
    /// </summary>
    public void FillNewTiles()
    {
       

        while(m_matchedTiles.Count > 0)
        {
            // 1 - 8 tile codes             
            Tile newTile = m_tileManager.GetTileFromFactory(Random.Range(1, Constants.MAX_TILE_CODES + 1));

            int iterations = 1;

            while(m_matchedTiles.Contains(newTile))
            {
                
                 newTile = m_tileManager.GetTileFromFactory(Random.Range(1, Constants.MAX_TILE_CODES + 1));
                 iterations++;

                if(iterations >= 100)
                {
                    newTile = m_tileManager.GetNewTileFromFactory(Random.Range(1, Constants.MAX_TILE_CODES + 1));
                    break;
                }
            }



         

            int x = m_matchedTiles[0].tileData.X;
            int y = m_matchedTiles[0].tileData.Y;

           

            newTile.transform.position = new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(height + 1));
          
            newTile.gameObject.SetActive(true);

            newTile.tileGraphics.tileFalling = true;

            m_tileManager.AnimateTile(newTile, new Vector2(x, y), newTile.tileGraphics.tileFallSpeed);

            PlaceTilesOnBoard(newTile, newTile.tileData.TILE_CODE, x, y);

            //print("Name " + newTile.gameObject.name + " Pos: " + new Vector2(x, y));

            m_matchedTiles.RemoveAt(0);

            m_audioManager.PlaySFX("OnFill");

            //yield return new WaitForSeconds(newTile.tileGraphics.tileFallSpeed);

           

        }


     
    }

    #endregion // CLEAR / COLLAPSE / FILL NEW


    //Functions related to reloading or loading a new level
    #region NEW LEVEL LOAD / CURRENT LEVEL RELOAD

    /// <summary>
    /// Called when level is restarting
    /// </summary>
    public void Restart()
    {        
        StopAllCoroutines();
        LoadPreviousTiles();
    }

    /// <summary>
    /// Called when a new level is loading
    /// </summary>
    public void NewLevel()
    {
        StopAllCoroutines();
        ClearAllTiles();
        StartCoroutine(InitializeBoard());
    }

    /// <summary>
    /// Clears all tiles while reloading / loading a level
    /// </summary>
    public void ClearAllTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                //Make it empty
                m_logicalBoard[i, j] = 0;

                //Clear Tile
                m_tilesOnBoard[i, j].OnDeSpawnTile();


            }
        }


        for (int i = 0; i < m_bgTiles.Count; i++)
        {
            m_bgTiles[i].gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Reloads all previously loaded tiles
    /// </summary>
    public void LoadPreviousTiles()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {

           

                //Restore old board
                m_logicalBoard[i, j] = m_storedLogicalBoard[i, j];

                //Clear Tile
                m_tilesOnBoard[i, j].OnDeSpawnTile();


                //Get the appropriate tile based on tile code
                Tile tile_to_place = m_tileManager.GetTileFromFactory(m_logicalBoard[i , j]);

                //Place the tile on board
                PlaceTilesOnBoard(tile_to_place, tile_to_place.tileData.TILE_CODE, i, j);

                //Activate so we can animate tile
                tile_to_place.gameObject.SetActive(true);

                //Animate tile on start
                m_tileManager.AnimateTile(tile_to_place, new Vector2(i, j), 1f);



            }
        }


        m_matchedTiles.Clear();
        m_bombPosition.Clear();
        possibleMoves.Clear();
        highlightedMoves.Clear();
        SuggestMoves();

        m_tileManager.canAcceptInputs = true;
    }

    #endregion // NEW LEVEL / CURRENT LEVEL RELOAD

    //NOTE: IDEALLY WOULD HAVE SEPARATE CLASSES FOR HANDLING DIFFERENT OPERATIONS

    /// <summary>
    /// Setups up camera based on board width and height
    /// </summary>
    public void SetupCamera()
    {


        Camera.main.transform.position = new Vector3((float)(width - 1) / 2, (float)(height - 1) / 2, -10);

        float aspectRatio = ((float)Screen.width / (float)Screen.height);

        float cameraVertical = (float)height / 2 + (float)cameraOffset;

        float cameraHorizontal = ((float)width / 2 + (float)cameraOffset) / (float)aspectRatio;

        Camera.main.orthographicSize = cameraVertical > cameraHorizontal ? cameraVertical : cameraHorizontal;
    }

    

  

}
