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
                                if (value > 0)
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualAmmo: {
                                if (value <= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualAmmo: {
                                if (value >= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeAmmo:
                            {
                                if (value > 0)
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualAmmo:
                            {
                                if (value <= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualAmmo:
                            {
                                if (value >= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
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
                                if (value == instance.intValueLeft)
                                {
                                    instance.earnedLeft = false;
                                }
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
                                if (value == instance.intValueRight)
                                {
                                    instance.earnedRight = false;
                                }
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
                                if (value > 0)
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualDamageModifier:
                            {
                                if (value <= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualDamageModifier:
                            {
                                if (value >= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeDamageModifier:
                            {
                                if (value > 0)
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualDamageModifier:
                            {
                                if (value <= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualDamageModifier:
                            {
                                if (value >= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
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
                                if (value > 0)
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualShootingSpeedModifier:
                            {
                                if (value <= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualShootingSpeedModifier:
                            {
                                if (value >= instance.intValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.dontTakeShootingSpeedModifier:
                            {
                                if (value > 0)
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectLessEqualShootingSpeedModifier:
                            {
                                if (value <= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.collectMoreEqualShootingSpeedModifier:
                            {
                                if (value >= instance.intValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
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
                        if (value >= instance.intValueLeft)
                        {
                            instance.earnedLeft = true;
                        }
                        else
                        {
                            instance.earnedLeft = false;
                        }
                    }
                    if (instance.challengeRight == SecondaryChallenges.scoreMoreEqual)
                    {
                        if (value >= instance.intValueRight)
                        {
                            instance.earnedRight = true;
                        }
                        else
                        {
                            instance.earnedRight = false;
                        }
                    }
                    break;
                }
            case EventTypes.changedAdrenalineTime:
                {
                    if (instance.challengeLeft == SecondaryChallenges.adrenalineTimeMoreEqual)
                    {
                        if (value >= (int)Mathf.Floor(instance.floatValueLeft * Application.targetFrameRate))
                        {
                            instance.earnedLeft = true;
                        }
                        else
                        {
                            instance.earnedLeft = false;
                        }
                    }
                    if (instance.challengeRight == SecondaryChallenges.adrenalineTimeMoreEqual)
                    {
                        if (value >= (int)Mathf.Floor(instance.floatValueRight * Application.targetFrameRate))
                        {
                            instance.earnedRight = true;
                        }
                        else
                        {
                            instance.earnedRight = false;
                        }
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
                                if (value <= instance.floatValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.timeMoreEqual:
                            {
                                if (value >= instance.floatValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.timeLessEqual:
                            {
                                if (value <= instance.floatValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.timeMoreEqual:
                            {
                                if (value >= instance.floatValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
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
                                if (value <= instance.floatValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.reachedMoreEqualDistance:
                            {
                                if (value >= instance.floatValueLeft)
                                {
                                    instance.earnedLeft = true;
                                }
                                else
                                {
                                    instance.earnedLeft = false;
                                }
                                break;
                            }
                    }
                    switch (instance.challengeRight)
                    {
                        case SecondaryChallenges.reachedLessEqualDistance:
                            {
                                if (value <= instance.floatValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
                                break;
                            }
                        case SecondaryChallenges.reachedMoreEqualDistance:
                            {
                                if (value >= instance.floatValueRight)
                                {
                                    instance.earnedRight = true;
                                }
                                else
                                {
                                    instance.earnedRight = false;
                                }
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
