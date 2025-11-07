using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFading_OverlapSphere_3_Multi : MonoBehaviour
{
    private LayerMask playerLayer;

    private List<Material> materials = new List<Material>();

    private bool isFaded;

    private Coroutine checkCoroutine;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");

        Renderer renderer1 = GetComponent<Renderer>();
        if(renderer1 != null)
        {
            materials.AddRange(renderer1.materials);
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if(renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                materials.AddRange(renderer.materials);
            }
        }
    }


    private void OnEnable()
    {
        isFaded = false;

        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
        checkCoroutine = StartCoroutine(checkPlayerCoroutine());
    }

    private IEnumerator checkPlayerCoroutine()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 3f), 3f, playerLayer);
            if (colliders.Length > 0)
            {
                if (!isFaded)
                {
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeToTransparent());
                    isFaded = true;
                }
            }
            else
            {
                if (isFaded)
                {
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeToOpaQue());
                    isFaded = false;
                }
            }

            yield return new WaitForSeconds(StaticValues.fadeObjectFadeCheckInterval);
        }
    }

    private IEnumerator FadeToTransparent()
    {

        while (materials[materials.Count-1].color.a > 0)
        {
            foreach(Material material in materials)
            {
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a - StaticValues.fadeObjectFadeTime * Time.deltaTime);
                material.color = color;
            }

            yield return null;
        }

        fadeCoroutine = null;
    }


    private IEnumerator FadeToOpaQue()
    {
        while (materials[materials.Count-1].color.a < 1)
        {
            foreach(Material material in materials){
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a + StaticValues.fadeObjectFadeTime * Time.deltaTime);
                material.color = color;
            }

            yield return null;
        }

        fadeCoroutine = null;
    }

}
