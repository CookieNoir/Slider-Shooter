using UnityEngine;
[AddComponentMenu("Gameplay/Enemy")]
public class Enemy : RunningEntity
{
    public Transform player;
    public float distance;

    void Update()
    {
        MakeStep();
        if (Mathf.Abs(transform.position.z - player.position.z) < distance)
            GameSettings.GameResult(false);
    }

    public void Dying()
    {
        Destroy(gameObject, 2f);
    }
}
