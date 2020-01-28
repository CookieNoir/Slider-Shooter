using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Player")]
public class Player : RunningEntity
{
    public GameObject playerBody; // ГО игрока
    public GameObject weapon; // начальная модель оружия игрока

    private bool stabbed = false;
    private bool adrenaline = false;
    private int ticks = 0;
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
    public bool endless = false; // флаг, отвечающий за бессмертие врага: если поднят, то игрок получает только очки, иначе враг получает урон 
    public int healthPoints; // начальное число единиц здоровья врага

    private int currentHealthPoints;
    private bool alive = true;
    //---------------------------------
    [Header("Fighting")] //
    public int startAmmo; // используется только при старте игры для начального числа патронов
    public int maxAmmo; // максимальное число патронов, предусмотренное оружием
    public float shootingCooldown; // задержка перед следующим выстрелом
    public int damage; // базовый урон оружия

    private int ammo; // текущее число патронов в магазине (не может быть больше максимального)
    private float shootingCooldownModifier = 1f; // модификатор задержки
    private float damageModifier = 1f; // модификатор урона

    private float currentShootingCooldown; // таймер, при достижении 0 происходит стрельба
    private int spent = -1; // хранит число сделанных подряд выстрелов
    private bool ready = false;
    //---------------------------------
    [Header("UI")]
    public UiMovement healthBarHandler; // используется для скрытия панели здоровья
    public Image healthBar; // используется для редактирования полоски ХП
    public Image healthBarBack; // движется за основной полоской ХП
    public UiMovement playerInterface;
    public Text ammoText; // используется для изменения текста, отвечающего за число патронов в магазине
    public Text shootingSpeedText; // используется для изменения текста, отвечающего за время на одиночный выстрел
    public Text damageText; // используется для изменения текста, отвечающего за урон от следующего выстрела
    public UiMovement winWindow;
    public UiMovement loseWindow;

    public Color[] colors; // должен иметь 8 элементов, первые 4 - для основной полосы, вторые - для фоновой полосы
    private bool healthChanged = false;
    //---------------------------------
    protected override float ModifiedSpeed(float speed)
    {
        float modifiedSpeed = speed;
        if (stabbed) modifiedSpeed -= 0.005f;
        if (ammo > 0)
        {
            if (!slider)
            {
                modifiedSpeed -= 0.005f;
                ticks++;
            }
            else if (ticks > 0)
            {
                modifiedSpeed += 0.005f; ticks--;
            }
        }
        else
        {
            if (ticks > 0)
            {
                modifiedSpeed += 0.005f; ticks--;
            }
        }
        if (adrenaline) modifiedSpeed += 0.01f;
        return modifiedSpeed;
    }

    public void PlayerPosition(float delta)
    {
        playerBody.transform.position = new Vector3(playerBody.transform.position.x + delta / sizeX * 2 * borders, playerBody.transform.position.y, playerBody.transform.position.z);
        if (playerBody.transform.position.x > borders + offsetX) playerBody.transform.position = new Vector3(borders + offsetX, playerBody.transform.position.y, playerBody.transform.position.z);
        else if (playerBody.transform.position.x < -borders + offsetX) playerBody.transform.position = new Vector3(-borders + offsetX, playerBody.transform.position.y, playerBody.transform.position.z);
    }

    private void Start()
    {
        sizeX = Screen.width;
        currentHealthPoints = healthPoints;
        UpdateHealthBar();
        if (startAmmo > maxAmmo) ammo = maxAmmo;
        else ammo = startAmmo;
        UpdateAmmoText();
        UpdateShootingSpeedText();
        UpdateDamageText();
        if (!endless) healthBarHandler.Translate();
        playerInterface.Translate();
    }

    private void Shooting()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 6f);
        if (!ready)
        {
            currentShootingCooldown = shootingCooldown
                //* shootingCooldownModifier // убрано, чтобы оставить время на разворот
                ;
            ready = true;
        }
        else
        {
            if (currentShootingCooldown > 0) currentShootingCooldown -= Time.deltaTime;
            else
            {
                ammo--;
                UpdateAmmoText();
                spent++;
                UpdateDamageText();
                if (!endless && alive)
                {
                    currentHealthPoints -= (int)(Mathf.Floor(damage * (damageModifier + spent * 0.33333f)));
                    if (currentHealthPoints <= 0)
                    {
                        currentHealthPoints = 0;
                        StartCoroutine(MonsterDying());
                    }
                    UpdateHealthBar();
                }
                currentShootingCooldown = shootingCooldown * shootingCooldownModifier;
            }
        }
    }

    private IEnumerator MonsterDying()
    {
        enemy.GetComponent<Enemy>().Dying();
        alive = false;
        healthBarHandler.Translate();
        playerInterface.Translate();
        yield return new WaitForSeconds(2f);
        //launch win animation
        GameSettings.GameResult(true);
        winWindow.Translate();
        Destroy(this);
    }

    private void Running()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 6f);
        if (ready)
        {
            ready = false;
            spent = -1;
            UpdateDamageText();
        }
        else
        {
            PlayerPosition(Input.mousePosition.x - lastX);
            lastX = Input.mousePosition.x;
        }
    }

    private void Update()
    {
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
                    ready = false;
                    spent = -1;
                    UpdateDamageText();
                }
                playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 6f);
            }
        }
        MakeStep();
        UpdateHealthBarBack();
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

    public void Stabbing()
    {
        stabbed = true;
        StartCoroutine(StabCycle());
    }

    IEnumerator StabCycle()
    {
        yield return new WaitForSeconds(3f);
        stabbed = false;
        adrenaline = true;
        yield return new WaitForSeconds(1.5f);
        adrenaline = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            if (alive)
            {
                Debug.Log("Im dead");
                Destroy(this);
            }
        }
        else if (other.tag == "Half Obstacle")
        {
            if (alive)
            {
                if (stabbed)
                {
                    Debug.Log("Im dead");
                    Destroy(this);
                }
                else
                {
                    Stabbing();
                    other.GetComponent<DynamicObstacle>().Change();
                }
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

    public void ModifyShootingCooldown(float value)
    {
        shootingCooldownModifier *= value;
        UpdateShootingSpeedText();
    }

    public void ModifyDamage(float value)
    {
        damageModifier *= value;
        UpdateDamageText();
    }

    public void FillAmmo(int amount)
    {
        ammo += amount;
        if (ammo > maxAmmo) ammo = maxAmmo;
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        ammoText.text = ammo.ToString() + "\n" + "<color=#eac418ff>" + maxAmmo.ToString() + "</color>";
    }

    private void UpdateDamageText()
    {
        if (spent < 0)
            damageText.text = ((int)(damage * damageModifier)).ToString();
        else
            damageText.text = ((int)(damage*damageModifier)).ToString() + "<color=#ff8636ff> + " + Mathf.Floor(damage * (spent + 1) * 0.33333f).ToString() + "</color>";
    }

    private void UpdateShootingSpeedText()
    {
        shootingSpeedText.text = (shootingCooldown*shootingCooldownModifier).ToString();
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

    public void GiveNewWeapon(GameObject newWeapon, int newMaxAmmo, float newShootingCooldown, int newDamage)
    {
        // сменить модель пушки
        maxAmmo = newMaxAmmo;
        FillAmmo(maxAmmo);
        shootingCooldown = newShootingCooldown;
        UpdateShootingSpeedText();
        damage = newDamage;
        UpdateDamageText();
        // обновить иконку пушки, изменить показатели урона и скорости на интерфейсе
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
