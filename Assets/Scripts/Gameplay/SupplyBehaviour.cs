﻿using UnityEngine;
[AddComponentMenu("Gameplay/Supply Behaviour")]
public class SupplyBehaviour : MonoBehaviour
{
    public enum supplyTypes {ammo, damage, shootingSpeed, weapon};
    public supplyTypes type;

    public float modifier; // для типов: damage, shootingSpeed

    public int amount; // для типов: ammo

    // для типов: weapon
    public GameObject weapon; // модель оружия
    public int maxAmmo; // максимальное число патронов
    public float shootingCooldown; // задержка перед следующим выстрелом
    public int damage; // базовый урон оружия

    public void GiveSuppliesTo(Player player)
    {
        switch (type)
        {
            case supplyTypes.ammo:
                {
                    player.FillAmmo(amount);
                    break;
                }
            case supplyTypes.damage:
                {
                    player.ModifyDamage(modifier);
                    break;
                }
            case supplyTypes.shootingSpeed:
                {
                    player.ModifyShootingCooldown(modifier);
                    break;
                }
            case supplyTypes.weapon:
                {
                    player.GiveNewWeapon(weapon, maxAmmo, shootingCooldown, damage);
                    break;
                }
        }
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
