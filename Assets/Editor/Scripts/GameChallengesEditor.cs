using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameChallenges))]
public class GameChallengesEditor : Editor
{
    public SerializedProperty
        challengeLeftProperty,
        challengeLeftTextProperty,
        floatValueLeftProperty,
        intValueLeftProperty,
        inGameStarIconLeftProperty,
        endGameStarIconLeftProperty,

        challengeMiddleProperty,
        floatValueMiddleProperty,
        endGameStarIconMiddleProperty,

        challengeRightProperty,
        challengeRightTextProperty,
        floatValueRightProperty,
        intValueRightProperty,
        inGameStarIconRightProperty,
        endGameStarIconRightProperty,

        colorEnabledProperty,
        colorDisabledProperty;

    void OnEnable()
    {
        challengeLeftProperty = serializedObject.FindProperty("challengeLeft");
        challengeLeftTextProperty = serializedObject.FindProperty("challengeLeftText");
        floatValueLeftProperty = serializedObject.FindProperty("floatValueLeft");
        intValueLeftProperty = serializedObject.FindProperty("intValueLeft");
        inGameStarIconLeftProperty = serializedObject.FindProperty("inGameStarIconLeft");
        endGameStarIconLeftProperty = serializedObject.FindProperty("endGameStarIconLeft");

        challengeMiddleProperty = serializedObject.FindProperty("challengeMiddle");
        floatValueMiddleProperty = serializedObject.FindProperty("floatValueMiddle");
        endGameStarIconMiddleProperty = serializedObject.FindProperty("endGameStarIconMiddle");

        challengeRightProperty = serializedObject.FindProperty("challengeRight");
        challengeRightTextProperty = serializedObject.FindProperty("challengeRightText");
        floatValueRightProperty = serializedObject.FindProperty("floatValueRight");
        intValueRightProperty = serializedObject.FindProperty("intValueRight");
        inGameStarIconRightProperty = serializedObject.FindProperty("inGameStarIconRight");
        endGameStarIconRightProperty = serializedObject.FindProperty("endGameStarIconRight");

        colorEnabledProperty = serializedObject.FindProperty("colorEnabled");
        colorDisabledProperty = serializedObject.FindProperty("colorDisabled");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(challengeLeftProperty, new GUIContent("Left Challenge"));
        GameChallenges.SecondaryChallenges leftType = (GameChallenges.SecondaryChallenges)challengeLeftProperty.enumValueIndex;
        switch (leftType)
        {
            case GameChallenges.SecondaryChallenges.dontTakeWeaponWithID:
            case GameChallenges.SecondaryChallenges.takeLessEqualAmmo:
            case GameChallenges.SecondaryChallenges.takeLessEqualDamageModifier:
            case GameChallenges.SecondaryChallenges.takeLessEqualShootingSpeedModifier:
            case GameChallenges.SecondaryChallenges.takeMoreEqualAmmo:
            case GameChallenges.SecondaryChallenges.takeMoreEqualDamageModifier:
            case GameChallenges.SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
            case GameChallenges.SecondaryChallenges.scoreMoreEqual:
                {
                    EditorGUILayout.PropertyField(intValueLeftProperty, new GUIContent("Int Value"));
                    break;
                }
            case GameChallenges.SecondaryChallenges.endWithTimeLessEqual:
            case GameChallenges.SecondaryChallenges.endWithTimeMoreEqual:
            case GameChallenges.SecondaryChallenges.reachLessEqualDistance:
            case GameChallenges.SecondaryChallenges.reachMoreEqualDistance:
            case GameChallenges.SecondaryChallenges.beInAdrenalineModeMoreEqual:
                {
                    EditorGUILayout.PropertyField(floatValueLeftProperty, new GUIContent("Float Value"));
                    break;
                }
        }
        EditorGUILayout.PropertyField(challengeLeftTextProperty, new GUIContent("Left Challenge Text"));
        EditorGUILayout.PropertyField(inGameStarIconLeftProperty, new GUIContent("In-Game Left Star"));
        EditorGUILayout.PropertyField(endGameStarIconLeftProperty, new GUIContent("End-Game Left Star"));

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(challengeMiddleProperty, new GUIContent("Middle Challenge"));
        GameChallenges.PrimaryChallenges middleType = (GameChallenges.PrimaryChallenges)challengeMiddleProperty.enumValueIndex;
        switch (middleType)
        {
            case GameChallenges.PrimaryChallenges.reachDistance:
            case GameChallenges.PrimaryChallenges.surviveForTime:
                {
                    EditorGUILayout.PropertyField(floatValueMiddleProperty, new GUIContent("Float Value"));
                    break;
                }
        }
        EditorGUILayout.PropertyField(endGameStarIconMiddleProperty, new GUIContent("End-Game Middle Star"));

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(challengeRightProperty, new GUIContent("Right Challenge"));
        GameChallenges.SecondaryChallenges rightType = (GameChallenges.SecondaryChallenges)challengeRightProperty.enumValueIndex;
        switch (rightType)
        {
            case GameChallenges.SecondaryChallenges.dontTakeWeaponWithID:
            case GameChallenges.SecondaryChallenges.takeLessEqualAmmo:
            case GameChallenges.SecondaryChallenges.takeLessEqualDamageModifier:
            case GameChallenges.SecondaryChallenges.takeLessEqualShootingSpeedModifier:
            case GameChallenges.SecondaryChallenges.takeMoreEqualAmmo:
            case GameChallenges.SecondaryChallenges.takeMoreEqualDamageModifier:
            case GameChallenges.SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
            case GameChallenges.SecondaryChallenges.scoreMoreEqual:
                {
                    EditorGUILayout.PropertyField(intValueRightProperty, new GUIContent("Int Value"));
                    break;
                }
            case GameChallenges.SecondaryChallenges.endWithTimeLessEqual:
            case GameChallenges.SecondaryChallenges.endWithTimeMoreEqual:
            case GameChallenges.SecondaryChallenges.reachLessEqualDistance:
            case GameChallenges.SecondaryChallenges.reachMoreEqualDistance:
            case GameChallenges.SecondaryChallenges.beInAdrenalineModeMoreEqual:
                {
                    EditorGUILayout.PropertyField(floatValueRightProperty, new GUIContent("Float Value"));
                    break;
                }
        }
        EditorGUILayout.PropertyField(challengeRightTextProperty, new GUIContent("Right Challenge Text"));
        EditorGUILayout.PropertyField(inGameStarIconRightProperty, new GUIContent("In-Game Right Star"));
        EditorGUILayout.PropertyField(endGameStarIconRightProperty, new GUIContent("End-Game Right Star"));

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}