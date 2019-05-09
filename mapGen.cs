using UnityEngine;
using UnityEngine.Tilemaps;

public class mapGen : MonoBehaviour
{
    public Texture2D map;
    public colorToTile[] colorMappings;
    public Camera playerCam;

    private int currPosX;
    private int currPosY;
    private int prevPosX;
    private int prevPosY;
    private int sgnPosX;
    private int sgnPosY;

    private int offsetX = 1;
    private int offsetY = 1;

    private float halfCamWidth;
    private float halfCamHeight;

    public Tilemap tilemapGrid;
    private Vector3 pos;

    private void Start()
    {
        halfCamHeight = 2f * playerCam.orthographicSize / 2;
        halfCamWidth = halfCamHeight * playerCam.aspect;

        pos = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, playerCam.transform.position.z);
        pos.x = Mathf.Clamp(pos.x, halfCamWidth, (map.width - halfCamWidth));
        pos.y = Mathf.Clamp(pos.y, halfCamHeight, (map.height - halfCamHeight));
        playerCam.transform.position = pos;

        currPosX = Mathf.CeilToInt(playerCam.transform.position.x);
        currPosY = Mathf.CeilToInt(playerCam.transform.position.y);

        GenerateLevel(subWidth(currPosX, -1), subWidth(currPosX, 1), subHeight(currPosY, -1), subHeight(currPosY, 1));
        prevPosX = currPosX;
        prevPosY = currPosY;
    }

    private void Update()
    {
        currPosX = Mathf.CeilToInt(playerCam.transform.position.x);
        currPosY = Mathf.CeilToInt(playerCam.transform.position.y);


        if (currPosX != prevPosX)
        {
            //MIND THE SIGNS !!!! XD
            sgnPosX = (int)Mathf.Sign(currPosX - prevPosX);
            GenerateLevel(Mathf.Min(subWidth(currPosX + sgnPosX * offsetX, sgnPosX), subWidth(prevPosX - sgnPosX * offsetX, sgnPosX)),
                          Mathf.Max(subWidth(currPosX + sgnPosX * offsetX, sgnPosX), subWidth(prevPosX - sgnPosX * offsetX, sgnPosX)),
                          subHeight(prevPosY, -1), subHeight(prevPosY, 1)
                          );

            degenerateLevel(Mathf.Min(subWidth(currPosX - sgnPosX * offsetX, -1 * sgnPosX), subWidth(prevPosX - sgnPosX * offsetX, -1 * sgnPosX)),
                            Mathf.Max(subWidth(currPosX - sgnPosX * offsetX, -1 * sgnPosX), subWidth(prevPosX - sgnPosX * offsetX, -1 * sgnPosX)),
                            subHeight(prevPosY - offsetY, -1), subHeight(prevPosY + offsetY, 1)
                            );
        }

        if (currPosY != prevPosY)
        {
            sgnPosY = (int)Mathf.Sign(currPosY - prevPosY);
            GenerateLevel(subWidth(prevPosX, -1), subWidth(prevPosX, 1),
                          Mathf.Min(subHeight(currPosY - offsetY, sgnPosY), subHeight(prevPosY + offsetY, sgnPosY)),
                          Mathf.Max(subHeight(currPosY - offsetY, sgnPosY), subHeight(prevPosY + offsetY, sgnPosY))
                          );

            degenerateLevel(subWidth(prevPosX - offsetX, -1), subWidth(prevPosX + offsetX, 1),
                            Mathf.Min(subHeight(currPosY - signum(sgnPosY) * 1, -1 * sgnPosY), subHeight(prevPosY - signum(sgnPosY) * 1, -1 * sgnPosY)),
                            Mathf.Max(subHeight(currPosY - signum(sgnPosY) * 1, -1 * sgnPosY), subHeight(prevPosY - signum(sgnPosY) * 1, -1 * sgnPosY))
                            );
        }

        prevPosX = currPosX;
        prevPosY = currPosY;

        pos = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, playerCam.transform.position.z);
        pos.x = Mathf.Clamp(pos.x, halfCamWidth, (map.width - halfCamWidth - offsetX));
        pos.y = Mathf.Clamp(pos.y, halfCamHeight, (map.height - halfCamHeight - offsetY));
        playerCam.transform.position = pos;
    }

    void GenerateLevel(int xMin, int xMax, int yMin, int yMax)
    {
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                GenerateTile(x, y);
            }
        }
    }
    void degenerateLevel(int xMin, int xMax, int yMin, int yMax)
    {
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                degenerateTile(x, y);
            }
        }
    }
    void GenerateTile(int x, int y)
    {
        Color32 pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            // The pixel is transparrent. Let's ignore it!
            return;
        }

        foreach (colorToTile colorMapping in colorMappings)
        {

            Tile tileSprite = ScriptableObject.CreateInstance<Tile>();
            if (colorMapping.color.Equals(pixelColor))
            {
                //Debug.Log(pixelColor);
                tileSprite.sprite = colorMapping.tile;

                Vector3Int currentCell = new Vector3Int(x, y, 0);
                tilemapGrid.SetTile(currentCell, tileSprite);
            }
        }
    }
    void degenerateTile(int x, int y)
    {
        Color32 pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            // The pixel is transparrent. Let's ignore it!
            return;
        }

        foreach (colorToTile colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector3Int currentCell = new Vector3Int(x, y, 0);
                tilemapGrid.SetTile(currentCell, null);
            }
        }
    }
    int subWidth(int x, int c)
    {
        return (int)(x + c * halfCamWidth);
    }
    int subHeight(int x, int c)
    {
        return (int)(x + c * halfCamHeight);
    }
    int signum(int x)
    {
        if (x > 0) return x;
        else return 0;
    }

    void clampCam()
    {
        pos = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, playerCam.transform.position.z);
        pos.x = Mathf.Clamp(pos.x, halfCamWidth, (map.width - halfCamWidth - 1));
        pos.y = Mathf.Clamp(pos.y, halfCamHeight, (map.height - halfCamHeight - 1));
        playerCam.transform.position = pos;
    }
}


