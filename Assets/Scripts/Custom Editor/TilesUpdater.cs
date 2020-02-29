using UnityEngine;
[AddComponentMenu("Custom Editor/Tiles Updater")]
public class TilesUpdater : MonoBehaviour
{
    public Transform tiles;
    public Transform tilesClones;

    private void ClearTilesClones()
    {
        int childCount = tilesClones.childCount;
        while (childCount > 0)
        {
            DestroyImmediate(tilesClones.GetChild(0).gameObject);
            childCount--;
        }
    }

    private void CloneAndValidateTiles()
    {
        int childCount = tiles.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            tiles.GetChild(i).GetComponent<TileProperties>().ValidateTile();
            Transform go = Instantiate(tiles.GetChild(i), tilesClones);
            go.position += new Vector3(15f, 0, 0);

            go.gameObject.SetActive(false);
        }
    }

    public void RefreshTiles()
    {
        ClearTilesClones();
        CloneAndValidateTiles();
    }

    public void ShowTiles(bool val)
    {
        int childCount = tiles.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            tiles.GetChild(i).gameObject.SetActive(val);
        }
    }

    public void ShowTilesClones(bool val)
    {
        int childCount = tilesClones.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            tilesClones.GetChild(i).gameObject.SetActive(val);
        }
    }
}
