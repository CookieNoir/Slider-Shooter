using UnityEngine;

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
