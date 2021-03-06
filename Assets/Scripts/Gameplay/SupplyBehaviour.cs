﻿using UnityEngine;
[AddComponentMenu("Gameplay/Supply Behaviour")]
public class SupplyBehaviour : MonoBehaviour
{
    public enum SupplyTypes { ammo, damage, shootingSpeed, weapon };
    public SupplyTypes type;
    public enum AddTypes { Fixed, Relative };
    public AddTypes addType;
    public float modifier; // для типов: damage, shootingSpeed

    public int amount; // для типов: ammo

    // для типов: weapon
    public int weapon; // ID оружия

    public void GiveSuppliesTo(Player player)
    {
        switch (type)
        {
            case SupplyTypes.ammo:
                {
                    switch (addType)
                    {
                        case AddTypes.Fixed:
                            {
                                player.FillAmmo(amount);
                                break;
                            }
                        case AddTypes.Relative:
                            {
                                player.FillAmmo(modifier);
                                break;
                            }
                    }
                    break;
                }
            case SupplyTypes.damage:
                {
                    player.ModifyDamage(modifier);
                    break;
                }
            case SupplyTypes.shootingSpeed:
                {
                    player.ModifyShootingCooldown(modifier);
                    break;
                }
            case SupplyTypes.weapon:
                {
                    if (amount < 0)
                        player.GiveNewWeapon(weapon);
                    else
                        player.GiveNewWeapon(weapon, amount);
                    break;
                }
        }
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        switch (type)
        {
            case SupplyTypes.ammo:
                {
                    Gizmos.DrawIcon(transform.position + new Vector3(0, 0.5f, 0), "Ammo Icon.png", true);
                    break;
                }
            case SupplyTypes.damage:
                {
                    Gizmos.DrawIcon(transform.position + new Vector3(0, 0.5f, 0), "Damage Icon.png", true);
                    break;
                }
            case SupplyTypes.shootingSpeed:
                {
                    Gizmos.DrawIcon(transform.position + new Vector3(0, 0.5f, 0), "Shooting Speed Icon.png", true);
                    break;
                }
            case SupplyTypes.weapon:
                {
                    Gizmos.DrawIcon(transform.position + new Vector3(0, 0.5f, 0), "Weapon Icon.png", true);
                    break;
                }
        }
    }
}
