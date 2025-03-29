using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float noiseScale = 5f;
    public float heightMultiplier = 2f;
    public GameObject[] obstacles;  // 바위, 선인장 등의 장애물 프리팹
    public float obstacleSpawnChance = 0.1f;
    /// <summary>
    /// 모래 텍스쳐
    /// </summary>
    private Texture2D sandTexture;
    /// <summary>
    /// 잔디 텍스처
    /// </summary>
    private Texture2D galssTexture;

    // 적 등장 위치
    public Color ColorResponse = new Color(64, 128, 128);

    public Transform Terrain;
    public Texture2D[] MapInfo;
    public float tileSize = 1.0f;
    private int mapWidth;
    private int mapHeight;

    /// <summary>
    /// 보물상자
    /// </summary>
    public GameObject PrefabChest;
    /// <summary>
    /// 플레이어
    /// </summary>
    Transform player;
    /// <summary>
    /// 함정들
    /// </summary>
    List<GameObject> traps = new List<GameObject>();
    public GameManager gameManager;
    public List<GameObject> chests = new List<GameObject>();
    public GameObject[] prefabItems;
    public GameObject prefabDoor;
    public Vector3 startPos;

    void Start()
    {
        CreateMap();
        GenerateDesertTerrain();
    }

    public void CreateMap()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //player = GameObject.Find("Player").transform;
        PrefabChest = Resources.Load<GameObject>("Prefabs/Chest_Closed");
        prefabItems = Resources.LoadAll<GameObject>("Prefabs/Items");
        prefabDoor = Resources.Load<GameObject>("Prefabs/Door");
        sandTexture = GenerateTexture(GameData.baseSandColor, GameData.mixSandColor);
        galssTexture = GenerateTexture(GameData.baseGlassColor, GameData.mixGlassColor);
        MapInfo = Resources.LoadAll<Texture2D>("MapData");
    }
    Texture2D GenerateTexture(Color baseColor, Color mixColor)
    {
        Texture2D texture = new Texture2D(GameData.textureWidth, GameData.textureHeight);

        for (int x = 0; x < GameData.textureWidth; x++)
        {
            for (int y = 0; y < GameData.textureHeight; y++)
            {
                float noise = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);
                Color pixelColor = Color.Lerp(baseColor, mixColor, noise);
                texture.SetPixel(x, y, pixelColor);

            }
        }
        texture.Apply();
        return texture;
    }
    void GenerateDesertTerrain()
    {
        GameObject Map = new GameObject("Map");
        mapWidth = MapInfo[GameData.Instance.statgeNum].width;
        mapHeight = MapInfo[GameData.Instance.statgeNum].height;
        GameData.Instance.stageSize = mapWidth;
        Color[] pixels = MapInfo[GameData.Instance.statgeNum].GetPixels();

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Color pixelColor = pixels[i * mapHeight + j];
                if (pixelColor == GameData.ColorNoneSandBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;
                }
                else if (pixelColor == Color.cyan)
                {
                    //GeneratePyramid(new Vector3(j * tileSize, -1f, i * tileSize));
                }
                else if (pixelColor == GameData.ColorNoneGlassBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = galssTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;
                }
                else if (pixelColor == GameData.ColorTreasure)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;

                    //GameObject chest = Instantiate(PrefabChest, new Vector3(j * tileSize, -1f, i * tileSize), PrefabChest.transform.rotation);
                    //chest.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    //chest.tag = "Chest";
                    //chests.Add(chest);
                }
                else if (pixelColor == GameData.ColorBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;

                    //GameObject Block = Instantiate(PrefabChest, new Vector3(j * tileSize, -1f, i * tileSize), PrefabChest.transform.rotation);
                    //Block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    //Block.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    //Block.transform.parent = Map.transform;
                    //Block.tag = "Obstacle";
                }
                else if (pixelColor == GameData.ColorTrap)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;

                    //GameObject trap = GenerateTraps();
                    //trap.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    //trap.transform.parent = Map.transform;

                }
                else if (pixelColor == GameData.colorRandom)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    //int randNum = Random.Range(0, prefabItems.Length);
                    //GameObject item = Instantiate(prefabItems[randNum], new Vector3(j * tileSize, -1f, i * tileSize), prefabItems[randNum].transform.rotation);
                    //item.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    //item.GetComponent<Item>().SetItem(randNum);
                    //item.tag = "Item";

                }
                else if (pixelColor == GameData.colorRobby)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;
                }
            }

        }
    }
}