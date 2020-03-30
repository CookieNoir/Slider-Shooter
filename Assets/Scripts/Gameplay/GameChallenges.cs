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
    [Header("Left Secondary Challenge")]
    public SecondaryChallenges challengeLeft;
    public TranslatableSentence challengeLeftText;
    public float floatValueLeft;
    public int intValueLeft;
    public MaskableGraphic inGameStarIconLeft;
    public MaskableGraphic endGameStarIconLeft;
    //---------------------------------
    [Header("Primary Challenge")]
    public PrimaryChallenges challengeMiddle;
    public float floatValueMiddle;
    public MaskableGraphic endGameStarIconMiddle;
    //---------------------------------
    [Header("Right Secondary Challenge")]
    public SecondaryChallenges challengeRight;
    public TranslatableSentence challengeRightText;
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
                        instance.earnedLeft = (value >= instance.floatValueLeft);
                    }
                    if (instance.challengeRight == SecondaryChallenges.beInAdrenalineModeMoreEqual)
                    {
                        instance.earnedRight = (value >= instance.floatValueRight);
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

    private void OnValidate()
    {
        if (challengeLeftText)
        {
            switch (challengeLeft)
            {
                case SecondaryChallenges.dontTakeAmmo:
                    {
                        challengeLeftText.key = "DontTakeAmmo";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeAmmo";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontEmptyAmmo:
                    {
                        challengeLeftText.key = "DontEmptyAmmo";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontEmptyAmmo";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeAnyWeapon:
                    {
                        challengeLeftText.key = "DontTakeAnyWeapon";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeAnyWeapon";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeWeaponWithID:
                    {
                        challengeLeftText.key = "DontTakeWeaponWithId*";
                        Player player = FindObjectOfType<Player>();
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeWeaponWithId* @" + player.weapons[intValueLeft].weaponName;
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = '@' + player.weapons[intValueLeft].weaponName;
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords('@' + player.weapons[intValueLeft].weaponName));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeDamageModifier:
                    {
                        challengeLeftText.key = "DontTakeDamageModifier";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeDamageModifier";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeShootingSpeedModifier:
                    {
                        challengeLeftText.key = "DontTakeShootingSpeedModifier";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeShootingSpeedModifier";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeInjury:
                    {
                        challengeLeftText.key = "DontTakeInjury";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#DontTakeInjury";
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualAmmo:
                    {
                        challengeLeftText.key = "TakeLessEqualAmmo*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeLessEqualAmmo* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualDamageModifier:
                    {
                        challengeLeftText.key = "TakeLessEqualDamageModifier*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeLessEqualDamageModifier* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualShootingSpeedModifier:
                    {
                        challengeLeftText.key = "TakeLessEqualShootingSpeedModifier*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeLessEqualShootingSpeedModifier* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualAmmo:
                    {
                        challengeLeftText.key = "TakeMoreEqualAmmo*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualAmmo* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualDamageModifier:
                    {
                        challengeLeftText.key = "TakeMoreEqualDamageModifier*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualDamageModifier* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
                    {
                        challengeLeftText.key = "TakeMoreEqualShootingSpeedModifier*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualShootingSpeedModifier* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.endWithTimeLessEqual:
                    {
                        challengeLeftText.key = "EndWithTimeLessEqual*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#EndWithTimeLessEqual* " + floatValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = floatValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueLeft.ToString()));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.endWithTimeMoreEqual:
                    {
                        challengeLeftText.key = "EndWithTimeMoreEqual*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#EndWithTimeMoreEqual* " + floatValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = floatValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.reachLessEqualDistance:
                    {
                        challengeLeftText.key = "ReachLessEqualDistance*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#ReachLessEqualDistance* " + floatValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = floatValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueLeft.ToString()));
                        earnedLeft = true;
                        break;
                    }
                case SecondaryChallenges.reachMoreEqualDistance:
                    {
                        challengeLeftText.key = "ReachMoreEqualDistance*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#ReachMoreEqualDistance* " + floatValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = floatValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.scoreMoreEqual:
                    {
                        challengeLeftText.key = "ScoreMoreEqual*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#ScoreMoreEqual* " + intValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = intValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
                case SecondaryChallenges.beInAdrenalineModeMoreEqual:
                    {
                        challengeLeftText.key = "BeInAdrenalineModeMoreEqual*";
                        challengeLeftText.gameObject.GetComponent<Text>().text = "#BeInAdrenalineModeMoreEqual* " + floatValueLeft.ToString();
                        if (challengeLeftText.dynamicWords.Count > 0)
                            challengeLeftText.dynamicWords[0].value = floatValueLeft.ToString();
                        else
                            challengeLeftText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueLeft.ToString()));
                        earnedLeft = false;
                        break;
                    }
            }
        }
        if (challengeRightText)
        {
            switch (challengeRight)
            {
                case SecondaryChallenges.dontTakeAmmo:
                    {
                        challengeRightText.key = "DontTakeAmmo";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeAmmo";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontEmptyAmmo:
                    {
                        challengeRightText.key = "DontEmptyAmmo";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontEmptyAmmo";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeAnyWeapon:
                    {
                        challengeRightText.key = "DontTakeAnyWeapon";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeAnyWeapon";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeWeaponWithID:
                    {
                        challengeRightText.key = "DontTakeWeaponWithId*";
                        Player player = FindObjectOfType<Player>();
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeWeaponWithId* @" + player.weapons[intValueRight].weaponName;
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = '@' + player.weapons[intValueRight].weaponName;
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords('@' + player.weapons[intValueRight].weaponName));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeDamageModifier:
                    {
                        challengeRightText.key = "DontTakeDamageModifier";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeDamageModifier";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeShootingSpeedModifier:
                    {
                        challengeRightText.key = "DontTakeShootingSpeedModifier";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeShootingSpeedModifier";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.dontTakeInjury:
                    {
                        challengeRightText.key = "DontTakeInjury";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#DontTakeInjury";
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualAmmo:
                    {
                        challengeRightText.key = "TakeLessEqualAmmo*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeLessEqualAmmo* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualDamageModifier:
                    {
                        challengeRightText.key = "TakeLessEqualDamageModifier*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeLessEqualDamageModifier* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.takeLessEqualShootingSpeedModifier:
                    {
                        challengeRightText.key = "TakeLessEqualShootingSpeedModifier*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeLessEqualShootingSpeedModifier* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualAmmo:
                    {
                        challengeRightText.key = "TakeMoreEqualAmmo*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualAmmo* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualDamageModifier:
                    {
                        challengeRightText.key = "TakeMoreEqualDamageModifier*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualDamageModifier* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.takeMoreEqualShootingSpeedModifier:
                    {
                        challengeRightText.key = "TakeMoreEqualShootingSpeedModifier*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#TakeMoreEqualShootingSpeedModifier* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.endWithTimeLessEqual:
                    {
                        challengeRightText.key = "EndWithTimeLessEqual*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#EndWithTimeLessEqual* " + floatValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = floatValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueRight.ToString()));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.endWithTimeMoreEqual:
                    {
                        challengeRightText.key = "EndWithTimeMoreEqual*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#EndWithTimeMoreEqual* " + floatValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = floatValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.reachLessEqualDistance:
                    {
                        challengeRightText.key = "ReachLessEqualDistance*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#ReachLessEqualDistance* " + floatValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = floatValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueRight.ToString()));
                        earnedRight = true;
                        break;
                    }
                case SecondaryChallenges.reachMoreEqualDistance:
                    {
                        challengeRightText.key = "ReachMoreEqualDistance*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#ReachMoreEqualDistance* " + floatValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = floatValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.scoreMoreEqual:
                    {
                        challengeRightText.key = "ScoreMoreEqual*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#ScoreMoreEqual* " + intValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = intValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(intValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
                case SecondaryChallenges.beInAdrenalineModeMoreEqual:
                    {
                        challengeRightText.key = "BeInAdrenalineModeMoreEqual*";
                        challengeRightText.gameObject.GetComponent<Text>().text = "#BeInAdrenalineModeMoreEqual* " + floatValueRight.ToString();
                        if (challengeRightText.dynamicWords.Count > 0)
                            challengeRightText.dynamicWords[0].value = floatValueRight.ToString();
                        else
                            challengeRightText.dynamicWords.Add(new TranslatableSentence.DynamicWords(floatValueRight.ToString()));
                        earnedRight = false;
                        break;
                    }
            }
        }
        UpdateStars();
    }

    private void UpdateStars()
    {
        if (earnedLeft) inGameStarIconLeft.color = colorEnabled;
        else inGameStarIconLeft.color = colorDisabled;
        if (earnedRight) inGameStarIconRight.color = colorEnabled;
        else inGameStarIconRight.color = colorDisabled;
    }
}
