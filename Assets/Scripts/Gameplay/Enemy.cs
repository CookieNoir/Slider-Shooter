using UnityEngine;
[AddComponentMenu("Gameplay/Enemy")]
[RequireComponent(typeof(Collider))]
public class Enemy : RunningEntity
{
    public Transform player;
    public bool alive = true;

    void Update()
    {
        if (alive)
        {
            MakeStep();
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x, 0.1f), transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player") GameSettings.GameResult(false);
    }

    private void OnTriggerEnter(Collider other)
    {/*
        if (other.tag == "Obstacle")
        {

        }*/
    }

    public void Dying()
    {
        alive = false;
        Destroy(gameObject, 2f);
    }
}
