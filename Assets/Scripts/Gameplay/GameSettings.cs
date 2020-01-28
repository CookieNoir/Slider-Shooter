﻿using System.Collections.Generic;
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

    public List<GameObject> tiles;
    public Player player;

    public static UiMovement winWindow;
    public static UiMovement loseWindow;

    private float counter = -10f;
    private float offset = 8f;
    private int lastIndex = -1;

    private void Start()
    {
        Application.targetFrameRate = 30;
        winWindow = GameObject.FindWithTag("Win Window").GetComponent<UiMovement>();
        loseWindow = GameObject.FindWithTag("Lose Window").GetComponent<UiMovement>();
        speed = setSpeed;
        counter = counter - offset;
    }

    private void ModifySpeed()
    {
        if (modifySpeed) { speed += 0.00001f; }
    }

    private void Update()
    {
        ModifySpeed();
        if (player && player.transform.position.z > counter)
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
            winWindow.Translate();
        }
        else
        {
            loseWindow.Translate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(-defaultBorders + defaultOffsetX, 0, -10000), new Vector3(-defaultBorders + defaultOffsetX, 0, 10000));
        Gizmos.DrawLine(new Vector3(defaultBorders + defaultOffsetX, 0, -10000), new Vector3(defaultBorders + defaultOffsetX, 0, 10000));
    }

    private void OnValidate()
    {
        player.borders = setDefaultBorders;
        player.offsetX = setDefaultOffsetX;
        defaultBorders = setDefaultBorders;
        defaultOffsetX = setDefaultOffsetX;
    }
}
