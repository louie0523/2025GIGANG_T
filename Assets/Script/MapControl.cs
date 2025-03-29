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
    }

    private void Update()
    {
        LimitPlayerMove();
    }

    public void CreateMap()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").transform;
        PrefabChest = Resources.Load<GameObject>("Prefabs/Chest_Closed");
        prefabItems = Resources.LoadAll<GameObject>("Prefabs/Items");
        prefabDoor = Resources.Load<GameObject>("Prefabs/Door");
        sandTexture = GenerateTexture(GameData.baseSandColor, GameData.mixSandColor);
        galssTexture = GenerateTexture(GameData.baseGlassColor, GameData.mixGlassColor);
        MapInfo = Resources.LoadAll<Texture2D>("MapData");
        GenerateDesertTerrain();
        if(!GameData.Instance.isGame)
        {
            GameData.Instance.statgeNum = 0;
            GameData.Instance.bagUpgradeNum = 0;
            GameData.Instance.bagData.Clear();
            GameData.Instance.chestData.Clear();
            foreach(GameObject obj in chests)
            {
                GameData.Instance.chestData.Add(true);
                Debug.Log(obj);
            }

        } else
        {
            for(int i = 0; i < GameData.Instance.chestData.Count; i++)
            {
                chests[i].SetActive(GameData.Instance.chestData[i]);
            }
        }
        gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
        GameData.Instance.isGame = true;
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

    void LimitPlayerMove()
    {
        if(player == null) {
            return;
        }

        float minX = 0;
        float maxX = mapWidth;
        float minZ = 0;
        float maxZ = mapHeight;

        Vector3 clampPostion = player.position;
        clampPostion.x = Mathf.Clamp(clampPostion.x, minX, maxX);
        clampPostion.z = Mathf.Clamp(clampPostion.z, minZ, maxZ);
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
                    GeneratePyramid(new Vector3(j * tileSize, -1f, i * tileSize));
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

                    GameObject chest = Instantiate(PrefabChest, new Vector3(j * tileSize, -1f, i * tileSize), PrefabChest.transform.rotation);
                    chest.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    chest.tag = "Chest";
                    chests.Add(chest);
                }
                else if (pixelColor == GameData.ColorBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;

                    GameObject Block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    Block.transform.position = new Vector3(j * tileSize, 0f, i * tileSize);
                    Block.tag = "Obstacle";
                    Block.transform.parent = Map.transform;
                }
                else if (pixelColor == GameData.ColorTrap)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;

                    GameObject trap = GenerateTraps();
                    trap.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    trap.transform.parent = Map.transform;

                }
                else if (pixelColor == GameData.colorRandom)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    int randNum = Random.Range(0, prefabItems.Length);
                    GameObject item = Instantiate(prefabItems[randNum], new Vector3(j * tileSize, -1f, i * tileSize), prefabItems[randNum].transform.rotation);
                    item.transform.position = new Vector3(j * tileSize, 0f, i * tileSize);
                    item.GetComponent<Item>().SetItem(randNum);
                    item.tag = "Item";

                }
                else if (pixelColor == GameData.colorRobby)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j * tileSize, 0f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j * tileSize + 0.5f, 0.25f, i * tileSize);
                    robby.tag = "Start";
                } else if(pixelColor == GameData.colorEND)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j * tileSize, 0f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j * tileSize + 0.5f, 0.25f, i * tileSize);
                    robby.tag = "End";
                } else if(pixelColor == GameData.ColorPatrolEnemyTop)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    gameManager.monsterControl.CreatePatrolMonsters(new Vector3(j * tileSize, 0f, i * tileSize));
                }
            }

        }
    }

    GameObject GenerateTraps()
    {
        GameObject trap = new GameObject("trap");
        for(int i = 0; i < 5; i++)
        {
            GameObject thorn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thorn.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
            thorn.transform.position = new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));
            thorn.transform.localPosition = new Vector3(0.1f, 0.4f, 0.1f);
            thorn.GetComponent<Renderer>().material.color = Color.white;
            thorn.transform.parent = trap.transform;
        }
        traps.Add(trap);
        return trap;
    }

    void GeneratePyramid(Vector3 pos)
    {
        GameObject pyramid = new GameObject("Pyramid");
        int baseSize = 8;
        float blockSize = 1f;

        for(int y = 0; y < baseSize; y++)
        {
            for (int x = 0; x < baseSize - y; x++)
            {
                for(int z = 0; z < baseSize - y; z++)
                {
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.transform.position = new Vector3(x - (baseSize - y) / 2f, y * blockSize, z - (blockSize - y) / 2f);
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                    block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    block.transform.parent = pyramid.transform;
                }
            }
        }
        pyramid.transform.position = pos;
    }
}