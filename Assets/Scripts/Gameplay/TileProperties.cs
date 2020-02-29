using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Gameplay/Tile Properties")]
public class TileProperties : MonoBehaviour
{
    public float length;
    public List<TileProperties> NextTiles; // Если список не пуст, будет выбран один из тайлов
    [HideInInspector] public int number; // Задается в Game Settings и используется для вызова через список NextTiles
    [HideInInspector] public List<DynamicObstacle> dynamicObstacles;
    [HideInInspector] public List<SupplyBehaviour> supplyBehaviours;


    public void ValidateTile()
    {
        dynamicObstacles.Clear();
        supplyBehaviours.Clear();
        foreach (Transform child in transform)
        {
            DynamicObstacle dynamicObstacle = child.GetComponent<DynamicObstacle>();
            SupplyBehaviour supplyBehaviour = child.GetComponent<SupplyBehaviour>();
            if (dynamicObstacle) dynamicObstacles.Add(dynamicObstacle);
            if (supplyBehaviour) supplyBehaviours.Add(supplyBehaviour);
        }
    }

    public void SetTile(float position)
    {
        TurnOnSupplies();
        RepairDynamics();
        gameObject.transform.position = new Vector3(0, 0, position);
    }

    private void TurnOnSupplies()
    {
        foreach (SupplyBehaviour thing in supplyBehaviours)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawCube(transform.position+new Vector3(GameSettings.defaultOffsetX, 1, length/2), new Vector3(GameSettings.defaultBorders*2, 2, length));
    }
}
