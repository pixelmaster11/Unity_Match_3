using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Tile : MonoBehaviour
{
    public TileData tileData;
    public TileGraphics tileGraphics;

    private TileManager m_tileManager;

    //Animate on spawn
    public virtual void OnSpawnTile(TileManager tm)
    {
        m_tileManager = tm;
       

    }

    //Animate on despawn
    public virtual void OnDeSpawnTile()
    {
        HighlightTile(false);
        this.gameObject.SetActive(false);
        this.transform.parent = m_tileManager.transform;
        //this.gameObject.name = "Cleared Tile";
    }


   


    public void HighlightTile(bool enable)
    {
        tileGraphics.tileHighlighted = enable;
        tileGraphics.highlightObject.SetActive(enable);
    }



    public void OnMouseDown()
    {
        tileGraphics.tileClicked = true;
        m_tileManager.ClickedTile(this);
    }

    public void OnMouseEnter()
    {
        //Tile Activated
        //HighlightTile(true);

        if (!tileGraphics.tileClicked)
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


    /// <summary>
    /// Animate swap movement from current tile position to destination position
    /// </summary>
    /// <param name="dest">Destination position this tile will move to</param>
    /// <param name="animTime">Time take for this tile to move from current position to destination position</param>
    public void Animate(Vector2 dest, float animTime)
    {
        if(gameObject.activeInHierarchy)
        {
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
        //If clearing animation after clearing tiles then fall immediately
        if(tileGraphics.swapAnimating)
        {
            //Then wait till it completes the movement then proceed
            yield return new WaitForSeconds(animateTime);
        }

        Vector2 startPos = this.transform.position;
        tileGraphics.swapAnimating = true;
        bool reachedDestination = false;
        float timeElapsed = 0;

        while(!reachedDestination)
        {
            if((destination - (Vector2)transform.position).SqrMagnitude() <= 0.0001f)
            {
                reachedDestination = true;
                transform.position = destination;
                tileGraphics.swapAnimating = false;
            }

            timeElapsed += Time.deltaTime;
             
            float t = timeElapsed / animateTime;

            ///t = t * t * t * (t * (t * 6 - 15) + 10);

            transform.position = Vector2.Lerp(startPos, destination, t);
            

            yield return null;
        }

    }


    private IEnumerator OnMatch()
    {
        yield return new WaitForSeconds(1);
    }

    






}


[System.Serializable]
public class TileData
{
    
    public int TILE_CODE;
    public int X;
    public int Y;
    
    
}

[System.Serializable]
public class TileGraphics
{
    public bool swapAnimating = false;
    public bool tileClicked = false;
    public bool tileHighlighted = false;
    public GameObject highlightObject;
}

