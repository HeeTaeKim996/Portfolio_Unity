// #PlayerHealth


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    [HideInInspector]
    public PlayerMovement playerMovement;
    [HideInInspector]
    public PlayerController playerController;
    private Animator playerAnimator;
    private PlayerAttack playerAttack;
    [HideInInspector]
    public PlayerStatusManager playerStatusManager;
    private GameOverManager gameOverManager;

    public Slider healthSlider;
    public Slider easeHealthSlider;

    public Slider staminaSlider;
    public Slider easeStaminaSlider;

    
    private bool isFirstSetUp = true;


    [HideInInspector]
    public float stamina;
    [HideInInspector]
    public float maxStamina;
    private float staminaRegen;

    [HideInInspector]
    public float dodgeStamina;
    [HideInInspector]
    public float runningStamina;

    private Coroutine staminaCoroutine;
    private float regenCoolDownTime = 1f;

    private float damageRate;
    private float defenseRate;

    private float correctionFactor
    {
        get
        {
            if (playerMovement.isShielding)
            {
                return 0.15f;
            }
            else if (playerMovement.isHeavyCharging)
            {
                return 0f;
            }
            else
            {
                return 1f;
            }
        }
    }

    private int staggered;
    [HideInInspector]
    public int normalKnockedDown;
    private int minusKnocedDown = 40;
    private int knockedDown
    {
        get
        {
            if (playerMovement.isStaminaKnockedDown)
            {
                return normalKnockedDown - minusKnocedDown;
            }
            else
            {
                return normalKnockedDown;
            }
        }
    }

    public enum StaminaState
    {
        Regening,
        NotRegen
    }
    [HideInInspector]
    public StaminaState staminaState;

    // HealthPotion
    [HideInInspector]
    public int healthPotionCount;
    private int healthPotionStartCount;
    [HideInInspector]
    public float healthPotionHealingAmount;

    public GameObject bloodEffectPrefab;

    private Coroutine chunkUpdateCoroutine;

    [HideInInspector]
    public float maxHealthForShelter;

    public List<String> stringsOfAttackTarget;

    [HideInInspector]
    public override bool isShielding
    {
        get
        {
            return playerMovement.isShielding;
        }
    }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerController = FindObjectOfType<PlayerController>();
        playerStatusManager = GetComponent<PlayerStatusManager>();
        playerAttack = GetComponent<PlayerAttack>();
        gameOverManager = GameManager.instance.GetComponentInChildren<GameOverManager>();
        playerAnimator = GetComponent<Animator>();
        staminaState = StaminaState.Regening;
        rigidbody1 = GetComponent<Rigidbody>();
        collider1 = GetComponent<Collider>();
    }


    protected new void OnEnable()
    {
        dead = false;

        playerMovement.enabled = true;
        playerController.gameObject.SetActive(true);
        playerAttack.enabled = true;
        playerStatusManager.enabled = true;


        healthSlider.gameObject.SetActive(true);
        easeHealthSlider.gameObject.SetActive(true);
        staminaSlider.gameObject.SetActive(true);
        easeStaminaSlider.gameObject.SetActive(true);
    }

    private void OnDisable()
    {

    }

    public void EndObject()
    {
        gameOverManager.ActiveGameOverManager();

        isFirstSetUp = true;

        playerMovement.enabled = false;
        playerController.gameObject.SetActive(false);
        playerAttack.enabled = false;
        playerStatusManager.enabled = false;
        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);
        staminaSlider.gameObject.SetActive(false);
        easeStaminaSlider.gameObject.SetActive(false);

        this.enabled = false;
    }
    public void SetUpToReplayMode()
    {
        playerController.gameObject.SetActive(false);
        playerMovement.enabled = false;
    }
    public void SetUpToGameMode()
    {
        playerController.gameObject.SetActive(true);
        playerMovement.enabled = true;
    }


    private void Start()
    {

        Vector3 loadPosition = GameManager.instance.playerStartPositionTester.ReturnPlayerStartPoint();
        transform.position = loadPosition;

    }





    private void Update()
    {

        if (healthSlider.value > health)
        {
            healthSlider.value = health;
        }
        else if (healthSlider.value < health)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, health, 0.2f);
        }

        if (easeHealthSlider.value != health)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, 0.05f);
        }

        if (staminaSlider.value != stamina)
        {
            staminaSlider.value = stamina;
        }

        if (easeStaminaSlider.value != stamina)
        {
            easeStaminaSlider.value = Mathf.Lerp(easeStaminaSlider.value, stamina, 0.05f); 
        }
        StaminaRegenerate();
    }
    public void SetUpHealthData(PlayerHealthData playerHealthData)
    {
        maxHealthForShelter = playerHealthData.maxHealth;
        maxHealth = playerHealthData.maxHealth;

        maxStamina = playerHealthData.maxStamina;
        staminaRegen = playerHealthData.staminaRegen;

        dodgeStamina = playerHealthData.dodgeStamina;
        runningStamina = playerHealthData.runningStamina;

        staggered = playerHealthData.staggered;
        normalKnockedDown = playerHealthData.normalKnockedDown;
        shieldPower = playerHealthData.shieldPower;

        healthPotionStartCount = playerHealthData.healthPotionStartCount;
        healthPotionCount = playerHealthData.healthPotionStartCount;
        healthPotionHealingAmount = playerHealthData.healthPotionHealingAmount;

        defenseRate = playerHealthData.defenseRate;

        damageRate = 1 - defenseRate;

        if (isFirstSetUp)
        {
            health = maxHealth;
            stamina = maxStamina;
            isFirstSetUp = false;
        }
        SetUpHealthStaminaBar();
        UIManager.instance.UpdateHealthPotionCount(healthPotionCount);
    }
    private void SetUpHealthStaminaBar()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = health;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        easeStaminaSlider.maxValue = maxStamina;
        easeStaminaSlider.value = stamina;
    }


    private void StaminaRegenerate()
    {
        if (staminaState == StaminaState.Regening && stamina < maxStamina)
        {
            stamina += staminaRegen * correctionFactor * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }

    public void UseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;

        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
        staminaCoroutine = StartCoroutine(SuspendRegenStamina());

    }

    public void InformStamina0()
    {
    }

    private IEnumerator SuspendRegenStamina()
    {
        staminaState = StaminaState.NotRegen;

        yield return new WaitForSeconds(regenCoolDownTime);

        staminaState = StaminaState.Regening;
    }


    public override void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
        if (playerMovement.isShielding)
        {
            Vector3 refinedFrom = new Vector3(fromNormal.x, 0, fromNormal.z).normalized;

            float diffAngle = Vector3.Angle(transform.forward, refinedFrom);

            Vector3 rotatedVector = attackDirection * refinedFrom;
            float rotatedDiffAngle = Vector3.Angle(transform.forward, rotatedVector);
            float crossY = Vector3.Cross(transform.forward, rotatedVector).y;

            if (diffAngle < 80f)
            {
                UseStamina(power);
                if (stamina == 0)
                {
                    playerMovement.InvokeStaminaKnockedDown(refinedFrom);
                }
                else
                {
                    playerMovement.InvokeShieldingBack(directionType, refinedFrom, rotatedDiffAngle, crossY);
                }
                return;
            }

        }

        health -= damage * damageRate;
        if (health <= 0 && !dead)
        {
            Die();
        }
        else
        {

            if (power >= staggered && power < knockedDown)
            {
                playerMovement.InvokeStaggered(directionType, fromNormal, attackDirection);
            }
            else if (power >= knockedDown)
            {
                playerMovement.InvokeKnockedDown(directionType, fromNormal);
            }
        }

        // hitEffect
        float effectDuration = 0.5f;

        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection));
        Destroy(bloodEffect, effectDuration);
        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordEffect(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection), effectDuration);
            }
        }
    }
    public override void GetFixedDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        health -= damage;

        float effectDuration = 0.5f;
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection));
        Destroy(bloodEffect, effectDuration);
        if(TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordEffect(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection), effectDuration);
            }
        }

        if(health <= 0 && !dead)
        {
            Debug.Log("PlayerHealth : DieCheck");
            Die(); ;
        }
    }


    public override void Die()
    {
        base.Die();
        transform.SetParent(null);
        playerMovement.InvokeDIe();

    }

    public override void InvokeGrappedAction(string grapAnimationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        playerMovement.InvokeGrappedAction(grapAnimationName, animationDuration, grappedTime, grappingPositionObject);
    }



    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isReplayMode)
            {
                if (TestScene_ReplayManager.instance.isIKOn)
                {
                    playerAnimator.SetLookAtWeight(1.0f, 1.0f, 1.0f, 0.5f, 0.1f);
                    playerAnimator.SetLookAtPosition(TestScene_ReplayManager.instance.ikLookPosition);
                }
                else
                {
                    playerAnimator.SetLookAtWeight(0);
                }
            }
        }
    }

    public override void InvokeShielderedBack(Vector3 enemyPosition)
    {
        playerMovement.InvokeShielderedBack(enemyPosition);
    }
}