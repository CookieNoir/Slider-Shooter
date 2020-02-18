using UnityEngine;
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
        dontTakeAnyWeapon,                      // не подбирай оружие                                                   +
        dontTakeWeaponWithID,                   // не подбирай особое оружие                                            +
        dontTakeDamageModifier,                 // не подбирай модификаторы урона (усилители)                           +
        dontTakeShootingSpeedModifier,          // не подбирай модификаторы скорости стрельбы (ускорители)              +
        dontTakeInjury,                         // не получай повреждений                                               +

        collectLessEqualAmmo,                   // подбери не более N патронов                                          +
        collectLessEqualDamageModifier,         // подбери не более N  модификаторов урона (усилителей)                 +
        collectLessEqualShootingSpeedModifier,  // подбери не более N модификаторов скорости стрельбы (ускорителей)     +

        collectMoreEqualAmmo,                   // подбери не менее N патронов                                          +
        collectMoreEqualDamageModifier,         // подбери не менее N модификаторов урона (усилителей)                  +
        collectMoreEqualShootingSpeedModifier,  // подбери не менее N модификаторов скорости стрельбы (ускорителей)     +

        timeLessEqual,                          // пройди меньше, чем за N секунд                                       +
        timeMoreEqual,                          // проживи не менее N секунд                                            +

        reachedLessEqualDistance,               // закончи игру, преодолев не более N метров                            +
        reachedMoreEqualDistance,               // закончи игру, преодолев не менее N метров                            +

        scoreMoreEqual,                         // набери N очков                                                       +

        adrenalineTimeMoreEqual,                // проведи в режиме адреналина не менее N секунд                        +
        dontEmptyAmmo                           // не опустошай боезапас                                                +
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
                        case SecondaryChallenges.collectLessEqualAmmo:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualAmmo:
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
                        case SecondaryChallenges.collectLessEqualAmmo:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualAmmo:
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
                        case SecondaryChallenges.collectLessEqualDamageModifier:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualDamageModifier:
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
                        case SecondaryChallenges.collectLessEqualDamageModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualDamageModifier:
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
                        case SecondaryChallenges.collectLessEqualShootingSpeedModifier:
                            {
                                instance.earnedLeft = (value <= instance.intValueLeft);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualShootingSpeedModifier:
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
                        case SecondaryChallenges.collectLessEqualShootingSpeedModifier:
                            {
                                instance.earnedRight = (value <= instance.intValueRight);
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualShootingSpeedModifier:
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
                    if (instance.challengeLeft == SecondaryChallenges.adrenalineTimeMoreEqual)
                    {
                        instance.earnedLeft = (value >= (int)Mathf.Floor(instance.floatValueLeft * Application.targetFrameRate));
                    }
                    if (instance.challengeRight == SecondaryChallenges.adrenalineTimeMoreEqual)
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
                        case SecondaryChallenges.timeLessEqual:
                            {
                                instance.earnedLeft = (value <= instance.floatValueLeft);
                                break;
                            }
                        case SecondaryChallenges.timeMoreEqual:
                            {
                                instance.earnedLeft = (value >= instance.floatValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.timeLessEqual:
                            {
                                instance.earnedRight = (value <= instance.floatValueRight);
                                break;
                            }
                        case SecondaryChallenges.timeMoreEqual:
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
                        case SecondaryChallenges.reachedLessEqualDistance:
                            {
                                instance.earnedLeft = (value <= instance.floatValueLeft);
                                break;
                            }
                        case SecondaryChallenges.reachedMoreEqualDistance:
                            {
                                instance.earnedLeft = (value >= instance.floatValueLeft);
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.reachedLessEqualDistance:
                            {
                                instance.earnedRight = (value <= instance.floatValueRight);
                                break;
                            }
                        case SecondaryChallenges.reachedMoreEqualDistance:
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
