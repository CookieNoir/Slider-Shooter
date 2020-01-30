using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SupplyBehaviour)), CanEditMultipleObjects]
public class SupplyBehaviourEditor : Editor
{
    public SerializedProperty
        typeProp,
        addTypeProp,
        modifierProp,
        amountProp,
        weaponProp,
        maxAmmoProp,
        shootingCooldownProp,
        damageProp,
        spentProp;

    void OnEnable()
    {
        typeProp = serializedObject.FindProperty("type");
        addTypeProp = serializedObject.FindProperty("addType");
        modifierProp = serializedObject.FindProperty("modifier");
        amountProp = serializedObject.FindProperty("amount");
        weaponProp = serializedObject.FindProperty("weapon");
        maxAmmoProp = serializedObject.FindProperty("maxAmmo");
        shootingCooldownProp = serializedObject.FindProperty("shootingCooldown");
        damageProp = serializedObject.FindProperty("damage");
        spentProp = serializedObject.FindProperty("spentModifier");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProp, new GUIContent("Supply Type"));

        SupplyBehaviour.SupplyTypes type = (SupplyBehaviour.SupplyTypes)typeProp.enumValueIndex;


        switch (type)
        {
            case SupplyBehaviour.SupplyTypes.ammo:
                {

                    EditorGUILayout.PropertyField(addTypeProp, new GUIContent("Add Type"));
                    SupplyBehaviour.AddTypes addType = (SupplyBehaviour.AddTypes)addTypeProp.enumValueIndex;
                    switch (addType)
                    {
                        case SupplyBehaviour.AddTypes.Fixed:
                            {
                                EditorGUILayout.PropertyField(amountProp, new GUIContent("Amount"));
                                break;
                            }
                        case SupplyBehaviour.AddTypes.Relative:
                            {
                                EditorGUILayout.PropertyField(modifierProp, new GUIContent("Part"));
                                break;
                            }
                    }
                    break;
                }
            case SupplyBehaviour.SupplyTypes.damage:
            case SupplyBehaviour.SupplyTypes.shootingSpeed:
                {
                    EditorGUILayout.PropertyField(modifierProp, new GUIContent("Modifier"));
                    break;
                }
            case SupplyBehaviour.SupplyTypes.weapon:
                {
                    EditorGUILayout.PropertyField(weaponProp, new GUIContent("Weapon Prefab"));
                    EditorGUILayout.PropertyField(maxAmmoProp, new GUIContent("Max Ammo"));
                    EditorGUILayout.PropertyField(shootingCooldownProp, new GUIContent("Shooting Cooldown"));
                    EditorGUILayout.PropertyField(damageProp, new GUIContent("Damage"));
                    EditorGUILayout.PropertyField(spentProp, new GUIContent("Spent Modifier"));
                    break;
                }
        }

        serializedObject.ApplyModifiedProperties();
    }
}