using System.Collections.Generic;
using UnityEngine;
using Match3Engine;
using System.Collections;

public class FieldManager : MonoSingleton<FieldManager>
{
    #region Fields
    public Vector2Int size;
    public Tile tilePrefab;
    public Tile[,] tiles;
    public Tile previousSelectedTile = null;
    public Tile selectedTile = null;    
    public List<Sprite> sprites = new List<Sprite>();
    #endregion

    #region Properties
    public bool IsFalling { get; set; }
    #endregion

    bool isInitialized = false;
    public void Init(int x, int y)
    {
        if (!isInitialized)
        {
            size = new Vector2Int(x, y);
            Vector2 offset = tilePrefab.render.sprite.bounds.size;
            CreateField(offset, tilePrefab, sprites);
            isInitialized = true;
        }
    }

    private void CreateField(Vector2 offset, Tile prefab, List<Sprite> sprites)
    {
        tiles = new Tile[size.x, size.y];

        Vector2 startPos = transform.position;
        Sprite[] previousLeft = new Sprite[size.y];
        Sprite previousBelow = null;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Tile newTile = Instantiate(prefab, new Vector3(startPos.x + (offset.x * x), startPos.y + (offset.y * y), 0), prefab.transform.rotation);
                tiles[x, y] = newTile;
                newTile.adress = new Vector2Int(x, y);
                newTile.transform.parent = transform;

                List<Sprite> possibleSprites = new List<Sprite>();
                possibleSprites.AddRange(sprites);

                possibleSprites.Remove(previousLeft[y]);
                possibleSprites.Remove(previousBelow);

                Sprite newSprite = possibleSprites[UnityEngine.Random.Range(0, possibleSprites.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        return sprites.Find(x => x.name == name);
    }

    public void TrySwapTiles()
    {
        if (previousSelectedTile != null && selectedTile != null)
        {
            previousSelectedTile.MoveTo(selectedTile.transform.position);
            selectedTile.MoveTo(previousSelectedTile.transform.position);
            
            SwapTiles(selectedTile, previousSelectedTile);

            selectedTile.ClearAllMatches();
            selectedTile.Deselect();
            previousSelectedTile.ClearAllMatches();
            previousSelectedTile.Deselect();
            StartCoroutine(GamePlayManager.Instance.SpendStep());
        }
    }

    private void SwapTiles(Tile tile1, Tile tile2)
    {
        var tempAdress = tile1.adress;
        tiles[tile1.adress.x, tile1.adress.y] = tile2;
        tiles[tile2.adress.x, tile2.adress.y] = tile1;

        tile1.adress = tile2.adress;
        tile2.adress = tempAdress;
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MoveTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator MoveTilesDown(int x, int yStart, float fallDelay = .03f)
    {
        IsFalling = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < size.y; y++)
        {
            SpriteRenderer render = tiles[x, y].gameObject.GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        {
            GamePlayManager.Instance.AddScores(50);
            yield return new WaitForSeconds(fallDelay);
            for (int j = 0; j < renders.Count - 1; j++)
            {
                renders[j].sprite = renders[j + 1].sprite;
                renders[j + 1].sprite = GetNewSprite(x, size.y - 1);
            }
        }
        IsFalling = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(sprites);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].gameObject.GetComponent<SpriteRenderer>().sprite);
        }
        if (x < size.x - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].gameObject.GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].gameObject.GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }

    public IEnumerator ClearField()
    {
        while(IsFalling)
        {
            yield return null;
        }
        foreach (var tile in GetComponentsInChildren<Tile>())
        {
            Destroy(tile.gameObject);
        }
        isInitialized = false;
    }
}
