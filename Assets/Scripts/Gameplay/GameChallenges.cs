﻿using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Game Challenges")]
public class GameChallenges : MonoBehaviour
{
    public enum PrimaryChallenges
    {
        killEnemy,          //  +
        surviveForTime,     //  +
        reachDistance       //  +
    };

    public enum SecondaryChallenges
    {
        dontTakeAmmo,                           // не подбирай патроны                                                  +
        dontEmptyAmmo,                          // не опустошай боезапас                                                +
        dontTakeAnyWeapon,                      // не подбирай оружие                                                   +
        dontTakeWeaponWithID,                   // не подбирай особое оружие                                            +
        dontTakeDamageModifier,                 // не подбирай модификаторы урона (усилители)                           +
        dontTakeShootingSpeedModifier,          // не подбирай модификаторы скорости стрельбы (ускорители)              +
        dontTakeInjury,                         // не получай повреждений                                               +

        takeLessEqualAmmo,                      // подбери не более N патронов                                          +
        takeLessEqualDamageModifier,            // подбери не более N  модификаторов урона (усилителей)                 +
        takeLessEqualShootingSpeedModifier,     // подбери не более N модификаторов скорости стрельбы (ускорителей)     +

        takeMoreEqualAmmo,                      // подбери не менее N патронов                                          +
        takeMoreEqualDamageModifier,            // подбери не менее N модификаторов урона (усилителей)                  +
        takeMoreEqualShootingSpeedModifier,     // подбери не менее N модификаторов скорости стрельбы (ускорителей)     +

        endWithTimeLessEqual,                   // пройди меньше, чем за N секунд                                       +
        endWithTimeMoreEqual,                   // проживи не менее N секунд                                            +

        reachLessEqualDistance,                 // закончи игру, преодолев не более N метров                            +
        reachMoreEqualDistance,                 // закончи игру, преодолев не менее N метров                            +

        scoreMoreEqual,                         // набери N очков                                                       +

        beInAdrenalineModeMoreEqual             // проведи в режиме адреналина не менее N секунд                        +
    };

    public enum EventTypes
    {
        tookAmmo,                   //int
        tookWeapon,                 //int(Weapon ID)
        tookDamageModifier,         //int
        tookShootingSpeedModifier,  //int
        tookInjury,                 //int

        changedTime,                //float
        changedScore,               //int
        changedAdrenalineTime,      //int(Ticks Spent)
        changedDistance,            //float

        emptyAmmo,                  //bool
        enemyKilled                 //bool
    };

    //---------------------------------
    [Header("Left Star Variables")]
    public SecondaryChallenges challengeLeft;
    public float floatValueLeft;
    public int intValueLeft;
    public MaskableGraphic inGameStarIconLeft;
    public MaskableGraphic endGameStarIconLeft;
    //---------------------------------
    [Header("Middle Star Variables")]
    public PrimaryChallenges challengeMiddle;
    public float floatValueMiddle;
    public MaskableGraphic endGameStarIconMiddle;
    //---------------------------------
    [Header("Right Star Variables")]
    public SecondaryChallenges challengeRight;
    public float floatValueRight;
    public int intValueRight;
    public MaskableGraphic inGameStarIconRight;
    public MaskableGraphic endGameStarIconRight;
    //---------------------------------
    [Header("Other")]
    public Color colorEnabled;
    public Color colorDisabled;

    private bool earnedLeft;
    private bool earnedRight;
    private static GameChallenges instance;

    private void Awake()
    {
        instance = this as GameChallenges;
    }

    public static void HandleEvent(EventTypes type)
    {
        bool left = instance.earnedLeft, right = instance.earnedRight;
        switch (type)
        {
            case EventTypes.emptyAmmo:
                {
                    if (instance.challengeLeft == SecondaryChallenges.dontEmptyAmmo)
                    {
                        instance.earnedLeft = false;
                    }
                    if (instance.challengeRight == SecondaryChallenges.dontEmptyAmmo)
                    {
                        instance.earnedRight = false;
                    }
                    break;
                }
            case EventTypes.enemyKilled:
                {
                    if (instance.challengeMiddle == PrimaryChallenges.killEnemy)
                    {
                        GameSettings.GameResult(true);
                    }
                    break;
                }
        }
        VisualizeChanges(left, right);
    }

    public static void HandleEvent(EventTypes type, int value)
    {
        bool left = instance.earnedLeft, right = instance.earnedRight;
        switch (type)
        {
            case EventTypes.tookAmmo:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.dontTakeAmmo:
                            {
                                instance.earnedLeft = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualAmmo:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualAmmo:
                            {
                                instance.earnedLeft = (value >= instance.intValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeAmmo:
                            {
                                instance.earnedRight = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualAmmo:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualAmmo:
                            {
                                instance.earnedRight = (value >= instance.intValueRight);
                                break;
                            }
                    }
                    break;
                }
            case EventTypes.tookWeapon:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.dontTakeAnyWeapon:
                            {
                                instance.earnedLeft = false;
                                break;
                            }
                        case SecondaryChallenges.dontTakeWeaponWithID:
                            {
                                instance.earnedLeft = (value != instance.intValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeAnyWeapon:
                            {
                                instance.earnedRight = false;
                                break;
                            }
                        case SecondaryChallenges.dontTakeWeaponWithID:
                            {
                                instance.earnedRight = (value != instance.intValueRight);
                                break;
                            }
                    }
                    break;
                }
            case EventTypes.tookDamageModifier:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.dontTakeDamageModifier:
                            {
                                instance.earnedLeft = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualDamageModifier:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualDamageModifier:
                            {
                                instance.earnedLeft = (value >= instance.intValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeDamageModifier:
                            {
                                instance.earnedRight = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualDamageModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualDamageModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                    }
                    break;
                }
            case EventTypes.tookShootingSpeedModifier:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.dontTakeShootingSpeedModifier:
                            {
                                instance.earnedLeft = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualShootingSpeedModifier:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeShootingSpeedModifier:
                            {
                                instance.earnedRight = (value <= 0);
                                break;
                            }
                        case SecondaryChallenges.takeLessEqualShootingSpeedModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                    }
                    break;
                }
            case EventTypes.tookInjury:
                {
                    if (instance.challengeLeft == SecondaryChallenges.dontTakeInjury)
                    {
                        instance.earnedLeft = false;
                    }
                    if (instance.challengeRight == SecondaryChallenges.dontTakeInjury)
                    {
                        instance.earnedRight = false;
                    }
                    break;
                }
            case EventTypes.changedScore:
                {
                    if (instance.challengeLeft == SecondaryChallenges.scoreMoreEqual)
                    {
                        instance.earnedLeft = (value >= instance.intValueLeft);
                    }
                    if (instance.challengeRight == SecondaryChallenges.scoreMoreEqual)
                    {
                        instance.earnedRight = (value >= instance.intValueRight);
                    }
                    break;
                }
            case EventTypes.changedAdrenalineTime:
                {
                    if (instance.challengeLeft == SecondaryChallenges.beInAdrenalineModeMoreEqual)
                    {
                        instance.earnedLeft = (value >= (int)Mathf.Floor(instance.floatValueLeft * Application.targetFrameRate));
                    }
                    if (instance.challengeRight == SecondaryChallenges.beInAdrenalineModeMoreEqual)
                    {
                        instance.earnedRight = (value >= (int)Mathf.Floor(instance.floatValueRight * Application.targetFrameRate));
                    }
                    break;
                }
        }
        VisualizeChanges(left, right);
    }

    public static void HandleEvent(EventTypes type, float value)
    {
        bool left = instance.earnedLeft, right = instance.earnedRight;
        switch (type)
        {
            case EventTypes.changedTime:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.endWithTimeLessEqual:
                            {
                                instance.earnedLeft = (value <= instance.floatValueLeft);
                                break;
                            }
                        case SecondaryChallenges.endWithTimeMoreEqual:
                            {
                                instance.earnedLeft = (value >= instance.floatValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.endWithTimeLessEqual:
                            {
                                instance.earnedRight = (value <= instance.floatValueRight);
                                break;
                            }
                        case SecondaryChallenges.endWithTimeMoreEqual:
                            {
                                instance.earnedRight = (value >= instance.floatValueRight);
                                break;
                            }
                    }
                    if (instance.challengeMiddle == PrimaryChallenges.surviveForTime)
                    {
                        if (value >= instance.floatValueMiddle)
                        {
                            GameSettings.GameResult(true);
                        }
                    }
                    break;
                }
            case EventTypes.changedDistance:
                {
                    switch (instance.challengeLeft)
                    {
                        case SecondaryChallenges.reachLessEqualDistance:
                            {
                                instance.earnedLeft = (value <= instance.floatValueLeft);
                                break;
                            }
                        case SecondaryChallenges.reachMoreEqualDistance:
                            {
                                instance.earnedLeft = (value >= instance.floatValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.reachLessEqualDistance:
                            {
                                instance.earnedRight = (value <= instance.floatValueRight);
                                break;
                            }
                        case SecondaryChallenges.reachMoreEqualDistance:
                            {
                                instance.earnedRight = (value >= instance.floatValueRight);
                                break;
                            }
                    }
                    if (instance.challengeMiddle == PrimaryChallenges.reachDistance)
                    {
                        GameSettings.GameResult(true);
                    }
                    break;
                }
        }
        VisualizeChanges(left, right);
    }

    private static void VisualizeChanges(bool left, bool right)
    {

        if (left != instance.earnedLeft)
        {
            if (instance.earnedLeft)
                instance.StartCoroutine(UIHelper.ColorChanger(instance.inGameStarIconLeft, instance.colorEnabled));
            else
                instance.StartCoroutine(UIHelper.ColorChanger(instance.inGameStarIconLeft, instance.colorDisabled));
        }
        if (right != instance.earnedRight)
        {
            if (instance.earnedRight)
                instance.StartCoroutine(UIHelper.ColorChanger(instance.inGameStarIconRight, instance.colorEnabled));
            else
                instance.StartCoroutine(UIHelper.ColorChanger(instance.inGameStarIconRight, instance.colorDisabled));
        }
    }

    public static void VisualizeStarsAtTheEnd()
    {
        if (instance.earnedLeft)
            instance.StartCoroutine(UIHelper.ColorChanger(instance.endGameStarIconLeft, instance.colorEnabled));

        instance.StartCoroutine(UIHelper.ColorChanger(instance.endGameStarIconMiddle, instance.colorEnabled));

        if (instance.earnedRight)
            instance.StartCoroutine(UIHelper.ColorChanger(instance.endGameStarIconRight, instance.colorEnabled));
    }
}
