using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole instance;
    [HideInInspector]
    public DebugConsole_CheckPanel checkPanel;
    public Text checkPanelText;
    public Text consoleText;
    private List<string> logLines = new List<string>();
    private List<string> checkPanelLines = new List<string>();
    private int maxLines = 10;
    private int checkPanelMaxLines = 18;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        checkPanel = GetComponentInChildren<DebugConsole_CheckPanel>();
    }
    private void Start()
    {
#if false
        Application.logMessageReceived += HandleLog;
        // Application.logMessageReceived �� ����Ƽ�� ž��� event��, Debug.Log�� �α׿����� ���� ������ ȣ���
#endif
        checkPanel.gameObject.SetActive(false);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // consoleText
        logLines.Add(logString);

        StartCoroutine(RemoveAfterTime(logString, 5f));

        if(logLines.Count > maxLines)
        {
            logLines.RemoveAt(0);
        }

        if (consoleText != null)
        {
            consoleText.text = string.Join("\n", logLines.ToArray());
        }



        // checkPanelText
        checkPanelLines.Add(logString);

        if(checkPanelLines.Count > checkPanelMaxLines)
        {
            checkPanelLines.RemoveAt(0);
        }

        if(checkPanelText != null)
        {
            checkPanelText.text = string.Join("\n", checkPanelLines.ToArray());
        }

    }
    private System.Collections.IEnumerator RemoveAfterTime(string newLogString, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (logLines.Contains(newLogString))
        {
            logLines.Remove(newLogString);
            
            if(consoleText != null)
            {
                consoleText.text = string.Join("\n", logLines.ToArray());
            }
        }
    }

    public void UpdateCheckPanelText()
    {
        if (checkPanelText != null)
        {
            checkPanelText.text = string.Join("\n", checkPanelLines.ToArray());
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

}
