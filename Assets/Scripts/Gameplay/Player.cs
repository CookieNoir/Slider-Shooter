using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Gameplay/Player")]
public class Player : RunningEntity
{
    public GameObject playerBody; // ГО игрока
    public GameObject weapon; // начальная модель оружия игрока
    //---------------------------------
    [Header("Slider")] // блок переменных, отвечающих за перемещение персонажа при помощи механики слайдера
    [Range(0f, 20)]
    public float borders; // Абсолютное значение отклонения по оси x (может изменяться при смене окружения)

    private int sizeX;
    private bool slider = false;
    private float lastX;
    //---------------------------------
    [Header("Enemy")] // блок переменных, связанных с врагом и его параметрами
    public GameObject enemy; // ГО врага
    public bool endless = false; // флаг, отвечающий за бессмертие врага: если поднят, то игрок получает только очки, иначе враг получает урон 
    public int healthPoints; // начальное число единиц здоровья врага

    private int currentHealthPoints;
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
    private int spent = 0; // хранит число сделанных подряд выстрелов
    private bool ready = false;
    //---------------------------------
    [Header("UI")]
    public RectTransform healthBar; // используется для редактирования полоски ХП
    public Text ammoText; // используется для изменения текста, отвечающего за число патронов в магазине

    private bool stabbed = false;
    private bool adrenaline = false;
    private int ticks = 0;
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
        if (playerBody.transform.position.x > borders) playerBody.transform.position = new Vector3(borders, playerBody.transform.position.y, playerBody.transform.position.z);
        else if (playerBody.transform.position.x < -borders) playerBody.transform.position = new Vector3(-borders, playerBody.transform.position.y, playerBody.transform.position.z);
    }

    private void Start()
    {
        sizeX = Screen.width;
        currentHealthPoints = healthPoints;
        if (startAmmo > maxAmmo) ammo = maxAmmo;
        else ammo = startAmmo;
        UpdateAmmoText();
    }

    private void Shooting()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 6f);
        if (!ready)
        {
            currentShootingCooldown = shootingCooldown
                //* shootingCooldownModifier // убрано, чтобы оставить время на разворот
                ;
            spent = -1;
            ready = true;
        }
        else
        {
            if (currentShootingCooldown > 0) currentShootingCooldown -= Time.deltaTime;
            else
            {
                ammo--;
                spent++;
                UpdateAmmoText();
                if (!endless)
                {
                    currentHealthPoints -= (int)(Mathf.Floor(damage * (damageModifier + spent * 0.33333f)));
                    if (currentHealthPoints <= 0)
                    {
                        currentHealthPoints = 0;
                        StartCoroutine(MonsterDyingAnimation());
                        enemy.GetComponent<Enemy>().Dying();
                    }
                    UpdateHealthBar();
                }
                currentShootingCooldown = shootingCooldown * shootingCooldownModifier;
            }
        }
    }

    private IEnumerator MonsterDyingAnimation()
    {
        yield return new WaitForSeconds(2f);
        //launch win animation
        GameSettings.GameResult(true);
        Destroy(this);
    }

    private void UpdateAmmoText()
    {
        ammoText.text = ammo.ToString() + '/' + maxAmmo.ToString();
    }

    private void UpdateHealthBar()
    {
        healthBar.localScale = new Vector3(currentHealthPoints * 1f / healthPoints, 1f, 1f);
    }

    private void Running()
    {
        playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 6f);
        if (ready)
        {
            ready = false;
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
                playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 6f);
        }
        MakeStep();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            Debug.Log("Im dead");
            Destroy(this);
        }
        else if (collision.collider.tag == "Half Obstacle")
        {
            if (stabbed)
            {
                Debug.Log("Im dead");
                Destroy(this);
            }
            else
            {
                Stabbing();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Half Obstacle")
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
        else if (other.tag == "Supply")
        {
            other.GetComponent<SupplyBehaviour>().GiveSuppliesTo(this);
        }
    }

    public void ModifyShootingCooldown(float value)
    {
        shootingCooldownModifier *= value;
    }

    public void ModifyDamage(float value)
    {
        damageModifier *= value;
    }

    public void FillAmmo(int amount)
    {
        ammo += amount;
        if (ammo > maxAmmo) ammo = maxAmmo;
        UpdateAmmoText();
    }

    public void GiveNewWeapon(GameObject newWeapon, int newMaxAmmo, float newShootingCooldown, int newDamage)
    {
        // сменить модель пушки
        maxAmmo = newMaxAmmo;
        FillAmmo(maxAmmo);
        shootingCooldown = newShootingCooldown;
        damage = newDamage;
        // обновить иконку пушки, изменить показатели урона и скорости на интерфейсе
    }
}
