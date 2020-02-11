using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Gameplay/Game Challenges")]
public class GameChallenges : MonoBehaviour
{
    public enum PrimaryChallenges
    {
        killEnemy,
        surviveForTime,
        reachDistance
    };

    public enum SecondaryChallenges
    {
        dontTake_Ammo, // не подбирай патроны
        dontTake_AnyWeapon, // не подбирай оружие
        dontTake_WeaponWithID, // не подбирай особое оружие
        dontTake_DamageModifier, // не подбирай модификаторы урона (усилители)
        dontTake_ShootingSpeedModifier, // не подбирай модификаторы скорости стрельбы (ускорители)
        dontTake_Injury, // не получай повреждений

        collect_LessEqual_Ammo, // подбери не более N патронов
        collect_LessEqual_DamageModifier, // подбери не более N  модификаторов урона (усилителей)
        collect_LessEqual_ShootingSpeedModifier, // подбери не более N модификаторов скорости стрельбы (ускорителей)

        collect_MoreEqual_Ammo, // подбери не менее N патронов
        collect_MoreEqual_DamageModifier, // подбери не менее N модификаторов урона (усилителей)
        collect_MoreEqual_ShootingSpeedModifier, // подбери не менее N модификаторов скорости стрельбы (ускорителей)

        time_LessEqual, // пройди меньше, чем за N секунд
        time_MoreEqual, // проживи не менее N секунд

        score_MoreEqual, // набери N очков

        adrenalineTime_MoreEqual, // проведи в режиме адреналина не менее N секунд
        dontEmptyAmmo // не опустошай боезапас
    };

    public enum EventTypes {
        // инкрементные события
        took_Ammo,
        took_Weapon,
        tookDamage_Modifier,
        took_ShootingSpeedModifier,
        took_Injury,
        // сравнительные события
        changed_Time,
        changed_Score,
        changed_AdrenalineTime,
        changed_Distance,
        // констатирующие события
        emptyAmmo,
        enemyKilled
    };


    public SecondaryChallenges challengeLeft;
    public SecondaryChallenges challengeRight;

    public float floatValueLeft;
    public float floatValueRight;
    public int intValueLeft;
    public int intValueRight;

    public bool earnedPrimary;
    public bool earnedLeft;
    public bool earnedRight;
}
