using System.Collections;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    private Camera mainCam;
    [HideInInspector]
    public Cinemachine.CinemachineVirtualCamera cinemachieCam;
    private CinemachineTransposer cineTransposer;
    private CinemachineBasicMultiChannelPerlin cinePerlin;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotaion;

    private bool isShelterCameraActive = false;

    //circlingM_Offset
    private Vector3 originalMOffset;
    private Vector2 mOffsetToVector2;
    private float mOffsetAngle = -90;

    private Coroutine impulseCoroutine;
   
    

    private void Awake()
    {
        mainCam = Camera.main;
        cinemachieCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        cineTransposer = cinemachieCam.GetCinemachineComponent<CinemachineTransposer>();
        cinePerlin = cinemachieCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinePerlin.m_AmplitudeGain = 0;


        originalMOffset = cineTransposer.m_FollowOffset;
        mOffsetToVector2 = new Vector2(originalMOffset.x, originalMOffset.z);
    }


    public void AdjustCameraToShelterRest(Vector3 shelterPosition, Vector3 playerPosition)
    {
        if (!isShelterCameraActive)
        {
            StartCoroutine(AdjustCameraTOShelterRestCoroutine(shelterPosition, playerPosition));
        }

    }
    private IEnumerator AdjustCameraTOShelterRestCoroutine(Vector3 shelterPosition, Vector3 playerPosition)
    {
        cineTransposer.m_XDamping = 0;
        cineTransposer.m_YDamping = 0;
        cineTransposer.m_ZDamping = 0;

        yield return null; 

        originalCameraPosition = mainCam.transform.position;
        originalCameraRotaion = mainCam.transform.rotation;

        cinemachieCam.enabled = false;

        isShelterCameraActive = true;

        Vector3 lookAtPosition = (shelterPosition + playerPosition) / 2 + new Vector3(5f, 0, 2f);
        Vector3 lookDistance = new Vector3(0, 15, -24);
        mainCam.transform.position = lookAtPosition + lookDistance;


        Quaternion lookRotation = Quaternion.LookRotation(lookAtPosition - mainCam.transform.position);
        mainCam.transform.rotation = lookRotation;

    }



    public void RestoreCameraToDefault()
    {
        if (isShelterCameraActive)
        {
            StartCoroutine(RestoreCameraToDefaultCoroutine());
        }
    }

    private IEnumerator RestoreCameraToDefaultCoroutine()
    {
        float duration = 0.5f;
        float elapsedTime = 0;

        Vector3 previousPosition = mainCam.transform.position;
        Quaternion previousRotation = mainCam.transform.rotation;

        while(elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;

            mainCam.transform.position = Vector3.Lerp(previousPosition, originalCameraPosition, elapsedTime / duration);
            mainCam.transform.rotation = Quaternion.Lerp(previousRotation, originalCameraRotaion, elapsedTime / duration);

            yield return null;
        }

        
        mainCam.transform.position = originalCameraPosition;
        mainCam.transform.rotation = originalCameraRotaion;

        cinemachieCam.enabled = true;
        cineTransposer.m_XDamping = 0.1f;
        cineTransposer.m_YDamping = 0.1f;
        cineTransposer.m_ZDamping = 0.1f;

        isShelterCameraActive = false;
    }

    public void ChangeMOffsetCircling (float newPlusAngle)
    {
        mOffsetAngle += newPlusAngle * 1.3f;

        float radian = mOffsetAngle * Mathf.Deg2Rad;
        float initialMagnitude = mOffsetToVector2.magnitude;
        

        float x = initialMagnitude * Mathf.Cos(radian);
        float z = initialMagnitude * Mathf.Sin(radian);

        cineTransposer.m_FollowOffset = new Vector3(x, originalMOffset.y, z);
    }
    public void ResetOffsetCircling()
    {
        mOffsetAngle = -90;
        cineTransposer.m_FollowOffset = originalMOffset;
    }

    public void MoveMainCam(Vector2 movec)
    {
        float movementSpeed = 20f;

        float angle = Mathf.Atan2(movec.y, movec.x);

        Vector3 direction = Mathf.Cos(angle) * mainCam.transform.right + Mathf.Sin(angle) * mainCam.transform.forward;

        mainCam.transform.position += direction.normalized * movementSpeed * movec.magnitude * Time.deltaTime;
    }
    public void MoveMainCamUpDown(float value)
    {
        float movementSpeed = 16f;

        mainCam.transform.position += mainCam.transform.up * movementSpeed * value * Time.deltaTime;
    }
    public void ChangeMainCamForwardAngle(Vector2 forvec)
    {
        Vector3 direction = new Vector3(-forvec.y, forvec.x, 0);

        Quaternion targetRotation = mainCam.transform.rotation * Quaternion.Euler(direction * 0.1f);

        Vector3 euler = targetRotation.eulerAngles;
        euler.z = 0;

        mainCam.transform.rotation = Quaternion.Euler(euler);
    }

    public void InvokeCameraImpulse(float impulseTime, float impulseIntensity)
    {
        if(impulseCoroutine != null)
        {
            StopCoroutine(impulseCoroutine);
        }
        impulseCoroutine = StartCoroutine(CameraImpulse(impulseTime, impulseIntensity));

    }

    private IEnumerator CameraImpulse(float impulseTime, float impulseIntensity)
    {
        float elapsedTime = 0;
        cinePerlin.m_AmplitudeGain = impulseIntensity;

        while(elapsedTime < impulseTime)
        {

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        cinePerlin.m_AmplitudeGain = 0;

        impulseCoroutine = null;
    }
}
