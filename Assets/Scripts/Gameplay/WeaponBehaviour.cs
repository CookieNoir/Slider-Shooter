using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Weapon Behaviour")]
public class WeaponBehaviour : MonoBehaviour
{
    public Sprite icon;
    public string weaponName;
    public int maxAmmo;
    public float shootingCooldown;
    public int damage;
    public float spentModifier;

    public Renderer fireRenderer;
    private int turn = 0;
    private IEnumerator firing;
    private float alpha;

    private void Start()
    {
        firing = Firing();
    }

    public void GetProperties(ref Image Icon, ref Text WeaponName, ref int MaxAmmo, ref float ShootingCooldown, ref int Damage, ref float SpentModifier)
    {
        Icon.sprite = icon;
        WeaponName.text = weaponName;
        MaxAmmo = maxAmmo;
        ShootingCooldown = shootingCooldown;
        Damage = damage;
        SpentModifier = spentModifier;
    }

    public void Fire()
    {
        switch (turn)
        {
            case 0: fireRenderer.material.mainTextureOffset = new Vector2(0, 0); break;
            case 1: fireRenderer.material.mainTextureOffset = new Vector2(0.5f, 0); break;
            case 2: fireRenderer.material.mainTextureOffset = new Vector2(0, 0.5f); break;
            case 3: fireRenderer.material.mainTextureOffset = new Vector2(0.5f, 0.5f); break;
        }
        turn++;
        StopCoroutine(firing);
        fireRenderer.gameObject.SetActive(true);
        alpha = 0;
        fireRenderer.material.SetFloat("_Alpha", alpha);
        firing = Firing();
        StartCoroutine(firing);
    }

    private IEnumerator Firing()
    {
        while (alpha < 1)
        {
            alpha += 0.25f;
            fireRenderer.material.SetFloat("_Alpha", alpha);
            yield return null;
        }
        yield return null;
        while (alpha > 0)
        {
            alpha -= 0.25f;
            fireRenderer.material.SetFloat("_Alpha", alpha);
            yield return null;
        }
        fireRenderer.gameObject.SetActive(false);
    }
}
