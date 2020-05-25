using UnityEngine;
using System.Collections;
[AddComponentMenu("Gameplay/Dynamic Obstacle/Destructable Wall")]
public class DestructableWall : DynamicObstacle
{
    private IEnumerator blocksAlive;

    private void Start()
    {
        blocksAlive = BlocksAlive();
    }

    private IEnumerator BlocksAlive()
    {
        yield return new WaitForSeconds(2f);
        foreach (Transform child in transform)
        {
            child.GetComponent<Collider>().enabled = false;
        }
        yield return new WaitForSeconds(2f);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public override void Refresh()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        foreach (Transform child in transform)
        {
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.GetComponent<Collider>().enabled = true;
            child.gameObject.SetActive(false);
        }
    }

    public override void Change()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        StopCoroutine(blocksAlive);
        blocksAlive = BlocksAlive();
        StartCoroutine(blocksAlive);
    }
    
    public void Change(Vector3 position)
    {
        Change();
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.3f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Wall Part")
            {
                Rigidbody rb = hitColliders[i].GetComponent<Rigidbody>();
                rb.AddExplosionForce(5f, position, 0.3f, 3.0F);
            }
        }
    }
}
