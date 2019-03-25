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



    public override void ManagedUpdate()
    {

    }



    private void OnEnable()
    {
        m_tileFactory.CreateTilePool();
        m_boardManager.InitializeBoard();
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
    public Tile GetTileFromFactoryByCode(int tileCode)
    {
        Tile tile = m_tileFactory.GetTile(tileCode);
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
        if (m_clickedTile == null)
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
        if (m_clickedTile != null)
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
        if (m_targetTile != null && m_clickedTile != null)
        {
           

            //Check if tiles are adjacent then swap them 
            AdjacencyCheck();
        }

        //Release the tiles
        m_clickedTile = null;
        m_targetTile = null;
    }




    /// <summary>
    /// Checks whether the clicked tile and target tiles are adjacent to swap
    /// </summary>
    public void AdjacencyCheck()
    {
        Vector2 startIndex = new Vector2(m_clickedTile.tileData.X, m_clickedTile.tileData.Y);
        Vector2 targetIndex = new Vector2(m_targetTile.tileData.X, m_targetTile.tileData.Y);

        //Adjacency check
        if ( (targetIndex.x == startIndex.x) && (Mathf.Abs(targetIndex.y - startIndex.y) == 1) ||
            (targetIndex.y == startIndex.y) && (Mathf.Abs(targetIndex.x - startIndex.x) == 1) )
        {
            //Swap Tiles
            //Utils.DebugUtils.Log("Clicked Tile: " + m_clickedTile.name + " & Target Tile : " + m_targetTile + " are adjacent and can be swapped");

            //If adjacent then swap tiles on the board
            m_boardManager.SwapTilesOnBoard(startIndex, targetIndex);

            //m_clickedTile.AnimateSwap(targetIndex, 0.5f);
            //m_targetTile.AnimateSwap(startIndex, 0.5f);

          
        }

        //Debug if tiles not adjacent
        else
        {
            Utils.DebugUtils.Log("Tiles: " + m_clickedTile.name + " & " + m_targetTile.name + " are not ADJACENT");
        }
       
    }



  





}
