using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterRestInfo : MonoBehaviour
{
    [HideInInspector]
    public ShelterRestInfo_ChoosePanel shelterRestInfoChoosePanel;
    private Shelter shelterClass;
    [HideInInspector]
    public ShelterInfo_StatusUpPanel statusUpPanel;
    private ShelterInfo_StatusPanel_ExitButton statusExitButton;
    [HideInInspector]
    public PlayerStatusManager playerStatusManager;

    private void Awake()
    {
        statusUpPanel = GetComponentInChildren<ShelterInfo_StatusUpPanel>();
        statusExitButton = GetComponentInChildren<ShelterInfo_StatusPanel_ExitButton>();
        playerStatusManager = FindObjectOfType<PlayerStatusManager>();
        shelterRestInfoChoosePanel = GetComponentInChildren<ShelterRestInfo_ChoosePanel>();
    }
    private void Start()
    {
        shelterRestInfoChoosePanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        shelterRestInfoChoosePanel.gameObject.SetActive(true);
        statusUpPanel.gameObject.SetActive(false);
    }

    public void ActiveInfoChoosePanel(Shelter newShelterClass)
    {
        shelterRestInfoChoosePanel.gameObject.SetActive(true);
        shelterClass = newShelterClass;
    }
    public void DeactiveInfoChoosePanel()
    {
        shelterRestInfoChoosePanel.gameObject.SetActive(false);
        shelterClass.InvokeShelterRestFinished();
    }


}
