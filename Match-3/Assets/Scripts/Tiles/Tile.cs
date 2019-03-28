using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Tile : MonoBehaviour
{
    public TileData tileData;
    public TileGraphics tileGraphics;
    private TileManager m_tileManager;

    public AnimationCurve animationCurve;

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
        this.tileGraphics.swapAnimating = false;
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


        if (tileGraphics.swapAnimating)
        {
            yield return new WaitForSeconds(animateTime);
        }

        Vector2 startPos = this.transform.position;
        tileGraphics.swapAnimating = true;
        bool reachedDestination = false;
        float timeElapsed = 0;

       

        while(!reachedDestination)
        {
           
            if ((destination - (Vector2)transform.position).sqrMagnitude <= 0.0001f)
            {
                reachedDestination = true;
                tileGraphics.swapAnimating = false;
                tileGraphics.tileFalling = false;
                transform.position = new Vector2((int)destination.x , (int)destination.y);
                
                
            }

            timeElapsed += Time.deltaTime;
             
            float t = timeElapsed / animateTime;

            t = Mathf.Clamp01(animationCurve.Evaluate(t));

            transform.position = Vector2.Lerp(startPos, destination, t);

            

            yield return null;
        }

       

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
    public float tileFallSpeed = 0.1f;
    public float tileSwapSpeed = 0.3f;
    public bool tileFalling = false;
    public bool swapAnimating = false;
    public bool tileClicked = false;
    public bool tileHighlighted = false;
    public GameObject highlightObject;
}

