
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using static PlayerMovement;

public class FieldEnemy_Mob1_Movement : MonoBehaviour
{
    private FieldEnemy_Mob1 mob1;
    protected Animator enemyAnimator;
    protected Rigidbody enemyRigidbody;
    public HitBox_Enemy hitBox_Mob1_RightHand;
    public HitBox_Enemy hitBox_Mob1_LeftHand;
    public HitBox_Enemy hitBox_GrabBox;
    public GameObject grappingPosition;
    public Transform jawTransform;
    public GameObject bloodStreamPrefab;
    public Transform bloodInstantiateTransform;
    public GameObject tempEffectPrefab;


    protected NavMeshAgent navMeshAgent;
    private LayerMask attackTargets;

    protected float distanceToTarget = float.PositiveInfinity;
    protected Transform targetTransform;
    protected Vector3 flatDirectionToTarget;
    protected Quaternion faceTargetRotation;
    protected float diffAngle;
    protected float crossY;



    [HideInInspector]
    public string animationCurrentState;
    private string Idle = "Idle";
    private string Walking = "Walking";
    private string StandOffRight = "StandOffRight";
    private string StandOffLeft = "StandOffLeft";
    private string Die = "Die";

    private string Attack_Chain1_Right1 = "Attack_Chain1_Right1";
    private string Attack_Chain1_Left21 = "Attack_Chain1_Left2";
    private string Attack_Chain1_Right31 = "Attack_Chain1_Right3";
    private string Attack_CloseLeftThrust1 = "Attack_CloseLeftThrust";

    private string Attack_Grab1 = "Attack_Grab";
    private string Attack_Grab_Failed = "Attack_Grab_Failed";

    private string Attack_LeftThrust1 = "Attack_LeftThrust";

    protected bool isInLoop = false;
    protected IEnumerator nextCoroutine;
    [HideInInspector]
    public Coroutine currentCoroutine;



    //Distancable
    protected float Distancable_Attack_1 = 3f;
    protected float distancable_Attak_TopDown = 3f;
    protected float distancable_Attack_BottomUp = 3f;

    // CheckForDieCoroutine
    private bool isDieActionOn = false;


    // ForTesterSpace 
    [HideInInspector]
    public bool isTest = false;

    //CalStack
    private int calStack_Chain1 = 2;
    private int calStack_CloseLeftThrust = 2;
    private int calStack_Grab = 2;


    // PhysicalMaterial
    private Collider enemyCollider;

    //ForGrab
    private bool didGrabSucced;
    private LivingEntity grappingTarget;
    private Rigidbody grapTargetRigidbody;

    // ������ Collision ���� ó��
    private List<Collider> ignoreColliders = new List<Collider>();

    private float notFightTime = 0;
    private bool didBackToInitialPointStarted = false;

    // Audio
    private AudioSource movementAudio;
    private AudioSource attackAudio;
    public List<AudioClip> walkingSounds;
    public List<AudioClip> runningSounds;
    public List<AudioClip> jumpLandSounds;
    public AudioClip impactAudioClip_Axe;
    public AudioClip impactAudioClip_Body;
    public AudioClip attackSwingSound;
    public AudioClip groundSmashClip;
    public AudioClip jumpStartSounds;
    protected enum DetectState
    {
        NotDetect,
        Detect
    }
    protected DetectState detectState;


    private bool hasTarget
    {
        get
        {
            if (targetTransform != null  && ( distanceToTarget < mob1.fieldEnemyData.giveUpRange || notFightTime < mob1.fieldEnemyData.giveUpTime) )
            {
                return true;

            }
            return false;
        }
    }


    protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody>();
        mob1 = GetComponent<FieldEnemy_Mob1>();
        enemyCollider = GetComponent<Collider>();
        movementAudio = gameObject.AddComponent<AudioSource>();
        attackAudio = gameObject.AddComponent<AudioSource>();

        // ForTesterSpace
        if(TestScene_EnemyController.instance != null)
        {
            TestScene_EnemyController.instance.OnSetIsTest += SetIsTest;
            TestScene_EnemyController.instance.OnTestAction += TestAction;
            TestScene_EnemyController.instance.OnTestAction1 += TestAction1;
            TestScene_EnemyController.instance.OnTestAction2 += TestAction2;
            TestScene_EnemyController.instance.OnTestAction3 += TestAction3;
            TestScene_EnemyController.instance.OnTestAction4 += TestAction4;
            TestScene_EnemyController.instance.OnTestAction5 += TestAction5;
            TestScene_EnemyController.instance.OnTestAction6 += TestAction6;
            TestScene_EnemyController.instance.OnTestAction7 += TestAction7;
            TestScene_EnemyController.instance.OnTestAction8 += TestAction8;
            TestScene_EnemyController.instance.OnTestAction9 += TestAction9;
            TestScene_EnemyController.instance.OnTestAction10+= TestAction10;
            TestScene_EnemyController.instance.OnTestAction11 += TestAction11;
            TestScene_EnemyController.instance.OnTestAction12 += TestAction12;
            TestScene_EnemyController.instance.OnTestAction13 += TestAction13;
            TestScene_EnemyController.instance.OnTestAction14 += TestAction14;
            TestScene_EnemyController.instance.OnTestAction15 += TestAction15;
        }

    }

    private void Start()
    {
        mob1.SetState(State.Normal);
        hitBox_Mob1_RightHand.gameObject.SetActive(false);
        hitBox_Mob1_LeftHand.gameObject.SetActive(false);
        hitBox_GrabBox.gameObject.SetActive(false);
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }

        LayerMask combinedLayerMask = 0;

        attackTargets = CommonMethods.GetStringsToLayerMask(mob1.stringsOfAttackTarget);
        mob1.OnTakeDamage += OnTakeDamage;
    }
    public void OnReviveReset()
    {
        enemyRigidbody.useGravity = true;
        mob1.SetState(State.Normal);
        isInLoop = false;
        hitBox_Mob1_RightHand.gameObject.SetActive(false);
        hitBox_Mob1_LeftHand.gameObject.SetActive(false);
        hitBox_GrabBox.gameObject.SetActive(false);
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }
    public void OnOnfieldReset()
    {
        if(animationCurrentState != null)
        {
            CoroutineChangeAnimation(animationCurrentState, 0, 1);
        }

    }

    public void TurnOffObject()
    {
        enemyRigidbody.useGravity = false;
    }
    private void OnDisable()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
        isDieActionOn = false;
        targetTransform = null;
    }

    // PrimaryMethods
    private void IgnoreCollisions()
    {

        ignoreColliders.Clear();

        
        Collider[] colliders = Physics.OverlapSphere(transform.position, 6f, attackTargets); 
        
        if(colliders.Length != 0)
        {
            foreach(Collider collider in colliders)
            {
                ignoreColliders.Add(collider);

                Physics.IgnoreCollision(enemyCollider, collider, true); 
            }
        }
    }
    private void RestoreIgnoreCollisions()
    {
        if(ignoreColliders.Count != 0)
        {
            foreach(Collider collider in ignoreColliders)
            {
                Physics.IgnoreCollision(enemyCollider, collider, false);
            }
        }
    }



    //
    private void Update()
    {

        notFightTime += Time.deltaTime;


        UpdateState();
        UpdateTargetDistance();
        UpdateActionState();
    }

 
    protected virtual void UpdateState()
    {
        if (hasTarget)
        {
            detectState = DetectState.Detect;
        }
        else
        {
            detectState = DetectState.NotDetect;
        }
    }



    protected virtual void UpdateActionState()
    {
        //if (isTest) return;

        if (mob1.state == State.Normal)
        {
            if (detectState == DetectState.NotDetect)
            {

                if( Vector3.Distance(transform.position, mob1.initialPosition) > 3f)
                {
                    if (!didBackToInitialPointStarted)
                    {
                        if (currentCoroutine != null)
                        {
                            StopCoroutineRoutine(currentCoroutine);
                        }

                        currentCoroutine = StartCoroutine(BackToInitialPosition());
                    }
                }
                else
                {
                    if (currentCoroutine != null)
                    {
                        StopCoroutineRoutine(currentCoroutine);
                    }
                    ChangeAnimation(Idle, 0.1f);
                    if(mob1.suicidableState != SuicidableState.Suicidable)
                    {
                        mob1.suicidableState = SuicidableState.Suicidable;
                    }
                }

                DetectTarget();
                if (isInLoop)
                {
                    isInLoop = false;
                }
            }
            else if (detectState == DetectState.Detect)
            {
                if (didBackToInitialPointStarted)
                {
                    didBackToInitialPointStarted = false;
                }


                if (!isInLoop)
                {
                    notFightTime = 0;
                    isInLoop = true;
                    mob1.suicidableState = SuicidableState.NotSuicidable;
                    StartCoroutine(StartNextCoroutine());
                }
            }
        }
    }
    public void GetGrabSucced(LivingEntity newGrappingTarget, Rigidbody newGrapTargetRigidbody)
    {
        didGrabSucced = true;
        grappingTarget = newGrappingTarget;
        grapTargetRigidbody = newGrapTargetRigidbody;
    }
    public void OnTakeDamage(float damage)
    {
        notFightTime = 0;
    }
    private void OnAttack()
    {
        notFightTime = 0;
        calStack_Chain1++;
        calStack_CloseLeftThrust++;
        calStack_Grab++;
    }

    // DecideCoroutines
    protected virtual IEnumerator DecideCoroutines()
    {
        List<int> weights = CalculateWeights();
        List<IEnumerator> coroutines = GetCoroutines();

        int totalWeight = weights.Sum();

        if (totalWeight == 0)
        {
        }

        int randomWeightPoint = Random.Range(0, totalWeight);
        int accumulateWeight = 0;


        for (int i = 0; i < weights.Count; i++)
        {
            accumulateWeight += weights[i];
            if (randomWeightPoint < accumulateWeight)
            {
                return coroutines[i];
            }
        }

        return null;

    }

    protected List<int> CalculateWeights()
    {
        int weight_Approach = Cal_Apporach(distanceToTarget);
        int weight_StandOff = Cal_StandOff(distanceToTarget);
        int weight_Attack_Chain1 = Cal_Attack_Chain1(distanceToTarget);
        int weight_Attack_LeftThrust = Cal_Attack_LeftThrust(distanceToTarget);
        int weight_Attack_CloseLeftThrust = Cal_Attack_CloseLeftThrust(distanceToTarget);
        int weight_attack_Grab = Cal_Attack_Grab(distanceToTarget);

        return new List<int> { weight_Approach, weight_StandOff, weight_Attack_Chain1, weight_Attack_LeftThrust, weight_Attack_CloseLeftThrust, weight_attack_Grab };
    }

    protected List<IEnumerator> GetCoroutines()
    {
        return new List<IEnumerator>
        {   
            Approach(),
            StandOff(),
            Attack_Chain1(),
            Attack_LeftThrust(),
            Attack_CloseLeftThrust(),
            Attack_Grab()
        };
    }

    protected virtual int Cal_Apporach(float distanceToTarget1)
    {
        if (distanceToTarget1 >= 4.56f)
        {
            return 30;
        }
        else if (distanceToTarget1 < 4.57f && distanceToTarget1 >= 2.5f)
        {
            return 20;
        }
        else
        {
            return 0;
        }
    }
    private int Cal_StandOff(float distanceToTarget)
    {
        if(distanceToTarget >= 2.5f && distanceToTarget < 3.42f)
        {
            return 10;
        }
        else if(distanceToTarget < 2.5f)
        {
            return 18;
        }
        else
        {
            return 0;
        }
    }
    private int Cal_Attack_Chain1(float distanceToTarget)
    {
        float interpolation = Mathf.Clamp01((float)(calStack_Chain1 + 1) / 5);

        if(distanceToTarget < 2.5f)
        {
            return Mathf.RoundToInt( 50 * interpolation);
        }
        else
        {
            return 0;
        }
    }
    private int Cal_Attack_CloseLeftThrust(float distanceToTarget)
    {

        float interpolation = Mathf.Clamp01((float)(calStack_CloseLeftThrust + 1) / 5);
        if (distanceToTarget < 2.5f)
        {
            return Mathf.RoundToInt(35 * interpolation);
        }
        else
        {
            return 0;
        }
    }

    private int Cal_Attack_Grab(float distanceToTarget)
    {
        float interpolation = Mathf.Clamp01((float)(calStack_Grab + 1) / 5);

        if(distanceToTarget < 2.5f)
        {
            return Mathf.RoundToInt(30 * interpolation);
        }
        else if(distanceToTarget < 3.42f)
        {
            return Mathf.RoundToInt(20f * interpolation);
        }
        else
        {
            return 0;
        }
    }



    private int Cal_Attack_LeftThrust(float distanceToTarget)
    {
        if (distanceToTarget < 4.56f && distanceToTarget >= 2.5f)
        {
            return 20;
        }
        else
        {
            return 0;
        }
    }

    // Loop Coroutines

    private IEnumerator BackToInitialPosition()
    {
        didBackToInitialPointStarted = true;
        enemyRigidbody.interpolation = RigidbodyInterpolation.None;
        navMeshAgent.enabled = true;

        ChangeAnimation(Walking, 0.2f);
        bool didStepSound = false;
        float audioStepTimePivot = 0.333f;

        while (Vector3.Distance(transform.position, mob1.initialPosition) >= 2f)
        {
            if (navMeshAgent.enabled)
            {
                if (navMeshAgent.speed != mob1.enemyCommonData.walkingSpeed)
                {
                    navMeshAgent.speed = mob1.enemyCommonData.walkingSpeed;
                }
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(mob1.initialPosition);
                }
            }
            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;

            if (didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, audioStepTimePivot, audioStepTimePivot + 0.5f) == true)
            {
                movementAudio.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Count - 1)]);
                didStepSound = true;
            }
            else if (didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, audioStepTimePivot, audioStepTimePivot + 0.5f) == false)
            {
                didStepSound = false;
            }

            yield return new WaitForSeconds(3f);
        }

        Debug.Log(Vector3.Distance(transform.position, mob1.initialPosition));

        didBackToInitialPointStarted = false;
        navMeshAgent.enabled = false;
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        currentCoroutine = null;
        yield break;
    }

    protected virtual IEnumerator Approach()
    {
        enemyRigidbody.interpolation = RigidbodyInterpolation.None;
        navMeshAgent.enabled = true;

        float elapsedTime = 0f;
        float elapsedLim = 4f;

        ChangeAnimation(Walking, 0.1f);
        float audioStepTimePivot = 0.333f;
        bool didStepSound = false;


        while (elapsedTime < elapsedLim)
        {
            if (navMeshAgent.enabled)
            {
                if (navMeshAgent.speed != mob1.enemyCommonData.walkingSpeed)
                {
                    navMeshAgent.speed = mob1.enemyCommonData.walkingSpeed;
                }
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(targetTransform.position);
                }
            }


            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;

            if (didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, audioStepTimePivot, audioStepTimePivot + 0.5f) == true)
            {
                movementAudio.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Count - 1)]);
                didStepSound = true;
            }
            else if (didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, audioStepTimePivot, audioStepTimePivot + 0.5f) == false)
            {
                didStepSound = false;
            }



            if (distanceToTarget < 4f)
            {
                navMeshAgent.enabled = false;
                enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

                yield return null;

                StartCoroutine(StartNextCoroutine());
                yield break;

            }
            

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.enabled = false;
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        yield return null;

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    private IEnumerator StandOff()
    {
        float elapsedTime = 0;
        float duration;

        
        if (distanceToTarget >= 4f)
        {
            duration = Random.Range(1f, 2.5f);
        }
        else
        {
            duration = Random.Range(0.25f, 0.8f);
        }


        bool isRight;
        if(crossY > 0)
        {
            isRight = true;
            ChangeAnimation(StandOffRight, 0.1f, 1);
        }
        else
        {
            isRight = false;
            ChangeAnimation(StandOffLeft, 0.1f, 1);
        }

        float stepSoundTime = 0.3f;
        bool didStepSound = false;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 1.5f * Time.fixedDeltaTime));

            if (isRight)
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.right * mob1.enemyStatData.walkingSpeed * 0.8f * Time.fixedDeltaTime);
            }
            else
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position - transform.right * mob1.enemyStatData.walkingSpeed * 0.8f * Time.fixedDeltaTime);
            }

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;

            if (didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == true)
            {
                movementAudio.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Count - 1)]);
                didStepSound = true;
            }
            else if (didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == false)
            {
                didStepSound = false;
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(StartNextCoroutine());
        yield break;
    }


    private IEnumerator Attack_Chain1()
    {
        OnAttack();
        calStack_Chain1 = 0;

        CoroutineChangeAnimation(Attack_Chain1_Right1, 0.2f);

        float elapsedTime = 0;
        float duration = 2.658f;
        float strikeTime = 0.896f;
        float strikeEndTime = 1.06f;
        float nextPhaseCheckTime = 1.217f;

        bool didHitBoxOn = false;
        bool didHitBoxOff = false;
        bool didCheckNextPhase = false;

        List<EventSoundTime> eventSoundTime_Attack_Chain1 = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = 0.13f * duration, audioClip = walkingSounds[Random.Range(0,walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.341f * duration, audioClip = attackSwingSound, audioSource = attackAudio, volume = 0.5f, abcEnum = ABCEnum.A },
            new EventSoundTime{triggerTime = 0.418f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A },
            new EventSoundTime{triggerTime = 0.785f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A },
            new EventSoundTime{triggerTime = 0.921f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A }
        };
        eventSoundTime_Attack_Chain1.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));

        int eventSoundIndex = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime < strikeTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));

                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 1.14f);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * limDistanceToTarget * mob1.enemyStatData.walkingSpeed * 0.5f * Time.fixedDeltaTime);
            }

            if(elapsedTime > strikeTime && !didHitBoxOn)
            {
                hitBox_Mob1_RightHand.gameObject.SetActive(true);
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_RightHand.gameObject, false, GameObjectType.HitBox, true);
                    }
                }
                mob1.SetCurrentAttackByName("Attack_Chain1_Right1", hitBox_Mob1_RightHand);
                hitBox_Mob1_RightHand.SetImpactSound(impactAudioClip_Axe, 0.5f);

                didHitBoxOn = true;
            }

            if(elapsedTime > strikeTime && elapsedTime < strikeEndTime)
            {
                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 1.5f);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * limDistanceToTarget * mob1.enemyStatData.walkingSpeed * 0.4275f * Time.fixedDeltaTime);
            }


            if(elapsedTime > strikeEndTime && !didHitBoxOff)
            {
                hitBox_Mob1_RightHand.gameObject.SetActive(false);

                if(TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_RightHand.gameObject, false, GameObjectType.HitBox, false);
                    }
                }

                didHitBoxOff = true;
            }

            if(elapsedTime > nextPhaseCheckTime && !didCheckNextPhase)
            {
                if(distanceToTarget < 2.5f)
                {
                    nextCoroutine = Attack_Chain1_Left2();
                    StartCoroutine(StartNextCoroutine());
                    yield break;
                }

                didCheckNextPhase = true;
            }

            //EventSOundTime
            while (eventSoundIndex < eventSoundTime_Attack_Chain1.Count && elapsedTime >= eventSoundTime_Attack_Chain1[eventSoundIndex].triggerTime)
            {

                EventSoundTime currentEventSoundTime = eventSoundTime_Attack_Chain1[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSoundTime.audioSource, currentEventSoundTime.audioClip, currentEventSoundTime.volume);
                
                eventSoundIndex++;
            }
            yield return new WaitForFixedUpdate();
        }
        

        StartCoroutine(StartNextCoroutine());
        yield break;
    }
    private IEnumerator Attack_Chain1_Left2()
    {
        OnAttack();
        CoroutineChangeAnimation(Attack_Chain1_Left21, 0.2f);

        float elapsedTime = 0;
        float duration = 2.517f;

        float strikeTime = 0.722f;
        bool didHitBoxOn = false;

        float strikeEndTime = 0.883f;
        bool didHitBoxOff = false;

        float nextPhaseCheckTime = 1.06f;
        bool didCheckNextPhase = false;

        List<EventSoundTime> eventSoundTime_Attack_Chain1_Left2 = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = 0.287f * duration, audioClip = attackSwingSound, audioSource = attackAudio, volume = 0.5f, abcEnum = ABCEnum.A },
            new EventSoundTime{triggerTime = 0.289f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.643f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.9f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
        };
        eventSoundTime_Attack_Chain1_Left2.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        int eventSoundIndex = 0;


        while(elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;


            if(elapsedTime < strikeTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));

                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 2f);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * limDistanceToTarget * mob1.enemyStatData.walkingSpeed  * 0.5f * Time.fixedDeltaTime);
            }

            if(elapsedTime >= strikeTime && !didHitBoxOn)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(true);
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, true);
                    }
                }
                mob1.SetCurrentAttackByName("Attack_Chain1_Left2", hitBox_Mob1_LeftHand);
                hitBox_Mob1_LeftHand.SetImpactSound(impactAudioClip_Axe, 0.5f);

                didHitBoxOn = true;
            }

            if (elapsedTime > strikeTime && elapsedTime < strikeEndTime)
            {
                float limDistancetoTarget = Mathf.Clamp(distanceToTarget, 0, 1.5f);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * limDistancetoTarget * mob1.enemyStatData.walkingSpeed * 0.4275f * Time.fixedDeltaTime);
            }

            if (elapsedTime >= strikeEndTime && !didHitBoxOff)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(false);

                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, false);
                    }
                }

                didHitBoxOff = true;
            }

            if(elapsedTime > nextPhaseCheckTime && !didCheckNextPhase)
            {
                if(distanceToTarget < 2.5f)
                {
                    float randomFloat = Random.Range(0f, 1f);
                    if( randomFloat < 0.4f) 
                    {
                        nextCoroutine = Attaack_Chain1_Right3();
                        StartCoroutine(StartNextCoroutine());
                        yield break;
                    }
                }

                didCheckNextPhase = true;
            }

            // EventSoundTime
            while (eventSoundIndex < eventSoundTime_Attack_Chain1_Left2.Count && elapsedTime >= eventSoundTime_Attack_Chain1_Left2[eventSoundIndex].triggerTime)
            {

                EventSoundTime currentEventSoundTime = eventSoundTime_Attack_Chain1_Left2[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSoundTime.audioSource, currentEventSoundTime.audioClip, currentEventSoundTime.volume);

                eventSoundIndex++;
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    private IEnumerator Attaack_Chain1_Right3()
    {
        OnAttack();
        CoroutineChangeAnimation(Attack_Chain1_Right31, 0.2f);

        float elapsedTime = 0;
        float duration = 2.15f;

        float strikeTime = 0.836f;
        bool didHitBoxOn = false;

        float smashFloorTime = 0.859f;
        bool didSmashFloor = false;

        float strikeEndTime = 0.992f;
        bool didHitBoxOff = false;

        float residualSpeed0Time = 1.4f;
        float lastMovementSpeed = 0;

        List<EventSoundTime> eventSoundTime_Attack_Chain1_Right3 = new List<EventSoundTime>
        {
            new EventSoundTime {triggerTime = 0.356f * duration, audioClip = attackSwingSound, audioSource = attackAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime {triggerTime = 0.408f * duration, audioClip = groundSmashClip, audioSource = attackAudio, volume = 1f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.998f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A}
        };
        eventSoundTime_Attack_Chain1_Right3.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        int eventSoundIndex = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;


            if(elapsedTime < strikeTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));


                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 2f);
                lastMovementSpeed = limDistanceToTarget * mob1.enemyStatData.walkingSpeed * 0.5f;
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * Time.fixedDeltaTime);
            }

            if(elapsedTime >= strikeTime && !didHitBoxOn)
            {
                hitBox_Mob1_RightHand.gameObject.SetActive(true);

                if(TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_RightHand.gameObject, false, GameObjectType.HitBox, true);
                    }
                }
                mob1.SetCurrentAttackByName("Attack_Chain1_Right3", hitBox_Mob1_RightHand);
                hitBox_Mob1_RightHand.SetImpactSound(impactAudioClip_Axe, 0.5f);

                didHitBoxOn = true;
            }
            
            if(elapsedTime >= smashFloorTime && !didSmashFloor)
            {
                GameManager.instance.cinemachineController.InvokeCameraImpulse(0.1f, 0.3f);

                Vector3 instantiatePosition = hitBox_Mob1_RightHand.transform.position;
                instantiatePosition.y += 2.5f;
                if (Physics.Raycast(instantiatePosition, Vector3.down, out RaycastHit hit, 5f))
                {
                    Vector3 spawnPosition = hit.point;
                    spawnPosition.y -= 2.64f;

                    GameObject tempEffect = Instantiate(tempEffectPrefab, spawnPosition, Quaternion.identity);
                    Destroy(tempEffect, 3f);
                    if (TestScene_ReplayManager.instance != null)
                    {
                        if (TestScene_ReplayManager.instance.isRecording)
                        {
                            TestScene_ReplayManager.instance.RecordEffect(tempEffectPrefab, spawnPosition, Quaternion.identity, 3f);
                        }
                    }
                }



                didSmashFloor = true;
            }


            if(elapsedTime >= strikeEndTime && !didHitBoxOff)
            {
                hitBox_Mob1_RightHand.gameObject.SetActive(false);

                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_RightHand.gameObject, false, GameObjectType.HitBox, false);
                    }
                }

                didHitBoxOff = true;
            }

            if(elapsedTime >= strikeEndTime && elapsedTime < residualSpeed0Time)
            {
                float interpolation = Mathf.Clamp((residualSpeed0Time - elapsedTime) / (residualSpeed0Time - strikeEndTime), 0, 1);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * interpolation * Time.fixedDeltaTime);
            }

            //EventSoundTime
            while(eventSoundIndex < eventSoundTime_Attack_Chain1_Right3.Count && elapsedTime >= eventSoundTime_Attack_Chain1_Right3[eventSoundIndex].triggerTime)
            {
                EventSoundTime currentEventSound = eventSoundTime_Attack_Chain1_Right3[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSound.audioSource, currentEventSound.audioClip, currentEventSound.volume);

                eventSoundIndex++;
            }

            yield return new WaitForFixedUpdate();
        }


        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    
    private IEnumerator Attack_CloseLeftThrust()
    {
        OnAttack();
        calStack_CloseLeftThrust = 0;

        CoroutineChangeAnimation(Attack_CloseLeftThrust1, 0.25f);

        float elapsedTime = 0;
        float duration = 2.808f;

        float strikeTime = 1.109f;
        bool didHitBoxOn = false;

        float strikeEndTime = 1.309f;
        bool didHitBoxOff = false;

        float rotateStopTime = 1.544f;


        float lastMovementSpeed = 0;
        float moveStopTime = 1.637f;

        List<EventSoundTime> eventSoundTime_Attack_CloseLeftThrust = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = 0.383f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.399f * duration, audioClip = attackSwingSound, audioSource = attackAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.514f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.8f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.948f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f, abcEnum = ABCEnum.A},
        };
        eventSoundTime_Attack_CloseLeftThrust.Sort ((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        int eventSoundIndex = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            if(elapsedTime < strikeTime)
            {
                float limDistance = Mathf.Clamp(distanceToTarget, 0, 2f);
                lastMovementSpeed = limDistance * mob1.enemyStatData.walkingSpeed * 0.5f;
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * Time.fixedDeltaTime);
            }
            if (elapsedTime < rotateStopTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));
            }

            if (elapsedTime >= strikeTime && !didHitBoxOn)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(true);

                if(TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, true);
                    }
                }
                mob1.SetCurrentAttackByName("Attack_CloseLeftThrust", hitBox_Mob1_LeftHand);
                hitBox_Mob1_LeftHand.SetImpactSound(impactAudioClip_Axe, 0.5f);

                didHitBoxOn = true;
            }

            if(elapsedTime >= strikeEndTime && !didHitBoxOff)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(false);

                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, false);
                    }
                }

                didHitBoxOff = true;
            }


            if(elapsedTime >= strikeTime && elapsedTime < moveStopTime)
            {
                float interpolation = Mathf.Clamp((moveStopTime - elapsedTime) / (moveStopTime - strikeTime), 0, 1);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * interpolation * Time.fixedDeltaTime);
            }

            while(eventSoundIndex < eventSoundTime_Attack_CloseLeftThrust.Count && elapsedTime >= eventSoundTime_Attack_CloseLeftThrust[eventSoundIndex].triggerTime)
            {
                EventSoundTime currentEventSound = eventSoundTime_Attack_CloseLeftThrust[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSound.audioSource, currentEventSound.audioClip, currentEventSound.volume);

                eventSoundIndex++;
            }


            yield return new WaitForFixedUpdate();
        }


        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    private IEnumerator Attack_Grab()
    {
        OnAttack();
        calStack_Grab = 0;

        CoroutineChangeAnimation(Attack_Grab1, 0.2f);


        float elapsedTime = 0;
        float duration = 6.308f;
        float failDuration = 3.983f;

        float jumpTime = 1.292f;
        bool didItJump = false;

        bool didHitBoxOn = false;


        float grabEndTime = 1.442f;
        bool didHitBoxOff = false;

        bool didgrabSuccedThisTime = false;
        didGrabSucced = false;

        grappingTarget = null;
        grapTargetRigidbody = null;

        float jumpFlyingEndTime = 2.213f;
        bool didJumpFlyingEnd = false;

        float damage = mob1.enemyStatData.attack_Grab_Damage;
        float restoreHealth = damage * 1f;

        float firstDamageInputTime = 2.561f;
        float lastDamageInputTime = 3.372f;
        int damageCount = 0;
        int damageTotalCount = 3;
        float damageInterval = (lastDamageInputTime - firstDamageInputTime) / (damageTotalCount - 1);

        float flyingAgainStartTime = 3.736f;
        bool didFlyingAgainStart = false;


        //Sound

        List<EventSoundTime> Attack_Grab_timedSoundEvents = new List<EventSoundTime>
        { 
            new EventSoundTime{triggerTime = 0.128f * duration, audioSource = movementAudio, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], volume = 0.5f, abcEnum = ABCEnum.A,},
            new EventSoundTime{triggerTime = 0.203f * duration, audioSource = movementAudio, audioClip = jumpLandSounds[Random.Range(0, jumpLandSounds.Count)], volume = 1f, abcEnum = ABCEnum.A},
            new EventSoundTime{triggerTime = 0.339f * duration, audioSource = movementAudio, audioClip = jumpLandSounds[Random.Range(0, jumpLandSounds.Count)], volume = 1f, abcEnum = ABCEnum.A,},
            new EventSoundTime{triggerTime = 0.579f * duration, audioSource = movementAudio, audioClip = impactAudioClip_Axe, volume = 0.5f, abcEnum = ABCEnum.B},
            new EventSoundTime {  triggerTime = 0.898f * duration, audioSource = movementAudio, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], volume = 0.5f, abcEnum = ABCEnum.B,   },
            new EventSoundTime{  triggerTime = 0.998f * duration, audioSource = movementAudio, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], volume = 0.5f, abcEnum = ABCEnum.B,},
            new EventSoundTime {   triggerTime = 0.675f * duration, audioSource = movementAudio, audioClip = jumpLandSounds[Random.Range(0, jumpLandSounds.Count)], volume = 1f,abcEnum = ABCEnum.B,   },
            new EventSoundTime { triggerTime = 0.681f * failDuration, audioSource = movementAudio, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], volume = 0.5f, abcEnum = ABCEnum.C, },
            new EventSoundTime { triggerTime = 0.929f * failDuration, audioSource = movementAudio, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], volume = 0.5f,  abcEnum = ABCEnum.C, }
        };
        Attack_Grab_timedSoundEvents.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));

        int eventSoundIndex = 0;

        List<ABCEnum> playableABCEnums = new List<ABCEnum>
        {
            ABCEnum.A, ABCEnum.B, ABCEnum.C
        };


        while(elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            if(elapsedTime < jumpTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 5f * Time.fixedDeltaTime));

                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 1.5f);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * limDistanceToTarget * mob1.enemyStatData.walkingSpeed * 0.57f * Time.fixedDeltaTime);
            }

            if(elapsedTime >= jumpTime && !didItJump)
            {
                IgnoreCollisions();

                enemyRigidbody.AddForce(transform.forward * StaticValues.addForce5f * 1.466f, ForceMode.Impulse);

                didItJump = true;
            }

            if(elapsedTime >= jumpTime && !didHitBoxOn)
            {
                hitBox_GrabBox.gameObject.SetActive(true);
                if(TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_GrabBox.gameObject, false, GameObjectType.HitBox, true);
                    }
                }

                didHitBoxOn = true;
            }

            if (elapsedTime >= grabEndTime && !didHitBoxOff)
            {
                hitBox_GrabBox.gameObject.SetActive(false);
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_GrabBox.gameObject, false, GameObjectType.HitBox, false);
                    }
                }

                didHitBoxOff = true;
            }

            // CommonToSplitPoint
            if (elapsedTime >= jumpFlyingEndTime && !didJumpFlyingEnd)
            {
                didJumpFlyingEnd = true;

                if (didGrabSucced)
                {
                    playableABCEnums.Remove(ABCEnum.C);
                }
                else
                {
                    CoroutineChangeAnimation(Attack_Grab_Failed, 0.05f, elapsedTime / failDuration);
                    duration = failDuration;
                    RestoreIgnoreCollisions();

                    // EventSoundData  ����
                    playableABCEnums.Remove(ABCEnum.B);
                }
            }


            // If Grab Succed
            if (didGrabSucced)
            {
                if (!didgrabSuccedThisTime)
                {
                    grappingTarget.InvokeGrappedAction("GrabFloorKick", 5.917f, elapsedTime, grappingPosition);
                    grapTargetRigidbody.isKinematic = true;
                    grappingTarget.transform.SetParent(grappingPosition.transform, true);

                    didgrabSuccedThisTime = true;
                }

                if(elapsedTime < flyingAgainStartTime && didgrabSuccedThisTime)
                {
                    grappingTarget.transform.localPosition = Vector3.zero;
                    Vector3 lookPosition = transform.position - grappingTarget.transform.position;
                    lookPosition.y = 0;
                    grapTargetRigidbody.transform.rotation = Quaternion.LookRotation(lookPosition);
                }


                if (elapsedTime >= firstDamageInputTime + damageInterval * damageCount && damageCount < damageTotalCount)
                {
                    Vector3 hitPoint = grappingTarget.GetComponent<Collider>().ClosestPoint(jawTransform.position);

                    grappingTarget.GetFixedDamage(damage, hitPoint, new Vector3(0,1,0));
                    mob1.RestoreHealth(restoreHealth);
                    CommonMethods.AudioPlayOneShot(attackAudio, impactAudioClip_Body, 0.5f);

                    damageCount++;
                }


                if (elapsedTime >= flyingAgainStartTime && !didFlyingAgainStart)
                {
                    grapTargetRigidbody.isKinematic = false;
                    grappingTarget.transform.SetParent(null);

                    enemyRigidbody.AddForce(-transform.forward * StaticValues.addForce5f * 0.7f, ForceMode.Impulse);

                    didFlyingAgainStart = true;
                }

            }


            //SoundEvents
            while(eventSoundIndex < Attack_Grab_timedSoundEvents.Count && elapsedTime >= Attack_Grab_timedSoundEvents[eventSoundIndex].triggerTime)
            {
                if (playableABCEnums.Contains(Attack_Grab_timedSoundEvents[eventSoundIndex].abcEnum))
                {
                    EventSoundTime currentEventSoundTime = Attack_Grab_timedSoundEvents[eventSoundIndex];
                    CommonMethods.AudioPlayOneShot(currentEventSoundTime.audioSource, currentEventSoundTime.audioClip, currentEventSoundTime.volume);
                }

                eventSoundIndex++;
            }
            yield return new WaitForFixedUpdate();
        }

        //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
        RestoreIgnoreCollisions();

        StartCoroutine(StartNextCoroutine());
        yield break;
    }
    

    private IEnumerator Attack_LeftThrust()
    {
        OnAttack();
        CoroutineChangeAnimation(Attack_LeftThrust1, 0.2f);

        float elapsedTime = 0;
        float duration = 4.458f;
        float strikeTime = 1.957f;
        float strikeEndTime = 2.157f;

        bool didHitBoxOn = false;
        bool didHitBoxOff = false;

        float beforeStrikeMovementSpeed = 0;


        List<EventSoundTime> eventSoundTime_Attack_LeftThrust = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = 0.157f * duration, audioClip = runningSounds[Random.Range(0, runningSounds.Count)], audioSource = movementAudio, volume = 0.6f},
            new EventSoundTime{triggerTime = 0.306f * duration, audioClip = runningSounds[Random.Range(0, runningSounds.Count)], audioSource = movementAudio, volume = 0.7f},
            new EventSoundTime{triggerTime = 0.426f * duration, audioClip = runningSounds[Random.Range(0, runningSounds.Count)], audioSource = movementAudio, volume = 0.8f},
            new EventSoundTime{triggerTime = 0.439f * duration, audioClip = attackSwingSound, audioSource = attackAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = 0.514f * duration, audioClip = runningSounds[Random.Range(0, runningSounds.Count)], audioSource = movementAudio, volume = 0.6f},
            new EventSoundTime{triggerTime = 0.748f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = 0.883f * duration, audioClip = walkingSounds[Random.Range(0, walkingSounds.Count)], audioSource = movementAudio, volume = 0.5f},
        };
        int eventSoundIndex = 0;

        while (elapsedTime < duration)
        {

            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime < strikeTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 6f * Time.fixedDeltaTime));

                float interpolation = Mathf.Clamp((elapsedTime + strikeTime / 3) / strikeTime, 0.25f, 1);
                float limDistanceToTarget = Mathf.Clamp(distanceToTarget, 0, 3f);

                beforeStrikeMovementSpeed = limDistanceToTarget * mob1.enemyStatData.walkingSpeed * 1.14f * interpolation;
 
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * beforeStrikeMovementSpeed * Time.fixedDeltaTime);
            }


            if (elapsedTime > strikeTime && !didHitBoxOn)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(true);
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, true);
                    }
                }
                mob1.SetCurrentAttackByName("Attack_LeftHand", hitBox_Mob1_LeftHand);
                hitBox_Mob1_LeftHand.SetImpactSound(impactAudioClip_Axe, 0.5f);

                didHitBoxOn = true;
            }

            if (elapsedTime > strikeTime && elapsedTime < (strikeTime + strikeEndTime) / 2)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 6f * Time.fixedDeltaTime));
            }

            if (elapsedTime > strikeTime && elapsedTime < strikeEndTime)
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * beforeStrikeMovementSpeed * 1.25f * Time.fixedDeltaTime);
            }

            if (elapsedTime > strikeEndTime && !didHitBoxOff)
            {
                hitBox_Mob1_LeftHand.gameObject.SetActive(false);
                if (TestScene_ReplayManager.instance != null)
                {
                    if (TestScene_ReplayManager.instance.isRecording)
                    {
                        TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox_Mob1_LeftHand.gameObject, false, GameObjectType.HitBox, false);
                    }
                }
                didHitBoxOff = true;
            }

            if (elapsedTime >= strikeEndTime && elapsedTime < duration * 0.633f)
            {
                float interpolation = Mathf.Clamp((duration * 0.633f - elapsedTime) / (duration * 0.633f - strikeEndTime), 0, 1);


                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * beforeStrikeMovementSpeed * 1.25f * interpolation * Time.fixedDeltaTime);
            }

            while(eventSoundIndex < eventSoundTime_Attack_LeftThrust.Count && elapsedTime >= eventSoundTime_Attack_LeftThrust[eventSoundIndex].triggerTime)
            {
                EventSoundTime currentEventSound = eventSoundTime_Attack_LeftThrust[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSound.audioSource, currentEventSound.audioClip, currentEventSound.volume);

                eventSoundIndex++;
            }


            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    // @@ Damaged

    public void InvokeStaggered(DirectionType directionType1, Vector3 fromNormal1, Quaternion attackDirection1)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(StaggeredAction(directionType1, fromNormal1, attackDirection1));
    }
    private IEnumerator StaggeredAction(DirectionType directionType1, Vector3 fromNormal1, Quaternion attackDirection1)
    {
        float flatDiffAngle;
        float crossY;
        Vector3 refinedRotatedVector;

        if (directionType1 == DirectionType.Normal)
        {
            Vector3 refinedFrom = new Vector3(fromNormal1.x, 0, fromNormal1.z).normalized;
            Vector3 rotatedVector = attackDirection1 * refinedFrom;

            refinedRotatedVector = new Vector3(rotatedVector.x, 0, rotatedVector.z);

            flatDiffAngle = Vector3.Angle(transform.forward, refinedRotatedVector);
            crossY = Vector3.Cross(transform.forward, refinedRotatedVector).y;
        }
        else
        {
            flatDiffAngle = 0f;
            crossY = 0f;
            refinedRotatedVector = new Vector3(fromNormal1.x, 0, fromNormal1.z).normalized * 0.5f;
        }

        string animationName;

        if (directionType1 == DirectionType.TopDown)
        {
            animationName = Idle;
        }
        else if (directionType1 == DirectionType.BottomUp)
        {
            animationName = Idle;
        }
        else if (flatDiffAngle < 45f)
        {
            animationName = Idle;
        }
        else if (flatDiffAngle >= 45f && flatDiffAngle < 135f)
        {
            if (crossY < 0)
            {
                animationName = Idle;
            }
            else
            {
                animationName = Idle;
            }
        }
        else
        {
            animationName = Idle;
        }

        CoroutineChangeAnimation(animationName, 0.05f);

        enemyRigidbody.AddForce(-refinedRotatedVector * StaticValues.addForceTiny * 1.3f, ForceMode.Impulse);


        float staggerDuration = 0.583f;
        float elapsedTime = 0f;

        while (elapsedTime < staggerDuration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    public void InvokeKnockedDown(DirectionType directionType1, Vector3 fromNormal1)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(KnockedDownAction(directionType1, fromNormal1));
    }
    private IEnumerator KnockedDownAction(DirectionType directionType1, Vector3 fromNormal1)
    {
        mob1.SetState(State.Invulnerable);


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
            animationName = Idle;
        }
        else if (directionType1 == DirectionType.BottomUp)
        {
            animationName = Idle;
        }
        else if (flatDiffAngle <= 90f)
        {
            animationName = Idle;
        }
        else
        {
            animationName = Idle;
        }

        CoroutineChangeAnimation(animationName, 0.05f);

        enemyRigidbody.AddForce(-refinedFrom * StaticValues.addForce5f, ForceMode.Impulse);

        if (directionType1 == DirectionType.Normal)
        {
            if (flatDiffAngle <= 90f)
            {
                enemyRigidbody.MoveRotation(Quaternion.LookRotation(refinedFrom));
            }
            else
            {
                enemyRigidbody.MoveRotation(Quaternion.LookRotation(-refinedFrom));
            }
        }

        float knockedDownDuration = 1.542f;

        float elapsedTime = 0f;

        while (elapsedTime < knockedDownDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mob1.SetState(State.Normal);

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    public void ShielderedBack(Vector3 targetPosition)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(ShielderedBackAction(targetPosition));
    }
    private IEnumerator ShielderedBackAction(Vector3 targetPosition1)
    {
        CoroutineChangeAnimation(Idle, 0.05f);

        enemyRigidbody.AddForce((transform.position - targetPosition1) * StaticValues.addForceTiny * 1.5f, ForceMode.Impulse);

        float shielderedBackDuration = 0.583f;

        yield return new WaitForSeconds(shielderedBackDuration);

        StartCoroutine(StartNextCoroutine());
        yield break;
    }

    public void InvokeDieAction()
    {
        StartCoroutine(DieCheckCoroutine());
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
    }
    private IEnumerator DieCheckCoroutine()
    {
        while (true)
        {
            if (currentCoroutine != null)
            {
                StopCoroutineRoutine(currentCoroutine);
                mob1.SetState(State.Invulnerable);
                
            }
            if (!isDieActionOn)
            {
                if(currentCoroutine != null)
                {
                    StopCoroutineRoutine(currentCoroutine);
                }

                yield return null;
                StartCoroutine(DieAction());
            }
            else
            {
                break;
            }
            yield return null;

        }
    }
    protected virtual IEnumerator DieAction()
    {
        isDieActionOn = true;
        mob1.SetState(State.Invulnerable);


        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        float currentNormalizedTime = stateInfo.normalizedTime % 1f;
        enemyAnimator.Play(animationCurrentState, 0, currentNormalizedTime);
        CoroutineChangeAnimation(Die, 0.02f);



        float elapsedTIme = 0;
        float duration = 1.883f;
        float bloodStreamInstantiateTime = 1.4f;
        bool didBloodStreamInstantiated = false;

        List<EventSoundTime> eventSoundTime_DieAction = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = 0.582f * duration, audioClip = jumpLandSounds[Random.Range(0, jumpLandSounds.Count)], audioSource = movementAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = 0.992f * duration, audioClip = jumpLandSounds[Random.Range(0, jumpLandSounds.Count)], audioSource = movementAudio, volume = 0.5f}
        };
        int eventSoundIndex = 0;

        while (elapsedTIme < duration)
        {
            elapsedTIme += Time.deltaTime;

            if(animationCurrentState != Die)
            {
                CoroutineChangeAnimation(Die, 0.02f);
            }

            
            if(elapsedTIme >= bloodStreamInstantiateTime && !didBloodStreamInstantiated)
            {

                Vector3 instantiatePosition = bloodInstantiateTransform.position;
                instantiatePosition.y += 2.5f;


                if (Physics.Raycast(instantiatePosition, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Terrain")))
                {
                    Vector3 spawnPosition = hit.point;
                    spawnPosition.y += 0f;

                    GameObject bloodStream = Instantiate(bloodStreamPrefab, spawnPosition, Quaternion.LookRotation(-transform.position));
                    Destroy(bloodStream, 20);
                    if (TestScene_ReplayManager.instance != null)
                    {
                        if (TestScene_ReplayManager.instance.isRecording)
                        {
                            TestScene_ReplayManager.instance.RecordEffect(bloodStreamPrefab, spawnPosition, Quaternion.LookRotation(-transform.position), 20f);
                        }
                    }
                }

                didBloodStreamInstantiated = true;
            }
            
            while(eventSoundIndex < eventSoundTime_DieAction.Count && elapsedTIme >= eventSoundTime_DieAction[eventSoundIndex].triggerTime)
            {
                EventSoundTime currentEventSound = eventSoundTime_DieAction[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(currentEventSound.audioSource, currentEventSound.audioClip, currentEventSound.volume);

                eventSoundIndex++;
            }

            yield return null;
        }

        mob1.TurnOffOBject();

        yield break;
    }
    public void InvokeGrappedAction(string animationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        if (currentCoroutine != null)
        {
            StopCoroutineRoutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(GrappedAction(animationName, animationDuration, grappedTime, grappingPositionObject));
        Debug.Log("Check");
    }
    private IEnumerator GrappedAction(string animationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        mob1.SetState(State.Invulnerable);
        float coroutineDuration = animationDuration;


        CoroutineChangeAnimation(animationName, 0.05f, grappedTime / coroutineDuration);


        float remainingTime = coroutineDuration - grappedTime;


        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            yield return null;
        }


        mob1.SetState(State.Normal);
        StartCoroutine(StartNextCoroutine());
        yield break;
    }


    // primary Methods
    private void ChangeAnimation(string newState, float blendTime, int usingNormalizedTime = 0)
    {
        if (newState == animationCurrentState)
        {
            return;
        }

        if (usingNormalizedTime == 1)
        {
            AnimatorStateInfo currentStateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float currentNormalizedTime = currentStateInfo.normalizedTime % 1;

            enemyAnimator.CrossFade(newState, blendTime, 0, currentNormalizedTime);
            animationCurrentState = newState;


            if(TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(false, 0, newState, blendTime, currentNormalizedTime);
                }
            }
        }
        else
        {
            enemyAnimator.CrossFade(newState, blendTime, 0);
            animationCurrentState = newState;

            if (TestScene_ReplayManager.instance != null)
            {
                if (TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordAnimationData(false, 0, newState, blendTime, 0);
                }
            }
        }
    }
    private void CoroutineChangeAnimation(string newState, float newBlendTime, float newNormalizedTime = 0)
    {
        enemyAnimator.CrossFade(newState, newBlendTime, 0, newNormalizedTime);
        animationCurrentState = newState;

        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordAnimationData(false, 0, newState, newBlendTime, newNormalizedTime);
            }
        }
    }
    protected virtual void UpdateTargetDistance()
    {
        if (targetTransform != null)
        {
            Vector3 directionToTarget = targetTransform.position - transform.position;
            flatDirectionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z);

            //result
            if (directionToTarget != Vector3.zero)
            {
                distanceToTarget = flatDirectionToTarget.magnitude;
                faceTargetRotation = Quaternion.LookRotation(flatDirectionToTarget.normalized);
                diffAngle = Vector3.Angle(transform.forward, flatDirectionToTarget);
                crossY = Vector3.Cross(transform.forward, flatDirectionToTarget).y;
            }
            else
            {
                distanceToTarget = float.PositiveInfinity;
            }
        }
    }

    private bool CheckIsDiffAngle(float threshold)
    {
        return diffAngle > threshold;
    }


    private void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, mob1.enemyCommonData.detectRange, attackTargets);

        float targetDistance = Mathf.Infinity;
        int targetInt = -1;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distanceToEntity = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (targetDistance > distanceToEntity)
            {
                targetDistance = distanceToEntity;
                targetInt = i;
            }
        }
        if (targetInt > -1)
        {
            targetTransform = colliders[targetInt].transform;
        }
    }


    // @@NextCoroutine
    protected virtual IEnumerator StartNextCoroutine()
    {
        if (mob1.dead) yield break;

        if (isTest) yield break;

        while (true)
        {
            if (nextCoroutine != null)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutineRoutine(currentCoroutine);
                }

                currentCoroutine = StartCoroutine(nextCoroutine);
                nextCoroutine = null;
                yield break;

            }
            else
            {
                nextCoroutine = DecideCoroutines();
            }

            yield return null;
        }
    }


    protected virtual IEnumerator StartRunningCoroutine()
    {
        if (mob1.dead) yield break;

        if (isTest) yield break;

        while (true)
        {
            if (nextCoroutine != null)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutineRoutine(currentCoroutine);
                }

                currentCoroutine = StartCoroutine(nextCoroutine);
                nextCoroutine = null;
                yield break;

            }
            else
            {
                nextCoroutine = DecideCoroutines();
            }

            yield return null;
        }
    }

    protected virtual void StopCoroutineRoutine(Coroutine currentCoroutine)
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;

        mob1.SetState(State.Normal);

        hitBox_Mob1_RightHand.gameObject.SetActive(false);
        hitBox_Mob1_LeftHand.gameObject.SetActive(false);
        hitBox_GrabBox.gameObject.SetActive(false);

        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }

        RestoreIgnoreCollisions();
    }

    public void SetIsTest()
    {
        if (isTest)
        {
            isInLoop = false;

            isTest = false;
        }
        else
        {
            isTest = true;
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }
    }
    public void TestAction()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        ChangeAnimation(Idle, 0.2f);
    }
    public void TestAction1()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Approach());
    }
    public void TestAction2()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(StandOff());
    }
    public void TestAction3()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_Chain1());
    }
    public void TestAction4()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_Chain1_Left2());
    }
    public void TestAction5()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attaack_Chain1_Right3());
    }
    public void TestAction6()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_Grab());
    }
    public void TestAction7()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_CloseLeftThrust());
    }
    public void TestAction8()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_LeftThrust());
    }
    public void TestAction9()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DieAction());
    }
    public void TestAction10()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction11()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction12()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction13()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction14()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }
    public void TestAction15()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
}

