using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SupplyBehaviour)), CanEditMultipleObjects]
public class SupplyBehaviourEditor : Editor
{
    public SerializedProperty
        typeProperty,
        addTypeProperty,
        modifierProperty,
        amountProperty,
        weaponProperty;

    void OnEnable()
    {
        typeProperty = serializedObject.FindProperty("type");
        addTypeProperty = serializedObject.FindProperty("addType");
        modifierProperty = serializedObject.FindProperty("modifier");
        amountProperty = serializedObject.FindProperty("amount");
        weaponProperty = serializedObject.FindProperty("weapon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProperty, new GUIContent("Supply Type"));

        SupplyBehaviour.SupplyTypes type = (SupplyBehaviour.SupplyTypes)typeProperty.enumValueIndex;


        switch (type)
        {
            case SupplyBehaviour.SupplyTypes.ammo:
                {

                    EditorGUILayout.PropertyField(addTypeProperty, new GUIContent("Add Type"));
                    SupplyBehaviour.AddTypes addType = (SupplyBehaviour.AddTypes)addTypeProperty.enumValueIndex;
                    switch (addType)
                    {
                        case SupplyBehaviour.AddTypes.Fixed:
                            {
                                EditorGUILayout.PropertyField(amountProperty, new GUIContent("Amount"));
                                break;
                            }
                        case SupplyBehaviour.AddTypes.Relative:
                            {
                                EditorGUILayout.PropertyField(modifierProperty, new GUIContent("Part"));
                                break;
                            }
                    }
                    break;
                }
            case SupplyBehaviour.SupplyTypes.damage:
            case SupplyBehaviour.SupplyTypes.shootingSpeed:
                {
                    EditorGUILayout.PropertyField(modifierProperty, new GUIContent("Modifier"));
                    break;
                }
            case SupplyBehaviour.SupplyTypes.weapon:
                {
                    EditorGUILayout.PropertyField(weaponProperty, new GUIContent("Weapon ID"));
                    EditorGUILayout.PropertyField(amountProperty, new GUIContent("Start Ammo"));
                    break;
                }
        }

        serializedObject.ApplyModifiedProperties();
    }
}