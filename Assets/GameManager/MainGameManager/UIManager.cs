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
            /*
             1) AddListner는 A.AddLisnter(B)는, B를 Event A에 구독하는 것. 

             람다식으로도, 매서드 외에 다른 것들도 직접 처리할 수 있음
             A.AddListner()) => { Debug.Log("저는매서드가아닙니다");});

            2) Event와 Action 구분 정리
                - Action은 사용자가 정의 및 호출 가능하나, Event는 유니티 자체 매서드(?) 라 볼 수 있고, 사용자가 Event를 정의 및 호출할 수 없음
             */
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
            // a.remove(A): a 의 요소 A 를 삭제. a.removeAt(n) : a의 index n 에 해당하는 요소를 삭제

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
            actionGroups[actionCategory] = null; // GPT가 보여준 코드인데, 위 if문 불충족시, 초기화 작업. (혹시 모르니까 처리한듯)
            actionExcutionOrder.Add(actionCategory);
            actionExcutionOrder = actionExcutionOrder.OrderBy(a => preOrderedList.IndexOf(a) == -1 ? int.MaxValue : preOrderedList.IndexOf(a)).ToList();
            // 위의 람다식과 OrderBy(a .. a는 리스트의 요소를 나타내고, OrderBy는 a와 거의 대부분 연계되며, Linq의 일부라 하는데.. 람다식과 Linq를 제대로 배워야 이해할듯..
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
