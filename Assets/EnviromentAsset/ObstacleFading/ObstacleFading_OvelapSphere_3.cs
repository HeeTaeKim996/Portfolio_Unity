using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleFading_Physics_OvelapSphere_3 : MonoBehaviour
{
    private LayerMask playerLayer;

    private float fadeProgressPerSecond = (float) 1f / 0.5f;
    private Renderer renderer1;
    private List<Material> materials = new List<Material>();

    private bool isFaded;

    private Coroutine checkCoroutine;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        renderer1 = GetComponent<Renderer>();
        materials.AddRange(renderer1.materials);
    }


    private void OnEnable()
    {
        isFaded = false;

        if(checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
        checkCoroutine = StartCoroutine(checkPlayerCoroutine());
    }

    private IEnumerator checkPlayerCoroutine()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0,0,3f), 3f, playerLayer);
            if(colliders.Length > 0)
            {
                if(!isFaded)
                {
                    if(fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeToTransparent());
                    isFaded = true;
                }
            }
            else
            {
                if(isFaded)
                {
                    if(fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeToOpaQue());
                    isFaded = false;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator FadeToTransparent()
    {
        while(materials[materials.Count-1].color.a > 0)
        {
            foreach (Material material in materials)
            {
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a - fadeProgressPerSecond * Time.deltaTime);
                material.color = color;
            }

            yield return null;
        }

        fadeCoroutine = null;
    }


    private IEnumerator FadeToOpaQue()
    {
        while (materials[materials.Count - 1].color.a < 1)
        {
            foreach (Material material in materials)
            {
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a + fadeProgressPerSecond * Time.deltaTime);
                material.color = color;
            }

            yield return null;
        }

        fadeCoroutine = null;
    }

}
