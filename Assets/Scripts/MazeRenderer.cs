using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEditor.Experimental.GraphView;
using UnityEngine.TextCore.Text;

public class MazeRenderer : MonoBehaviour
{
    /**
     * Much of this code was borrowed from Assignment 5.
     **/

    public float floor_height;
    public float wall_thickness_x;
    public float wall_thickness_z;
    public float corner_thickness_x;
    public float corner_thickness_z;
    public float poof_trap_radius;
    public float storey_height;
    private float RENDER_EPSILON = 1e-6f;
    private float EDGE_EPSILON = 0.02f;
    public GameObject fps_prefab;
    public GameObject fps_player_obj;
    public static readonly int playerStartingHealth = 3;
    public int playerHealth;
    public static readonly float SPEED_BOOST_MODIFIER = 0.5f;
    public static readonly float SPEED_BOOST_TIME = 2.0f;  // Seconds
    public float playerSpeedModifier;
    public float speedBoostTimer;
    public static string PLAYER_NAME = "PLAYER";
    private float playerHeight;
    public Bounds bounds;
    private List<GameObject> nullFacedPickups;

    public GameObject treasureChestPrefab;
    public GameObject treasureChest;
    public Color GROUND_COLOR = Color.grey;
    private Color DEFAULT_COLOR = new Color(0.3f, 0.4f, 0.2f);
    private Color LIGHTEST_FLOOR_COLOR = new Color(0.6f, 0.8f, 0.8f);
    public Color[] floorColors;
    public Material[] floorMaterials;
    public Material[] wallMaterials;
    public Color START_TILE_COLOR = new Color(0.0f, 0.0f, 1.0f);
    public Material POOF_TRAP_MATERIAL;
    public static int PICKUP_FONT_SIZE = 15;
    public static Color PICKUP_FONT_COLOR_DEFAULT = new Color(1.0f, 1.0f, 1.0f);
    public static Color BOOST_FONT_COLOR_DEFAULT = new Color(1.0f, 0.0f, 0.0f);
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset arrowFont;
    public static Material PICKUP_MATERIAL;
    public Canvas canvas;
    private Minimap[] minimaps;
    private GameObject playerPointer;
    public string pathToMinimapBackground;
    public string pathToMinimapFilter;
    public string pathToPlayerPointer;
    public Color minimapWallColor;
    public Color minimapFloorColor;
    public int minimapWidth;
    public int minimapHeight;
    private Vector2 minimapOffset;

    private MazeGenerator generator;
    private string[][,][] maze;

    private void Awake()
    {
        PICKUP_MATERIAL = Resources.Load<Material>("PickupMaterial");
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = playerStartingHealth;
        speedBoostTimer = 0.0f;
        playerSpeedModifier = 1.0f;

        if (defaultFont != null && arrowFont != null)
        {
            if (defaultFont.fallbackFontAssetTable == null)
                defaultFont.fallbackFontAssetTable = new List<TMP_FontAsset>();
            if (!defaultFont.fallbackFontAssetTable.Contains(arrowFont))
            {
                defaultFont.fallbackFontAssetTable.Add(arrowFont);
            }
            Debug.Log(defaultFont.fallbackFontAssetTable[0]);
        }

        nullFacedPickups = new List<GameObject>();

        // fps_prefab.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        bounds = GetComponent<Collider>().bounds;
        Debug.Log(bounds.min);
        Debug.Log(bounds.max);
        Debug.Log(bounds.size);

        playerHeight = storey_height / 3.0f;  // TODO : CHANGE THIS TO BE INSTANTIATED RELATIVE TO CAMERA

        generator = gameObject.GetComponent<MazeGenerator>();

        Color gradient = (LIGHTEST_FLOOR_COLOR - DEFAULT_COLOR) / generator.numFloors;
        List<Color> colors = new List<Color>() { DEFAULT_COLOR + gradient };
        for (int i = 1; i < generator.numFloors; i++)
        {
            colors.Add(colors[i - 1] + gradient);
        }
        floorColors = colors.ToArray();

        maze = generator.Generate();

        float offsetOffsetInWidth = 0.01f;
        float offsetOffsetInHeight = (Screen.width / Screen.height) * offsetOffsetInWidth;
        minimapOffset = new Vector2(((minimapWidth / Screen.width) / 2) + offsetOffsetInWidth, ((minimapHeight / Screen.height) / 2) + offsetOffsetInHeight);

        // Generate the minimaps
        minimaps = new Minimap[maze.Length];
        for (int i = 0; i < maze.Length; i++)
        {
            Minimap minimap = new Minimap();
            minimap.canvas = canvas;
            minimap.minimapOffset = minimapOffset;
            minimap.minimapWidth = minimapWidth;
            minimap.minimapHeight = minimapHeight;
            minimap.pathToMinimapBackground = pathToMinimapBackground;
            minimap.pathToMinimapFilter = pathToMinimapFilter;
            minimap.wallColor = minimapWallColor;
            minimap.floorColor = minimapFloorColor;
            minimaps[i] = minimap;
        }
        // Generate the player pointer on top of the minimaps
        playerPointer = new GameObject("Player Pointer");
        RectTransform trans = playerPointer.AddComponent<RectTransform>();
        trans.transform.SetParent(canvas.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchorMin = new Vector2(1.0f - (minimapWidth / Screen.width), 1.0f - (minimapHeight / Screen.height));
        trans.anchorMax = new Vector2(1.0f, 1.0f);
        trans.pivot = new Vector2(0.5f, 0.5f);
        //Debug.Log(Screen.width);
        trans.anchoredPosition = new Vector2(-minimapWidth / 2, -minimapHeight / 2);
        trans.sizeDelta = new Vector2(minimapWidth, minimapHeight); // custom size
        Image image = playerPointer.AddComponent<Image>();
        byte[] imageData = File.ReadAllBytes(pathToPlayerPointer);
        Texture2D tex = new Texture2D(2, 2);  // This width and height are overridden by LoadIMage()
        tex.LoadImage(imageData);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        playerPointer.transform.SetParent(canvas.transform);

        wall_thickness_x = ((bounds.size[0] / maze[0].GetLength(0)) / 11.0f) / 2;
        wall_thickness_z = ((bounds.size[2] / maze[0].GetLength(1)) / 11.0f) / 2;
        // I added the corner_thickness initialization here too, I'm assuming they should always be 2x
        corner_thickness_x = wall_thickness_x * 3;
        corner_thickness_z = wall_thickness_z * 3;
        generator.draw(maze);
        render(maze);
    }

    public void createPickup(string tileType, Vector3 position, Vector3 size)
    {
        createPickup(tileType, position, size, PICKUP_FONT_COLOR_DEFAULT);
    }

    public void createPickup(string tileType, Vector3 position, Vector3 size, Color textColor)
    {
        createPickup(tileType, position, size, null, textColor);
    }

    public void createPickup(string tileType, Vector3 position, Vector3 size, GameObject thingToFace, Color textColor)
    {
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.GetComponent<Renderer>().material = PICKUP_MATERIAL;
        pickup.GetComponent<Renderer>().enabled = true;
        pickup.name = tileType;
        pickup.transform.position = position;
        pickup.transform.localScale = size;
        Rigidbody rb = pickup.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        BoxCollider collider = pickup.GetComponent<BoxCollider>();
        Debug.Log(collider);
        collider.size = size;
        collider.isTrigger = true;
        pickup.AddComponent<PickUp>();
        if(thingToFace is not null)
        {
            pickup.GetComponent<PickUp>().thingToFace = fps_player_obj;
        }
        else
        {
            nullFacedPickups.Add(pickup);
        }

        TextMeshPro textMesh = pickup.AddComponent<TextMeshPro>();
        textMesh.text = tileType;
        textMesh.fontSize = PICKUP_FONT_SIZE;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.lineSpacing = 0;
        textMesh.font = defaultFont;

        GameObject backGroundBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backGroundBox.GetComponent<Renderer>().material = PICKUP_MATERIAL;
        backGroundBox.GetComponent<Renderer>().enabled = true;
        backGroundBox.name = tileType + " BackGroundBox";
        backGroundBox.transform.position = position;
        backGroundBox.transform.localScale = Vector3.Scale(size, new Vector3(1.5f, 1.5f, 0.1f));
        BoxCollider collider2 = backGroundBox.GetComponent<BoxCollider>();
        collider2.isTrigger = true;
        pickup.GetComponent<PickUp>().backGroundBox = backGroundBox;
        pickup.GetComponent<PickUp>().renderer = this;
    }

    private void createCorner(Vector3 position, int floor, float widthUnitSize)
    {
        GameObject cornerBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cornerBlock.name = MazeGenerator.WALL;
        cornerBlock.transform.localScale = new Vector3(corner_thickness_x, storey_height + RENDER_EPSILON, corner_thickness_z);
        cornerBlock.transform.position = position;
        cornerBlock.GetComponent<Renderer>().material.color = floorColors[floor];
        cornerBlock.GetComponent<Renderer>().material = wallMaterials[floor % wallMaterials.Length];
        cornerBlock.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.75f * corner_thickness_x / widthUnitSize, 0.75f);
    }

    private void render(string[][,][] floors)
    {
        // Minimaps
        for(int i = 0;  i < floors.Length; i++)
        {
            minimaps[i].mapDraw(i, floors[i]);
            minimaps[i].setVisible(true);
        }

        GetComponent<Renderer>().material.color = GROUND_COLOR;

        for (int f = 0; f < floors.Length; f++)
        {
            string[,][] floor = floors[f];
            float width = (float)floor.GetLength(0);
            float length = (float)floor.GetLength(1);
            for (int i = 0; i < floor.GetLength(0); i++)
            {
                for(int j = 0; j < floor.GetLength(1); j++)
                {
                    float floorY = bounds.min[1] + (f * storey_height) + RENDER_EPSILON;
                    float wallY = bounds.min[1] + ((f + 1) * storey_height - (storey_height / 2));

                    // These original coords do not correctly scale by dimensions of the bounds collider, due to the constant +0.5f which doesn't always result in the center
                    // This causes problems when rendering a maze when the MazeManager scale is not (1,1,1)
                    //Vector2 centerCoords2 = new Vector2(bounds.min[0] + (i * (bounds.size[0] / width - RENDER_EPSILON)) + 0.5f,
                    //                                   bounds.min[2] + (j * (bounds.size[2] / length - RENDER_EPSILON)) + 0.5f);
                    //Vector2 upLeft2 = new Vector2(bounds.min[0] + ((i + 0.5f) * (bounds.size[0] / width - RENDER_EPSILON)) + EDGE_EPSILON,
                    //                             bounds.min[2] + ((j + 0.5f) * (bounds.size[2] / length - RENDER_EPSILON)) + 0.5f);
                    //Vector2 downRight2 = new Vector2(bounds.min[0] + ((i + 0.5f) * (bounds.size[0] / width - RENDER_EPSILON)) + 0.5f,
                    //                                bounds.min[2] + ((j + 0.5f) * (bounds.size[2] / length - RENDER_EPSILON)) + EDGE_EPSILON);

                    // Fixed and cleaned up here
                    float widthUnitSize = (bounds.size[0] / width - RENDER_EPSILON);
                    float lengthUnitSize = (bounds.size[2] / length - RENDER_EPSILON);
                    float widthStart = bounds.min[0];
                    float lengthStart = bounds.min[2];
                    Vector2 centerCoords = new Vector2(widthStart + (i * widthUnitSize) + widthUnitSize / 2,
                                                       lengthStart + (j * lengthUnitSize) + lengthUnitSize / 2);
                    Vector2 upLeft = new Vector2(widthStart + (i * widthUnitSize) + EDGE_EPSILON,
                                                 lengthStart + ((j + 1) * lengthUnitSize) + EDGE_EPSILON);
                    Vector2 downRight = new Vector2(widthStart + ((i + 1) * widthUnitSize) + EDGE_EPSILON,
                                                    lengthStart + (j * lengthUnitSize) + EDGE_EPSILON);
                    float centerX = centerCoords.x;
                    float centerZ = centerCoords.y;
                    float leftX = upLeft.x;
                    float rightX = downRight.x;
                    float upZ = upLeft.y;
                    float downZ = downRight.y;
                    string tileType = floor[i, j][MazeGenerator.directionIndex(MazeGenerator.CENTER)];
                    if(!tileType.Equals(MazeGenerator.EMPTY) && !tileType.Contains(MazeGenerator.PIT_TRAP))
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = MazeGenerator.FLOOR + f;
                        block.transform.localScale = new Vector3(bounds.size[0] / width + RENDER_EPSILON, floor_height, bounds.size[2] / length + RENDER_EPSILON);
                        block.transform.position = new Vector3(centerX, floorY, centerZ);
                        block.GetComponent<Renderer>().material.color = floorColors[f];
                        if((f == 0) && (i == generator.start.Item1) && (j == generator.start.Item1))
                        {
                            block.GetComponent<Renderer>().material.color = START_TILE_COLOR;
                            treasureChest = Instantiate(treasureChestPrefab);
                            treasureChest.name = "Prize Chest";
                            treasureChest.transform.position = new Vector3(centerX, floorY + 0.0f, centerZ);
                            treasureChest.transform.rotation = Quaternion.Euler(0, 180, 0);
                            treasureChest.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);  // Adjust as necessary
                            BoxCollider collider = treasureChest.AddComponent<BoxCollider>();
                            // The following two lines actually mess with the already correct box collider, so I removed them
                            //collider.center = treasureChest.transform.position;
                            //collider.size = treasureChest.GetComponent<MeshRenderer>().bounds.size;
                        }
                        block.GetComponent<Renderer>().material = floorMaterials[f % floorMaterials.Length];
                    }
                    if(tileType.Contains(MazeGenerator.PIT_TRAP))
                    {
                        // Leave hole in floor.
                    }
                    if(tileType.Contains(MazeGenerator.POOF_TRAP))
                    {
                        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        barrier.name = MazeGenerator.POOF_TRAP;
                        barrier.transform.localScale = new Vector3(poof_trap_radius, storey_height * 0.2f, poof_trap_radius);
                        float placementRange_x = poof_trap_radius - wall_thickness_x;
                        float placementRange_z = poof_trap_radius - wall_thickness_z;
                        barrier.transform.position = new Vector3(centerX + UnityEngine.Random.Range(-placementRange_x, placementRange_x),
                                                                 wallY - (storey_height * 0.1f),
                                                                 centerZ + UnityEngine.Random.Range(-placementRange_z, placementRange_z));
                        barrier.GetComponent<Renderer>().material = POOF_TRAP_MATERIAL;
                        barrier.AddComponent<PoofTrap>();
                        barrier.GetComponent<PoofTrap>().floor = f;
                        barrier.GetComponent<PoofTrap>().storey_height = storey_height;
                        barrier.GetComponent<PoofTrap>().bounds = bounds;
                        barrier.GetComponent<PoofTrap>().maze = maze;
                    }
                    if(MazeGenerator.digits.Contains(tileType) || MazeGenerator.operators.Contains(tileType))
                    {
                        Vector3 pos = new Vector3(centerX,
                                                  floorY + (playerHeight / 2),
                                                  centerZ);
                        Vector3 size = new Vector3((playerHeight / 4) / 2, (playerHeight / 4) / 2, storey_height / 50.0f);
                        createPickup(tileType, pos, size);
                    }
                    if(MazeGenerator.boosts.Contains(tileType))
                    {
                        Vector3 pos = new Vector3(centerX,
                                                  floorY + (playerHeight / 2),
                                                  centerZ);
                        Vector3 size = new Vector3((playerHeight / 4) / 2, (playerHeight / 4) / 2, storey_height / 50.0f);
                        createPickup(tileType, pos, size, MazeGenerator.boostColors[Array.IndexOf(MazeGenerator.boosts, tileType)]);
                    }
                    if (floor[i, j][MazeGenerator.directionIndex(MazeGenerator.UP)].Equals(MazeGenerator.WALL))
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = MazeGenerator.WALL;
                        block.transform.localScale = new Vector3(bounds.size[0] / width + RENDER_EPSILON, storey_height + RENDER_EPSILON, wall_thickness_z);
                        block.transform.position = new Vector3(centerX, wallY, upZ);
                        block.GetComponent<Renderer>().material.color = ((f == 0) && (i == generator.start.Item1) && (j == generator.start.Item1)) ? START_TILE_COLOR : floorColors[f];
                        block.GetComponent<Renderer>().material = wallMaterials[f % wallMaterials.Length];

                        if ((i > 0) && (j < floor.GetLength(1) - 1) &&
                            floor[i - 1, j + 1][MazeGenerator.directionIndex(MazeGenerator.RIGHT)].Equals(MazeGenerator.WALL) &&
                            !floor[i - 1, j][MazeGenerator.directionIndex(MazeGenerator.RIGHT)].Equals(MazeGenerator.WALL) &&
                            !floor[i - 1, j][MazeGenerator.directionIndex(MazeGenerator.UP)].Equals(MazeGenerator.WALL))
                        {
                            createCorner(new Vector3(leftX, wallY, upZ), f, widthUnitSize);
                        }
                        if ((i < floor.GetLength(0) - 1) && (j < floor.GetLength(1) - 1) &&
                            floor[i + 1, j + 1][MazeGenerator.directionIndex(MazeGenerator.LEFT)].Equals(MazeGenerator.WALL) &&
                            !floor[i + 1, j][MazeGenerator.directionIndex(MazeGenerator.LEFT)].Equals(MazeGenerator.WALL) &&
                            !floor[i + 1, j][MazeGenerator.directionIndex(MazeGenerator.UP)].Equals(MazeGenerator.WALL))
                        {
                            createCorner(new Vector3(rightX, wallY, upZ), f, widthUnitSize);
                        }
                    }
                    if (floor[i, j][MazeGenerator.directionIndex(MazeGenerator.DOWN)].Equals(MazeGenerator.WALL))
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = MazeGenerator.WALL;
                        block.transform.localScale = new Vector3(bounds.size[0] / width + RENDER_EPSILON, storey_height + RENDER_EPSILON, wall_thickness_z);
                        block.transform.position = new Vector3(centerX, wallY, downZ);
                        block.GetComponent<Renderer>().material.color = ((f == 0) && (i == generator.start.Item1) && (j == generator.start.Item1)) ? START_TILE_COLOR : floorColors[f];
                        block.GetComponent<Renderer>().material = wallMaterials[f % wallMaterials.Length];

                        if ((i > 0) && (j > 0) &&
                            floor[i - 1, j - 1][MazeGenerator.directionIndex(MazeGenerator.RIGHT)].Equals(MazeGenerator.WALL) &&
                            !floor[i - 1, j][MazeGenerator.directionIndex(MazeGenerator.RIGHT)].Equals(MazeGenerator.WALL) &&
                            !floor[i - 1, j][MazeGenerator.directionIndex(MazeGenerator.DOWN)].Equals(MazeGenerator.WALL))
                        {
                            createCorner(new Vector3(leftX, wallY, downZ), f, widthUnitSize);
                        }
                        if ((i < floor.GetLength(0) - 1) && (j > 0) &&
                            floor[i + 1, j - 1][MazeGenerator.directionIndex(MazeGenerator.LEFT)].Equals(MazeGenerator.WALL) &&
                            !floor[i + 1, j][MazeGenerator.directionIndex(MazeGenerator.LEFT)].Equals(MazeGenerator.WALL) &&
                            !floor[i + 1, j][MazeGenerator.directionIndex(MazeGenerator.DOWN)].Equals(MazeGenerator.WALL))
                        {
                            createCorner(new Vector3(rightX, wallY, downZ), f, widthUnitSize);
                        }
                    }
                    if (floor[i, j][MazeGenerator.directionIndex(MazeGenerator.RIGHT)].Equals(MazeGenerator.WALL))
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = MazeGenerator.WALL;
                        block.transform.localScale = new Vector3(wall_thickness_x, storey_height + RENDER_EPSILON, bounds.size[2] / length + RENDER_EPSILON);
                        block.transform.position = new Vector3(rightX, wallY, centerZ);
                        block.GetComponent<Renderer>().material.color = ((f == 0) && (i == generator.start.Item1) && (j == generator.start.Item1)) ? START_TILE_COLOR : floorColors[f];
                        block.GetComponent<Renderer>().material = wallMaterials[f % wallMaterials.Length];
                    }
                    if (floor[i, j][MazeGenerator.directionIndex(MazeGenerator.LEFT)].Equals(MazeGenerator.WALL))
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = MazeGenerator.WALL;
                        block.transform.localScale = new Vector3(wall_thickness_x, storey_height + RENDER_EPSILON, bounds.size[2] / length + RENDER_EPSILON);
                        block.transform.position = new Vector3(leftX, wallY, centerZ);
                        block.GetComponent<Renderer>().material.color = ((f == 0) && (i == generator.start.Item1) && (j == generator.start.Item1)) ? START_TILE_COLOR : floorColors[f];
                        block.GetComponent<Renderer>().material = wallMaterials[f % wallMaterials.Length];
                    }
                }
            }
        }
        fps_player_obj = Instantiate(fps_prefab);
        fps_player_obj.name = PLAYER_NAME;
        // due to the fix above, the center of the maze is always 0,0
        fps_player_obj.transform.position = new Vector3(0, storey_height, 0);
        fps_player_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        fps_player_obj.AddComponent<Inventory>();
        //Debug.Log(fps_player_obj.transform.position);
        foreach(GameObject pickup in nullFacedPickups)
        {
            pickup.GetComponent<PickUp>().thingToFace = fps_player_obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        minimaps[(int)(fps_player_obj.transform.position.y / storey_height)].imgObject.GetComponent<RectTransform>().SetAsLastSibling();
        playerPointer.transform.SetAsLastSibling();
        playerPointer.GetComponent<RectTransform>().anchoredPosition = new Vector2((-minimapWidth / 2.0f) * (1 + (fps_player_obj.transform.position.x / bounds.max[0])), (-minimapHeight / 2.0f) * (1 + (fps_player_obj.transform.position.z / bounds.max[2])));
        playerPointer.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, (180 - fps_player_obj.transform.rotation.eulerAngles.y) % 360.0f);
        if(playerHealth <= 0)
        {
            // DISPLAY LOSS SCREEN
        }
        speedBoostTimer -= Time.deltaTime;
        if(speedBoostTimer <= 0.0f)
        {
            speedBoostTimer = 0.0f;
            playerSpeedModifier = 1.0f;
        }
        // USE PLAYER SPEED
    }
}
