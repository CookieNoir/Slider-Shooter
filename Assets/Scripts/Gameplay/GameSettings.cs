using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Game Settings")]
public class GameSettings : MonoBehaviour
{
    public float setSpeed;
    public float setDefaultBorders;
    public float setDefaultOffsetX;

    public static float speed;
    public static bool modifySpeed = true;
    public static float defaultBorders;
    public static float defaultOffsetX;
    public static bool gameOver = false;

    [System.Serializable]
    public class TileDouble
    {
        private bool first;
        public GameObject firstTile;
        public GameObject secondTile;

        public bool IsFirst()
        {
            return first;
        }
        public void NotFirst()
        {
            first = !first;
        }
        public TileDouble(GameObject first, GameObject second)
        {
            firstTile = first;
            secondTile = second;
        }
    }
    [Header("Tiles")]
    public TilesUpdater tilesUpdater;
    public List<TileDouble> tiles;

    [Header("Global Moving")]
    public int maxValue;
    public Player player;
    public Transform enemy;
    public Transform cameraPos;

    public static UiMovement winWindow;
    public static UiMovement loseWindow;
    public static AlphaChanger darkScreen;

    private float counter = -10f;
    private float offset = 8f;
    private int lastIndex = -1;
    private int lastTileArrayIndex = 0;
    private GameObject[] lastTiles;
    private Vector3 stepBack;

    private static GameSettings instance;
    private static IEnumerator dark;
    private static IEnumerator hurtIndicatorColor;

    private void Start()
    {
        Application.targetFrameRate = 30;
        instance = this as GameSettings;
        winWindow = GameObject.FindWithTag("Win Window").GetComponent<UiMovement>();
        loseWindow = GameObject.FindWithTag("Lose Window").GetComponent<UiMovement>();
        darkScreen = GameObject.FindWithTag("Dark Screen").GetComponent<AlphaChanger>();
        dark = darkScreen.ChangeAlpha();
        instance.StartCoroutine(dark);
        speed = setSpeed;
        counter = counter - offset;
        modifySpeed = true;
        defaultBorders = setDefaultBorders;
        defaultOffsetX = setDefaultOffsetX;
        gameOver = false;
        lastTiles = new GameObject[3];
        stepBack = new Vector3(0, 0, maxValue);
    }

    private void ModifySpeed()
    {
        if (modifySpeed) { speed += 0.00001f; }
    }

    private void Update()
    {
        ModifySpeed();
        SetTiles();
    }

    public void SetTiles()
    {
        if (player && player.transform.position.z > counter)
        {
            int index = Random.Range(0, tiles.Count);
            if (index == lastIndex) index = (index + 1) % tiles.Count;
            if (lastTiles[lastTileArrayIndex]) lastTiles[lastTileArrayIndex].SetActive(false);
            if (tiles[index].IsFirst())
            {
                tiles[index].firstTile.SetActive(true);
                tiles[index].firstTile.GetComponent<TileProperties>().SetTile(counter + offset);
                counter += tiles[index].firstTile.GetComponent<TileProperties>().length;
                lastTiles[lastTileArrayIndex] = tiles[index].firstTile;
            }
            else
            {
                tiles[index].secondTile.SetActive(true);
                tiles[index].secondTile.GetComponent<TileProperties>().SetTile(counter + offset);
                counter += tiles[index].secondTile.GetComponent<TileProperties>().length;
                lastTiles[lastTileArrayIndex] = tiles[index].secondTile;
            }
            tiles[index].NotFirst();
            lastIndex = index;
            lastTileArrayIndex = (lastTileArrayIndex + 1) % 3;
        }
        if (counter > maxValue)
        {
            for (int i = 0; i < 3; ++i) lastTiles[i].transform.position -= stepBack;
            player.transform.position -= stepBack;
            enemy.position -= stepBack;
            cameraPos.position -= stepBack;
            counter -= maxValue;
        }
    }

    public static void GameResult(bool alive)
    {
        if (alive)
        {
            winWindow.Translate();
        }
        else
        {
            loseWindow.Translate();      
        }
        gameOver = true;
        instance.StopCoroutine(dark);
        dark = darkScreen.ChangeAlpha();
        instance.StartCoroutine(dark);
        instance.StartCoroutine(hurtIndicatorColor);
    }

    public void FillTiles()
    {
        tiles.Clear();
        tilesUpdater.RefreshTiles();
        int count = tilesUpdater.tiles.childCount;
        for (int i = 0; i < count; ++i)
        {
            if (tilesUpdater.tiles.GetChild(i).gameObject.activeSelf == true)
            {
                tiles.Add(new TileDouble(tilesUpdater.tiles.GetChild(i).gameObject, tilesUpdater.tilesClones.GetChild(i).gameObject));
                tilesUpdater.tiles.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public static void ChangeHurtIndicator(MaskableGraphic component, Color targetColor, Color baseColor)
    {
        instance.StartCoroutine(ColorChanger(component, targetColor));
        hurtIndicatorColor = ColorChanger(component, baseColor);
    }
    public static void ChangeHurtIndicator(MaskableGraphic component, Color targetColor)
    {
        hurtIndicatorColor = ColorChanger(component, targetColor);
    }

    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor)
    {
        Color baseColor = component.color;
        for (float f = 0.02f; f <= 1; f += 0.02f)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, f * (2 - f));
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(-defaultBorders + defaultOffsetX, 0, 0), new Vector3(-defaultBorders + defaultOffsetX, 0, maxValue));
        Gizmos.DrawLine(new Vector3(defaultBorders + defaultOffsetX, 0, 0), new Vector3(defaultBorders + defaultOffsetX, 0, maxValue));
        Gizmos.DrawLine(new Vector3(defaultBorders + defaultOffsetX, 0, 0), new Vector3(-defaultBorders + defaultOffsetX, 0, 0));
        Gizmos.DrawLine(new Vector3(-defaultBorders + defaultOffsetX, 0, maxValue), new Vector3(defaultBorders + defaultOffsetX, 0, maxValue));
    }

    private void OnValidate()
    {
        player.borders = setDefaultBorders;
        player.offsetX = setDefaultOffsetX;
        defaultBorders = setDefaultBorders;
        defaultOffsetX = setDefaultOffsetX;
    }
}
