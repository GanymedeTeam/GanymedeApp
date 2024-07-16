using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct WorldMap // tile = the pictures, grid = a grid on the map
{
    public string mapName;
    public int origineX; //X Position of the [0,0] Map
    public int origineY; //Y Position of the [0,0] Map
    public float gridCellWidth; // grid width size
    public float gridCellHeight;  // grid height size
    public int totalWidth; //Total Width size of all the tile image
    public int totalHeight; //Total Height size of all the tile image
    public int WidthNumberOfTile; // Number of tile in width
    public int HeightNumberOfTile; // Number of tile in height
    public int WidthNumberOfGridCell; // Number of grid cell in width
    public int HeightNumberOfGridCell; // Number of grid cell in height
    public float gridOffsetX; // Offset of the grid in X
    public float gridOffsetY; // Offset of the grid in Y
    public int firstPositionX; // X coordinate of the top left position
    public int firstPositionY; // Y coordinate of the top left position
    public float mapParentScale;

    public WorldMap(string mapName, int origineX, int origineY, float gridCellWidth, float gridCellHeight, int totalWidth, int totalHeight, float mapScale)
    {
        this.mapName = mapName;
        this.gridCellWidth = gridCellWidth;
        this.gridCellHeight = gridCellHeight;
        this.origineX = origineX;
        this.origineY = origineY;
        this.totalWidth = totalWidth;
        this.totalHeight = totalHeight;
        mapParentScale = mapScale;

        if (totalWidth % 250 == 0)
        {
            WidthNumberOfTile = totalWidth / 250;
        }
        else
        {
            WidthNumberOfTile = totalWidth / 250 + 1;
        }

        if (totalHeight % 250 == 0)
        {
            HeightNumberOfTile = totalHeight / 250;
        }
        else
        {
            HeightNumberOfTile = totalHeight / 250 + 1;
        }

        if (totalWidth % gridCellWidth == 0)
        {
            WidthNumberOfGridCell = (int)(totalWidth / gridCellWidth);
        }
        else
        {
            WidthNumberOfGridCell = (int)(totalWidth / gridCellWidth) + 1;
        }

        if (totalHeight % gridCellHeight == 0)
        {
            HeightNumberOfGridCell = (int)(totalHeight / gridCellHeight);
        }
        else
        {
            HeightNumberOfGridCell = (int)(totalHeight / gridCellHeight) + 1;
        }

        if (origineX % gridCellWidth == 0)
        {
            gridOffsetX = 0f;
            firstPositionX = -(int)(origineX / gridCellWidth);
        }
        else // Calculate the full size on the left of origine X then substract the origine X to get the offset to know the start of the grid on the left
        {
            float insideSize = origineX % gridCellWidth;
            gridOffsetX = insideSize - gridCellWidth;
            firstPositionX = -1 - (int)(origineX / gridCellWidth);
        }

        if (origineY % gridCellHeight == 0)
        {
            gridOffsetY = 0f;
            firstPositionY = -(int)(origineY / gridCellHeight);
        }
        else // Calculate the full size on the top of origine Y then substract the origine Y to get the offset to know the start of the grid on the top 
        {
            float insideSize = origineY % gridCellHeight;
            gridOffsetY = -insideSize + gridCellHeight;// different from the width calcul because Y to the top is positive while X to the left is negative
            firstPositionY = -1 - (int)(origineY / gridCellHeight);
        }

    }
}

[System.Serializable]
public struct WorldTile
{
    public int x;
    public int y;
    public GameObject tileObject;

    public WorldTile(int x, int y, GameObject tileObject)
    {
        this.x = x;
        this.y = y;
        this.tileObject = tileObject;
    }
}
public class MapManager : MonoBehaviour
{
    public RectTransform MapMask;
    public int whereToGoX = 0;
    public int whereToGoY = 0;
    public WorldMap douze = new WorldMap("Douze", 6480, 4944, 69.5f, 49.7000008f, 10000, 8000, 1.3f);
    public WorldMap incarnam = new WorldMap("Incarnam", 1800, 1940, 380, 260, 4750, 3904, 0.3f);
    public WorldMap minotoror = new WorldMap("LabyMino", 9130, 2980, 210f, 150f, 2463, 1820, 0.5f);
    public WorldMap dragonCochon = new WorldMap("LabyDC", 900, -4330, 211f, 150f, 3200, 2000, 0.5f);
    public WorldMap maitreCorbac = new WorldMap("BibliMC", 3770, 9900, 210f, 150f, 2100, 2100, 0.5f);
    public WorldMap caverneGivrefoux = new WorldMap("Givrefoux", 17214, 11845, 210f, 150f, 1800, 1400, 0.5f);
    public WorldMap canauxMephitiques = new WorldMap("CanauxMephitiques", 9860, 11200, 260f, 184f, 2840, 2162, 0.3f);
    public WorldMap egoutsBrakmar = new WorldMap("EgoutsBrakmar", 7454, -5652, 257f, 182f, 2840, 2162, 0.3f);
    public WorldMap canopee = new WorldMap("Canopee", 12855, -1225, 209f, 150f, 3559, 2804, 0.5f);
    public WorldMap harebourg = new WorldMap("ChateauHarebourg", 6882, 6108, 91.5f, 65.5f, 2500, 1904, 1.1f);
    public WorldMap enutrosor = new WorldMap("Enutrosor", 2678, 1925, 204f, 147f, 4089, 2589, 0.5f);
    public WorldMap srambad = new WorldMap("Srambad", 1050, 1440, 204f, 147f, 4750, 3904, 0.5f);
    public WorldMap xelorium = new WorldMap("Xelorium", 1248, 1543, 204f, 147f, 4171, 3576, 0.5f);
    public WorldMap ecaflipus = new WorldMap("Ecaflipus", 3200, 4200, 203f, 146f, 5069, 5584, 0.5f);
    public WorldMap abysses = new WorldMap("Abysses", -1810, -2950, 150f, 122f, 3333, 1719, 0.6f);
    public WorldMap pyramide = new WorldMap("PyramideTalKasha", -3000, 17820, 415f, 225f, 3529, 2357, 0.3f);
    public WorldMap epave = new WorldMap("EpavesSilencieuses", 8385, 10700, 150f, 122f, 3333, 1719, 0.8f);
    public WorldMap pwak = new WorldMap("Pwak", 2268, 2565, 286f, 205f, 3560, 2802, 0.3f);
    public WorldMap crocuzko = new WorldMap("Crocuzko", 14604, 2635, 164f, 114f, 2016, 1719, 0.7f);
    public WorldMap songes = new WorldMap("Songes", 1208, 800, 91.5f, 65.5f, 2500, 1904, 1.0f);
    public WorldMap guerre = new WorldMap("Guerre", 790, 1165, 289f, 195f, 2840, 2170, 0.5f);
    public WorldMap corruption = new WorldMap("Corrupiton", 775, 718, 260f, 182f, 2840, 2162, 0.5f);
    public WorldMap misere = new WorldMap("Misere", 770, 717, 258f, 185f, 2840, 2170, 0.5f);
    public WorldMap servitude = new WorldMap("Servitude", 775, 1266, 257f, 183f, 2840, 2162, 0.5f);
    public WorldMap encrePapier = new WorldMap("EncrePapier", 1960, 1390, 352f, 251.699997f, 4000, 2900, 0.3f);
    public WorldMap cauchemar = new WorldMap("Cauchemar", 773, 532, 257f, 183f, 2840, 2162, 0.5f);
    public WorldMap ereboria = new WorldMap("Ereboria", -6953, 13663, 257.5f, 185f, 2840, 2881, 0.4f);
    public GameObject tilePrefab;
    public GameObject TileGrid;
    public Sprite[] incarnamScaleOneTileSprites;
    public Sprite[] douzeScaleOneTileSprites;
    public Sprite[] minotororScaleOneTileSprites;
    public Sprite[] dragonCochonScaleOneTileSprites;
    public Sprite[] maitreCorbacScaleOneTileSprites;
    public Sprite[] caverneGivrefouxScaleOneTileSprites;
    public Sprite[] canauxMephitiquesScaleOneTileSprites;
    public Sprite[] egoutsBrakmarScaleOneTileSprites;
    public Sprite[] canopeeScaleOneTileSprites;
    public Sprite[] harebourgScaleOneTileSprites;
    public Sprite[] enutrosorScaleOneTileSprites;
    public Sprite[] srambadScaleOneTileSprites;
    public Sprite[] xeloriumScaleOneTileSprites;
    public Sprite[] ecaflipusScaleOneTileSprites;
    public Sprite[] abyssesScaleOneTileSprites;
    public Sprite[] pyramideScaleOneTileSprites;
    public Sprite[] epaveScaleOneTileSprites;
    public Sprite[] pwakScaleOneTileSprites;
    public Sprite[] crocuzkoScaleOneTileSprites;
    public Sprite[] songesScaleOneTileSprites;
    public Sprite[] guerreScaleOneTileSprites;
    public Sprite[] corruptionScaleOneTileSprites;
    public Sprite[] misereScaleOneTileSprites;
    public Sprite[] servitudeScaleOneTileSprites;
    public Sprite[] encrePapierScaleOneTileSprites;
    public Sprite[] cauchemarScaleOneTileSprites;
    public Sprite[] ereboriaScaleOneTileSprites;
    public List<WorldTile> worldTiles = new List<WorldTile>();


    public GameObject ButtontilePrefab;
    public GameObject ButtonTileGrid;
    public GameObject MapParent;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SpawnTiles());
        //StartCoroutine(SpawnMapsButtons(incarnam));
    }

    public void updateMapFromStep(int x, int y, string mapName)
    {
        if (mapName == "Incarnam")
        {
            CenterMapAround(x, y, incarnam, incarnamScaleOneTileSprites);
        }
        else if (mapName == "Douze")
        {
            CenterMapAround(x, y, douze, douzeScaleOneTileSprites);
        }
        else if (mapName == "Minotoror")
        {
            CenterMapAround(x, y, minotoror, minotororScaleOneTileSprites);
        }
        else if (mapName == "Dragon Cochon")
        {
            CenterMapAround(x, y, dragonCochon, dragonCochonScaleOneTileSprites);
        }
        else if (mapName == "Maitre Corbac")
        {
            CenterMapAround(x, y, maitreCorbac, maitreCorbacScaleOneTileSprites);
        }
        else if (mapName == "Caverne Givrefoux")
        {
            CenterMapAround(x, y, caverneGivrefoux, caverneGivrefouxScaleOneTileSprites);
        }
        else if (mapName == "Canaux Mephitiques")
        {
            CenterMapAround(x, y, canauxMephitiques, canauxMephitiquesScaleOneTileSprites);
        }
        else if (mapName == "Egouts Brakmar")
        {
            CenterMapAround(x, y, egoutsBrakmar, egoutsBrakmarScaleOneTileSprites);
        }
        else if (mapName == "Canopee")
        {
            CenterMapAround(x, y, canopee, canopeeScaleOneTileSprites);
        }
        else if (mapName == "Harebourg")
        {
            CenterMapAround(x, y, harebourg, harebourgScaleOneTileSprites);
        }
        else if (mapName == "Enutrosor")
        {
            CenterMapAround(x, y, enutrosor, enutrosorScaleOneTileSprites);
        }
        else if (mapName == "Srambad")
        {
            CenterMapAround(x, y, srambad, srambadScaleOneTileSprites);
        }
        else if (mapName == "Xelorium")
        {
            CenterMapAround(x, y, xelorium, xeloriumScaleOneTileSprites);
        }
        else if (mapName == "Ecaflipus")
        {
            CenterMapAround(x, y, ecaflipus, ecaflipusScaleOneTileSprites);
        }
        else if (mapName == "Abysses")
        {
            CenterMapAround(x, y, abysses, abyssesScaleOneTileSprites);
        }
        else if (mapName == "Pyramide")
        {
            CenterMapAround(x, y, pyramide, pyramideScaleOneTileSprites);
        }
        else if (mapName == "Epave")
        {
            CenterMapAround(x, y, epave, epaveScaleOneTileSprites);
        }
        else if (mapName == "Pwak")
        {
            CenterMapAround(x, y, pwak, pwakScaleOneTileSprites);
        }
        else if (mapName == "Crocuzko")
        {
            CenterMapAround(x, y, crocuzko, crocuzkoScaleOneTileSprites);
        }
        else if (mapName == "Songes")
        {
            CenterMapAround(x, y, crocuzko, songesScaleOneTileSprites);
        }
        else if (mapName == "Guerre")
        {
            CenterMapAround(x, y, guerre, guerreScaleOneTileSprites);
        }
        else if (mapName == "Corruption")
        {
            CenterMapAround(x, y, corruption, corruptionScaleOneTileSprites);
        }
        else if (mapName == "Misere")
        {
            CenterMapAround(x, y, misere, misereScaleOneTileSprites);
        }
        else if (mapName == "Servitude")
        {
            CenterMapAround(x, y, servitude, servitudeScaleOneTileSprites);
        }
        else if (mapName == "Encre Papier")
        {
            CenterMapAround(x, y, encrePapier, encrePapierScaleOneTileSprites);
        }
        else if (mapName == "Cauchemar")
        {
            CenterMapAround(x, y, cauchemar, cauchemarScaleOneTileSprites);
        }
        else if (mapName == "Ereboria")
        {
            CenterMapAround(x, y, ereboria, ereboriaScaleOneTileSprites);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CenterMapAround(whereToGoX, whereToGoY, douze, douzeScaleOneTileSprites);
        }
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CenterMapAround(whereToGoX, whereToGoY, incarnam, incarnamScaleOneTileSprites);
    */
    }

    IEnumerator SpawnTiles()
    {
        foreach (Sprite tileSprite in incarnamScaleOneTileSprites)
        {
            GameObject newTile = Instantiate(tilePrefab, TileGrid.transform);
            newTile.GetComponent<Image>().sprite = tileSprite;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator SpawnMapsButtons(WorldMap map)
    {
        int index = 0;
        ButtonTileGrid.transform.localPosition = new Vector3(map.gridOffsetX, map.gridOffsetY, 0f);

        while (index < map.WidthNumberOfGridCell * map.HeightNumberOfGridCell)
        {
            GameObject newMapButton = Instantiate(ButtontilePrefab, ButtonTileGrid.transform);
            newMapButton.GetComponentInChildren<TMP_Text>().text = "[" + (index % map.WidthNumberOfGridCell + map.firstPositionX) + "," + (index / map.WidthNumberOfGridCell + map.firstPositionY) + "]";
            worldTiles.Add(new WorldTile(index % map.WidthNumberOfGridCell + map.firstPositionX, index / map.WidthNumberOfGridCell + map.firstPositionY, newMapButton));
            index++;
            yield return new WaitForSeconds(0.003f);
        }
        ButtonTileGrid.transform.localPosition = new Vector3(map.gridOffsetX, map.gridOffsetY, 0f);
    }

    public void CenterMapAround(int x, int y, WorldMap map, Sprite[] spriteList)
    {
        if (!(x >= map.firstPositionX && x <= map.firstPositionX + map.WidthNumberOfGridCell && y >= map.firstPositionY && y <= map.firstPositionY + map.HeightNumberOfGridCell))
        {
            return;
        }

        if(map.mapName == "Incarnam")
        {
            //Set map size to 0.3
        }

        MapParent.transform.localScale = new Vector3(map.mapParentScale, map.mapParentScale, 1f);

        int rightFromFirstPositionX = x - map.firstPositionX;
        int bottomFromFirstPositionY = y - map.firstPositionY;

        //Calculate X and Y offset to center the grid on the position
        // Add half of the rect mask to center it inside it
        float offsetX = MapMask.rect.width / 2;
        float offsetY = -MapMask.rect.height / 2;
        // Remove half of the grid cell size to center it because the anchor point of the grid is top left, it must be multiplied by the scale to get the correct size
        offsetX -= map.gridCellWidth * MapParent.transform.localScale.x / 2;
        offsetY += map.gridCellHeight * MapParent.transform.localScale.y / 2;
        // Update it with the scale of the grids parent
        offsetX /= MapParent.transform.localScale.x;
        offsetY /= MapParent.transform.localScale.y;

        TileGrid.transform.localPosition = new Vector3(-rightFromFirstPositionX * map.gridCellWidth - map.gridOffsetX + offsetX, bottomFromFirstPositionY * map.gridCellHeight - map.gridOffsetY + offsetY, 0f);
        ButtonTileGrid.transform.localPosition = new Vector3(-rightFromFirstPositionX * map.gridCellWidth + offsetX, bottomFromFirstPositionY * map.gridCellHeight + offsetY, 0f);

        //calculate how many grid cells there must be from left to right + calculate how many grid cells there must be from top to bottom
        float gridCellWidth = map.gridCellWidth * MapParent.transform.localScale.x;
        float gridCellHeight = map.gridCellHeight * MapParent.transform.localScale.y;

        int numberOfCellInMapMaskSizeWidth = (int)(MapMask.rect.width / gridCellWidth) + 2;
        int numberOfCellInMapMaskSizeHeight = (int)(MapMask.rect.height / gridCellHeight) + 2;
        if (numberOfCellInMapMaskSizeWidth % 2 == 0)
        {
            numberOfCellInMapMaskSizeWidth++;
        }
        if (numberOfCellInMapMaskSizeHeight % 2 == 0)
        {
            numberOfCellInMapMaskSizeHeight++;
        }

        //calculate the position of the first grid cell to spawn
        float firstPositionX = (rightFromFirstPositionX - ((numberOfCellInMapMaskSizeWidth - 1) / 2)) * map.gridCellWidth;
        float firstPositionY = (-bottomFromFirstPositionY + ((numberOfCellInMapMaskSizeHeight - 1) / 2)) * map.gridCellHeight;
        //calculate the coordinates of the first grid cell to spawn
        int firstCoordinateX = x - ((numberOfCellInMapMaskSizeWidth - 1) / 2);
        int firstCoordinateY = y - ((numberOfCellInMapMaskSizeHeight - 1) / 2);

        //from there make a loop to spawn them all and update their name with their position
        foreach (Transform child in ButtonTileGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfCellInMapMaskSizeWidth * numberOfCellInMapMaskSizeHeight; i++)
        {
            GameObject newMapButton = Instantiate(ButtontilePrefab, ButtonTileGrid.transform);
            newMapButton.GetComponent<Button>().onClick.AddListener(delegate { GUIUtility.systemCopyBuffer = newMapButton.transform.Find("Text").GetComponent<TMP_Text>().text; });
            newMapButton.GetComponentInChildren<TMP_Text>().text = "[" + (i % numberOfCellInMapMaskSizeWidth + firstCoordinateX) + "," + ((i / numberOfCellInMapMaskSizeWidth) + firstCoordinateY) + "]";
            if(newMapButton.GetComponentInChildren<TMP_Text>().text == "[" + x + "," + y + "]")
            {
                newMapButton.GetComponentsInChildren<Image>()[1].enabled = true;
                newMapButton.GetComponentInChildren<TMP_Text>().enabled = true;
            }
            float tilePositionX = firstPositionX + (i % numberOfCellInMapMaskSizeWidth) * map.gridCellWidth;
            float tilePositionY = firstPositionY - (i / numberOfCellInMapMaskSizeWidth) * map.gridCellHeight;
            // Set the position of the new tile
            newMapButton.transform.localPosition = new Vector3(tilePositionX, tilePositionY, 0f);
            newMapButton.GetComponent<RectTransform>().sizeDelta = new Vector2(map.gridCellWidth, map.gridCellHeight);
        }


        //TileGrid is the grid of the tile
        //Calculate how much tile width size we can add to the tileGrid localposition to go above 0 and remove one.
        int horizontalFirstTileIndex = (int)(-TileGrid.transform.localPosition.x / 250) + 1;
        int verticalFirstTileIndex = (int)(TileGrid.transform.localPosition.y / 250) + 1;

        int horizontalLastTileIndex = (int)((-TileGrid.transform.localPosition.x + (MapMask.rect.width / MapParent.transform.localScale.x)) / 250) + 1;
        int verticalLastTileIndex = (int)((TileGrid.transform.localPosition.y + (MapMask.rect.height / MapParent.transform.localScale.y)) / 250) + 1;

        int numberOfHorizontalTileShown = horizontalLastTileIndex - horizontalFirstTileIndex + 1;
        int numberOfVerticalTileShown = verticalLastTileIndex - verticalFirstTileIndex + 1;

        foreach (Transform child in TileGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfHorizontalTileShown * numberOfVerticalTileShown; i++)
        {
            int SpriteIndexInList = (i % numberOfHorizontalTileShown + horizontalFirstTileIndex -1) + (i / numberOfHorizontalTileShown + verticalFirstTileIndex - 1) * map.WidthNumberOfTile;
            if (SpriteIndexInList < spriteList.Length)
            {
                GameObject newTile = Instantiate(tilePrefab, TileGrid.transform);
                newTile.GetComponent<Image>().sprite = spriteList[SpriteIndexInList];
                newTile.transform.localPosition = new Vector3(((i % numberOfHorizontalTileShown + horizontalFirstTileIndex) - 1) * 250, -((i / numberOfHorizontalTileShown + verticalFirstTileIndex) - 1) * 250, 0f);
                newTile.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
            }
        }
    }

}
