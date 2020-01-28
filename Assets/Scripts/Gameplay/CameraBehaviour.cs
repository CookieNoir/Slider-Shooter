using UnityEngine;
[AddComponentMenu("Gameplay/Camera Behaviour")]
public class CameraBehaviour : RunningEntity
{
    public Transform target;
    public bool smooth = false;
    public float moveBack;

    protected override float ModifiedSpeed(float speed)
    {
        return speed - moveBack;
    }

    void Update()
    {
        if (smooth)
        {
            if (!GameSettings.gameOver) MakeStep();
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, target.position.z, 0.05f));
        }
        else transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z);
    }
}
