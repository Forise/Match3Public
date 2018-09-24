using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SpriteRenderer))]
public class Tile : MonoBehaviour
{
    #region Fields
    bool isSelected = false;    
    Vector2Int[] adjacentDirections = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    FieldManager fieldManager;
    [SerializeField]
    Color selectedColor = new Color();
    [SerializeField]
    float movementTime;

    public Vector2Int adress = new Vector2Int();
    public SpriteRenderer render;
    #endregion

    #region Properties
    #endregion
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        fieldManager = FieldManager.Instance;
    }

    private void Select()
    {
        fieldManager.previousSelectedTile = fieldManager.selectedTile;
        if (fieldManager.previousSelectedTile != null)
            fieldManager.previousSelectedTile.Deselect();
        isSelected = true;
        render.color = selectedColor;
        fieldManager.selectedTile = this;
    }

    public void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        fieldManager.selectedTile = null;
    }

    void OnMouseDown()
    {
        if (render.sprite == null)
            return;

        if (isSelected)
            Deselect();
        else
        {
            Select();
            
            if (GetAllAdjacentTiles().Contains(fieldManager.previousSelectedTile))
            {
                fieldManager.TrySwapTiles();
            }                
        }
    }

    private Tile GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.rigidbody != null)
        {
            return hit.rigidbody.gameObject.GetComponent<Tile>();
        }
        return null;
    }

    public List<Tile> GetAllAdjacentTiles()
    {
        List<Tile> adjacentTiles = new List<Tile>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            var tempAdress = adress + adjacentDirections[i];
            if (tempAdress.x < fieldManager.size.x && tempAdress.x >= 0 &&
                tempAdress.y < fieldManager.size.y && tempAdress.y >= 0)
            adjacentTiles.Add(fieldManager.tiles[tempAdress.x, tempAdress.y]);
        }
        return adjacentTiles;
    }

    private bool IsSameSpritesInTiles(Tile tile1, Tile tile2)
    {
        if (tile1.render.sprite == tile2.render.sprite)
            return true;
        else
            return false;
    }

    private List<Tile> FindMatch(Vector2Int castDir)
    {
        List<Tile> matchingTiles = new List<Tile>();
        var tempAdress = adress + castDir;

        if(castDir == Vector2Int.up)
        {
            for (int y = tempAdress.y; y < fieldManager.size.y; y++)
            {
                if (IsSameSpritesInTiles(this, fieldManager.tiles[adress.x, y]))
                    matchingTiles.Add(fieldManager.tiles[adress.x, y]);
                else
                    break;
            }
        }
        else if(castDir == Vector2Int.down)
        {
            for (int y = tempAdress.y; y >= 0; y--)
            {
                if (IsSameSpritesInTiles(this, fieldManager.tiles[adress.x, y]))
                    matchingTiles.Add(fieldManager.tiles[adress.x, y]);
                else
                    break;
            }
        }
        else if (castDir == Vector2Int.right)
        {
            for (int x = tempAdress.x; x < fieldManager.size.x; x++)
            {
                if (IsSameSpritesInTiles(this, fieldManager.tiles[x, adress.y]))
                    matchingTiles.Add(fieldManager.tiles[x, adress.y]);
                else
                    break;
            }
        }
        else if (castDir == Vector2Int.left)
        {
            for (int x = tempAdress.x; x >= 0; x--)
            {
                if (IsSameSpritesInTiles(this, fieldManager.tiles[x, adress.y]))
                    matchingTiles.Add(fieldManager.tiles[x, adress.y]);
                else
                    break;
            }
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2Int[] paths)
    {
        List<Tile> matchingTiles = new List<Tile>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }

        if (matchingTiles.Count >= 3)
            GamePlayManager.Instance.AddStesp();
    }

    private bool matchFound = false;
    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2Int[2] { Vector2Int.left, Vector2Int.right });
        ClearMatch(new Vector2Int[2] { Vector2Int.up, Vector2Int.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(fieldManager.FindNullTiles());
            StartCoroutine(fieldManager.FindNullTiles());
            //PlaySomeSound
        }
    }

    public void MoveTo(Vector3 endPos)
    {
        StartCoroutine(MoveCoroutine(endPos, movementTime));
    }

    IEnumerator MoveCoroutine(Vector3 endPos, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(transform.position, endPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
    }
}
