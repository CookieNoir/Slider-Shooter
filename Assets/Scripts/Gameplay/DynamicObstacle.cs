using UnityEngine;
[AddComponentMenu("Gameplay/Dynamic Obstacle")]
public class DynamicObstacle : MonoBehaviour
{
    public virtual void Refresh()
    {
    }

    public virtual void Change()
    {
        gameObject.SetActive(false);
    }
}
