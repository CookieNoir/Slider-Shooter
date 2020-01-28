using System.Collections.Generic;
using UnityEngine;
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
    }
    [Header("Tiles")]
    public List<TileDouble> tiles;
    [Header("Global Moving")]
    public int maxValue;
    public Player player;
    public Transform enemy;
    public Transform cameraPos;

    public static UiMovement winWindow;
    public static UiMovement loseWindow;

    private float counter = -10f;
    private float offset = 8f;
    private int lastIndex = -1;
    private int lastTileArrayIndex = 0;
    private GameObject[] lastTiles;
    private Vector3 stepBack;

    private void Start()
    {
        Application.targetFrameRate = 30;
        winWindow = GameObject.FindWithTag("Win Window").GetComponent<UiMovement>();
        loseWindow = GameObject.FindWithTag("Lose Window").GetComponent<UiMovement>();
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
            gameOver = true;
        }
        else
        {
            loseWindow.Translate();
            gameOver = true;
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
