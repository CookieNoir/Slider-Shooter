using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[AddComponentMenu("Gameplay/Game Settings")]
public class GameSettings : MonoBehaviour
{
    public float setSpeed;
    public static float speed;
    public static bool modifySpeed = true;

    public List<GameObject> tiles;
    public Transform player;
    private float counter = -10f;
    private float offset = 8f;
    private int lastIndex = -1;

    private void Start()
    {
        Application.targetFrameRate = 30;
        speed = setSpeed;
        counter = counter - offset;
    }

    private void ModifySpeed()
    {
        if (modifySpeed) { }
    }

    private void Update()
    {
        if (player.position.z > counter)
        {
            int index = Random.Range(0, tiles.Count);
            if (index == lastIndex) index = (index + 1) % tiles.Count;
            tiles[index].GetComponent<TileProperties>().SetTile(counter + offset);
            counter += tiles[index].GetComponent<TileProperties>().length;
            lastIndex = index;
        }
    }

    public static void GameResult(bool alive)
    {
        if (alive)
        {
            //win window
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
