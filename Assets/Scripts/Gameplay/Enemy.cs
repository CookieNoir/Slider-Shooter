using UnityEngine;
[AddComponentMenu("Gameplay/Enemy")]
public class Enemy : RunningEntity
{
    public Player player;
    public Transform target;
    public float distance = 1f;
    [HideInInspector] public bool alive = true;

    void Update()
    {
        if (alive)
        {
            CheckDistance();
            MakeStep();
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, 0.1f), transform.position.y, transform.position.z);
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

    private void CheckDistance()
    {
        if (target.position.z - transform.position.z < distance)
        {
            if (player) player.Dying();
            GameSettings.GameResult(false);
            // анимация поедания
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            DestructableWall wall = other.gameObject.GetComponent<DestructableWall>();
            if (wall)
            {
                wall.Change(transform.position);
            }
        }
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
