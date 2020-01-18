using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SupplyBehaviour)), CanEditMultipleObjects]
public class SupplyBehaviourEditor : Editor
{
    public SerializedProperty
        typeProp,
        modifierProp,
        amountProp,
        weaponProp,
        maxAmmoProp,
        shootingCooldownProp,
        damageProp;

    void OnEnable()
    {
        typeProp = serializedObject.FindProperty("type");
        modifierProp = serializedObject.FindProperty("modifier");
        amountProp = serializedObject.FindProperty("amount");
        weaponProp = serializedObject.FindProperty("weapon");
        maxAmmoProp = serializedObject.FindProperty("maxAmmo");
        shootingCooldownProp = serializedObject.FindProperty("shootingCooldown");
        damageProp = serializedObject.FindProperty("damage");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProp);

        SupplyBehaviour.supplyTypes type = (SupplyBehaviour.supplyTypes)typeProp.enumValueIndex;

        switch (type)
        {
            case SupplyBehaviour.supplyTypes.ammo:
                {
                    EditorGUILayout.PropertyField(amountProp, new GUIContent("Amount"));
                    break;
                }
            case SupplyBehaviour.supplyTypes.damage:
            case SupplyBehaviour.supplyTypes.shootingSpeed:
                {
                    EditorGUILayout.PropertyField(modifierProp, new GUIContent("Modifier"));
                    break;
                }
            case SupplyBehaviour.supplyTypes.weapon:
                {
                    EditorGUILayout.PropertyField(weaponProp, new GUIContent("Weapon Prefab"));
                    EditorGUILayout.PropertyField(maxAmmoProp, new GUIContent("Max Ammo"));
                    EditorGUILayout.PropertyField(shootingCooldownProp, new GUIContent("Shooting Cooldown"));
                    EditorGUILayout.PropertyField(damageProp, new GUIContent("Damage"));
                    break;
                }
        }

        serializedObject.ApplyModifiedProperties();
    }
}