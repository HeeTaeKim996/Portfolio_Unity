using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterRestInfo_ChoosePanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }
}
