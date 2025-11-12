//#UIManager

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{

    public static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }

    public Button oKButton;
    private Dictionary<string, Action> actionGroups = new Dictionary<string, Action>();
    private List<String> actionExcutionOrder = new List<string>();
    private List<String> preOrderedList = new List<String> { "ItemPickUp", "Shelter1" };

    public Text healthPotionCountText;

    public Text shardCountText;
    public Text shardPlusText;
    private Coroutine shardCoroutine;

    private void Awake()
    {
        if(oKButton != null)
        {
            oKButton.gameObject.SetActive(false);
            oKButton.onClick.AddListener(OnOkButtonClicked);

        }
    }

    private void OnOkButtonClicked()
    {
        if(actionGroups.Count > 0)
        {
            string currentCategory = actionExcutionOrder[0];
            actionExcutionOrder.RemoveAt(0);

            if (actionGroups.ContainsKey(currentCategory))
            {
                actionGroups[currentCategory]?.Invoke();
                actionGroups.Remove(currentCategory);
            }        

            if(actionExcutionOrder.Count == 0)
            {
                oKButton.gameObject.SetActive(false);
            }
        }
    }

    public void ShowOkButton(string actionCategory, Action newAction) 
    {
        if (!actionGroups.ContainsKey(actionCategory))
        {
            actionGroups[actionCategory] = null;
            actionExcutionOrder.Add(actionCategory);
            actionExcutionOrder = actionExcutionOrder.OrderBy(a => preOrderedList.IndexOf(a) == -1 ? int.MaxValue : preOrderedList.IndexOf(a)).ToList();
        }

        actionGroups[actionCategory] += newAction;
        oKButton.gameObject.SetActive(true);
    }

    public void HideOKButton(string actionCategory, Action removeAction)
    {
        if (actionGroups.ContainsKey(actionCategory))
        {
            actionGroups[actionCategory] -= removeAction;

            if (actionGroups[actionCategory] == null)
            {
                actionGroups.Remove(actionCategory);
                actionExcutionOrder.Remove(actionCategory);
            }

            if(actionExcutionOrder.Count == 0)
            {
                oKButton.gameObject.SetActive(false);
            }
        }
    }



    public void UpdateHealthPotionCount(int potionCount)
    {
        if (healthPotionCountText != null)
        {
            healthPotionCountText.text = $"Health Potion : {potionCount}";
        }
    }
    
    public void UpdateShardText(int shardCount, int shardPlusCount)
    {
        if(shardCountText != null)
        {
            shardCountText.gameObject.SetActive(true);
            shardCountText.text = $"Shard : {shardCount}";

        }
        
        if(shardPlusCount != 0)
        {
            if (shardPlusCount > 0)
            {
                shardPlusText.gameObject.SetActive(true);
                shardPlusText.text = $"+ {shardPlusCount}";
            }
            else
            {
                shardPlusText.gameObject.SetActive(true);
                shardPlusText.text = $"{shardPlusCount}";
            }            
        }
        else
        {
            shardPlusText.gameObject.SetActive(false);
        }

        if (shardCoroutine != null)
        {
            StopCoroutine(shardCoroutine);
        }
        shardCoroutine = StartCoroutine(ShardOffCoroutine());

    }
    private IEnumerator ShardOffCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        if(shardPlusText.gameObject.activeSelf)
        {
            shardPlusText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1f);
        shardCountText.gameObject.SetActive(false);
        

    }
    


    



}
