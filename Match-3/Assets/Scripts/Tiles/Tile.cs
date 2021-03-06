﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a base class for tiles. All types of tiles will inherit from this base class
/// </summary>
public abstract class Tile : MonoBehaviour
{

    #region VARIABLES

    public AnimationCurve animationCurve;
    public TileData tileData;
    public TileGraphics tileGraphics;
    protected TileManager m_tileManager;

    #endregion // VARIABLES

    //fUNCTIONS common to all types of tiles inherting this class
    #region COMMON TILE FUNCTIONS
    //Animate on spawn
    public virtual void OnSpawnTile(TileManager tm)
    {
        m_tileManager = tm;
        tileData.AcceptClick = true;

    }

    //Animate on despawn
    public virtual void OnDeSpawnTile()
    {
        StopAllCoroutines();
        HighlightTile(false);
        this.transform.parent = m_tileManager.transform;

        transform.position = Vector2.zero;
        tileData.AcceptClick = false;
        tileGraphics.swapAnimating = false;
        tileGraphics.tileFalling = false;
    
        this.gameObject.SetActive(false);
       

        //this.gameObject.name = "Cleared Tile";
    }


   


    public void HighlightTile(bool enable)
    {
        tileGraphics.tileHighlighted = enable;
        tileGraphics.highlightObject.SetActive(enable);
    }

    #endregion // COMMON TILE FUNCTIONS

    //Mouse input on this tile
    #region MOUSE INPUT

    public void OnMouseDown()
    {
        if(tileData.AcceptClick)
        {
            tileGraphics.tileClicked = true;
            m_tileManager.ClickedTile(this);
        }
       
    }


    public void OnMouseEnter()
    {
        //Tile Activated
        //HighlightTile(true);

        if (!tileGraphics.tileClicked && tileData.AcceptClick)
        {
            m_tileManager.DragTile(this);
        }

    }


    public void OnMouseExit()
    {

        if (!tileGraphics.tileClicked)
        {
            //HighlightTile(false);
        }
    }



    public void OnMouseUp()
    {
        //HighlightTile(false);
        tileGraphics.tileClicked = false;
        m_tileManager.ReleaseTile();
    }

    #endregion // MOUSE INPUT

    //Tile animation and movement coroutines
    #region TILE MOVEMENT / ANIMATION
    /// <summary>
    /// Animate swap movement from current tile position to destination position
    /// </summary>
    /// <param name="dest">Destination position this tile will move to</param>
    /// <param name="animTime">Time take for this tile to move from current position to destination position</param>
    public void Animate(Vector2 dest, float animTime)
    {
        if(gameObject.activeSelf)
        {
            if(tileGraphics.tileFalling)
            {
                StopAllCoroutines();
            }

            //Utils.DebugUtils.Log(this.gameObject.name + " Destination: " + dest);
            StartCoroutine(AnimateTileMovement(dest, animTime));
        }
       
    }

  
    /// <summary>
    /// Couroutine to interpolate movement
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="animateTime"></param>
    /// <returns></returns>
    private IEnumerator AnimateTileMovement(Vector2 destination, float animateTime)
    {
        //If this tile is already or still moving to destination but this is not the clearing animation

      

        Vector2 startPos = this.transform.position;
        startPos = new Vector2(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y));
        bool reachedDestination = false;
        tileGraphics.swapAnimating = true;
       
        float timeElapsed = 0;

        while (!reachedDestination)
        {
            float distance = Vector3.Distance(destination, transform.position);
            //float distance = (destination - (Vector2)transform.position).sqrMagnitude;


            if (distance <= 0.01f)
            {
                reachedDestination = true;
                tileGraphics.swapAnimating = false;
                tileGraphics.tileFalling = false;
                transform.position = destination;
            }

            timeElapsed += Time.deltaTime;

            float t = timeElapsed / animateTime;

            t = Mathf.Clamp01(animationCurve.Evaluate(t));

            transform.position = Vector2.Lerp(startPos, destination, t);

            yield return null;
        }


        transform.position = destination;
        reachedDestination = true;


    }

    #endregion // TILE MOVEMENMT / ANIMATION

}


[System.Serializable]
public class TileData
{
    
    public int TILE_CODE;
    public int X;
    public int Y;
    public bool AcceptClick;
    public Enums.TileType tileType;

}

[System.Serializable]
public class TileGraphics
{
    public float tileFallSpeed = 0.1f;
    public float tileSwapSpeed = 0.3f;
    public bool tileFalling = false;
    public bool swapAnimating = false;
    public bool tileClicked = false;
    public bool tileHighlighted = false;
    public GameObject highlightObject;
}

