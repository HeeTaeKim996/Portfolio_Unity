// #HealthBarLookAtController_Enemy

using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarLookAtController_Enemy : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    public Text damageText;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        cam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    private void OnEnable()
    {
        damageText.gameObject.SetActive(false);
    }

    public void DisplayDamage(float damage)
    {
        damageText.text = Mathf.RoundToInt(damage).ToString();
        damageText.gameObject.SetActive(true);
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(HideDamageText());
    }
    private IEnumerator HideDamageText()
    {
        yield return new WaitForSeconds(1f);
        damageText.gameObject.SetActive(false);
    }

}
