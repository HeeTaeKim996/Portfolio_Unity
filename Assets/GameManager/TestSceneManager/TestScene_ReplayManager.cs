using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class ReplayFrame
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public float upperLayerWeight;

    public Vector3 enemyPosition;
    public Quaternion enemyRotation;

    public List<AnimationData> animationDatas = new List<AnimationData>();
    public List<OnAnimatorIKData> onAnimatorIKData = new List<OnAnimatorIKData>();
    public List<EffectData> effects = new List<EffectData>();
    public List<GameObjectOnOffData> objectOnOffs = new List<GameObjectOnOffData>();
    public List<SoundData> soundDatas = new List<SoundData>();
}


[System.Serializable]
public class AnimationData
{
    public bool isPlayer;
    public int layerIndex;
    public string animationName;
    public float blendTime;
    public float normalizedTime;
}

[System.Serializable]
public class OnAnimatorIKData
{
    public Vector3 lookAtPosition;
    public bool isIKOn;
}

[System.Serializable]
public class EffectData
{
    public float time;
    public float duration;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject effectPrefab;
}
public class SoundData
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public float volume;
    public float startTime;
    public RecordSoundDataType recordSoundDataType;
}

[System.Serializable]
public class GameObjectOnOffData
{
    public GameObject onOffGameObject;
    public bool isPlayers;
    public GameObjectType gameObjectType;
    public bool isOn;
}

public class TestScene_ReplayManager : MonoBehaviour
{
    public static TestScene_ReplayManager instance; 

    private List<ReplayFrame> frames = new List<ReplayFrame>();
    [HideInInspector]
    public bool isRecording = false;
    [HideInInspector]
    public bool isReplayMode = false;

    private PlayerHealth playerHealth;
    private Transform playerTransform;
    [HideInInspector]
    public Animator playerAnimator;
    private Enemy enemy;
    private Transform enemyTransform;
    [HideInInspector]
    public Animator enemyAimator;
    private TestScene_SliderManager sliderManager;

    private Coroutine currentCoroutine;



    //  Buttons
    public GameObject recordObjects;
    public Text recordTimeText;
    private float recordTime = 0;
    private TestScene_RecordStartButton recordStartButton;
    private TestScene_RecordEndButton recordEndButton;

    public GameObject playObjects;
    private CanvasGroup playCanvasGroup;
    private TestScene_PlayModeOnbutton playModeOnButton;
    private TestScene_PlayModeOffButton playModeOffButton;

    private TestScene_ReplayPlayButton replayPlayButton;
    private TestScene_ReplayPauseButton replayPauseButton;

    public GameObject equipmentStartButton;
    public GameObject testerStartButton1;
    public GameObject testerStartButton2;

    public GameObject cameraControllerObjects;
    private CanvasGroup cameraControllerCanvasGroup;

    // Play & Pause
    private int frameIndex;

    private string playerBaseLayerAnimationName;
    private string playerUpperLayerAnimationName;
    private float playerBaseLayerNormalizedTime;
    private float playerUpperLayerNormalizedTime;
    private string enemyAnimationName;
    private float enemyNormalizedTime;
    

    // Slider
    [HideInInspector]
    public int totalFrame;

    // For Player's OnAnimatorIK
    [HideInInspector]
    public bool isIKOn;
    [HideInInspector]
    public Vector3 ikLookPosition;




    // HitBoxes
    private Material playerWeaponHitBoxOriginalMaterial;
    public Material checkHitBoxMaterialPrefab;




    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        playerHealth = FindObjectOfType<PlayerHealth>();
        playerTransform = playerHealth.transform;
        playerAnimator = playerHealth.gameObject.GetComponent<Animator>();

        enemy = FindObjectOfType<Enemy>();
        enemyTransform = enemy.transform;
        enemyAimator = enemy.gameObject.GetComponent<Animator>();
        sliderManager = GetComponentInChildren<TestScene_SliderManager>();

        recordStartButton = GetComponentInChildren<TestScene_RecordStartButton>();
        recordEndButton = GetComponentInChildren<TestScene_RecordEndButton>();

        playModeOnButton = GetComponentInChildren<TestScene_PlayModeOnbutton>();
        playModeOffButton = GetComponentInChildren<TestScene_PlayModeOffButton>();

        replayPlayButton = GetComponentInChildren<TestScene_ReplayPlayButton>();
        replayPauseButton = GetComponentInChildren<TestScene_ReplayPauseButton>();

        playCanvasGroup = playObjects.GetComponent<CanvasGroup>();
        playCanvasGroup.alpha = 1;

        cameraControllerCanvasGroup = cameraControllerObjects.GetComponent<CanvasGroup>();
        cameraControllerCanvasGroup.alpha = 1;
    }
    private void Start()
    {
        recordEndButton.gameObject.SetActive(false);
        replayPauseButton.gameObject.SetActive(false);
        playObjects.gameObject.SetActive(false);
        playModeOffButton.gameObject.SetActive(false);
    }
    public void StartRecording()
    {
        frames.Clear();
        recordTime = 0;
        isRecording = true;
        recordStartButton.gameObject.SetActive(false);
        recordEndButton.gameObject.SetActive(true);
    }

    public void StopRecording()
    {
        isRecording = false;
        recordStartButton.gameObject.SetActive(true);
        recordEndButton.gameObject.SetActive(false);
    }

    public void StartPlayMode()
    {
        playerHealth.SetUpToReplayMode();
        enemy.SetUpToReplayMode();
        isReplayMode = true;

        frameIndex = 0;
        totalFrame = frames.Count - 1;
        playerBaseLayerAnimationName = null;
        playerUpperLayerAnimationName = null;
        enemyAnimationName = null;

        recordObjects.gameObject.SetActive(false);
        playObjects.gameObject.SetActive(true);
        playModeOnButton.gameObject.SetActive(false);
        playModeOffButton.gameObject.SetActive(true);

        equipmentStartButton.gameObject.SetActive(false);
        testerStartButton1.gameObject.SetActive(false);
        testerStartButton2.gameObject.SetActive(false);

    }
    public void EndPlayMode()
    {
        playerHealth.SetUpToGameMode();
        enemy.SetUpToGameMode();
        isReplayMode = false;

        playerAnimator.speed = 1;
        enemyAimator.speed = 1;

        recordObjects.gameObject.SetActive(true);
        playObjects.gameObject.SetActive(false);
        playModeOnButton.gameObject.SetActive(true);
        playModeOffButton.gameObject.SetActive(false);

        equipmentStartButton.gameObject.SetActive(true);
        testerStartButton1.gameObject.SetActive(true);
        testerStartButton2.gameObject.SetActive(true);

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        Time.timeScale = 1;
    }

    public void PlayReplay()
    {
        replayPlayButton.gameObject.SetActive(false);
        replayPauseButton.gameObject.SetActive(true);

        playerAnimator.speed = 1;
        enemyAimator.speed = 1;

        if (frames.Count == 0)
        {
            Debug.LogWarning("TestScene_ReplayManager : 리플레이데이터가 없습니다");
            return;
        }

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(PlayFrames());
    }
    public void PauseReplay()
    {
        replayPlayButton.gameObject.SetActive(true);
        replayPauseButton.gameObject.SetActive(false);

        SaveAnimationState();
        playerAnimator.speed = 0;
        enemyAimator.speed = 0;

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private void FixedUpdate()
    {
        if (isRecording)
        {
            RecordFrames();
            recordTime += Time.deltaTime;
            recordTimeText.text = $"RecordTime : {Mathf.RoundToInt(recordTime)}";
        }
    }

    private void RecordFrames()
    {
        var frame = new ReplayFrame
        {
            playerPosition = playerTransform.position,
            playerRotation = playerTransform.rotation,

            upperLayerWeight = playerAnimator.GetLayerWeight(1),

            enemyPosition = enemyTransform.position,
            enemyRotation = enemyTransform.rotation,

            effects = new List<EffectData>()
        };

        frames.Add(frame);
    }

    

    public void RecordAnimationData(bool newIsPlayer, int newLayerIndex, string newAnimationName, float newBlendTime, float newNormalizedTime)
    {
        if(frames.Count > 0)
        {
            var currentFrame = frames[frames.Count - 1];
            currentFrame.animationDatas.Add(new AnimationData
            {
                isPlayer = newIsPlayer,
                layerIndex = newLayerIndex,
                animationName = newAnimationName,
                blendTime = newBlendTime,
                normalizedTime = newNormalizedTime
            });
        }
    }
    public void RecordOnAnimatorIKData(bool newIsIKOn, Vector3 newLookAtPosition)
    {
        if(frames.Count > 0)
        {
            var currentFrame = frames[frames.Count - 1];
            currentFrame.onAnimatorIKData.Add(new OnAnimatorIKData
            {
                isIKOn = newIsIKOn,
                lookAtPosition = newLookAtPosition
            });
        }
    }

    public void RecordEffect(GameObject newEffectPrefab, Vector3 newPosition, Quaternion newRotation, float newDuration)
    {
        if (frames.Count > 0)
        {
            var currentFrame = frames[frames.Count - 1];
            currentFrame.effects.Add(new EffectData
            {
                time = Time.time,
                effectPrefab = newEffectPrefab,
                position = newPosition,
                rotation = newRotation,
                duration = newDuration
            });
        }
    }

    public void RecordGameObjectOnOff(GameObject newOnOffGameObject, bool newIsPlayers, GameObjectType newGameObjectType , bool newIsOn)
    {
        if(frames.Count > 0)
        {
            var currentFrame = frames[frames.Count - 1];
            currentFrame.objectOnOffs.Add(new GameObjectOnOffData
            {
                onOffGameObject = newOnOffGameObject,
                isPlayers = newIsPlayers,
                gameObjectType = newGameObjectType,
                isOn = newIsOn
            });
        }
    }
    
    public void RecordSoundData(AudioSource audioSource, AudioClip audioClip, float volume, float startTime, RecordSoundDataType recordSoundDataType)
    {
        if(frames.Count > 0)
        {
            var currentFrame = frames[frames.Count - 1];
            currentFrame.soundDatas.Add(new SoundData
            {
                audioSource = audioSource,
                audioClip = audioClip,
                volume = volume,
                startTime = startTime,
                recordSoundDataType = recordSoundDataType
            });
        }
    }

    private IEnumerator PlayFrames()
    {
        if (playerBaseLayerAnimationName != null)
        {
            playerAnimator.CrossFade(playerBaseLayerAnimationName, 0, 0, playerBaseLayerNormalizedTime);
        }
        if (playerUpperLayerAnimationName != null)
        {
            playerAnimator.CrossFade(playerUpperLayerAnimationName, 0, 1, playerUpperLayerNormalizedTime);
        }
        if (enemyAnimationName != null)
        {
            enemyAimator.CrossFade(enemyAnimationName, 0, 0, enemyNormalizedTime);
        }

        for (int i = frameIndex; i < frames.Count; i++)
        {
            ApplyFrame(i);

            yield return new WaitForFixedUpdate();
        }

        currentCoroutine = null;

        frameIndex = 0;
        playerBaseLayerAnimationName = null;
        playerUpperLayerAnimationName = null;
        enemyAnimationName = null;

        replayPlayButton.gameObject.SetActive(true);
        replayPauseButton.gameObject.SetActive(false);
    }
    public void ApplyFrame(int index)
    {
        var frame = frames[index];

        frameIndex = index;
        sliderManager.UpdateSliderValue(index);


        playerTransform.position = frame.playerPosition;
        playerTransform.rotation = frame.playerRotation;

        playerAnimator.SetLayerWeight(1, frame.upperLayerWeight);

        enemyTransform.position = frame.enemyPosition;
        enemyTransform.rotation = frame.enemyRotation;


        foreach (var animationData in frame.animationDatas)
        {
            if (animationData.isPlayer)
            {
                playerAnimator.CrossFade(animationData.animationName, animationData.blendTime, animationData.layerIndex, animationData.normalizedTime);
              if(animationData.layerIndex == 0)
              {
                    playerBaseLayerAnimationName = animationData.animationName;
              }
                else
                {
                    playerUpperLayerAnimationName = animationData.animationName;
                }
            }
            else
            {
                enemyAimator.CrossFade(animationData.animationName, animationData.blendTime, animationData.layerIndex, animationData.normalizedTime);
                enemyAnimationName = animationData.animationName;
            }
        }
        foreach(var OnAnimatorIKData in frame.onAnimatorIKData)
        {
            isIKOn = OnAnimatorIKData.isIKOn;
            ikLookPosition = OnAnimatorIKData.lookAtPosition;
        }
        foreach (var effect in frame.effects)
        {
            GameObject newEffect = Instantiate(effect.effectPrefab, effect.position, effect.rotation);
            Destroy(newEffect, effect.duration);
        }
        foreach(var objectOnOff in frame.objectOnOffs)
        {
            if (objectOnOff.isPlayers)
            {
                if(objectOnOff.gameObjectType == GameObjectType.Trailer)
                {
                    GameObject playertrailer = objectOnOff.onOffGameObject;

                    if (objectOnOff.isOn)
                    {
                        playertrailer.gameObject.SetActive(true);
                    }
                    else if (!objectOnOff.isOn)
                    {
                        playertrailer.gameObject.SetActive(false);
                    }
                }
                else if(objectOnOff.gameObjectType == GameObjectType.HitBox)
                {
                    GameObject playerHitBox = objectOnOff.onOffGameObject;
                    if (objectOnOff.isOn)
                    {
                        playerHitBox.gameObject.SetActive(true);
                        Collider hitBoxCollider = playerHitBox.GetComponent<Collider>();
                        hitBoxCollider.enabled = false;

                        Renderer playerHitBoxRenderer = playerHitBox.GetComponent<Renderer>();
                        playerWeaponHitBoxOriginalMaterial = playerHitBoxRenderer.material;
                        playerHitBoxRenderer.material = checkHitBoxMaterialPrefab;
                    }
                    else if (!objectOnOff.isOn)
                    {
                        Collider hitBoxCollider = playerHitBox.GetComponent<Collider>();
                        hitBoxCollider.enabled = true;

                        Renderer playerHitBoxRenderer = playerHitBox.GetComponent<Renderer>();
                        playerHitBoxRenderer.material = playerWeaponHitBoxOriginalMaterial;

                        playerHitBox.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                GameObject enemyHitBox = objectOnOff.onOffGameObject;
                if (objectOnOff.isOn)
                {
                    enemyHitBox.gameObject.SetActive(true);
                    Collider hitBoxCollider = enemyHitBox.GetComponent<Collider>();
                    hitBoxCollider.enabled = false;

                    Renderer enemyHitBoxRenderer = enemyHitBox.GetComponent<Renderer>();
                    playerWeaponHitBoxOriginalMaterial = enemyHitBoxRenderer.material;
                    enemyHitBoxRenderer.material = checkHitBoxMaterialPrefab;
                }
                else if (!objectOnOff.isOn)
                {
                    Collider hitBoxCollider = enemyHitBox.GetComponent<Collider>();
                    hitBoxCollider.enabled = true;

                    Renderer enemyHitBoxRenderer = enemyHitBox.GetComponent<Renderer>();
                    enemyHitBoxRenderer.material = playerWeaponHitBoxOriginalMaterial;

                    enemyHitBox.gameObject.SetActive(false);
                }
            }
        }
        foreach(var soundData in frame.soundDatas)
        {
            if(soundData.recordSoundDataType == RecordSoundDataType.Play)
            {
                soundData.audioSource.clip = soundData.audioClip;
                soundData.audioSource.volume = soundData.volume;
                soundData.audioSource.time = soundData.startTime;
                soundData.audioSource.Play();
            }
            else if(soundData.recordSoundDataType == RecordSoundDataType.PlayOneShot)
            {
                soundData.audioSource.PlayOneShot(soundData.audioClip, soundData.volume);
            }
        }
    }

    private void SaveAnimationState()
    {
        AnimatorStateInfo playerBaseStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        playerBaseLayerNormalizedTime = playerBaseStateInfo.normalizedTime % 1;
        AnimatorStateInfo playerUpperStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(1);
        playerUpperLayerNormalizedTime = playerUpperStateInfo.normalizedTime % 1;
        AnimatorStateInfo enemyStateInfo = enemyAimator.GetCurrentAnimatorStateInfo(0);
        enemyNormalizedTime = enemyStateInfo.normalizedTime % 1;
    }


}
