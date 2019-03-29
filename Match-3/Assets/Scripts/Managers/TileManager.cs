using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Manager
{
    [SerializeField]
    private BoardManager m_boardManager;

    [SerializeField]
    private TileFactory m_tileFactory;

    private Tile m_clickedTile;
    private Tile m_targetTile;

    public bool canAcceptInputs = false;

    float m_timer = 0;

    public override void ManagedUpdate()
    {

       

    }


    private void Update()
    {

        if (m_timer >= 5)
        {
            //m_boardManager.GetRandomSuggestedMove(1);
            m_timer = 0;
        }

        if (canAcceptInputs)
        {
            m_timer += Time.deltaTime;
        }

        else
        {
            m_timer = 0;
        }
       

        
    }

    private void OnEnable()
    {
        m_tileFactory.CreateTilePool();  
       StartCoroutine(m_boardManager.InitializeBoard());
              
    }



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
    /// Called when a tile is touched / clicked for the first time
    /// </summary>
    /// <param name="tile">Which tile is selected</param>
    public void ClickedTile(Tile tile)
    {
        //If no tile is clicked i.e first touch on tile
        if (m_clickedTile == null && canAcceptInputs)
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
        if (m_clickedTile != null && m_targetTile == null && canAcceptInputs)
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
        if (m_targetTile != null && m_clickedTile != null && canAcceptInputs)
        {

            //Check if tiles are adjacent then swap them 
            AdjacencyCheck();
          
        }

      
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
                     
            //Animate tiles again to their previous positions
            AnimateTile(m_clickedTile, startIndex, m_clickedTile.tileGraphics.tileSwapSpeed);
            AnimateTile(m_targetTile, targetIndex, m_targetTile.tileGraphics.tileSwapSpeed);


            //Wait for swap animation to finish
            yield return new WaitForSeconds(m_clickedTile.tileGraphics.tileSwapSpeed);

            //Swap back if no match
            m_boardManager.SwapTilesOnBoard(targetIndex, startIndex);
        

        }

        else
        {

            do
            {
                //Wait for swap animation to finish
                //yield return new WaitForSeconds(m_targetTile.tileGraphics.tileSwapSpeed);

                m_boardManager.ClearTiles();

                //Wait for collapse animation to finish
                yield return new WaitForSeconds(m_targetTile.tileGraphics.tileFallSpeed);

                //print("Begin Fill new tiles");

                m_boardManager.FillNewTiles();

                yield return new WaitForSeconds(m_targetTile.tileGraphics.tileFallSpeed);

                m_boardManager.CheckAllBoardForMatch();

                while (!m_boardManager.IsThereAPossibleMatch())
                {
                    m_boardManager.ShuffleBoard();
                    yield return null;
                }



            }

            while (m_boardManager.BoardHasMatch()); 

        
            



        } 

        //Release the tiles
        m_clickedTile = null;
        m_targetTile = null;

        canAcceptInputs = true;
    }


    

    public void AnimateTile(Tile tile, Vector2 destination, float animationTime)
    {
        
        tile.Animate(destination, animationTime);
    }




}
