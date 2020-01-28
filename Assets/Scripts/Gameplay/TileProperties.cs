using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Gameplay/Tile Properties")]
public class TileProperties : MonoBehaviour
{
    public float length;
    public List<DynamicObstacle> dynamicObstacles;
    public List<SupplyBehaviour> supplies;

    public void SetTile(float position)
    {
        TurnOnSupplies();
        RepairDynamics();
        gameObject.transform.position = new Vector3(0, 0, position);
    }

    private void TurnOnSupplies()
    {
        foreach (SupplyBehaviour thing in supplies)
        {
            thing.gameObject.SetActive(true);
        }
    }
    private void RepairDynamics()
    {
        foreach (DynamicObstacle thing in dynamicObstacles)
        {
            thing.gameObject.SetActive(true);
            thing.Refresh();
        }
    }
}
