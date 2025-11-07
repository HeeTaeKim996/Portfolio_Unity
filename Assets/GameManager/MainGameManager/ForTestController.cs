//#ForTestCotroller

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForTestCotroller : MonoBehaviour
{
    private RectTransform rectTransform;

    private int activeTouchId;
    public RectTransform inventoryStartTransform;
    private bool didItTouchStartButton;

    public GameObject testerButtons;
    private CanvasGroup testerCanvasGroup;

    public List<RectTransform> testRects;

    public GameObject timeScaleController;
    private int timeScaleControllerInt = 0;

    // Rect5 : TempSphere
    public GameObject tempSpherePrefab;
    private GameObject tempSphere;
    private bool didInstamtiateTempSphere = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        testerCanvasGroup = testerButtons.GetComponent<CanvasGroup>();
        testerCanvasGroup.alpha = 1;
    }

    private void Start()
    {
        testerButtons.gameObject.SetActive(false);
        timeScaleController.gameObject.SetActive(false);
    }


    private void Update()
    {
        for(int i=0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (activeTouchId == -1|| activeTouchId == touch.fingerId )
            {
                if(touch.phase == TouchPhase.Began && isTouchWithinRect(touch.position, inventoryStartTransform))
                {
                    didItTouchStartButton = true;

                    testerButtons.gameObject.SetActive(true);
                }

                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchStartButton)
                    {
                        if (isTouchWithinRect(touch.position, testRects[0]))
                        {
                            SaveSystem.ResetPlayerJsonData();                           
                        }
                        else if(isTouchWithinRect(touch.position, testRects[1]))
                        {
                            SaveSystem.ResetEquipmentJsonData();
                        }
                        else if (isTouchWithinRect(touch.position, testRects[2]))
                        {
                            DebugConsole.instance.checkPanel.gameObject.SetActive(true);
                        }
                        else if(isTouchWithinRect(touch.position, testRects[3]))
                        {
                            if(timeScaleControllerInt == 0)
                            {
                                timeScaleControllerInt++;
                                timeScaleController.gameObject.SetActive(true);
                            }
                            else if(timeScaleControllerInt == 1)
                            {
                                timeScaleControllerInt--;
                                timeScaleController.gameObject.SetActive(false);
                            }
                        }
                        else if(isTouchWithinRect(touch.position, testRects[4]))
                        {
                            if (!didInstamtiateTempSphere)
                            {
                                tempSphere = Instantiate(tempSpherePrefab);
                                didInstamtiateTempSphere = true;
                            }
                            else
                            {
                                Destroy(tempSphere);
                                didInstamtiateTempSphere = false;
                            }
                        }
                        else if(isTouchWithinRect(touch.position, testRects[5]))
                        {
                            SceneManager.LoadScene("MainMenuScene");
                        }
                        else if(isTouchWithinRect(touch.position, testRects[6]))
                        {
                            UIInformController.instance.PostMidInformPanel("Hi", 2f);
                        }
                        else if(isTouchWithinRect(touch.position, testRects[7]))
                        {
                            GameManager.instance.chunkManager.DeActiveAllChunks();
                        }
                        else if(isTouchWithinRect(touch.position, testRects[8]))
                        {
                            GameManager.instance.chunkManager.ReviveAllChunks();
                        }
                        else if(isTouchWithinRect(touch.position, testRects[9]))
                        {
                            GameManager.instance.chunkManager.SuicideActiveChunks();
                        }
                        else if(isTouchWithinRect(touch.position, testRects[10]))
                        {
                            GameManager.instance.chunkManager.ActiveActiveChunks();
                            Debug.Log("Check1");
                        }

                        activeTouchId = -1;
                        didItTouchStartButton = false;

                        testerButtons.gameObject.SetActive(false);
                    }
                }
            }
            
        }
    }


    private bool isTouchWithinRect(Vector2 touchPosition, RectTransform rectTransform)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, null, out localPoint);

        return rectTransform.rect.Contains(localPoint);
    }
}
