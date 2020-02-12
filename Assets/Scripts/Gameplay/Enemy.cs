using System.Collections;
using UnityEngine;
[AddComponentMenu("Gameplay/Enemy")]
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
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= 0.002f;
                transform.position += new Vector3(0, 0, currentSpeed);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>()) collision.gameObject.GetComponent<Player>().Dying();
            GameSettings.GameResult(false);
            // анимация поедания
            Destroy(this);
        }
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
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
        // анимация смерти
    }

    public void Lost()
    {
        // анимация поражения
    }
}
