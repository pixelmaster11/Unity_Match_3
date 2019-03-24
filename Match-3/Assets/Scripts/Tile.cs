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
    public abstract void OnDeSpawnTile();


   


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
        HighlightTile(true);

        if (!tileGraphics.tileClicked)
        {
            m_tileManager.DragTile(this);
        }

    }


    public void OnMouseExit()
    {

        if (!tileGraphics.tileClicked)
        {
            HighlightTile(false);
        }
    }



    public void OnMouseUp()
    {
        HighlightTile(false);
        tileGraphics.tileClicked = false;
        m_tileManager.ReleaseTile();
    }


    public void AnimateSwap(Vector2 dest, float animTime)
    {
        StartCoroutine(AnimateTileSwapMovement(dest, animTime));
    }


    private IEnumerator AnimateTileSwapMovement(Vector2 destination, float animateTime)
    {
        Vector2 startPos = this.transform.position;

        bool reachedDestination = false;

        float timeElapsed = 0;

        while(!reachedDestination)
        {
            if((destination - (Vector2)transform.position).SqrMagnitude() <= 0.0001f)
            {
                reachedDestination = true;
                transform.position = destination;
            }

            timeElapsed += Time.deltaTime;

            float t = timeElapsed / animateTime;

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
    public bool tileClicked = false;
    public bool tileHighlighted = false;
    public GameObject highlightObject;
}

