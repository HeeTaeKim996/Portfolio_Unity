using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObstacleFading_OverlapBox_Custom_Multi : MonoBehaviour
{
    private LayerMask playerLayer;
    private List<Material> materials = new List<Material>();
    private bool isFaded;
    private Coroutine checkCoroutine;
    private Coroutine fadeCoroutine;

    public Vector3 boxPosition;
    public Vector3 boxSize;
    public Vector3 boxRotation;

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
            foreach(Renderer renderer in renderers)
            {
                materials.AddRange(renderer.materials);
            }
        }
    }

    private void OnEnable()
    {
        isFaded = false;

        if(checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
        checkCoroutine = StartCoroutine(CheckPlayerCoroutine());
    }

    private IEnumerator CheckPlayerCoroutine()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position + boxPosition, boxSize / 2f, Quaternion.Euler(boxRotation), playerLayer); // Size에서 2로 나누는 이유는, Physics.OverlapBox의 사이즈는 2로 나눈 것을 기준으로 작동. 반대로 기즈모 생성 박스는 2로 나눌 필요 없음
            if(colliders.Length > 0)
            {
                if (!isFaded)
                {
                    if(fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(fadeToTransparent());
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
                    fadeCoroutine = StartCoroutine(fadeToOpaque());
                    isFaded = false;
                }
            }

            yield return new WaitForSeconds(StaticValues.fadeObjectFadeCheckInterval);
        }
    }

    private IEnumerator fadeToTransparent()
    {
        while (materials[materials.Count - 1].color.a > 0)
        {
            foreach (Material material in materials)
            {
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a - StaticValues.fadeObjectFadeTime * Time.deltaTime);
                material.color = color;
                //Debug.Log(material.color.a);
            }

            yield return null;
        }
        fadeCoroutine = null;
    }

    private IEnumerator fadeToOpaque()
    {
        while (materials[materials.Count - 1].color.a < 1)
        {
            foreach(Material material in materials)
            {
                Color color = material.color;
                color.a = Mathf.Clamp01(color.a + StaticValues.fadeObjectFadeTime * Time.deltaTime);
                material.color = color;
            }
            yield return null;
        }

        fadeCoroutine = null;
    }

}
