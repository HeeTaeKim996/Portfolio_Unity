
using UnityEngine;
using UnityEngine.UI;

public class StatusInfoPanel : MonoBehaviour
{
    private PlayerStatusManager playerStatusManager;

    public Text levelText;
    public Text shardText;
    public Text healthText;
    public Text staminaText;
    public Text damageText;
    public Text weightText;
    public Text defenseText;

    private void Awake()
    {
        playerStatusManager = FindObjectOfType<PlayerStatusManager>();

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
    }

    public void UpdateStatusInfo(InformStatusData newInformStatusData)
    {
        levelText.text = $"Level : {newInformStatusData.level}";
        shardText.text = $"Shard : {newInformStatusData.shard}";
        healthText.text = $"Health : {newInformStatusData.health}";
        staminaText.text = $"Stamina : {newInformStatusData.stamina}";
        damageText.text = $"Damage : {newInformStatusData.damage}";
        weightText.text = $"Weight : {newInformStatusData.weight}";
        defenseText.text = $"Defense : {newInformStatusData.defense}";

    }



}
