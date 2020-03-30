using UnityEngine;
[AddComponentMenu("Gameplay/Camera Behaviour")]
public class CameraBehaviour : RunningEntity
{
    public Transform target;
    public float moveBack;

    protected override float ModifiedSpeed(float speed)
    {
        return speed*Time.deltaTime - moveBack;
    }

    void Update()
    {
        if (!GameSettings.gameOver) MakeStep();
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, target.position.z, 0.05f));
    }
}
