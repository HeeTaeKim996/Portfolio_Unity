using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FieldEnemyCoomonData
{
    public float giveUpRange;
    public int giveUpTime;
    public int shard;
}


public class FieldEnemy : Enemy
{
    [HideInInspector]
    public SkinnedMeshRenderer skinMeshRenderer;
    [HideInInspector]
    private WorldChunk worldChunk;
    private LocalChunkManager localChunkManager;

    //Chunk
    private LocalChunk currentChunk;
    [HideInInspector]
    public Vector3 initialPosition;

    [HideInInspector]
    public ResetState resetState;

    [HideInInspector]
    public SuicidableState suicidableState;
    [HideInInspector]
    public Coroutine suicideCoroutine;

    public Slider healthSlider;
    public Slider easeHealthSlider;
    protected HealthBarLookAtController_Enemy healthBarLookAtController_Enemy;
    public event System.Action<float> OnTakeDamage; // System. 을 붙인 이유는, using System 과 using UnityEngine에서 중복으로 충돌

    protected int originalLayer;

    [HideInInspector]
    public FieldEnemyCoomonData fieldEnemyData;

    protected override void Awake()
    {
        base.Awake();
        worldChunk = GetComponentInParent<WorldChunk>();
        localChunkManager = worldChunk.GetComponentInChildren<LocalChunkManager>();
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        skinMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        healthBarLookAtController_Enemy = GetComponentInChildren<HealthBarLookAtController_Enemy>();
        fieldEnemyData = new FieldEnemyCoomonData();
    }
 
    protected virtual void Start()
    {
        currentChunk = localChunkManager.GetChunkAtPosition(transform.position);
        currentChunk.RegisterEnemy(this);

        health = maxHealth;
        dead = false;

        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        easeHealthSlider.gameObject.SetActive(true);
        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = health;

        if (healthBarLookAtController_Enemy != null)
        {
            OnTakeDamage += healthBarLookAtController_Enemy.DisplayDamage;
        }

        resetState = ResetState.Normal;
        suicidableState = SuicidableState.Suicidable;
        originalLayer = gameObject.layer;
    }

    protected new virtual void OnEnable()
    {
        if (resetState == ResetState.Revive)
        {
            OnReviveReset();
        }
        else if(resetState == ResetState.OnField)
        {
            OnOnfieldReset();
        }

        resetState = ResetState.Normal;
    }
    protected virtual void OnReviveReset()
    {
        health = maxHealth;
        dead = false;
        transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        suicidableState = SuicidableState.Suicidable;

        currentChunk = localChunkManager.GetChunkAtPosition(transform.position);
        currentChunk.RegisterEnemy(this);

        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        easeHealthSlider.gameObject.SetActive(true);
        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = health;

        if (healthBarLookAtController_Enemy != null)
        {
            OnTakeDamage += healthBarLookAtController_Enemy.DisplayDamage;
        }

        enemyCollider.enabled = true;
        gameObject.layer = originalLayer;
    }
    protected virtual void OnOnfieldReset()
    {

    }
    public virtual void Suicide()
    {
        suicideCoroutine = StartCoroutine(SuicideCoroutine());
    }
    protected IEnumerator SuicideCoroutine()
    {
        while(suicidableState != SuicidableState.Suicidable)
        {
            yield return new WaitForSeconds(1f);
        }
        gameObject.SetActive(false);
    }
    public virtual void StopSuicide()
    {
        StopCoroutine(suicideCoroutine);
        suicideCoroutine = null;
    }

    protected virtual void OnDisable()
    {
        if(suicideCoroutine != null)
        {
            suicideCoroutine = null;
        }
    }

    public override void Die()
    {
        base.Die();
        gameObject.layer = 0;
    }
    public override void TurnOffOBject()
    {
        base.TurnOffOBject();
        LocalChunk nowChunk = localChunkManager.GetChunkAtPosition(transform.position);
        if(nowChunk != currentChunk)
        {
            currentChunk.UnRegisterEnemy(this);
            nowChunk.RegisterEnemy(this);
            currentChunk = nowChunk;
            if(suicideCoroutine != null)
            {
                StopCoroutine(suicideCoroutine);
            }
            suicideCoroutine = null;
        }

        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);

        if (healthBarLookAtController_Enemy != null)
        {
            OnTakeDamage -= healthBarLookAtController_Enemy.DisplayDamage;
        }

        enemyCollider.enabled = false;

        suicidableState = SuicidableState.Suicidable;

    }

    public override void SetUpToReplayMode()
    {
        base.SetUpToReplayMode();
    }
    public override void SetUpToGameMode()
    {
        base.SetUpToGameMode();
    }



    protected virtual void Update()
    {
        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (easeHealthSlider.value != health)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, 0.05f); // (3)항은 비율값
        }
    }

    public override void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
        OnTakeDamage?.Invoke(damage); // 여기서 ? 는 이벤트 OnTakeDamage의 구성요소(구독한 매서드)가 null 일시, 아무것도 안하고, != null 일시, 구독한 매서드를 발동
    }
}
