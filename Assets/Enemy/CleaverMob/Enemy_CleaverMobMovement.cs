
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_CleaverMobMovement : MonoBehaviour
{
    //@@Common
    private Coroutine fixedUpdateCoroutine;
    private float fixedUpdateInterval = 0.4f;
    private Rigidbody enemyRigidbody;
    private Collider enemyCollider;
    private List<Collider> ignoreColliders = new List<Collider>();
    private LayerMask detectLayers;
    private Transform targetTransform;
    private Quaternion faceTargetRotation;
    private float diffAngle;
    private float crossY;
    private float distanceToTarget = float.PositiveInfinity;
    private NavMeshAgent navMeshAgent;
    private Animator enemyAnimator;
    [HideInInspector]
    public string animationCurrentState;
    private bool isInLoop = false;
    [HideInInspector]
    public Coroutine currentCoroutine;
    private IEnumerator nextCoroutine;
    private bool isDieActionOn = false;
    // @@ForTesterSpace 
    [HideInInspector]
    public bool isTest = false;
    private bool didBackToInitialPointStarted = false;
    private float notFightTime = 0;
    private bool hasTarget
    {
        get
        {
            if (targetTransform != null && (distanceToTarget < enemy.fieldEnemyData.giveUpRange || notFightTime < enemy.fieldEnemyData.giveUpTime))
            {
                return true;

            }
            return false;
        }
    }

    // @@Private
    private Enemy_CleaverMob enemy;

    //AnimationName
    private string Idle = "Idle";
    private string Walking = "Walk";
    private string StandOffRight = "StandOffRight";
    private string StandOffLeft = "StandOffLeft";
    private string Attack_SwingCross = "Attack_SwingCross";
    private string DieMotion = "DieMotion";
    private string Attack_Stab = "Attack_Stab";
    private string StaggerRight = "StaggerRight";
    private string StaggerDown = "StaggerDown";
    private string ShilederedBack = "ShilederedBack";
    private string StandOffRight_Shielding = "StandOffRight_Shielding";
    private string StandOffLeft_Shielding = "StandOffLeft_Shielding";

    // distancable
    private float closeRangeDistancable = 1.8f;
    private float middleRangeDistancable = 3f;

    //CalStack
    private int calStack_SwingCross = 5;
    private int calStack_Stab = 5;

    // Audio
    private AudioSource oneShotAudio;
    public AudioClip hitImpactSound;
    public List<AudioClip> stepSounds_Gravel;
    public List<AudioClip> jumpLandSounds;
    public AudioClip attackSwingSound;

    //HitBox
    public HitBox_Enemy hitBox;

    private void Awake()
    {
        //@@Common
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy_CleaverMob>();
        enemyAnimator = GetComponent<Animator>();
        // @@ForTesterSpace 
        if (TestScene_EnemyController.instance != null)
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
            TestScene_EnemyController.instance.OnTestAction10 += TestAction10;
            TestScene_EnemyController.instance.OnTestAction11 += TestAction11;
            TestScene_EnemyController.instance.OnTestAction12 += TestAction12;
            TestScene_EnemyController.instance.OnTestAction13 += TestAction13;
            TestScene_EnemyController.instance.OnTestAction14 += TestAction14;
            TestScene_EnemyController.instance.OnTestAction15 += TestAction15;
        }

        // @@Private
        enemy = GetComponent<Enemy_CleaverMob>();
        oneShotAudio = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    { 
        // @@Common
        enemy.SetState(State.Normal);
        detectLayers = CommonMethods.GetStringsToLayerMask(enemy.stringsOfAttackTarget);
        enemy.OnTakeDamage += OnTakeDamage;

        //@@Private
        hitBox.gameObject.SetActive(false);
    }
    private void OnEnable()
    { 
        //@@Common
        if (fixedUpdateCoroutine != null)
        {
            StopCoroutine(fixedUpdateCoroutine);
        }
        fixedUpdateCoroutine = StartCoroutine(FixedIntervalUpdateCoroutine());
    }
    public void OnReviveReset()
    {
        //@@Common
        isInLoop = false;
        enemyRigidbody.useGravity = true;
        enemy.SetState(State.Normal);
    }
    public void OnOnfieldReset()
    {
        //@@Common
        if (animationCurrentState != null)
        {
            CoroutineChangeAnimation(animationCurrentState, 0, 1);
        }
    }

    private void OnDisable()
    {
        //@@Common
        isDieActionOn = false;
        if (fixedUpdateCoroutine != null)
        {
            StopCoroutine(fixedUpdateCoroutine);
            fixedUpdateCoroutine = null;
        }
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        targetTransform = null;
        //@@Commont+_HitBoxOff
        hitBox.gameObject.SetActive(false);
    }
    public void TurnOffObject()
    {
        //@@Common
        if (fixedUpdateCoroutine != null)
        {
            StopCoroutine(fixedUpdateCoroutine);
            fixedUpdateCoroutine = null;
        }
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        enemyRigidbody.useGravity = false;
        //@@Commont+_HitBoxOff
        hitBox.gameObject.SetActive(false);
    }
    private void Update()
    {
        //@@Common
        UpdateTargetDistance();
        //@@Private
        UpdateActionState();
    }
    private void FIxedIntervalUpdate()
    {
        DetectTarget();
        notFightTime += fixedUpdateInterval;
    }


    //@@Private
    private void UpdateActionState()
    {

        if (enemy.state == State.Normal)
        {
            if (!hasTarget)
            {

                if (Vector3.Distance(transform.position, enemy.initialPosition) > 3f)
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
                    if (enemy.suicidableState != SuicidableState.Suicidable)
                    {
                        enemy.suicidableState = SuicidableState.Suicidable;
                    }
                }
                if (isInLoop)
                {
                    isInLoop = false;
                }
            }
            else if (hasTarget)
            {
                if (didBackToInitialPointStarted)
                {
                    didBackToInitialPointStarted = false;
                }


                if (!isInLoop)
                {
                    notFightTime = 0;
                    isInLoop = true;
                    enemy.suicidableState = SuicidableState.NotSuicidable;
                    StartNextCoroutine();
                }
            }
        }
    }
    public void OnAttack()
    {
        notFightTime = 0;
        calStack_SwingCross++;
        calStack_Stab++;
    }

    // DecideCoroutines
    private IEnumerator DecideCoroutines()
    {
        List<int> weights = CalculateWeights();
        List<IEnumerator> coroutines = GetCoroutines();

        int totalWeight = weights.Sum();

        if (totalWeight == 0)
        {
            Debug.LogError("Enemy : DecideAction Total Weight �� 0");
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

        Debug.LogError("Enemy : �ൿã��������");
        return null;

    }

    private List<int> CalculateWeights()
    {
        int weight_Approach = Cal_Apporach(distanceToTarget);
        int weight_StandOff = Cal_StandOff(distanceToTarget);
        int weight_Attack_SwingCross = Cal_Attack_SwingCross(distanceToTarget);
        int weight_Attack_Stab = Cal_Attack_Stab(distanceToTarget);

        return new List<int> { weight_Approach, weight_StandOff, weight_Attack_SwingCross, weight_Attack_Stab };
    }

    private List<IEnumerator> GetCoroutines()
    {
        return new List<IEnumerator>
        {
            Approach(),
            StandOff(),
            Attack_SwingCrossAction(),
            Attack_StabAction()
        };
    }

    private int Cal_Apporach(float distanceToTarget1)
    {
        if (distanceToTarget1 >= closeRangeDistancable)
        {
            return 30;
        }
        else
        {
            return 0;
        }
    }
    private int Cal_StandOff(float distanceToTarget)
    {
        if (distanceToTarget >= closeRangeDistancable && distanceToTarget < middleRangeDistancable)
        {
            return 6;
        }
        else if (distanceToTarget < closeRangeDistancable)
        {
            return 12;
        }
        else
        {
            return 0;
        }
    }
    private int Cal_Attack_SwingCross(float distanceToTarget)
    {
        if(distanceToTarget < closeRangeDistancable)
        {
            return Mathf.RoundToInt(30 * Mathf.Clamp01((float)(calStack_SwingCross + 2) / 5));
        }
        else
        {
            return 0;
        }
    }
    private int Cal_Attack_Stab(float distanceToTarget)
    {
        if (distanceToTarget < closeRangeDistancable)
        {

            return Mathf.RoundToInt(30 * Mathf.Clamp01((float)(calStack_Stab + 2) / 5));
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
        float stepSoundTime = 0.200f;


        while (Vector3.Distance(transform.position, enemy.initialPosition) >= 2f)
        {
            if (navMeshAgent.enabled)
            {
                if (navMeshAgent.speed != enemy.enemyCommonData.walkingSpeed)
                {
                    navMeshAgent.speed = enemy.enemyCommonData.walkingSpeed;
                }
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(enemy.initialPosition);
                }
            }

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;
            if(didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == true)
            {
                CommonMethods.AudioPlayOneShot(oneShotAudio, stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], 0.5f);
                didStepSound = true;
            }
            else if(didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == false)
            {
                didStepSound = false;
            }

                yield return null;
        }


        didBackToInitialPointStarted = false;
        navMeshAgent.enabled = false;
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        currentCoroutine = null;
        yield break;
    }

    private IEnumerator Approach()
    {
        enemyRigidbody.interpolation = RigidbodyInterpolation.None;
        navMeshAgent.enabled = true;

        float elapsedTime = 0f;
        float elapsedLim = 4f;

        ChangeAnimation(Walking, 0.2f);

        bool didStepSound = false;
        float stepSoundTime = 0.200f;

        while (elapsedTime < elapsedLim)
        {
            elapsedTime += Time.deltaTime;

            if (navMeshAgent.enabled)
            {
                if (navMeshAgent.speed != enemy.enemyCommonData.walkingSpeed)
                {
                    navMeshAgent.speed = enemy.enemyCommonData.walkingSpeed;
                }
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(targetTransform.position);
                }
            }

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;
            if (didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == true)
            {
                CommonMethods.AudioPlayOneShot(oneShotAudio, stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], 0.5f);
                didStepSound = true;
            }
            else if (didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == false)
            {
                didStepSound = false;
            }


            if (distanceToTarget < closeRangeDistancable)
            {
                nextCoroutine = DecideCoroutines();
                navMeshAgent.enabled = false;
                enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                yield return null;

                StartNextCoroutine();
                yield break;

            }

            yield return null;
        }

        navMeshAgent.enabled = false;
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        yield return null;

        StartNextCoroutine();
        yield break;
    }

    private IEnumerator StandOff()
    {
        float elapsedTime = 0;
        float duration;

        if (distanceToTarget >= 4f)
        {
            float lowFloat;
            float highFloat;
            if(enemy.cleaveMobType == Enemy_CleaverMob.CleaveMobType.Normal)
            {
                lowFloat = 1f;
                highFloat = 2.5f;
            }
            else
            {
                lowFloat = 2f;
                highFloat = 3f;
            }
            duration = Random.Range(lowFloat, highFloat);
        }
        else
        {
            float lowFloat;
            float highFloat;

            if (enemy.cleaveMobType == Enemy_CleaverMob.CleaveMobType.Normal)
            {
                lowFloat = 0.25f;
                highFloat = 0.8f;
            }
            else
            {
                lowFloat = 1.5f;
                highFloat = 2.5f;
            }


            duration = Random.Range(lowFloat, highFloat);
        }


        bool isRight;

        string animationName;
        if (animationCurrentState == StandOffRight || animationCurrentState == StandOffLeft)
        {
            int randomInt = Random.Range(0, 5);
            if (animationCurrentState == StandOffRight)
            {
                if(randomInt == 0)
                {
                    isRight = false;
                }
                else
                {
                    isRight = true;
                }
            }
            else
            {
                if(randomInt == 0)
                {
                    isRight = true;
                }
                else
                {
                    isRight = false;
                }
            }
        }
        else
        {
            if (crossY > 0)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }
        }
        if (isRight)
        {
            if (enemy.cleaveMobType == Enemy_CleaverMob.CleaveMobType.Normal)
            {
                animationName = StandOffRight;
            }
            else
            {
                animationName = StandOffRight_Shielding;
            }
        }
        else
        {
            if (enemy.cleaveMobType == Enemy_CleaverMob.CleaveMobType.Normal)
            {
                animationName = StandOffLeft;
            }
            else
            {
                animationName = StandOffLeft_Shielding;
            }
        }
        ChangeAnimation(animationName, 0.1f, 1);

        bool didStepSound = false;
        float stepSoundTime = 0.749f;

        if (enemy.cleaveMobType == Enemy_CleaverMob.CleaveMobType.Shield)
        {
            enemy.isShielding = true;
        }


            while (elapsedTime < duration)
            {
            elapsedTime += Time.fixedDeltaTime;

            enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 1.5f * Time.deltaTime));

            if (isRight)
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.right * enemy.enemyStatData.walkingSpeed * 0.8f * Time.fixedDeltaTime);
            }
            else
            {
                enemyRigidbody.MovePosition(enemyRigidbody.position - transform.right * enemy.enemyStatData.walkingSpeed * 0.8f * Time.fixedDeltaTime);
            }

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;
            if (didStepSound == false && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == true)
            {
                CommonMethods.AudioPlayOneShot(oneShotAudio, stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], 0.5f);
                didStepSound = true;
            }
            else if (didStepSound == true && CommonMethods.GetQuadrantsBooleanFromTwoPoints(normalizedTime, stepSoundTime, stepSoundTime + 0.5f) == false)
            {
                didStepSound = false;
            }

            yield return new WaitForFixedUpdate();
            }

        enemy.isShielding = false;

        StartNextCoroutine();
        yield break;
    }
    private IEnumerator Attack_SwingCrossAction()
    {
        OnAttack();
        calStack_SwingCross = 0;

        float elapsedTime = 0; float duration = 2f; float strikeTime = 0.916f; float strikeEndTime = 1.06f; float moveStopTime = 1.186f; float rotateStopTime = 1.186f;
        bool didHitBoxOn = false; bool didHitBoxOff = false;

        float lastMovementSpeed = 0;

        List<EventSoundTime> eventSoundTimes = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = duration * 0.177f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.460f, audioClip = attackSwingSound, audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.491f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.950f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f}
        };
        int eventSoundIndex = 0;

        CoroutineChangeAnimation(Attack_SwingCross, 0.2f);


        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            
            if(elapsedTime < strikeTime)
            {
                float limDistanceToTarget = Mathf.Clamp01(distanceToTarget);
                lastMovementSpeed = limDistanceToTarget * 1.7f;
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * Time.fixedDeltaTime);
            }
            if(elapsedTime < rotateStopTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));
            }
            if(elapsedTime >= strikeTime && !didHitBoxOn)
            {
                hitBox.gameObject.SetActive(true);
                hitBox.SetImpactSound(hitImpactSound, 0.5f);
                enemy.SetCurrentAttackByName("Attack_SwingCross", hitBox);
                if (TestScene_ReplayManager.instance != null && TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox.gameObject, false, GameObjectType.HitBox, true);
                }

                didHitBoxOn = true;
            }
            if(elapsedTime >= strikeTime && elapsedTime < moveStopTime)
            {
                float interpolation = (moveStopTime - elapsedTime) / (moveStopTime - strikeTime);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * interpolation * Time.fixedDeltaTime);
            }
            if(elapsedTime >= strikeEndTime && !didHitBoxOff)
            {
                hitBox.gameObject.SetActive(false);
                if(TestScene_ReplayManager.instance != null && TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox.gameObject, false, GameObjectType.HitBox, false);
                }

                didHitBoxOff = true;
            }

            while(eventSoundIndex < eventSoundTimes.Count && elapsedTime >= eventSoundTimes[eventSoundIndex].triggerTime)
            {
                EventSoundTime eventSoundTime = eventSoundTimes[eventSoundIndex];
                CommonMethods.AudioPlayOneShot(eventSoundTime.audioSource, eventSoundTime.audioClip, eventSoundTime.volume);
                eventSoundIndex++;
            }

            yield return new WaitForFixedUpdate();
        }

        StartNextCoroutine();
        yield break;
    }

    private IEnumerator Attack_StabAction()
    {
        OnAttack();
        calStack_Stab = 0;

        float elapsedTime = 0; float duration = 1.825f; float strikeTime = 0.871f; float strikeEndTIme = 0.942f; float moveEndTime = 1.044f; float rotateEndTime = 1.192f;
        bool didHitBoxOn = false; bool didHitBoxOff = false;

        float lastMovementSpeed = 0;

        List<EventSoundTime> eventSoundTimes = new List<EventSoundTime>
        {
            new EventSoundTime{triggerTime = duration * 0.173f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.454f, audioClip = attackSwingSound, audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.489f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f},
            new EventSoundTime{triggerTime = duration * 0.957f, audioClip = stepSounds_Gravel[Random.Range(0, stepSounds_Gravel.Count - 1)], audioSource = oneShotAudio, volume = 0.5f}
        };
        int eventSoundInex = 0;
        
        CoroutineChangeAnimation(Attack_Stab, 0.2f);

        while(elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            if(elapsedTime < strikeTime)
            {
                float limDistanceToTarget = Mathf.Clamp01(distanceToTarget);
                lastMovementSpeed = limDistanceToTarget * 1.7f;
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * Time.fixedDeltaTime);
            }
            if(elapsedTime < rotateEndTime)
            {
                enemyRigidbody.MoveRotation(Quaternion.Slerp(enemyRigidbody.rotation, faceTargetRotation, 4f * Time.fixedDeltaTime));
            }
            if(elapsedTime >= strikeTime && !didHitBoxOn)
            {
                hitBox.gameObject.SetActive(true);
                hitBox.SetImpactSound(hitImpactSound, 0.5f);
                enemy.SetCurrentAttackByName("Attack_Stab", hitBox);
                if(TestScene_ReplayManager.instance != null && TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox.gameObject, false, GameObjectType.HitBox, true);
                }

                didHitBoxOn = true;
            }
            if(elapsedTime >= strikeTime && elapsedTime < moveEndTime)
            {
                float interpolation = (moveEndTime - elapsedTime) / (moveEndTime - strikeTime);
                enemyRigidbody.MovePosition(enemyRigidbody.position + transform.forward * lastMovementSpeed * interpolation * Time.fixedDeltaTime);
            }
            if(elapsedTime >= strikeEndTIme && !didHitBoxOff)
            {
                hitBox.gameObject.SetActive(false);
                if (TestScene_ReplayManager.instance != null && TestScene_ReplayManager.instance.isRecording)
                {
                    TestScene_ReplayManager.instance.RecordGameObjectOnOff(hitBox.gameObject, false, GameObjectType.HitBox, false);
                }

                didHitBoxOff = true;
            }

            while(eventSoundInex < eventSoundTimes.Count && elapsedTime >= eventSoundTimes[eventSoundInex].triggerTime)
            {
                EventSoundTime eventSoundTime = eventSoundTimes[eventSoundInex];
                CommonMethods.AudioPlayOneShot(eventSoundTime.audioSource, eventSoundTime.audioClip, eventSoundTime.volume);
                eventSoundInex++;
            }


            yield return new WaitForFixedUpdate();
        }

        StartNextCoroutine();
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
            animationName = StaggerDown;
        }
        else if (directionType1 == DirectionType.BottomUp)
        {
            animationName = StaggerDown;
        }
        else if (flatDiffAngle < 45f)
        {
            animationName = StaggerDown;
        }
        else if (flatDiffAngle >= 45f && flatDiffAngle < 135f)
        {
            if (crossY < 0)
            {
                animationName = StaggerRight;
            }
            else
            {
                animationName = StaggerDown;
            }
        }
        else
        {
            animationName = StaggerDown;
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

        StartNextCoroutine();
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
        enemy.SetState(State.Invulnerable);


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

        enemy.SetState(State.Normal);

        StartNextCoroutine();
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
        CoroutineChangeAnimation(ShilederedBack, 0.05f);

        enemyRigidbody.AddForce((transform.position - targetPosition1) * StaticValues.addForceTiny * 1.5f, ForceMode.Impulse);

        float shielderedBackDuration = 0.917f;

        yield return new WaitForSeconds(shielderedBackDuration);

        StartNextCoroutine();
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
                enemy.SetState(State.Invulnerable);

            }
            if (!isDieActionOn)
            {
                if (currentCoroutine != null)
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
    private IEnumerator DieAction()
    {
        isDieActionOn = true;
        enemy.SetState(State.Invulnerable);


        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        float currentNormalizedTime = stateInfo.normalizedTime % 1f;
        enemyAnimator.Play(animationCurrentState, 0, currentNormalizedTime);
        
        CoroutineChangeAnimation(DieMotion, 0.02f);



        float elapsedTIme = 0;
        float duration = 0.458f;


        while (elapsedTIme < duration)
        {
            elapsedTIme += Time.deltaTime;

            if (animationCurrentState != DieMotion)
            {
                CoroutineChangeAnimation(DieMotion, 0.02f);
            }

            yield return null;
        }
        

        enemy.TurnOffOBject();

        yield break;
    }
    private void StartNextCoroutine()
    {
        if (enemy.dead) return;
        if (isTest) return;

        if(nextCoroutine == null)
        {
            nextCoroutine = DecideCoroutines();
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(nextCoroutine);
        nextCoroutine = null;
    }
    private void StopCoroutineRoutine(Coroutine currentCoroutine)
    {
        //@@Common
        StopCoroutine(currentCoroutine);
        enemy.SetState(State.Normal);
        if (navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
        }
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        RestoreIgnoreCollisions();
        currentCoroutine = null;
        //@@Common_HitBoxOff
        hitBox.gameObject.SetActive(false);

        //@@Private
        enemy.isShielding = false;
    }

 

    //@@Common
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


            if (TestScene_ReplayManager.instance != null)
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
    private void IgnoreCollisions_Sphere(float radius, LayerMask ignoreLayerMask)
    {
        ignoreColliders.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, ignoreLayerMask);

        if (colliders.Length != 0)
        {
            foreach (Collider collider in colliders)
            {
                ignoreColliders.Add(collider);

                Physics.IgnoreCollision(enemyCollider, collider, true);
            }
        }
    }
    private void RestoreIgnoreCollisions()
    {
        if (ignoreColliders.Count != 0)
        {
            foreach (Collider collider in ignoreColliders)
            {
                Physics.IgnoreCollision(enemyCollider, collider, false);
            }
        }
    }
    private void UpdateTargetDistance()
    {
        if (targetTransform != null)
        {
            Vector3 directionToTarget = targetTransform.position - transform.position;
            Vector3 flatDirectionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z);

            //result

            distanceToTarget = flatDirectionToTarget.magnitude;
            if (flatDirectionToTarget == Vector3.zero)
            {
                faceTargetRotation = Quaternion.identity;
            }
            else
            {
                faceTargetRotation = Quaternion.LookRotation(flatDirectionToTarget.normalized);
            }
            diffAngle = Vector3.Angle(transform.forward, flatDirectionToTarget);
            crossY = Vector3.Cross(transform.forward, flatDirectionToTarget).y;
        }
        else
        {
            distanceToTarget = Mathf.Infinity;
            faceTargetRotation = Quaternion.identity;
            diffAngle = 0;
            crossY = 0;
        }
    }
    private IEnumerator FixedIntervalUpdateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fixedUpdateInterval);
            FIxedIntervalUpdate();
        }
    }
    private void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemy.enemyCommonData.detectRange, detectLayers);

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
    public void OnTakeDamage(float damage)
    {
        notFightTime = 0;
    }





    // @@ForTesterSpace
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
            if (currentCoroutine != null)
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
        if (currentCoroutine != null)
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
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_SwingCrossAction());
    }
    public void TestAction4()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Attack_StabAction());
    }
    public void TestAction5()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction6()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction7()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction8()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }
    public void TestAction9()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

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


