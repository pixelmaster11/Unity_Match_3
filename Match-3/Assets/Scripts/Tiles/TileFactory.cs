using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for creating a pool of tiles
/// </summary>
public class TileFactory : MonoBehaviour
{
    #region VARIABLE AND REFERENCES

    [SerializeField]
    Tile[] m_tilePrefabs;

    [SerializeField]
    int m_poolAmount;

    private List<Tile> m_tilePool;

    #endregion //VARIABLES AND REFERENCES

    //TILE POOL CREATION AND RETURNING TILE FROM POOL
    #region TILE POOL

    public void CreateTilePool()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        m_tilePool = new List<Tile>();

        for (int i = 0; i < m_poolAmount; i++)
        {
            Tile tile = Instantiate(m_tilePrefabs[Random.Range(0, m_tilePrefabs.Length)]) as Tile;
            tile.gameObject.SetActive(false);
            tile.transform.parent = this.transform;
            m_tilePool.Add(tile);
        }
    }

    public Tile GetTile()
    {
        for(int i = 0; i < m_tilePool.Count; i++)
        {
            if(!m_tilePool[i].gameObject.activeSelf)
            {                         
                return m_tilePool[i];
            }
        }

        
        Tile tile = Instantiate(m_tilePrefabs[Random.Range(0, m_tilePrefabs.Length)]) as Tile;
        tile.gameObject.SetActive(false);
        tile.transform.parent = this.transform;
        m_tilePool.Add(tile);

        return tile;
    }



   

    /// <summary>
    /// Returns a tile from tile pool with corresponding tile ID code
    /// </summary>
    /// <param name="tileCode">Tile ID code to fetch tile</param>
    /// <returns></returns>
    public Tile GetTile(int tileCode)
    {
        for (int i = 0; i < m_tilePool.Count; i++)
        {
            if (!m_tilePool[i].gameObject.activeSelf && tileCode == m_tilePool[i].tileData.TILE_CODE)
            {
                return m_tilePool[i];
            }
        }


        Tile tile = null;

        for (int i = 0; i < m_tilePrefabs.Length; i++)
        {
            if(m_tilePrefabs[i].tileData.TILE_CODE == tileCode)
            {
                tile = Instantiate(m_tilePrefabs[i]) as Tile;
                tile.gameObject.SetActive(false);
                tile.transform.parent = this.transform;
                m_tilePool.Add(tile);
                break;
            }    
        
        }

        if(tile == null)
        { 
          Utils.DebugUtils.Log("Tile code: " + tileCode + " is invalid");          
        }

        return tile;
    }



    /// <summary>
    /// Returns a New tile corresponding tile ID code
    /// </summary>
    /// <param name="tileCode">Tile ID code to fetch tile</param>
    /// <returns></returns>
    public Tile GetNewTile(int tileCode)
    {
         
        Tile tile = null;

        for (int i = 0; i < m_tilePrefabs.Length; i++)
        {
            if (m_tilePrefabs[i].tileData.TILE_CODE == tileCode)
            {
                tile = Instantiate(m_tilePrefabs[i]) as Tile;
                tile.gameObject.SetActive(false);
                tile.transform.parent = this.transform;
                m_tilePool.Add(tile);
                break;
            }

        }

        if (tile == null)
        {
            Utils.DebugUtils.Log("Tile code: " + tileCode + " is invalid");
        }

        return tile;
    }

    #endregion // TILE POOL

}
