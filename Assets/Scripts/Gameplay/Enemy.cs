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
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        GameSettings.GameResult(true);
        Destroy(this);
    }
}
