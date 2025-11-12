using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;
    private PlayerHealth playerHealth;
    private WeaponManager weaponManager;
    [HideInInspector]
    public Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private PlayerAttack playerAttack;
    private AudioSource attackAudio;


    [HideInInspector]
    public float movementSpeed;




    // Animation
    private const int baseLayerIndex = 0;
    private const int upperLayerIndex = 1;


    private string baseLayerCurrentState;
    private string upperLayerCurrentState;

    const string Idle = "Idle";
    const string ForwardMoving = "ForwardMoving";
    const string Shielding = "Shielding";
    const string FastRunning = "FastRunning";
    const string RightMoving = "RightMoving";
    const string LeftMoving = "LeftMoving";
    const string BackMoving = "BackMoving";
    const string DodgeForward = "DodgeForward";
    const string DodgeRight = "DodgeRight";
    const string DodgeLeft = "DodgeLeft";
    const string DodgeBack = "DodgeBack";
    const string StaggerForward = "StaggerForward";
    const string StaggerLeft = "StaggerLeft";
    const string StaggerRight = "StaggerRight";
    const string StaggerBack = "StaggerBack";
    const string StaggerTop = "StaggerDown";
    const string StaggerBottom = "StaggerUp";
    const string KnockDownBack = "KnockDownBack";
    const string KnockDownForward = "KnockDownForward";
    const string KnockDownUp = "KnockDownUp";
    const string KnockDownDown = "KnockDownDown";
    const string Die1 = "Die2";
    const string Shielding1 = "Shielding";
    const string ShieldingBackFront = "ShieldingBackFront";
    const string ShieldingBackLeft = "ShieldingBackLeft";
    const string ShieldingBackRight = "ShieldingBackRight";
    const string ShieldingBackTop = "ShieldingBackTop";
    const string ShieldingBackBottom = "ShieldinbBackBottom";
    const string StaminaKnockedDown = "StaminaKnockedDown";
    const string DrinkingPotion = "DrinkingPotion";
    const string NoMoreHealthPotion = "NoMoreHealthPotion";
    const string ShelterRestStart = "ShelterRestStart";
    const string ShelterRestDoing1 = "ShelterRestDoing1";
    private string ShielderedBack = "ShielderedBack";

    private float runningAceelRation
    {
        get
        {
            if (movingState == MovingState.Run)
            {
                return 1.8f;
            }
            else
            {
                return 1f;
            }
        }
    }

    private Vector3 movingVector;
    private float runningInputTime;

    private LayerMask detectLayers;
    private float closestDistance;
    private Transform closestEnemy;
    Quaternion faceEnemyRotation;
    Vector3 crossProduct;
    private float playerEnemyAngleDifference;

    public bool isShielding
    {
        get
        {
            if (playerController.isShielding || isShieldingOn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private bool isShieldingOn = false;

    [HideInInspector]
    public bool isStaminaKnockedDown = false;

    public enum MovingState
    {
        Forward,
        Right,
        Back,
        Left,
        Run
    }
    public MovingState movingState { get; private set; }

    [HideInInspector]
    public Dictionary<string, AnimationInfo> animationInfoDictionary;
    private string heavyAttackCharging_Animation;
    [HideInInspector]
    public float heavyAttackCharging_maxChargeTime;
    private float heavyAttackCharging_minChargeTime;
    private string[] lightAttackAnimations;
    private float[] lightAttackDurations;
    private float[] lightAttackHitOnTime;
    private float[] lightAttackHitOffTime;
    private float[] lightAttackSoundTime;
    private AudioClip[] lightAttackSwingSound;
    private AudioClip[] lightAttackImpactSound;
    private float[] lightAttackStaminas;
    private string[] selectedKeys;


    private int currentLightAttackIndex = 0;
    private string nextAction = null;
    private float remainingTime = 0f;
    private float coroutineDuration;

    private float lastLightAttackTime = 0f;
    private float lastRunningTime = 0f;

    private GameObject hitBox;
    private GameObject weaponTrailer;

    private Vector3 queuedMovingVector;

    private int discerningState = 0;


    private float alertTimeMax = 6f;
    private float alertTime = 0f;

    [HideInInspector]
    public float heavyAttackChargeTime = 0f;
    [HideInInspector]
    public bool isHeavyCharging = false;
    private int isHeavyAttackTriggered = 0;

    private bool isItemUsing = false;

    private Coroutine currentCoroutine = null;

    public Transform leftHandTransform;
    public Transform upperChestTransform;



    [HideInInspector]
    public Collider playerCollider;

    // MovementAudio
    private AudioSource movementAudio;
    public List<AudioClip> footPrintSounds_Rock;
    public List<AudioClip> footPrintRunSounds_Rock;
    private bool didStepSound = false;
    public List<AudioClip> dodgeSound_Rock;

    private float attackFaceDistance = 3.7f;
    private Coroutine UpDisceringCoroutine = null;

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        attackAudio = gameObject.AddComponent<AudioSource>();
        movementAudio = gameObject.AddComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
    }


    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        playerController = FindObjectOfType<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth.SetState(State.Normal);
        playerAnimator.Play("Idle");
        playerAnimator.SetLayerWeight(upperLayerIndex, 0);
        playerRigidbody.drag = StaticValues.defaultDrag;
        playerCollider.material = StaticValues.defaultMaterial;

        detectLayers = CommonMethods.GetStringsToLayerMask(playerHealth.stringsOfAttackTarget);
    }


    private void OnEnable()
    {
        playerHealth.SetState(State.Normal);
    }


    private void Update()
    {

        // Idle Animation Update
        if ((playerHealth.state == State.Normal || playerHealth.state == State.VulMovable) && !playerController.isMoving && !isShielding && (baseLayerCurrentState != ShelterRestDoing1))
        {
            bool isInAttackAnimations = false;
            AnimatorStateInfo currentStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);

            if (lightAttackAnimations != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (currentStateInfo.IsName(lightAttackAnimations[i])) 
                    {
                        isInAttackAnimations = true;
                        break;
                    }
                }
            }

            float blendTime = isInAttackAnimations ? 0.6f : 0.15f;
            BaseLayerChangeAnimation(Idle, blendTime);
        }

        // Shieldin gAnimation Update
        else if (playerHealth.state == State.Normal && !playerController.isMoving && isShielding && (baseLayerCurrentState != ShelterRestDoing1))
        {
            float blendTime = discerningState >= 1 ? 0.05f : 0.01f;
            BaseLayerChangeAnimation(Shielding1, blendTime);
        }


        if (isShielding && !isShieldingOn)
        {
            UpperLayerChangeAnimation(Shielding, 0.05f);
        }

        if (isShielding || isItemUsing)
        {
            UpperLayerWeightTo1(0.05f);
        }
        else
        {
            UpperLayerWeightTo0(0.1f);

        }


        // Running Count Update
        if (playerController.runCountStart)
        {
            runningInputTime += Time.deltaTime;
        }
        else
        {
            runningInputTime = 0;
        }


        DetectEnemy();
        UpdateDiscerningState();

        MovingStateClassify();
    }
    private void FixedUpdate()
    {
        RotateTransform();
    }


    // @@PrimaryMethods
    public void BaseLayerChangeAnimation(string newState, float blendTime, int usingNormalizedTime = 0)
    {
        if (newState == baseLayerCurrentState)
        {
            return;
        }

        if (usingNormalizedTime == 1)
        {
            AnimatorStateInfo currentStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);
            float currentNormalizedTime = currentStateInfo.normalizedTime % 1;

            playerAnimator.CrossFade(newState, blendTime, baseLayerIndex, currentNormalizedTime);
            baseLayerCurrentState = newState;


            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(true, 0, newState, blendTime, currentNormalizedTime);
                }
            }
        }
        else
        {
            playerAnimator.CrossFade(newState, blendTime, baseLayerIndex);
            baseLayerCurrentState = newState;

            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(true, 0, newState, blendTime, 0);
                }
            }
        }
    }

    private void UpperLayerChangeAnimation(string newState, float blendTime, int usingNormalizedTime = 0)
    {
        if (newState == upperLayerCurrentState)
        {
            return;
        }

        if (usingNormalizedTime == 1)
        {
            AnimatorStateInfo currentStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(upperLayerIndex);
            float currentNormalizedTime = currentStateInfo.normalizedTime % 1;

            playerAnimator.CrossFade(newState, blendTime, upperLayerIndex, currentNormalizedTime);
            upperLayerCurrentState = newState;

            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(true, 1, newState, blendTime, currentNormalizedTime);
                }
            }
        }
        else
        {
            playerAnimator.CrossFade(newState, blendTime, upperLayerIndex);
            upperLayerCurrentState = newState;

            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(true, 1, newState, blendTime, 0);
                }
            }
        }
    }
    private void CoroutineChangeAnimation(string newState, float newBlendTime, float startNormalizedTime = 0)
    {
        playerAnimator.CrossFade(newState, newBlendTime, 0, startNormalizedTime);
        baseLayerCurrentState = newState;

        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordAnimationData(true, 0, newState, newBlendTime, startNormalizedTime);
            }
        }
    }
    private void UpperLayerCoroutineChangeAnimation(string newState, float newBlendTime)
    {
        playerAnimator.CrossFade(newState, newBlendTime, upperLayerIndex, 0);
        upperLayerCurrentState = newState;

        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordAnimationData(true, 1, newState, newBlendTime, 0);
            }
        }
    }



    public void InvokeShielding()
    {
        if (playerHealth.state == State.Normal)
        {
            playerController.isShielding = true;
        }
    }

    public void InvokeRunning()
    {
        playerController.runCountStart = true;
    }

    public void UpperLayerWeightTo0(float duration)
    {
        if (playerAnimator.GetLayerWeight(upperLayerIndex) == 0) {
            return;
        }
        else
        {
            StartCoroutine(coroutine_UpperLayerWeightTo0(duration));
        }
    }
    private IEnumerator coroutine_UpperLayerWeightTo0(float duration)
    {
        float startWeight = playerAnimator.GetLayerWeight(upperLayerIndex);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newWeight = Mathf.Lerp(startWeight, 0, elapsedTime / duration);
            playerAnimator.SetLayerWeight(upperLayerIndex, newWeight);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        playerAnimator.SetLayerWeight(upperLayerIndex, 0);
    }

    private void UpperLayerWeightTo1(float duration)
    {
        if (playerAnimator.GetLayerWeight(upperLayerIndex) == 1)
        {
            return;
        }
        else
        {
            StartCoroutine(coroutine_UpperLayerWeightTo1(duration));
        }
    }
    private IEnumerator coroutine_UpperLayerWeightTo1(float duration)
    {
        float startWeight = playerAnimator.GetLayerWeight(upperLayerIndex);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newWeight = Mathf.Lerp(startWeight, 1, elapsedTime / duration);
            playerAnimator.SetLayerWeight(upperLayerIndex, newWeight);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        playerAnimator.SetLayerWeight(upperLayerIndex, 1);
    }

    private void DetectEnemy()
    {
        float detectRange = 7f;


        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRange, detectLayers);
        Transform[] detectedTransform = new Transform[colliders.Length];

        if (colliders.Length == 0)
        {
            closestEnemy = null;
            crossProduct = Vector3.zero;
            closestDistance = Mathf.Infinity;
        }
        else
        {
            closestDistance = Mathf.Infinity;

            for (int i = 0; i < colliders.Length; i++)
            {
                detectedTransform[i] = colliders[i].transform;
                float distanceToEnemy = Vector3.Distance(transform.position, detectedTransform[i].position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = detectedTransform[i];
                }
            }
            Vector3 directionToEnemy = closestEnemy.position - transform.position;

            // result
            if (directionToEnemy != Vector3.zero)
            {
                Vector3 flatDirectionToEnemy = new Vector3(directionToEnemy.x, 0f, directionToEnemy.z).normalized;
                faceEnemyRotation = Quaternion.LookRotation(flatDirectionToEnemy);
                playerEnemyAngleDifference = Vector3.Angle(movingVector, flatDirectionToEnemy);
                crossProduct = Vector3.Cross(movingVector, flatDirectionToEnemy);
            }

            // For automaticDiscerningStateUpgrade
            if(alertTime <= 0)
            {
                if(closestDistance <= attackFaceDistance)
                {
                    if (UpDisceringCoroutine == null)
                    {
                        UpDisceringCoroutine = StartCoroutine(UpDiscerningCoroutine1());
                    }
                }
            }
        }
    }
    private IEnumerator UpDiscerningCoroutine1()
    {
        float upDiscernTime = 3f;
        float elapsedTime = 0;
        float interpolationTime = 0.1f;


        while (elapsedTime < upDiscernTime)
        {
            elapsedTime += interpolationTime;

            if(closestDistance > attackFaceDistance)
            {
                StopCoroutine(UpDisceringCoroutine);
                UpDisceringCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(interpolationTime);
        }


        alertTime = alertTimeMax;
        UpDisceringCoroutine = null;
    }
    public void SetHitBox(GameObject newHitBox)
    {
        hitBox = newHitBox;
        hitBox.gameObject.SetActive(false);
    }
    public void SetWeaponTrailer(GameObject newWeaponTrailer)
    {
        weaponTrailer = newWeaponTrailer;
        weaponTrailer.SetActive(false);
    }
    public void SetUpMovementSpeed(float newMovementSpeed)
    {
        movementSpeed = newMovementSpeed;
    }

    public void ChangeAnimationData(WeaponData weaponData, PlayerAttackStaminaInfo playerAttackStaminaInfo)
    {
        animationInfoDictionary = new Dictionary<string, AnimationInfo>
        {
            { "lightAttack_1", new AnimationInfo(weaponData.lightAttack_1_Animation, weaponData.lightAttack_1_Duration, weaponData.lightAttack_1_HitBoxOnTime, weaponData.lightAttack_1_HitBoxOffTime, weaponData.lightAttack_1_swingSoundTime, weaponData.lightAttack_1_SwingSound, weaponData.lightAttack_1_ImpactSound, playerAttackStaminaInfo.lightAttack_1_Stamina) },
            { "lightAttack_2", new AnimationInfo(weaponData.lightAttack_2_Animation, weaponData.lightAttack_2_Duration, weaponData.lightAttack_2_HitBoxOnTime, weaponData.lightAttack_2_HitBoxOffTime, weaponData.lightAttack_2_swingSoundTime, weaponData.lightAttack_2_SwingSound, weaponData.lightAttack_2_ImpactSound, playerAttackStaminaInfo.lightAttack_2_Stamina) },
            { "lightAttack_3", new AnimationInfo(weaponData.lightAttack_3_Animation, weaponData.lightAttack_3_Duration, weaponData.lightAttack_3_HitBoxOnTime, weaponData.lightAttack_3_HitBoxOffTime, weaponData.lightAttack_3_swingSoundTime, weaponData.lightAttack_3_SwingSound, weaponData.lightAttack_3_ImpactSound, playerAttackStaminaInfo.lightAttack_3_Stamina) },
            { "lightAttack_4", new AnimationInfo(weaponData.lightAttack_4_Animation, weaponData.lightAttack_4_Duration, weaponData.lightAttack_4_HitBoxOnTime, weaponData.lightAttack_4_HitBoxOffTime, weaponData.lightAttack_4_swingSoundTime, weaponData.lightAttack_4_SwingSound, weaponData.lightAttack_4_ImpactSound, playerAttackStaminaInfo.lightAttack_4_Stamina) },
            { "lightAttack_5", new AnimationInfo(weaponData.lightAttack_5_Animation, weaponData.lightAttack_5_Duration, weaponData.lightAttack_5_HitBoxOnTime, weaponData.lightAttack_5_HitBoxOffTime, weaponData.lightAttack_5_swingSoundTime, weaponData.lightAttack_5_SwingSound, weaponData.lightAttack_5_ImpactSound, playerAttackStaminaInfo.lightAttack_5_Stamina) },
            { "heavyAttack", new AnimationInfo(weaponData.heavyAttack_Animation, weaponData.heavyAttack_Duration, weaponData.heavyAttack_HitBoxOnTime, weaponData.heavyAttack_HitBoxOffTime, weaponData.heavyAttack_swingSoundTime, weaponData.heavyAttack_SwingSound, weaponData.heavyAttack_ImpactSound, playerAttackStaminaInfo.heavyAttack_Stamina) },
            { "runningAttack", new AnimationInfo(weaponData.runningAttack_Animation, weaponData.runningAttack_Duration, weaponData.runningAttack_HitBoxOnTime, weaponData.runningAttack_HitBoxOffTime, weaponData.runningAttack_swingSoundTime, weaponData.runningAttack_SwingSound, weaponData.runningAttack_ImpactSound, playerAttackStaminaInfo.runningAttack_Stamina) },
        };

        heavyAttackCharging_Animation = weaponData.heavyAttackCharging_Animation;
        heavyAttackCharging_maxChargeTime = weaponData.heavyAttackCharging_maxChargeTime;
        heavyAttackCharging_minChargeTime = weaponData.heavyAttackCharging_minChargeTime;


        selectedKeys = new string[] { "lightAttack_1", "lightAttack_2", "lightAttack_3", "lightAttack_4", "lightAttack_5" };

        lightAttackAnimations = selectedKeys.Select(key => animationInfoDictionary[key].Animation).ToArray();
        lightAttackDurations = selectedKeys.Select(key => animationInfoDictionary[key].Duration).ToArray();
        lightAttackHitOnTime = selectedKeys.Select(key => animationInfoDictionary[key].HitBoxOnTime).ToArray();
        lightAttackHitOffTime = selectedKeys.Select(key => animationInfoDictionary[key].HitBoxOffTime).ToArray();
        lightAttackSoundTime = selectedKeys.Select(key => animationInfoDictionary[key].SwingSoundTime).ToArray();
        lightAttackSwingSound = selectedKeys.Select(key => animationInfoDictionary[key].SwingSound).ToArray();
        lightAttackImpactSound = selectedKeys.Select(key => animationInfoDictionary[key].ImpactSound).ToArray();
        lightAttackStaminas = selectedKeys.Select(key => animationInfoDictionary[key].Stamina).ToArray();
    }
    // @@MovingState
    private void UpdateDiscerningState()
    {
        if (alertTime > 0)
        {
            if(closestDistance > attackFaceDistance )
            {
                alertTime -= Time.deltaTime;
            }
            else
            {
                alertTime = Mathf.Min(alertTimeMax, alertTime + Time.deltaTime / 3f);
            }
        }
        if (alertTime > 0 && closestEnemy != null)
        {
            discerningState = 2;
        }
        else if (closestEnemy != null)
        {
            discerningState = 1;
        }
        else if (closestEnemy == null)
        {
            discerningState = 0;
        }
    }

    private void MovingStateClassify()
    {
        if (runningInputTime > 0.5f && playerHealth.stamina > 0)
        {
            movingState = MovingState.Run;
            playerHealth.UseStamina(playerHealth.runningStamina * Time.deltaTime);
        }
        else if (discerningState == 2 || (discerningState == 1 && isShielding))
        {
            if (playerEnemyAngleDifference < 45f)
            {
                movingState = MovingState.Forward;
            }
            else if (playerEnemyAngleDifference >= 45f && playerEnemyAngleDifference < 135f)
            {
                if (crossProduct.y > 0)
                {
                    movingState = MovingState.Left;
                }
                else
                {
                    movingState = MovingState.Right;
                }
            }
            else
            {
                movingState = MovingState.Back;
            }
        }
        else if (discerningState <= 1)
        {
            movingState = MovingState.Forward;
        }

    }
    private void OnAnimatorIK(int layerIndex)
    {
        if ((playerHealth.state == State.Normal || playerHealth.state == State.VulMovable) && movingState != MovingState.Run && (discerningState == 2 || (discerningState == 1 && isShielding)))
        {
            playerAnimator.SetLookAtWeight(1f, 1f, 0f, 0f, 0f);


            float theta = -0.07f;



            Vector3 enemyPosition = closestEnemy.position;
            Vector3 playerPosition = transform.position;

            Vector3 direction = enemyPosition - playerPosition;

            float radius = direction.magnitude;

            direction.Normalize();

            float cosTheta = Mathf.Cos(theta);
            float sinTheta = Mathf.Sin(theta);

            Vector3 rotatedPosition = new Vector3(direction.x * cosTheta - direction.z * sinTheta, 0, direction.x * sinTheta + direction.z * cosTheta);

            Vector3 lookAtPosition = playerPosition + rotatedPosition * radius;



            // y�� ������� ���
            float interpolationCoefficientForY;
            float x1 = 12.2328f;
            float y1 = -2.579375f;
            float x2 = 1.3883429f;
            float y2 = 0.2566508f;
            float m = (y2 - y1) / (x2 - x1);
            interpolationCoefficientForY = m * closestDistance + (y1 - m * x1);


            lookAtPosition.y = upperChestTransform.position.y + interpolationCoefficientForY;

            playerAnimator.SetLookAtPosition(lookAtPosition);

            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordOnAnimatorIKData(true, lookAtPosition);
                }
            }
        }
        else if (playerHealth.state == State.Normal && movingState != MovingState.Run && (discerningState == 0 && isShielding) && playerController.isMoving)
        {
            playerAnimator.SetLookAtWeight(1f, 1f, 0f, 0f, 0f);
            float theta = -0.07f;
            Vector3 norminalPosition = transform.position + transform.forward * 6f;
            Vector3 playerPosition = transform.position;
            Vector3 direction = norminalPosition - playerPosition;
            float radius = direction.magnitude;
            direction.Normalize();
            float cosTheta = Mathf.Cos(theta);
            float sinTheta = Mathf.Sin(theta);
            Vector3 rotatedPosition = new Vector3(direction.x * cosTheta - direction.z * sinTheta, 0, direction.x * sinTheta + direction.z * cosTheta);
            Vector3 lookAtPosition = playerPosition + rotatedPosition * radius;
            lookAtPosition.y = upperChestTransform.position.y - 0.7f;
            playerAnimator.SetLookAtPosition(lookAtPosition);
            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordOnAnimatorIKData(true, lookAtPosition);
                }
            }
        }
        else
        {
            playerAnimator.SetLookAtWeight(0);
            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordOnAnimatorIKData(false, Vector3.zero);
                }
            }
        }
    }


    public void InvokeMoving(Vector2 movingVector2)
    {
        movingVector = new Vector3(movingVector2.x, 0f, movingVector2.y);

        if (playerHealth.state == State.Normal || playerHealth.state == State.VulMovable)
        {
            playerController.isMoving = true;


            Vector3 refinedMovingVector;

            if (movingState == MovingState.Run)
            {
                refinedMovingVector = movingVector.normalized;
            }
            else if (playerHealth.state == State.VulMovable)
            {
                float maxMagnitude = 0.5f;
                refinedMovingVector = movingVector.magnitude > maxMagnitude ? movingVector.normalized * maxMagnitude : movingVector;
            }
            else
            {
                refinedMovingVector = movingVector;
            }

            Vector3 moveDistnace = refinedMovingVector * movementSpeed * runningAceelRation * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistnace);

            if (movingState == MovingState.Forward)
            {
                float blendTime = playerAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex).IsName(Idle) ? 0.05f : 0.2f;
                BaseLayerChangeAnimation(ForwardMoving, blendTime, 1);
                playerAnimator.SetFloat("Blend", refinedMovingVector.magnitude);
            }
            else if (movingState == MovingState.Run)
            {
                BaseLayerChangeAnimation(FastRunning, 0.2f, 1);
                lastRunningTime = Time.time;
            }
            else if (movingState == MovingState.Right)
            {
                BaseLayerChangeAnimation(RightMoving, 0.2f, 1);
                playerAnimator.SetFloat("Blend", refinedMovingVector.magnitude);
            }
            else if (movingState == MovingState.Left)
            {
                BaseLayerChangeAnimation(LeftMoving, 0.2f, 1);
                playerAnimator.SetFloat("Blend", refinedMovingVector.magnitude);
            }
            else if (movingState == MovingState.Back)
            {
                BaseLayerChangeAnimation(BackMoving, 0.2f, 1);
                playerAnimator.SetFloat("Blend", refinedMovingVector.magnitude);
            }

            
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);
            float normalizedTime = stateInfo.normalizedTime % 1;

            float footStepSoundTime = 0.22f;

            

    

            if(didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, footStepSoundTime, footStepSoundTime + 0.5f) == true)
            {
                if(movingState == MovingState.Run)
                {
                    CommonMethods.AudioPlayOneShot(movementAudio, footPrintRunSounds_Rock[Random.Range(0, footPrintRunSounds_Rock.Count - 1)], 1.24f);
                }
                else
                {
                    CommonMethods.AudioPlayOneShot(movementAudio, footPrintSounds_Rock[Random.Range(0, footPrintSounds_Rock.Count - 1)], 1f);
                }

                didStepSound = true;
            }
            else if(didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, footStepSoundTime, footStepSoundTime + 0.5f) == false)
            {
                didStepSound = false;
            }

            

        }
    }


    private void RotateTransform()
    {
        if (playerHealth.state == State.Normal || playerHealth.state == State.VulMovable)
        {
            Quaternion moveVecTowardRotation = Quaternion.identity; 
            float rotationSpeed = 15f;

            if (playerController.isMoving)
            {
                if (movingVector != Vector3.zero)
                {
                    moveVecTowardRotation = Quaternion.LookRotation(movingVector);
                }
            }
            else if (discerningState >= 2 || (!playerController.isMoving && discerningState == 1 && isShielding))
            {
                moveVecTowardRotation = faceEnemyRotation;
            }
            else
            {
                moveVecTowardRotation = Quaternion.LookRotation(transform.forward);
            }

            if (movingState == MovingState.Forward || movingState == MovingState.Run || (!playerController.isMoving && discerningState >= 1))
            {
                playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, moveVecTowardRotation, rotationSpeed * Time.deltaTime));
            }
            else if (movingState == MovingState.Left)
            {
                Quaternion rightRotation = Quaternion.Euler(0, +90f, 0) * moveVecTowardRotation;
                playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, rightRotation, rotationSpeed * Time.deltaTime));
            }
            else if (movingState == MovingState.Right)
            {
                Quaternion leftRotation = Quaternion.Euler(0, -90f, 0) * moveVecTowardRotation;
                playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, leftRotation, rotationSpeed * Time.deltaTime));
            }
            else if (movingState == MovingState.Back)
            {
                Quaternion backRotation = Quaternion.Euler(0, 180f, 0) * moveVecTowardRotation;
                playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, backRotation, rotationSpeed * Time.deltaTime));
            }
        }
    }

    // @@ExcuteNextAction Coroutine
    private void ExcuteNextAction()
    {
        if (nextAction == "LightAttack")
        {
            nextAction = null;
            queuedMovingVector = Vector3.zero;
            InvokeLightAttack();
        }
        else if (nextAction == "Dodge")
        {
            nextAction = null;
            InvokeDodge();
        }
        else if (nextAction == "HeavyAttack")
        {
            nextAction = null;
            queuedMovingVector = Vector3.zero;
            InvokeHeavyAttackCharging();
        }
        else if (nextAction == "HealthPotionDrink")
        {
            nextAction = null;
            queuedMovingVector = Vector3.zero;
            InvokeHealthPotionDrink();
        }
    }
    public void InvokeDodge()
    {
        if (playerHealth.state == State.Normal || playerHealth.state == State.InvulDodgable)
        {
            Vector3 dodgeDirection;
            if (queuedMovingVector != Vector3.zero)
            {
                dodgeDirection = queuedMovingVector;
                queuedMovingVector = Vector3.zero;
            }
            else if (movingVector != Vector3.zero)
            {
                dodgeDirection = movingVector.normalized;
            }
            else
            {
                dodgeDirection = transform.forward;
            }

            if (currentCoroutine != null)
            {
                StopCoroutineRoutine(currentCoroutine);
            }

            if (movingState == MovingState.Forward)
            {
                playerRigidbody.MoveRotation(Quaternion.LookRotation(dodgeDirection));
                currentCoroutine = StartCoroutine(DodgeAction(dodgeDirection, DodgeForward));
            }
            else if (movingState == MovingState.Right)
            {
                playerRigidbody.MoveRotation(Quaternion.LookRotation(-Vector3.Cross(Vector3.up, dodgeDirection)));
                currentCoroutine = StartCoroutine(DodgeAction(dodgeDirection, DodgeRight));
            }
            else if (movingState == MovingState.Left)
            {
                playerRigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(Vector3.up, dodgeDirection)));
                currentCoroutine = StartCoroutine(DodgeAction(dodgeDirection, DodgeLeft));
            }
            else if (movingState == MovingState.Back)
            {
                playerRigidbody.MoveRotation(Quaternion.LookRotation(-dodgeDirection));
                currentCoroutine = StartCoroutine(DodgeAction(dodgeDirection, DodgeBack));
            }
        }
        else if ((playerHealth.state == State.Invulnerable || playerHealth.state == State.Vulnerable || playerHealth.state == State.VulMovable) && (remainingTime < 0.2f && (remainingTime / coroutineDuration) < 0.2f))
        {
            nextAction = "Dodge";
            if (movingVector != Vector3.zero)
            {
                queuedMovingVector = movingVector.normalized;
            }
        }
    }

    private IEnumerator DodgeAction(Vector3 dodgeDirection, string animationName)
    {
        if (playerHealth.stamina > 0)
        {
            playerHealth.UseStamina(playerHealth.dodgeStamina);
        }
        else
        {
            playerHealth.InformStamina0();
            ResetRoutine();
            yield break;
        }

        playerHealth.SetState(State.Invulnerable);

        CommonMethods.AudioPlay(movementAudio, dodgeSound_Rock[Random.Range(0, dodgeSound_Rock.Count)], 1f, 0);

        float dodgeDuration = 0.35f;
        float dodgeRestDuration = 0.35f;

        coroutineDuration = dodgeDuration + dodgeRestDuration;
        remainingTime = coroutineDuration;
        float dodgeSpeed = movementSpeed * 1.9f;
        float dodgeRestSpeed = movementSpeed * 0.2f;

        CoroutineChangeAnimation(animationName, 0.05f);


        while (remainingTime > dodgeRestDuration)
        {
            Vector3 moveDistance = dodgeDirection * dodgeSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

            remainingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        playerHealth.SetState(State.Vulnerable);
        while (remainingTime > 0)
        {
            Vector3 moveDistance = dodgeDirection * dodgeRestSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

            remainingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        playerHealth.SetState(State.Normal);
        remainingTime = 0f;
        currentCoroutine = null;
        ExcuteNextAction();

    }
    public void InvokeLightAttack()
    {
        if (playerHealth.state == State.Normal)
        {
            float currentTime = Time.time;

            float lightAttackComboMax = 0.4f;

            float runningAttackMaxInterval = 0.4f;

            if (currentTime - lastRunningTime <= runningAttackMaxInterval)
            {
                currentCoroutine = StartCoroutine(RunningAttackAction());
                return;
            }

            if (currentTime - lastLightAttackTime <= lightAttackComboMax)
            {
                currentLightAttackIndex = (currentLightAttackIndex + 1) % lightAttackAnimations.Length;
            }
            else
            {
                currentLightAttackIndex = 0;
            }

            string animationName = lightAttackAnimations[currentLightAttackIndex];
            float attackDuration = lightAttackDurations[currentLightAttackIndex];
            float hitOnTime = lightAttackHitOnTime[currentLightAttackIndex];
            float hitOffTime = lightAttackHitOffTime[currentLightAttackIndex];
            float swingSoundTime = lightAttackSoundTime[currentLightAttackIndex];
            AudioClip swingSound = lightAttackSwingSound[currentLightAttackIndex];
            float stamina = lightAttackStaminas[currentLightAttackIndex];
            string infoKey = selectedKeys[currentLightAttackIndex];

            playerAttack.SetImpactAudio(lightAttackImpactSound[currentLightAttackIndex]);

            currentCoroutine = StartCoroutine(LightAttackAction(animationName, stamina, attackDuration, hitOnTime, hitOffTime, swingSoundTime, swingSound, infoKey));
        }
        else if ((playerHealth.state == State.Invulnerable || playerHealth.state == State.Vulnerable || playerHealth.state == State.VulMovable) && remainingTime <= 0.2f)
        {
            nextAction = "LightAttack";
        }
    }

    private IEnumerator LightAttackAction(string animationName1, float stamina1, float attackingDuration1, float hitOnTime1, float hitOffTime1, float newSwingSoundTime, AudioClip newSwingSound, string infoKey1)
    {
        if (playerHealth.stamina > 0)
        {
            playerHealth.UseStamina(stamina1);
        }
        else
        {
            playerHealth.InformStamina0();
            ResetRoutine();
            if(currentLightAttackIndex > 0)
            {
                currentLightAttackIndex -= 1;
            }
            yield break;
        }


        playerHealth.SetState(State.Vulnerable);

        Quaternion startRotation = transform.rotation;
        float rotationElapsedTime = 0f;

        coroutineDuration = attackingDuration1;
        remainingTime = coroutineDuration;

        float lightAttackingSpeed = movementSpeed * 0.3f;


        bool isHitBoxOn = false;
        bool isHitBoxOff = false;

        float soundingTime = newSwingSoundTime;


        bool didSwingSound = false;


        BaseLayerChangeAnimation(animationName1, 0.05f);

        playerAttack.SetCurrentAttackByName(infoKey1);


        if (closestDistance <= attackFaceDistance)
        {
            alertTime = alertTimeMax;
        }

        float rotationDuration = 0.15f;

        while (remainingTime > 0)
        {
            if (discerningState == 2 || (discerningState == 1 && closestDistance <= attackFaceDistance))
            {
                rotationElapsedTime += Time.fixedDeltaTime;
                float rotationProgress = Mathf.Clamp01(rotationElapsedTime / rotationDuration);
                playerRigidbody.MoveRotation(Quaternion.Slerp(startRotation, faceEnemyRotation, rotationProgress));
            }
            Vector3 moveDistance = transform.forward * lightAttackingSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

            if(remainingTime <= (attackingDuration1 - hitOnTime1) && !isHitBoxOn)
            {
                hitBox.SetActive(true);
                weaponTrailer.SetActive(true);
                isHitBoxOn = true;
                if(TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(weaponTrailer ,true, GameObjectType.Trailer, true);
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox, true, GameObjectType.HitBox, true);
                    }
                }
            }


            if(remainingTime <= attackingDuration1 - newSwingSoundTime && !didSwingSound)
            {
                CommonMethods.AudioPlay(attackAudio, newSwingSound, 0.5f, 0);

                didSwingSound = true;
            }


            if(remainingTime <= (attackingDuration1 - hitOffTime1) && !isHitBoxOff){
                hitBox.SetActive(false);
                weaponTrailer.SetActive(false);
                isHitBoxOff = true;
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(weaponTrailer, true, GameObjectType.Trailer, false);
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox, true, GameObjectType.HitBox, false);
                    }
                }
            }


            remainingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        playerAttack.SetCurrentAttackByName(null);

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        lastLightAttackTime = Time.time;
        remainingTime = 0f;

        ExcuteNextAction();
    }
  
    //HeavyAttack
    public void InvokeHeavyAttackCharging()
    {
        if(playerHealth.state == State.Normal && !isHeavyCharging)
        {
            isHeavyCharging = true;
            heavyAttackChargeTime = 0f;
            BaseLayerChangeAnimation(heavyAttackCharging_Animation, 0.1f);
            currentCoroutine = StartCoroutine(HeavyAttackChargingRoutine());
            
        }
        else if ((playerHealth.state == State.Invulnerable || playerHealth.state == State.Vulnerable || playerHealth.state == State.VulMovable) && remainingTime <= 0.2f && !isHeavyCharging)
        {
            nextAction = "HeavyAttack";
        }
    }
    private IEnumerator HeavyAttackChargingRoutine()
    {

        if(animationInfoDictionary["heavyAttack"].Stamina <= 0)
        {
            playerHealth.InformStamina0();
            ResetRoutine();
            yield break;
        }

        playerHealth.SetState(State.Vulnerable);
        Quaternion startRotation = playerRigidbody.rotation;
        float rotationElapsedTime = 0f;
        float rotationDuration = 0.15f;


        while (heavyAttackChargeTime < heavyAttackCharging_maxChargeTime)
        {
            heavyAttackChargeTime += Time.deltaTime;

            if (discerningState == 2 || ( discerningState ==  1 & closestDistance <= attackFaceDistance))
            {
                rotationElapsedTime += Time.deltaTime;
                float rotationProgress = Mathf.Clamp01(rotationElapsedTime / rotationDuration);
                playerRigidbody.MoveRotation(Quaternion.Slerp(startRotation, faceEnemyRotation, rotationProgress));
            }

            yield return new WaitForFixedUpdate();

            if(isHeavyAttackTriggered > 0 && heavyAttackChargeTime >= heavyAttackCharging_minChargeTime)
            {
                currentCoroutine = StartCoroutine(PerformHeavyAttackRoutine(heavyAttackChargeTime));
                yield break;
            }
        }
        playerController.InvokeAttackButtonEndPhase();
        currentCoroutine = StartCoroutine(PerformHeavyAttackRoutine(heavyAttackChargeTime));
        yield break;
    }

    public void InvokeHeavyAttack()
    {
        isHeavyAttackTriggered += 1;
    }
    private IEnumerator PerformHeavyAttackRoutine(float heavyAttackChargeTime1)
    {
        playerHealth.UseStamina(animationInfoDictionary["heavyAttack"].Stamina);
        playerAttack.SetImpactAudio(animationInfoDictionary["heavyAttack"].ImpactSound);
        isHeavyCharging = false;
        Quaternion startRotation = playerRigidbody.rotation;
        float rotationElapsedTime = 0f;
        float rotationDuration = 0.15f;

        coroutineDuration = animationInfoDictionary["heavyAttack"].Duration;
        remainingTime = coroutineDuration;
        float heavyAttackSpeed = movementSpeed * 0.6f;

        BaseLayerChangeAnimation(animationInfoDictionary["heavyAttack"].Animation, 0.05f);


        bool isHitBoxOn = false;
        bool isHitBoxOff = false;

        bool didSwingSound = false;

        playerAttack.SetCurrentAttackByName("heavyAttack");

        if (closestDistance <= attackFaceDistance)
        {
            alertTime = alertTimeMax;
        }

        while (remainingTime > 0)
        {

            if (discerningState == 2 || (discerningState == 1 & closestDistance <= attackFaceDistance))
            {
                rotationElapsedTime += Time.fixedDeltaTime;
                float rotationProgress = Mathf.Clamp01(rotationElapsedTime / rotationDuration);
                playerRigidbody.MoveRotation(Quaternion.Slerp(startRotation, faceEnemyRotation, rotationProgress));
            }

            Vector3 moveDistance = transform.forward * heavyAttackSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);


            if(remainingTime <= (coroutineDuration - animationInfoDictionary["heavyAttack"].HitBoxOnTime) && !isHitBoxOn)
            {
                hitBox.SetActive(true);
                weaponTrailer.SetActive(true);
                isHitBoxOn = true;
            }

            if(remainingTime <= (coroutineDuration - animationInfoDictionary["heavyAttack"].HitBoxOffTime) && !isHitBoxOff)
            {
                hitBox.SetActive(false);
                weaponTrailer.SetActive(false);
                isHitBoxOff = true;
            }

            if(remainingTime <= (coroutineDuration - animationInfoDictionary["heavyAttack"].SwingSoundTime) && !didSwingSound)
            {
                CommonMethods.AudioPlay(attackAudio, animationInfoDictionary["heavyAttack"].SwingSound, 0.5f, 0);

                didSwingSound = true;
            }


            remainingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();           
        }

        if (isHeavyAttackTriggered > 0)
        {
            isHeavyAttackTriggered -= 1;
        }

        playerAttack.SetCurrentAttackByName(null);

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }
    private IEnumerator RunningAttackAction()
    {
        if (animationInfoDictionary["runningAttack"].Stamina > 0)
        {
            playerHealth.UseStamina(animationInfoDictionary["runningAttack"].Stamina);
        }
        else
        {
            playerHealth.InformStamina0();
            ResetRoutine();
            yield break;
        }

        playerHealth.SetState(State.Vulnerable);

        playerAttack.SetCurrentAttackByName("runningAttack");
        playerAttack.SetImpactAudio(animationInfoDictionary["runningAttack"].ImpactSound);

        bool isHitBoxOn = false;
        bool isHitBoxOff = false;


        coroutineDuration = animationInfoDictionary["runningAttack"].Duration;
        remainingTime = coroutineDuration;
        float runningAttackingSpeed = movementSpeed * 0.7f;

        float swingSoundTime = animationInfoDictionary["runningAttack"].SwingSoundTime;
        bool didSwingSound = false;

        BaseLayerChangeAnimation(animationInfoDictionary["runningAttack"].Animation, 0.1f);
        while (remainingTime > 0)
        {
            Vector3 moveDistance = transform.forward * runningAttackingSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

            if (remainingTime < coroutineDuration - animationInfoDictionary["runningAttack"].HitBoxOnTime && !isHitBoxOn)
            {
                hitBox.SetActive(true);
                weaponTrailer.SetActive(true);
                isHitBoxOn = true;
            }

            if (remainingTime < coroutineDuration - animationInfoDictionary["runningAttack"].HitBoxOffTime && !isHitBoxOff)
            {
                hitBox.SetActive(false);
                weaponTrailer.SetActive(false);
                isHitBoxOff = true;
            }

            if(remainingTime <= coroutineDuration - swingSoundTime && !didSwingSound)
            {
                CommonMethods.AudioPlay(attackAudio, animationInfoDictionary["runningAttack"].SwingSound, 0.5f, 0);

                didSwingSound = true;
            }

            remainingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        playerAttack.SetCurrentAttackByName(null);

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }


    public void InvokeHealthPotionDrink()
    {
        if (playerHealth.state == State.Normal )
        {
            if(playerHealth.healthPotionCount > 0)
            {
                currentCoroutine = StartCoroutine(HealthPotionDrinkAction());
            }
            else
            {
                currentCoroutine = StartCoroutine(NoMoreHealthPotionAction());
            }
        }
        else if ((playerHealth.state == State.Invulnerable || playerHealth.state == State.Vulnerable || playerHealth.state == State.VulMovable) && remainingTime <= 0.2f)
        {
            nextAction = "HealthPotionDrink";
        }
    }
    private IEnumerator HealthPotionDrinkAction()
    {
        playerHealth.SetState(State.VulMovable);

        isItemUsing = true;

        UpperLayerCoroutineChangeAnimation(DrinkingPotion, 0.05f);

        float healthPotionDrinkingDuration = 1.4875f;

        float healthHealActiveTime = 0.595f;
        bool isHealActive = false;


        float potionPickUpTime = 0.2737f;
        bool didPickupPotion = false;

        float potionDropTime = 1.428f;
        bool didDropPotion = false;

        coroutineDuration = healthPotionDrinkingDuration;
        remainingTime = coroutineDuration;
        while(remainingTime > 0)
        {
            if(remainingTime < healthPotionDrinkingDuration - healthHealActiveTime && !isHealActive)
            {
                playerHealth.RestoreHealth(playerHealth.healthPotionHealingAmount);
                playerHealth.healthPotionCount --;
                UIManager.instance.UpdateHealthPotionCount(playerHealth.healthPotionCount);
                isHealActive = true;
            }

            if(remainingTime < healthPotionDrinkingDuration - potionPickUpTime && !didPickupPotion)
            {
                weaponManager.currentWeapon.SetActive(false);
                weaponManager.potionObject.SetActive(true);
                didPickupPotion = true;
            }
            if(remainingTime < healthPotionDrinkingDuration - potionDropTime && !didDropPotion)
            {
                weaponManager.currentWeapon.SetActive(true);
                weaponManager.potionObject.SetActive(false);
                didDropPotion = true;
            }


            remainingTime -= Time.deltaTime;

            yield return null;
        }

        isItemUsing = false;

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }  
    private IEnumerator NoMoreHealthPotionAction()
    {
        playerHealth.SetState(State.VulMovable);
        isItemUsing = true;

        UpperLayerCoroutineChangeAnimation(NoMoreHealthPotion, 0.05f);

        float noMoreHealthPotionDuration = 1.1375f;

        coroutineDuration = noMoreHealthPotionDuration;
        remainingTime = coroutineDuration;
        while(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        isItemUsing = false;

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }



    // @@StopCoroutine Coroutine

    public void InvokeStaggered(DirectionType directionType1, Vector3 fromNormal1, Quaternion attackDirection1)
    {
        if(currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(StaggeredAction(directionType1,fromNormal1, attackDirection1));

    }

    private IEnumerator StaggeredAction(DirectionType directionType1, Vector3 fromNormal1, Quaternion attackDirection1)
    {
        playerHealth.SetState(State.Vulnerable);


        float flatDiffAngle;
        float crossY;
        Vector3 refinedRotatedVector;

        if (directionType1 == DirectionType.Normal)
        {
            Vector3 refinedFrom = new Vector3(fromNormal1.x, 0f, fromNormal1.z).normalized;
            Vector3 rotatedVector = attackDirection1 * refinedFrom;

            refinedRotatedVector = new Vector3(rotatedVector.x, 0f, rotatedVector.z);

            flatDiffAngle = Vector3.Angle(transform.forward, refinedRotatedVector);
            crossY = Vector3.Cross(transform.forward, refinedRotatedVector).y;

        }
        else
        {
            flatDiffAngle = 0f;
            crossY = 0f;
            refinedRotatedVector = new Vector3(fromNormal1.x, 0f, fromNormal1.z).normalized * 0.5f;
        }


        string animationName;

        if (directionType1 == DirectionType.TopDown)
        {
            animationName = StaggerTop;
        }
        else if (directionType1 == DirectionType.BottomUp)
        {
            animationName = StaggerBottom;
        }
        else if (flatDiffAngle < 45f)
        {
            animationName = StaggerForward;
        }
        else if (flatDiffAngle >= 45f && flatDiffAngle < 135f)
        {
            if (crossY < 0)
            {
                animationName = StaggerRight;
            }
            else
            {
                animationName = StaggerLeft;
            }
        }
        else
        {
            animationName = StaggerBack;
        }

        CoroutineChangeAnimation(animationName, 0.05f);

        playerRigidbody.AddForce(-refinedRotatedVector * StaticValues.addForceTiny, ForceMode.Impulse);

        float staggerDuration = 0.7f;

        coroutineDuration = staggerDuration;
        float remainingTime = coroutineDuration;
        float backwardStartTime = staggerDuration * 0.3f;
        float backwardEndTime = staggerDuration * 0.9f;


        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            yield return null;
        }

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }


    public void InvokeKnockedDown(DirectionType directionType1, Vector3 fromNormal1)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(KnockDownAction(directionType1, fromNormal1));
    }

    private IEnumerator KnockDownAction(DirectionType directionType1, Vector3 fromNormal1)
    {
        playerHealth.SetState(State.Invulnerable);
        GameManager.instance.cinemachineController.InvokeCameraImpulse(0.1f, 0.6f);

        float flatDiffAngle;

        Vector3 refinedFrom;

        if (directionType1 == DirectionType.Normal)
        {
            refinedFrom = new Vector3(fromNormal1.x, 0f, fromNormal1.z).normalized;

            flatDiffAngle = Vector3.Angle(transform.forward, refinedFrom);
        }
        else
        {
            refinedFrom = new Vector3(fromNormal1.x, 0f, fromNormal1.z).normalized * 0.3f;
            flatDiffAngle = 0f;
        }

        string animationName;

        if (directionType1 == DirectionType.TopDown)
        {
            animationName = KnockDownDown;
        }
        else if (directionType1 == DirectionType.BottomUp)
        {
            animationName = KnockDownUp;
        }
        else if (flatDiffAngle <= 90f)
        {
            animationName = KnockDownBack;
        }
        else
        {
            animationName = KnockDownForward;
        }

        CoroutineChangeAnimation(animationName, 0.05f);



        Vector3 newPosition = gameObject.transform.position;
        newPosition.y += 1f;

        gameObject.transform.position = newPosition;

        playerRigidbody.AddForce(-refinedFrom * StaticValues.addForce5f, ForceMode.Impulse);

        if(directionType1 == DirectionType.Normal)
        {
            if(flatDiffAngle <= 90f)
            {
                transform.rotation = Quaternion.LookRotation(refinedFrom);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(-refinedFrom);
            }
        }

        float knockDownDuration = 1.875f;
        float knockDownDurationFirst = 0.572f;

        coroutineDuration = knockDownDuration;
        float remainingTime = coroutineDuration;

        while (remainingTime > knockDownDuration - knockDownDurationFirst)
        {
            remainingTime -= Time.deltaTime;

            yield return null;
        }

        playerHealth.SetState(State.InvulDodgable);

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }



        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }

    public void InvokeShieldingBack(DirectionType directionType, Vector3 refinedFrom, float rotatedDiffAngle, float crossY )
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        Vector3 backForceVector;

        if (directionType == DirectionType.Normal)
        {
            backForceVector = -refinedFrom * StaticValues.addForceTiny;
        }
        else
        {
            backForceVector = -refinedFrom * StaticValues.addForceTiny * 0.2f;
        }

        string animationName;

        if (directionType == DirectionType.TopDown)
        {
            animationName = ShieldingBackTop;
        }
        else if (directionType == DirectionType.BottomUp)
        {
            animationName = ShieldingBackBottom;
        }
        else if (rotatedDiffAngle > 45)
        {
            if(crossY > 0)
            {
                animationName = ShieldingBackRight;
            }
            else
            {
                animationName = ShieldingBackLeft;
            }
        }
        else
        {
            animationName = ShieldingBackFront;
        }

        currentCoroutine = StartCoroutine(ShieldingBackAction(animationName, backForceVector));
    }
    private IEnumerator ShieldingBackAction(string animationName1, Vector3 backForceVector1)
    {
        playerHealth.SetState(State.Vulnerable);
        isShieldingOn = true;

        CoroutineChangeAnimation(animationName1, 0.05f);
        UpperLayerCoroutineChangeAnimation(animationName1, 0.05f);

        playerRigidbody.AddForce(backForceVector1, ForceMode.Impulse);
;
        coroutineDuration = 0.6f;
        remainingTime = coroutineDuration;

        while(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            yield return null;
        }


        isShieldingOn = false;

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }
    public void InvokeShielderedBack(Vector3 enemyPosition)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShielderedBackAction(enemyPosition));
    }
    private IEnumerator ShielderedBackAction(Vector3 enemyPosition)
    {
        playerHealth.SetState(State.Vulnerable);

        CoroutineChangeAnimation(ShielderedBack, 0.05f);
        playerRigidbody.AddForce((transform.position - enemyPosition) * StaticValues.addForceTiny * 1.5f, ForceMode.Impulse);

      
        float shielderedBackDuration = 0.917f;

        yield return new WaitForSeconds(shielderedBackDuration);


        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }

    public void InvokeStaminaKnockedDown(Vector3 refinedFrom)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(StaminaKnockedDownAction(refinedFrom));
    }

    private IEnumerator StaminaKnockedDownAction(Vector3 refinedFrom1)
    {
        playerHealth.SetState(State.Vulnerable);
        isStaminaKnockedDown = true;

        CoroutineChangeAnimation(StaminaKnockedDown, 0.05f);

        float StaminaKnockedDownDuration = 1.5f;

        float backwardDuration = StaminaKnockedDownDuration * 0.583f;

        coroutineDuration = StaminaKnockedDownDuration;
        remainingTime = coroutineDuration;

        while(remainingTime > 0)
        {
            remainingTime -= Time.fixedDeltaTime;

            if(remainingTime > StaminaKnockedDownDuration - backwardDuration)
            {
                Vector3 moveDistance = -refinedFrom1 * movementSpeed * 0.5f * Time.fixedDeltaTime;
                playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
            }

            yield return new WaitForFixedUpdate();
        }

        isStaminaKnockedDown = false;

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }




    public void InvokeDIe()
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DieAction());
    }
    private  IEnumerator DieAction()
    {
        playerHealth.SetState(State.Invulnerable);

        CoroutineChangeAnimation(Die1, 0.05f);

        float EndOjbectDuration = 3.5f;

        yield return new WaitForSeconds(EndOjbectDuration);

        playerHealth.EndObject();
        yield break;
    }
    
    public void InvokeGrappedAction(string animationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        if(currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(GrappedAction(animationName, animationDuration, grappedTime, grappingPositionObject));
    }
    private IEnumerator GrappedAction(string animationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        playerHealth.SetState(State.Invulnerable);
        coroutineDuration = animationDuration;


        CoroutineChangeAnimation(animationName, 0.05f, grappedTime / coroutineDuration);


        remainingTime = coroutineDuration - grappedTime;


        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            yield return null;
        }


        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;
        ExcuteNextAction();
    }




    // @@StopCoroutineRoutine
    private void StopCoroutineRoutine(Coroutine currentCoroutine1)
    {
        StopCoroutine(currentCoroutine1);
        ResetRoutine();
    }
    private void ResetRoutine()
    {
        remainingTime = 0f;
        playerController.InvokeAttackButtonEndPhase();
        if (isHeavyAttackTriggered > 0)
        {
            isHeavyAttackTriggered -= 1;
        }

        nextAction = null;
        queuedMovingVector = Vector3.zero;

        isHeavyCharging = false;
        isItemUsing = false;

        isShieldingOn = false;
        isStaminaKnockedDown = false;



        weaponManager.currentWeapon.SetActive(true);
        weaponManager.potionObject.SetActive(false);
        hitBox.SetActive(false);
        weaponTrailer.SetActive(false);
        playerAttack.SetCurrentAttackByName(null);
        playerRigidbody.isKinematic = false;

        currentCoroutine = null;
    }





    // @@ ETC Actions
    public void InvokeShelterRestStartAction(Vector3 shelterPosition)
    {
        if(playerHealth.state == State.Normal)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(ShelterRestStartAction(shelterPosition));
        }
    }
    private IEnumerator ShelterRestStartAction(Vector3 shelterPosition)
    {
        playerHealth.SetState(State.Invulnerable);

        if (baseLayerCurrentState != ShelterRestDoing1)
        {
            BaseLayerChangeAnimation(ShelterRestStart, 0.1f);
        }


        coroutineDuration = 0.917f;

        remainingTime = coroutineDuration;

        playerHealth.SetState(State.Normal);
        currentCoroutine = null;
        remainingTime = 0f;

        yield break;
    }

}
