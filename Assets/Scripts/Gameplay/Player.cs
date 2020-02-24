using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Player")]
public class Player : RunningEntity
{
    public GameObject playerBody; // ГО игрока
    public List<WeaponBehaviour> weapons; // модели оружия, индекс = ID, должны быть скрыты перед стартом
    public int startWeapon = 0; // ID начального оружия

    private int currentWeapon = -1;
    private bool gotWeapon = false; // влияет на отображение интерфейса

    private int stabbedTicks = 0;
    private int adrenalineTicks = 0;
    private int shootingTicks = 0;
    private float scoreModifier;
    private IEnumerator stabCycle;
    // блок переменных, которые будут записаны
    private int score = 0; // игровой счет
    private float timeSpent = 0; // длительность игры
    private float millisecondsSpent = 0;
    private int secondsSpent = 0;
    private int minutesSpent = 0;
    private int takenAmmo = 0; // количество патронов, полученных за игру
    private int takenDamageModifiers = 0; // количество модификаторов урона, полученных за игру
    private int takenShootingSpeedModifiers = 0; // количество модификаторов скорости стрельбы, полученных за игру
    private int takenInjuries = 0; // количество ранений, полученных за игру
    private int takenWeapons = 0; // количество подобранных оружий за игру
    private float distance = 0; // пройденная игроком дистанция
    private int adrenalineTicksSpent = 0; // число кадров, проведенных в режима Адреналина
    //---------------------------------
    [Header("Slider")] // блок переменных, отвечающих за перемещение персонажа при помощи механики слайдера
    [Range(0f, 20)]
    public float borders; // Абсолютное значение отклонения по оси x (может изменяться при смене окружения)
    public float offsetX; // смещение центра по оси Х

    private float scaleX; // добавочный радиус обводки по оси X
    private float scaleZ; // радиус обводки по оси Z

    private int sizeX;
    private bool slider = false;
    private float lastX;
    //---------------------------------
    [Header("Enemy")] // блок переменных, связанных с врагом и его параметрами
    public GameObject enemy; // ГО врага
    public bool invulnerable = false; // флаг, отвечающий за бессмертие врага: если поднят, то игрок получает только очки, иначе враг получает урон 
    public int healthPoints; // начальное число единиц здоровья врага

    private int currentHealthPoints;
    private bool alive = true;
    //---------------------------------
    [Header("Fighting")] //
    public int startAmmo; // используется только при старте игры для начального числа патронов
    public float shootingDelay; //  задержка перед первым выстрелом

    private int maxAmmo = 0; // максимальное число патронов, предусмотренное оружием
    private float shootingCooldown; // задержка перед следующим выстрелом
    private int damage; // базовый урон оружия
    private float spentModifier; // дополнительный модификатор урона в зависимости от числа потраченных подряд патронов 

    private int ammo; // текущее число патронов в магазине (не может быть больше максимального)
    private float shootingCooldownModifier = 1f; // модификатор задержки
    private float damageModifier = 1f; // модификатор урона
    private float divDelay;
    private float currentShootingCooldown; // таймер, при достижении 0 происходит стрельба
    private int spent = -1; // хранит число сделанных подряд выстрелов
    private bool ready = false;
    private int reservedDamage;
    private int reservedDamageCap;
    private int ticksCap;
    //---------------------------------
    [Header("UI")]
    public MaskableGraphic hurtIndicator; // изменяет прозрачность элемента, отрисовывающего ранения персонажа
    public UiMovement healthBarHandler; // используется для скрытия панели здоровья
    public Image healthBar; // используется для редактирования полоски ХП
    public Image healthBarBack; // движется за основной полоской ХП
    public UiMovement playerInterface;
    public Text ammoText; // используется для изменения текста, отвечающего за число патронов в магазине
    public Text shootingSpeedText; // используется для изменения текста, отвечающего за время на одиночный выстрел
    public Text damageText; // используется для изменения текста, отвечающего за урон от следующего выстрела
    public Image weaponIcon; // объект, в который помещается иконка оружия
    public Text weaponNameText; // отображает название оружия
    public Text scoreText; // отображает игровой счёт
    public Text timeText; // отображает время, проведенное со старта
    public Color[] colors;
    /*
        0-3 - основная полоса здоровья,
        4-7 - фоновая полоса здоровья,
        8-11 - индикаторы состояния персонажа       
    */
    private IEnumerator colorChanger;
    private bool healthChanged = false;
    //---------------------------------
    protected override float ModifiedSpeed(float speed)
    {
        float modifiedSpeed = speed;
        if (stabbedTicks > 0)
        {
            modifiedSpeed -= 0.008f; stabbedTicks--;
        }
        if (ammo > 0)
        {
            if (!slider)
            {
                modifiedSpeed -= 0.008f;
                shootingTicks++;
            }
            else if (shootingTicks > 0)
            {
                RecalculateReservedDamage();
                if (shootingTicks > 60)
                {
                    modifiedSpeed += 0.024f; shootingTicks -= 3;
                }
                else if (shootingTicks > 30)
                {
                    modifiedSpeed += 0.016f; shootingTicks -= 2;
                }
                else
                {
                    modifiedSpeed += 0.008f; shootingTicks--;
                }
            }
        }
        else
        {
            if (shootingTicks > 0)
            {
                RecalculateReservedDamage();
                if (shootingTicks > 60)
                {
                    modifiedSpeed += 0.024f; shootingTicks -= 3;
                }
                else if (shootingTicks > 30)
                {
                    modifiedSpeed += 0.016f; shootingTicks -= 2;
                }
                else
                {
                    modifiedSpeed += 0.008f; shootingTicks--;
                }
            }
        }
        if (adrenalineTicks > 0)
        {
            modifiedSpeed += 0.016f;
            adrenalineTicks--;
            adrenalineTicksSpent++;
            GameChallenges.HandleEvent(GameChallenges.EventTypes.changedAdrenalineTime, adrenalineTicksSpent);
        }
        return modifiedSpeed;
    }

    public void PlayerPosition(float delta)
    {
        playerBody.transform.position = new Vector3(playerBody.transform.position.x + delta / sizeX *
            (1 + 60 * GameSettings.speed) // Модификатор скорости слайдинга
            * borders, playerBody.transform.position.y, playerBody.transform.position.z);
        if (playerBody.transform.position.x > borders + offsetX) playerBody.transform.position = new Vector3(borders + offsetX, playerBody.transform.position.y, playerBody.transform.position.z);
        else if (playerBody.transform.position.x < -borders + offsetX) playerBody.transform.position = new Vector3(-borders + offsetX, playerBody.transform.position.y, playerBody.transform.position.z);
    }

    private void Start()
    {
        sizeX = Screen.width;
        divDelay = 5f / shootingDelay;
        stabCycle = StabCycle();
        colorChanger = UIHelper.ColorChanger(hurtIndicator, colors[8]);

        GiveNewWeapon(startWeapon, startAmmo);

        scoreText.text = score.ToString();
        if (!invulnerable)
        {
            currentHealthPoints = healthPoints;
            UpdateHealthBar();
            healthBarHandler.Translate();
        }
    }

    private void Shooting()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * divDelay);
        if (!ready)
        {
            if (currentShootingCooldown < shootingDelay)
                currentShootingCooldown = shootingDelay;
            ready = true;
        }
        else
        {
            if (currentShootingCooldown <= 0)
            {
                ammo--;
                if (ammo == 0)
                {
                    GameChallenges.HandleEvent(GameChallenges.EventTypes.emptyAmmo);
                }
                UpdateAmmoText();
                spent++;
                UpdateDamageText();
                {
                    int currentDamage = (int)(Mathf.Floor(damage * damageModifier)) + (int)(Mathf.Floor(damage * damageModifier * spent * spentModifier)) + reservedDamage;
                    UpdateScore(currentDamage);
                    if (!invulnerable)
                    {
                        currentHealthPoints -= currentDamage;
                        if (currentHealthPoints <= 0)
                        {
                            currentHealthPoints = 0;
                            if (alive) StartCoroutine(MonsterDying());
                        }
                        UpdateHealthBar();
                    }
                    if (currentWeapon != -1) weapons[currentWeapon].Fire();
                }
                currentShootingCooldown = shootingCooldown * shootingCooldownModifier;
            }
        }
    }

    private IEnumerator MonsterDying()
    {
        enemy.GetComponent<Enemy>().Dying();
        alive = false;
        if (!invulnerable) healthBarHandler.Translate();
        if (gotWeapon) playerInterface.Translate();
        GameSettings.ChangeHurtIndicator(hurtIndicator, colors[8]);
        yield return new WaitForSeconds(2f);
        //launch win animation
        GameChallenges.HandleEvent(GameChallenges.EventTypes.enemyKilled);
        Destroy(enemy);
        Destroy(this);
    }

    private void GameOver()
    {
        enemy.GetComponent<Enemy>().Lost();
        if (!invulnerable) healthBarHandler.Translate();
        if (gotWeapon) playerInterface.Translate();
        GameSettings.ChangeHurtIndicator(hurtIndicator, colors[8]);
        //launch win animation
        Destroy(enemy);
        Destroy(this);
    }

    private void Running()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * divDelay);
        if (ready)
        {
            SetNotReady();
        }
        else
        {
            PlayerPosition(Input.mousePosition.x - lastX);
            lastX = Input.mousePosition.x;
        }
    }

    private void SetNotReady()
    {
        ready = false;
        if (reservedDamage > 0)
            reservedDamageCap = reservedDamage + (int)Mathf.Floor(damage * damageModifier * (spent + 1) * spentModifier);
        else
            reservedDamageCap = (int)Mathf.Floor(damage * damageModifier * (spent + 1) * spentModifier);
        ticksCap = shootingTicks;
        spent = -1;
        scoreModifier = 1f;
    }

    private void RecalculateReservedDamage()
    {
        reservedDamage = (int)Mathf.Floor(((float)shootingTicks / ticksCap) * reservedDamageCap);
        UpdateDamageText();
    }

    private void Update()
    {
        if (GameSettings.gameOver) GameOver();
        if (currentShootingCooldown > 0) currentShootingCooldown -= Time.deltaTime;
        if (slider)
        {
            Running();
        }
        else
        {
            if (ammo > 0)
            {
                Shooting();
            }
            else
            {
                if (ready)
                {
                    SetNotReady();
                }
                playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * divDelay);
            }
        }
        MakeStep();
        UpdateHealthBarBack();
        if (alive) UpdateTimer();
    }

    protected override void MakeStep()
    {
        currentSpeed = ModifiedSpeed(GameSettings.speed);
        transform.position += new Vector3(0, 0, currentSpeed);
        distance += currentSpeed;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.changedDistance, distance);
    }

    public void MouseDown()
    {
        slider = true;
        lastX = Input.mousePosition.x;
    }

    public void MouseUp()
    {
        slider = false;
    }

    private void UpdateTimer()
    {
        timeSpent += Time.deltaTime;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.changedTime, timeSpent);
        millisecondsSpent += Time.deltaTime;
        if (millisecondsSpent >= 1)
        {
            secondsSpent++;
            millisecondsSpent--;
        }
        if (secondsSpent > 60)
        {
            minutesSpent++;
            secondsSpent -= 60;
        }
        if (minutesSpent == 0)
            timeText.text = secondsSpent.ToString() + millisecondsSpent.ToString(".##");
        else
            timeText.text = minutesSpent.ToString() + ':' + secondsSpent.ToString("D2") + millisecondsSpent.ToString(".##");
    }

    public void Stabbing()
    {
        stabbedTicks = Application.targetFrameRate / 2 * 6;
        StopCoroutine(stabCycle);
        stabCycle = StabCycle();
        StartCoroutine(stabCycle);
        takenInjuries++;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.tookInjury, takenInjuries);
    }

    private IEnumerator StabCycle()
    {
        StopCoroutine(colorChanger);
        colorChanger = UIHelper.ColorChanger(hurtIndicator, colors[9]);
        StartCoroutine(colorChanger);
        while (stabbedTicks > 0)
            yield return null;
        adrenalineTicks = Application.targetFrameRate / 2 * 3;
        StopCoroutine(colorChanger);
        colorChanger = UIHelper.ColorChanger(hurtIndicator, colors[10]);
        StartCoroutine(colorChanger);
        while (adrenalineTicks > 0)
            yield return null;

        StopCoroutine(colorChanger);
        colorChanger = UIHelper.ColorChanger(hurtIndicator, colors[8]);
        StartCoroutine(colorChanger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            Debug.Log("Im dead");
            Dying();
        }
        else if (other.tag == "Half Obstacle")
        {
            if (stabbedTicks > 0)
            {
                Debug.Log("Im dead");
                Dying();
            }
            else
            {
                Stabbing();
                other.GetComponent<DynamicObstacle>().Change();
            }
        }
        else if (other.tag == "Supply")
        {
            other.GetComponent<SupplyBehaviour>().GiveSuppliesTo(this);
        }
        else if (other.tag == "Border Changer")
        {
            StartCoroutine(other.GetComponent<BorderChanger>().ChangeBordersOf(this));
        }
    }

    public void Dying()
    {
        Destroy(GetComponent<Rigidbody>());
        if (alive)
        {
            if (!invulnerable) healthBarHandler.Translate();
            if (gotWeapon) playerInterface.Translate();
            StopCoroutine(colorChanger);
            GameSettings.ChangeHurtIndicator(hurtIndicator, colors[11], colors[8]);
        }
        else
        {
            GameSettings.ChangeHurtIndicator(hurtIndicator, colors[8]);
            GameChallenges.HandleEvent(GameChallenges.EventTypes.enemyKilled);
            Destroy(enemy);
        }
        // dying animation
        Destroy(this);
    }

    public void ModifyShootingCooldown(float value)
    {
        shootingCooldownModifier *= value;
        UpdateShootingSpeedText();
        takenShootingSpeedModifiers++;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.tookShootingSpeedModifier, takenShootingSpeedModifiers);
    }

    public void ModifyDamage(float value)
    {
        damageModifier *= value;
        reservedDamage = (int)Mathf.Floor(reservedDamage * value);
        UpdateDamageText();
        takenDamageModifiers++;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.tookShootingSpeedModifier, takenDamageModifiers);
    }

    public void FillAmmo(int amount)
    {
        ammo += amount;
        int difference = ammo - maxAmmo;
        if (difference > 0)
            ammo = maxAmmo;
        else
            difference = 0;
        UpdateAmmoText();
        takenAmmo+=amount - difference;
        GameChallenges.HandleEvent(GameChallenges.EventTypes.tookAmmo, takenAmmo);
    }

    public void FillAmmo(float part)
    {
        int amount = (int)(Mathf.Floor(maxAmmo * part));
        FillAmmo(amount);
    }

    private void UpdateAmmoText()
    {
        ammoText.text = ammo.ToString() + "\n" + "<color=#eac418ff>" + maxAmmo.ToString() + "</color>";
    }

    private void UpdateDamageText()
    {
        if (spent < 0)
        {
            if (shootingTicks <= 1 || reservedDamage <= 0) damageText.text = ((int)(Mathf.Floor(damage * damageModifier))).ToString();
            else damageText.text = ((int)(Mathf.Floor(damage * damageModifier))).ToString() + "<color=#ff8636ff> + " + reservedDamage.ToString() + "</color>";
        }
        else
            damageText.text = ((int)(Mathf.Floor(damage * damageModifier))).ToString() + "<color=#ff8636ff> + " + ((int)(Mathf.Floor(damage * damageModifier * (spent + 1) * spentModifier)) + reservedDamage).ToString() + "</color>";
    }

    private void UpdateShootingSpeedText()
    {
        shootingSpeedText.text = (shootingCooldown * shootingCooldownModifier).ToString();
    }

    private void UpdateScore(int value)
    {
        score += (int)Mathf.Floor(value * scoreModifier);
        scoreModifier *= 1 + spentModifier;
        scoreText.text = score.ToString();
        GameChallenges.HandleEvent(GameChallenges.EventTypes.changedScore, score);
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(currentHealthPoints * 1f / healthPoints, 1f, 1f);
        if (healthBar.transform.localScale.x > 0.85f)
        {
            healthBar.color = colors[0];
            healthBarBack.color = colors[4];
        }
        else if (healthBar.transform.localScale.x > 0.75f)
        {
            healthBar.color = Vector4.Lerp(colors[1], colors[0], (healthBar.transform.localScale.x - 0.75f) / 0.1f);
            healthBarBack.color = Vector4.Lerp(colors[5], colors[4], (healthBar.transform.localScale.x - 0.75f) / 0.1f);
        }
        else if (healthBar.transform.localScale.x > 0.6f)
        {
            healthBar.color = colors[1];
            healthBarBack.color = colors[5];
        }
        else if (healthBar.transform.localScale.x > 0.5f)
        {
            healthBar.color = Vector4.Lerp(colors[2], colors[1], (healthBar.transform.localScale.x - 0.5f) / 0.1f);
            healthBarBack.color = Vector4.Lerp(colors[6], colors[5], (healthBar.transform.localScale.x - 0.5f) / 0.1f);
        }
        else if (healthBar.transform.localScale.x > 0.3f)
        {
            healthBar.color = colors[2];
            healthBarBack.color = colors[6];
        }
        else if (healthBar.transform.localScale.x > 0.2f)
        {
            healthBar.color = Vector4.Lerp(colors[3], colors[2], (healthBar.transform.localScale.x - 0.2f) / 0.1f);
            healthBarBack.color = Vector4.Lerp(colors[7], colors[6], (healthBar.transform.localScale.x - 0.2f) / 0.1f);
        }
        else
        {
            healthBar.color = colors[3];
            healthBarBack.color = colors[7];
        }
        healthChanged = true;
    }

    private void UpdateHealthBarBack()
    {
        if (healthChanged)
        {
            healthBarBack.transform.localScale -= new Vector3(0.001f, 0, 0);
            if (healthBarBack.transform.localScale.x - healthBar.transform.localScale.x <= 0)
            {
                healthBarBack.transform.localScale = healthBar.transform.localScale;
                healthChanged = false;
            }
            else
            {
                if (healthBarBack.transform.localScale.x - healthBar.transform.localScale.x > 0.3)
                {
                    healthBarBack.transform.localScale -= new Vector3(0.005f, 0, 0);
                }
                else if (healthBarBack.transform.localScale.x - healthBar.transform.localScale.x > 0.2)
                {
                    healthBarBack.transform.localScale -= new Vector3(0.003f, 0, 0);
                }
                else if (healthBarBack.transform.localScale.x - healthBar.transform.localScale.x > 0.1)
                {
                    healthBarBack.transform.localScale -= new Vector3(0.001f, 0, 0);
                }
            }
        }
    }

    public void GiveNewWeapon(int newWeapon)
    {
        if (newWeapon != currentWeapon)
        {
            if (currentWeapon != -1)
            {
                weapons[currentWeapon].gameObject.SetActive(false);
            }
            if (newWeapon != -1)
            {
                currentWeapon = newWeapon;
                weapons[currentWeapon].gameObject.SetActive(true);
                weapons[currentWeapon].GetProperties(weaponIcon, weaponNameText, ref maxAmmo, ref shootingCooldown, ref damage, ref spentModifier);
                ammo = 0;
                FillAmmo(weapons[currentWeapon].ammo);
                UpdateShootingSpeedText();
                spent = -1;
                scoreModifier = 1f;
                reservedDamage = 0;
                UpdateDamageText();
                if (!gotWeapon)
                {
                    playerInterface.Translate();
                    gotWeapon = true;
                }
                //изменить анимацию под данную пушку
                takenWeapons++;
                GameChallenges.HandleEvent(GameChallenges.EventTypes.tookWeapon, newWeapon);
            }
        }
        else
        {
            FillAmmo(maxAmmo/2);
        }
    }

    public void GiveNewWeapon(int newWeapon, int amount)
    {
        if (newWeapon != currentWeapon)
        {
            if (currentWeapon != -1)
            {
                weapons[currentWeapon].gameObject.SetActive(false);
            }
            if (newWeapon != -1)
            {
                currentWeapon = newWeapon;
                weapons[currentWeapon].gameObject.SetActive(true);
                weapons[currentWeapon].GetProperties(weaponIcon, weaponNameText, ref maxAmmo, ref shootingCooldown, ref damage, ref spentModifier);
                ammo = 0;
                FillAmmo(amount);
                UpdateShootingSpeedText();
                spent = -1;
                scoreModifier = 1f;
                reservedDamage = 0;
                UpdateDamageText();
                if (!gotWeapon)
                {
                    playerInterface.Translate();
                    gotWeapon = true;
                }
                //изменить анимацию под данную пушку
                takenWeapons++;
                GameChallenges.HandleEvent(GameChallenges.EventTypes.tookWeapon, newWeapon);
            }
        }
        else
        {
            FillAmmo(amount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        scaleX = transform.localScale.x / 2;
        scaleZ = transform.localScale.z / 2;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-(borders + scaleX) + offsetX, transform.position.y, -scaleZ + transform.position.z), new Vector3(-(borders + scaleX) + offsetX, transform.position.y, scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(-(borders + scaleX) + offsetX, transform.position.y, scaleZ + transform.position.z), new Vector3(borders + scaleX + offsetX, transform.position.y, scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(borders + scaleX + offsetX, transform.position.y, scaleZ + transform.position.z), new Vector3(borders + scaleX + offsetX, transform.position.y, -scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(borders + scaleX + offsetX, transform.position.y, -scaleZ + transform.position.z), new Vector3(-(borders + scaleX) + offsetX, transform.position.y, -scaleZ + transform.position.z));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(new Vector3(-borders + offsetX, transform.position.y, -scaleZ + transform.position.z), new Vector3(-borders + offsetX, +transform.position.y, scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(-borders + offsetX, +transform.position.y, scaleZ + transform.position.z), new Vector3(borders + offsetX, transform.position.y, scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(borders + offsetX, transform.position.y, scaleZ + transform.position.z), new Vector3(borders + offsetX, +transform.position.y, -scaleZ + transform.position.z));
        Gizmos.DrawLine(new Vector3(borders + offsetX, +transform.position.y, -scaleZ + transform.position.z), new Vector3(-borders + offsetX, transform.position.y, -scaleZ + transform.position.z));
    }

    private void OnValidate()
    {
        GameSettings.defaultBorders = borders;
        GameSettings.defaultOffsetX = offsetX;
    }
}
