using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected string ItemPickUp = "ItemPickUp";
    private Coroutine coroutine;
    private bool isInSphere = false;

    protected void OnEnable()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(CheckCoroutine());
    }
    protected void OnDisable()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    protected IEnumerator CheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Player"));
            if(colliders.Length > 0)
            {
                if (!isInSphere)
                {
                    UIManager.instance.ShowOkButton(ItemPickUp, OnOkButtonClicked);
                    isInSphere = true;
                }
            }
            else
            {
                if (isInSphere)
                {
                    UIManager.instance.HideOKButton(ItemPickUp, OnOkButtonClicked);
                    isInSphere = false;
                }
            }
        }
    }


    protected void OnOkButtonClicked()
    {
        if (EquipmentController.instance != null)
        {
            if (EquipmentController.instance.FindEmptySlot() != null)
            {

                EquipmentController.instance.addItemToInventory(gameObject);
                UIManager.instance.HideOKButton(ItemPickUp, OnOkButtonClicked);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(oneFrameWaitShowOkButton());
            }
        }
    }
    private IEnumerator oneFrameWaitShowOkButton()
    {
        yield return null;
        UIManager.instance.ShowOkButton(ItemPickUp, OnOkButtonClicked);
    }
}
