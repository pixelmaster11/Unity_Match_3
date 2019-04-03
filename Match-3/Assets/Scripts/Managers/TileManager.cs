using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be responsible for handling tile managent such as tile swapping / or swapping back / checking for adajceny of tiles
/// </summary>
public class TileManager : Manager
{
    //All variable definiation and references
    #region VARIABLES AND REFERENCES

    [SerializeField]
    private BoardManager m_boardManager;

    [SerializeField]
    private TileFactory m_tileFactory;

    [SerializeField]
    private AudioManager m_audioManager;

    [SerializeField]
    private ScoreManager m_scoreManager;

    [SerializeField]
    private UIManager m_uiManager;

    private Tile m_clickedTile;
    private Tile m_targetTile;

    public bool canAcceptInputs = false;
    public bool gameOver = false;
    float m_timer = 0;
    private bool m_pause = false;

    #endregion // VARIABLES AND REFERENCES

    //Quick way to get pause and suggest move done
    //For pause would ideally have game states 
    #region MONO METHODS

    private void Update()
    {

        if (m_timer >= 5f && canAcceptInputs && !gameOver)
        {
            m_boardManager.ClearHighlightedMoves();
            m_boardManager.GetRandomSuggestedMove(1);
            m_timer = 0;
        }

        if (canAcceptInputs && !gameOver)
        {
            m_timer += Time.deltaTime;
        }

        else
        {
            m_timer = 0;
        }
          
        
    }

    #endregion // MONO METHODS


    //Would ideally have a separate input script
    #region INPUT HANDLE

    /// <summary>
    /// Called when a tile is touched / clicked for the first time
    /// </summary>
    /// <param name="tile">Which tile is selected</param>
    public void ClickedTile(Tile tile)
    {
        //If no tile is clicked i.e first touch on tile
        if (m_clickedTile == null && canAcceptInputs && !gameOver)
        {

            m_clickedTile = tile;
        }
    }


    /// <summary>
    /// Called when swiped or pointer dragged
    /// </summary>
    /// <param name="tile">which tile is dragged to</param>
    public void DragTile(Tile tile)
    {
        //If we already have a tile clicked, then assign the dragged tile as target tile to swap with
        if (m_clickedTile != null && m_targetTile == null && canAcceptInputs && !gameOver)
        {
            m_targetTile = tile;

        }
    }

    /// <summary>
    /// Called when swipe end/ pointer release
    /// </summary>
    public void ReleaseTile()
    {
        //If we have both start and target tiles then swap if adjacent
        if (m_targetTile != null && m_clickedTile != null && canAcceptInputs && !gameOver)
        {

            //Check if tiles are adjacent then swap them 
            AdjacencyCheck();

        }

        else
        {
            m_targetTile = null;
            m_clickedTile = null;
            canAcceptInputs = true;

        }


    }

    #endregion //INPUT HANDLE


    //All tile management like swapping / adjacency / etc 
    #region TILE MANAGEMENT

    /// <summary>
    /// Return a tile from the tile pool
    /// </summary>
    /// <returns>Return a tile from the tile pool</returns>
    public Tile GetTileFromFactory()
    {
        Tile tile = m_tileFactory.GetTile();
        tile.OnSpawnTile(this);

        return tile;
    }


    /// <summary>
    /// Return a tile from the tile pool with corresponding tileCode
    /// </summary>
    /// <param name="tileCode">Tile of tilecode to return</param>
    /// <returns>Return a tile from the tile pool with corresponding tileCode</returns>
    public Tile GetTileFromFactory(int tileCode)
    {
        Tile tile = m_tileFactory.GetTile(tileCode);
        tile.OnSpawnTile(this);

        return tile;
    }

    /// <summary>
    /// Return a tile from the tile pool with corresponding tileCode
    /// </summary>
    /// <param name="tileCode">Tile of tilecode to return</param>
    /// <returns>Return a tile from the tile pool with corresponding tileCode</returns>
    public Tile GetNewTileFromFactory(int tileCode)
    {
        Tile tile = m_tileFactory.GetNewTile(tileCode);
        tile.OnSpawnTile(this);

        return tile;
    }



   
    /// <summary>
    /// Checks whether the clicked tile and target tiles are adjacent to swap
    /// </summary>
    public void AdjacencyCheck()
    {
    
        Vector2 startIndex = new Vector2(m_clickedTile.tileData.X, m_clickedTile.tileData.Y);
        Vector2 targetIndex = new Vector2(m_targetTile.tileData.X, m_targetTile.tileData.Y);

        //Adjacency check
        if ((targetIndex.x == startIndex.x) && (Mathf.Abs(targetIndex.y - startIndex.y) == 1) ||
            (targetIndex.y == startIndex.y) && (Mathf.Abs(targetIndex.x - startIndex.x) == 1))
        {

            canAcceptInputs = false;

            //Utils.DebugUtils.Log("Clicked Tile: " + m_clickedTile.name + " & Target Tile : " + m_targetTile + " are adjacent and can be swapped");

            //Clear any highlighted moves
            m_boardManager.ClearHighlightedMoves();

            //Swap Tiles
            StartCoroutine(SwapTiles(startIndex, targetIndex));
        }

        //Debug if tiles not adjacent
        else
        {

            //Utils.DebugUtils.Log("Tiles: " + m_clickedTile.name + " & " + m_targetTile.name + " are not ADJACENT");
            canAcceptInputs = true;

            m_clickedTile = null;
            m_targetTile = null;
        }

    }

  

    /// <summary>
    /// This is the main game loop 
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="targetIndex"></param>
    /// <returns></returns>
    private IEnumerator SwapTiles(Vector2 startIndex, Vector2 targetIndex)
    {

      
        //Animate Swap      
        AnimateTile(m_clickedTile, targetIndex, m_clickedTile.tileGraphics.tileSwapSpeed);
        AnimateTile(m_targetTile, startIndex, m_targetTile.tileGraphics.tileSwapSpeed);
   

        //Wait for swap animation to finish
        yield return new WaitForSeconds(m_targetTile.tileGraphics.tileSwapSpeed);
      

        //If adjacent then swap tiles on the board
        m_boardManager.SwapTilesOnBoard(startIndex, targetIndex);


       


        //No Match Found // Move Tiles back to their original position before swap
        if (!m_boardManager.FindMatch(startIndex, targetIndex, true))
        {

            m_audioManager.PlaySFX("NoSwap");

            //Animate tiles again to their previous positions
            AnimateTile(m_clickedTile, startIndex, m_clickedTile.tileGraphics.tileSwapSpeed);
            AnimateTile(m_targetTile, targetIndex, m_targetTile.tileGraphics.tileSwapSpeed);


            //Wait for swap animation to finish
            yield return new WaitForSeconds(m_clickedTile.tileGraphics.tileSwapSpeed);

            //Swap back if no match
            m_boardManager.SwapTilesOnBoard(targetIndex, startIndex);
        

        }

        //Match Found
        else
        {
            m_scoreManager.UpdateMoves();

            do
            {
                

                //Clear and collapse
                m_boardManager.ClearTiles();

                //Wait for collapse animation to finish
                yield return new WaitForSeconds(m_targetTile.tileGraphics.tileFallSpeed);

         
                //Fill new
                m_boardManager.FillNewTiles();

                yield return new WaitForSeconds(m_targetTile.tileGraphics.tileFallSpeed);

                //Check for matches
                m_boardManager.CheckAllBoardForMatch();


                //Check for possible matches
                //TODO: Keep a shufflingg limit, otherwise load new tiles
                while (!m_boardManager.IsThereAPossibleMatch())
                {
                    //Check for already present matches if any before shuffle
                    m_boardManager.CheckAllBoardForMatch();

                    //If there is a match already present then break no need to shuffle
                    if (m_boardManager.BoardHasMatch())
                    {
                        break;
                    }

                    //Shuffle board if no possible match
                    m_boardManager.Shuffling = true;
                    m_boardManager.ShuffleBoard();

                    yield return new WaitForSeconds(1);
                                   
                }

                //Check for already present matches if any after shuffle
                m_boardManager.CheckAllBoardForMatch();

                m_boardManager.Shuffling = false;
              
            }

            //Loop till there is match
            while (m_boardManager.BoardHasMatch());


           



        } 

        //Release the tiles
        m_clickedTile = null;
        m_targetTile = null;
        m_boardManager.Shuffling = false;
        canAcceptInputs = true;
    }


    /// <summary>
    /// This will animate a given tile from its current position to destination position
    /// </summary>
    /// <param name="tile">Tile to animate</param>
    /// <param name="destination">Destination position to move / animate tile to</param>
    /// <param name="animationTime">Time which willl take the animation / movement to complete</param>
    public void AnimateTile(Tile tile, Vector2 destination, float animationTime)
    {        
        tile.Animate(destination, animationTime);
    }

    #endregion // TILE MANAGEMENT


    // A quick hacky way to get ui functionality done. Normally would be done in a separate scripts
    #region UI FUNCTIONALITY



    /// <summary>
    /// This function gets called when pause button is clicked
    /// </summary>
    public void OnPause()
    {
        m_pause = !m_pause;

        if (m_pause)
        {
            canAcceptInputs = false;
            Time.timeScale = 0;
        }

        else
        {
            canAcceptInputs = true;
            Time.timeScale = 1;
        }


        m_uiManager.Pause(m_pause);
    }


    /// <summary>
    /// This function gets called when a new level load button is clicked
    /// </summary>
    public void OnNewLevel()
    {
        m_timer = 0;
        canAcceptInputs = false;
        gameOver = false;
        m_pause = false;

        StopAllCoroutines();

        m_scoreManager.NewLevel();
        m_boardManager.NewLevel();
        m_uiManager.NewLevel();
    }


    /// <summary>
    /// This function gets called when a restart level button is clicked
    /// This will reload the current level
    /// </summary>
    public void OnRestart()
    {
        m_timer = 0;
        canAcceptInputs = false;
        gameOver = false;


        StopAllCoroutines();
        OnPause();

        m_scoreManager.Restart();
        m_boardManager.Restart();

    }



    //Again a hacky way to get Main Menu functionality done. Normally woudl have completely different scripts for main menu / ui / etc

    /// <summary>
    /// This is called when play button is clicked from Main Menu
    /// </summary>
    public void OnPlay()
    {
        StartCoroutine(OnPlayRoutine());
    }


    /// <summary>
    /// Start the play coroutine by fading away main menu
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnPlayRoutine()
    {
        yield return StartCoroutine(m_uiManager.PlayRoutine(0, 2, false));

        gameOver = false;
        m_tileFactory.CreateTilePool();
        StartCoroutine(m_boardManager.InitializeBoard());
        m_scoreManager.OnPlay();

    }


    /// <summary>
    /// This function gets called when quit button is clicked after win / lose
    /// </summary>
    public void OnQuit()
    {
        m_pause = false;
        canAcceptInputs = false;
        m_timer = 0;

        m_boardManager.ClearAllTiles();
        m_uiManager.NewLevel();
        StartCoroutine(m_uiManager.PlayRoutine(1, 2, true));
    }




    /// <summary>
    /// If won or lost / stop accepting inputs
    /// </summary>
    /// <param name="won"></param>
    public void Won(bool won)
    {
        gameOver = true;
        canAcceptInputs = false;
    }

    public override void ManagedUpdate()
    {
        throw new System.NotImplementedException();
    }

    #endregion // UI FUNCTIONALITY

}
