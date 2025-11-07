using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole_CheckPanel : MonoBehaviour
{
    private DebugConsole debugConsole;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        debugConsole = GetComponentInParent<DebugConsole>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    private void OnEnable()
    {
        if(debugConsole != null)
        {
            debugConsole.UpdateCheckPanelText();
        }
    }


}
