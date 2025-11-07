
using System.Collections;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    private WorldChunk worldChunk;

    [HideInInspector]
    private LayerMask detectedEnemy;
    private PlayerHealth playerHealth;
    private Coroutine coroutine1;
    private Vector3 respawnPosition;
    private FadeEffect fadeEffect;
    private ShelterRestInfo shelterRestInfo;
    private string Shelter1 = "Shelter1";
    private Coroutine checkCoroutine;
    private bool isPlayerIn = false;
    private LayerMask playerLayer;

    private void Awake()
    {
        worldChunk = GetComponentInParent<WorldChunk>();
        detectedEnemy = 1 << LayerMask.NameToLayer("Enemy");

        fadeEffect = UIManager.instance.GetComponentInChildren<FadeEffect>();
        shelterRestInfo = UIManager.instance.GetComponentInChildren<ShelterRestInfo>();

        respawnPosition = transform.position + new Vector3(-2f, -1f, -2f); 
    }
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }
    private void OnEnable()
    {
        if(checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
        checkCoroutine = StartCoroutine(CheckCoroutine());
    }
    private void OnDisable()
    {
        if(checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }       
    }
    private IEnumerator CheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);
            if(colliders.Length > 0)
            {
                foreach(Collider col in colliders)
                {
                    if ( ( (1 << col.gameObject.layer ) & playerLayer) != 0)
                    {
                        playerHealth = col.GetComponent<PlayerHealth>();
                        break;
                    }
                }
                Collider[] collider = Physics.OverlapSphere(transform.position, 5f, detectedEnemy);
                if (collider.Length == 0)
                {
                    if (!isPlayerIn)
                    {
                        isPlayerIn = true;
                        UIManager.instance.ShowOkButton(Shelter1, OnShelterButtonClicked);
                    }
                }
                else
                {
                    if (isPlayerIn)
                    {
                        UIManager.instance.HideOKButton(Shelter1, OnShelterButtonClicked);
                        isPlayerIn = false;
                    }
                }
            }
            else
            {
                if (isPlayerIn)
                {
                    UIManager.instance.HideOKButton(Shelter1, OnShelterButtonClicked);
                    isPlayerIn = false;
                }
            }
        }
    }


    public void OnShelterButtonClicked()
    {
        if (playerHealth.state != State.Normal)
        {
            return;
        }
        if(coroutine1 != null)
        {
            StopCoroutine(coroutine1);
        }

        GameManager.instance.gameOverManager.UpdateRespawnTransform(respawnPosition);
        coroutine1 = StartCoroutine(ResetRoutine());
        playerHealth.playerStatusManager.SaveJsonData();
        EquipmentController.instance.SaveEquipment();
    }
    private IEnumerator ResetRoutine()
    {
        playerHealth.playerController.gameObject.SetActive(false);
        playerHealth.health = playerHealth.maxHealthForShelter;
        playerHealth.playerMovement.InvokeShelterRestStartAction(transform.position);

        EquipmentController.instance.startRect.gameObject.SetActive(false);

        fadeEffect.InvokeFadeIn();

        yield return new WaitForSeconds(0.5f);


        GameManager.instance.chunkManager.DeActiveAllChunks();
        GameManager.instance.chunkManager.ReviveAllChunks();

        playerHealth.transform.position = respawnPosition;
        Vector3 lookingVector = transform.position - playerHealth.transform.position;
        Vector3 refinedLookingVector = new Vector3(lookingVector.x, 0, lookingVector.z);
        playerHealth.transform.rotation = Quaternion.LookRotation(refinedLookingVector);

        GameManager.instance.databaseManager.SavePlayerPosition(playerHealth.transform.position);

        playerHealth.playerMovement.BaseLayerChangeAnimation("ShelterRestDoing1", 0f);

        yield return null;

        fadeEffect.InvokeFadeOut();
        shelterRestInfo.ActiveInfoChoosePanel(this);
        GameManager.instance.cinemachineController.AdjustCameraToShelterRest(transform.position, playerHealth.transform.position);
    }
    public void InvokeShelterRestFinished()
    {
        StartCoroutine(shelterRestFinsihedCoroutine());
    }
    private IEnumerator shelterRestFinsihedCoroutine()
    {
        playerHealth.playerController.gameObject.SetActive(true);
        EquipmentController.instance.startRect.gameObject.SetActive(true);
        GameManager.instance.cinemachineController.RestoreCameraToDefault();
        isPlayerIn = false;
        yield return null;
    }



}
