
using UnityEngine;
using UnityEngine.UI;


public class ShelterInfo_StatusUpPanel : MonoBehaviour
{
    private ShelterRestInfo shelterRestInfo;
    private CanvasGroup canvasGroup;

    public Text levelText;
    public Text shardText;
    public Text shardToLevelUpText;
    public Text hPLevelText;
    public Text hPText;
    public Text fPLevelText;
    public Text fPText;
    public Text dMLevelText;
    public Text dMText;

    private StatusPanel_HPUpButton hpUpButton;
    private StatusPanel_HPUpCancelButton hpUpCancelButton;
    private StatusPanel_FPUpButton fpUpButton;
    private StatusPanel_FPUPCancelButton fpUpCancelButton;
    private StatusPanel_DMUpButton dmUpButton;
    private StatusPanel_DMUpCancelButton dmUpCancelButton;
    private StatusPanel_ConfirmButton confirmButton;
    private StatusPanel_CancelButton cancelButton;

    private int shard;
    private int shardToLevelUp;

    private int stackForHP;
    private int stackForFP;
    private int stackForDM;




    private void Awake()
    {
        shelterRestInfo = GetComponentInParent<ShelterRestInfo>();
        hpUpButton = GetComponentInChildren<StatusPanel_HPUpButton>();
        hpUpCancelButton = GetComponentInChildren<StatusPanel_HPUpCancelButton>();
        fpUpButton = GetComponentInChildren<StatusPanel_FPUpButton>();
        fpUpCancelButton = GetComponentInChildren<StatusPanel_FPUPCancelButton>();
        dmUpButton = GetComponentInChildren<StatusPanel_DMUpButton>();
        dmUpCancelButton = GetComponentInChildren<StatusPanel_DMUpCancelButton>();
        confirmButton = GetComponentInChildren<StatusPanel_ConfirmButton>();
        cancelButton = GetComponentInChildren<StatusPanel_CancelButton>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        canvasGroup.alpha = 1;
    }

    private void OnEnable()
    {
        CleanUpAllButton();

        stackForHP = 0;
        stackForFP = 0;
        stackForDM = 0;

        if (shelterRestInfo.playerStatusManager != null)
        {
            shelterRestInfo.playerStatusManager.tentativeHpUpLevelPlus = 0;
            shelterRestInfo.playerStatusManager.tentativeFpUpLevelPlus = 0;
            shelterRestInfo.playerStatusManager.tentativeDmUpLevelPlus = 0;
            shelterRestInfo.playerStatusManager.tentativeShard = shelterRestInfo.playerStatusManager.shard;

            shelterRestInfo.playerStatusManager.UpdateTentativeData(0);
        }
    }

    private bool CheckWhetherCanUpButtons(int shard, int shardToLevelUp)
    {
        if (shard >= shardToLevelUp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void UpdatePanelInfoData(StatusPanel_Data newStatusPanelData)
    {
        levelText.text = $"Level : {newStatusPanelData.level}";
        shardText.text = $"Shard : {newStatusPanelData.shard}";
        shardToLevelUpText.text = $"Needed Shard : {newStatusPanelData.shardToLevelUp}";
        hPLevelText.text = $"HPLevel : {newStatusPanelData.hpLevel}";
        fPLevelText.text = $"FPLevel : {newStatusPanelData.fpLevel}";
        dMLevelText.text = $"DMLevel : {newStatusPanelData.dmLevel}";
        hPText.text = $"HP : {newStatusPanelData.hp}";
        fPText.text = $"FP : {newStatusPanelData.fp}";
        dMText.text = $"DM : {newStatusPanelData.dm}";

        shard = newStatusPanelData.shard;
        shardToLevelUp = newStatusPanelData.shardToLevelUp;

        CleanUpAllButton();
        if (CheckWhetherCanUpButtons(shard, shardToLevelUp))
        {
            PostUpUpButtons();
        }
        if(stackForHP > 0)
        {
            hpUpCancelButton.gameObject.SetActive(true);
        }
        if(stackForFP > 0)
        {
            fpUpCancelButton.gameObject.SetActive(true);
        }
        if(stackForDM > 0)
        {
            dmUpCancelButton.gameObject.SetActive(true);
        }
        if(stackForHP + stackForFP + stackForDM > 0)
        {
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
        }
    }


    public void CleanUpAllButton()
    {
        hpUpButton.gameObject.SetActive(false);
        hpUpCancelButton.gameObject.SetActive(false);
        fpUpButton.gameObject.SetActive(false);
        fpUpCancelButton.gameObject.SetActive(false);
        dmUpButton.gameObject.SetActive(false);
        dmUpCancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }
    public void PostUpUpButtons()
    {
        hpUpButton.gameObject.SetActive(true);
        fpUpButton.gameObject.SetActive(true);
        dmUpButton.gameObject.SetActive(true);
    }
    public void PostUpCancelButtons()
    {
        hpUpCancelButton.gameObject.SetActive(true);
        fpUpCancelButton.gameObject.SetActive(true);
        dmUpCancelButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    public void OnPressedConfirmButton()
    {
        stackForHP = 0;
        stackForFP = 0;
        stackForDM = 0;

        shelterRestInfo.playerStatusManager.ApplyTenTativeData();
        shelterRestInfo.playerStatusManager.UpdateTentativeData(0);
    }
    public void OnPressedCancelButton()
    {
        stackForHP = 0;
        stackForFP = 0;
        stackForDM = 0;

        shelterRestInfo.playerStatusManager.tentativeHpUpLevelPlus = 0;
        shelterRestInfo.playerStatusManager.tentativeFpUpLevelPlus = 0;
        shelterRestInfo.playerStatusManager.tentativeDmUpLevelPlus = 0;
        shelterRestInfo.playerStatusManager.tentativeShard = shelterRestInfo.playerStatusManager.shard;

        shelterRestInfo.playerStatusManager.UpdateTentativeData(0);

    }
    public void OnPressedHPUpButton()
    {
        stackForHP++;


        shelterRestInfo.playerStatusManager.tentativeHpUpLevelPlus++;


        shelterRestInfo.playerStatusManager.UpdateTentativeData(-1);
    }
    public void OnPressedHPUpCancelButton()
    {
        stackForHP--;

        shelterRestInfo.playerStatusManager.tentativeHpUpLevelPlus--;


        shelterRestInfo.playerStatusManager.UpdateTentativeData(1);
    }
    public void OnPressedFPUpButton()
    {
        stackForFP++;

        shelterRestInfo.playerStatusManager.tentativeFpUpLevelPlus++;


        shelterRestInfo.playerStatusManager.UpdateTentativeData(-1);
    }
    public void OnPressedFPUpCancelButton()
    {
        stackForFP--;

        shelterRestInfo.playerStatusManager.tentativeFpUpLevelPlus--;


        shelterRestInfo.playerStatusManager.UpdateTentativeData(1);
    }
    public void OnPressedDMUpButton()
    {
        stackForDM++;

        shelterRestInfo.playerStatusManager.tentativeDmUpLevelPlus++;


        shelterRestInfo.playerStatusManager.UpdateTentativeData(-1);
    }
    public void OnPressedDMUpCancelButton()
    {
        stackForDM--;

        shelterRestInfo.playerStatusManager.tentativeDmUpLevelPlus--;

        shelterRestInfo.playerStatusManager.UpdateTentativeData(1);
    }


}
