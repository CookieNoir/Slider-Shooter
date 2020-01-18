﻿using UnityEngine;

public class RunningEntity : MonoBehaviour
{
    protected float currentSpeed;

    protected virtual float ModifiedSpeed(float speed)
    {
        return speed;
    }

    protected void MakeStep()
    {
        currentSpeed = ModifiedSpeed(GameSettings.speed);
        transform.position += new Vector3(0, 0, currentSpeed);
    }
}
