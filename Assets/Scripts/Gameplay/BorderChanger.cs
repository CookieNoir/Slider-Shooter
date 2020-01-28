using System.Collections;
using UnityEngine;
[AddComponentMenu("Gameplay/Border Changer")]
[RequireComponent(typeof(BoxCollider))]
public class BorderChanger : MonoBehaviour
{
    public bool setDefault = false;
    public float borders = 1.25f;
    public float offsetX = 0;
    public float length = 1f;

    public BorderChanger previous; // нужен для отображения связи (в игровых вычислениях не участвует)

    public IEnumerator ChangeBordersOf(Player player)
    {
        float playerBorders = player.borders;
        float playerOffsetX = player.offsetX;
        float q = (player.transform.position.z - transform.position.z) / length;
        while (q < 1)
        {
            player.borders = Mathf.Lerp(playerBorders, borders, q);
            player.offsetX = Mathf.Lerp(playerOffsetX, offsetX, q);
            yield return null;
            q = (player.transform.position.z - transform.position.z) / length;
        }
        player.borders = borders;
        player.offsetX = offsetX;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position + new Vector3(0, 0.5f, 0), "Borders Changer.png", true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if (!previous)
        {
            Gizmos.DrawLine(new Vector3(-GameSettings.defaultBorders + GameSettings.defaultOffsetX + transform.position.x, transform.position.y, transform.position.z), new Vector3(GameSettings.defaultBorders + GameSettings.defaultOffsetX + transform.position.x, transform.position.y, transform.position.z));
            Gizmos.DrawLine(new Vector3(GameSettings.defaultBorders + GameSettings.defaultOffsetX + transform.position.x, transform.position.y, transform.position.z), new Vector3(borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length));
            Gizmos.DrawLine(new Vector3(borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length), new Vector3(-borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length));
            Gizmos.DrawLine(new Vector3(-borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length), new Vector3(-GameSettings.defaultBorders + GameSettings.defaultOffsetX + transform.position.x, transform.position.y, transform.position.z));
        }
        else
        {
            Gizmos.DrawLine(new Vector3(-previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z), new Vector3(previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z));
            Gizmos.DrawLine(new Vector3(previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z), new Vector3(borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length));
            Gizmos.DrawLine(new Vector3(borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length), new Vector3(-borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length));
            Gizmos.DrawLine(new Vector3(-borders + offsetX + transform.position.x, transform.position.y, transform.position.z + length), new Vector3(-previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z));

            Gizmos.DrawLine(new Vector3(-previous.borders + previous.offsetX + previous.transform.position.x, previous.transform.position.y, previous.transform.position.z + previous.length), new Vector3(-previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z));
            Gizmos.DrawLine(new Vector3(previous.borders + previous.offsetX + previous.transform.position.x, previous.transform.position.y, previous.transform.position.z + previous.length), new Vector3(previous.borders + previous.offsetX + transform.position.x, transform.position.y, transform.position.z));
        }
    }

    private void OnValidate()
    {
        if (setDefault)
        {
            borders = GameSettings.defaultBorders;
            offsetX = GameSettings.defaultOffsetX;
        }
        if (length < 0) length = 0;
        GetComponent<BoxCollider>().center = new Vector3(GameSettings.defaultOffsetX, 0, length / 2);
        GetComponent<BoxCollider>().size = new Vector3(GameSettings.defaultBorders * 2, 1, length);
    }
}
