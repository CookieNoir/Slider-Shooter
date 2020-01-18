using UnityEngine;
[AddComponentMenu("Gameplay/Camera Behaviour")]
public class CameraBehaviour : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, target.position.z, 0.05f));
    }
}
