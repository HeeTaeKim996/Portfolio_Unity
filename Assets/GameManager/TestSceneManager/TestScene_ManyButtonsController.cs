//#ForTestCotroller

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScene_ManyButtonsController : MonoBehaviour
{
    private RectTransform rectTransform;

    private int activeTouchId;
    public RectTransform manyButtonsStartRect;
    private bool didItTouchStartButton;

    public GameObject testerButtons;
    private CanvasGroup testerCanvasGroup;


    public List<RectTransform> rectTransforms;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        testerCanvasGroup = testerButtons.GetComponent<CanvasGroup>();
        testerCanvasGroup.alpha = 1;

    }

    private void Start()
    {
        testerButtons.gameObject.SetActive(false);
    }


    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (activeTouchId == -1 || activeTouchId == touch.fingerId)
            {
                if (touch.phase == TouchPhase.Began && isTouchWithinRect(touch.position, manyButtonsStartRect))
                {
                    didItTouchStartButton = true;

                    testerButtons.gameObject.SetActive(true);
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchStartButton)
                    {
                        if(isTouchWithinRect(touch.position, manyButtonsStartRect))
                        {
                            TestScene_EnemyController.instance.SetIsTest();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[0]))
                        {
                            TestScene_EnemyController.instance.EnemyAction();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[1]))
                        {
                            TestScene_EnemyController.instance.EnemyAction1();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[2]))
                        {
                            TestScene_EnemyController.instance.EnemyAction2();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[3]))
                        {
                            TestScene_EnemyController.instance.EnemyAction3();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[4]))
                        {
                            TestScene_EnemyController.instance.EnemyAction4();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[5]))
                        {
                            TestScene_EnemyController.instance.EnemyAction5();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[6]))
                        {
                            TestScene_EnemyController.instance.EnemyAction6();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[7]))
                        {
                            TestScene_EnemyController.instance.EnemyAction7();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[8]))
                        {
                            TestScene_EnemyController.instance.EnemyAction8();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[9]))
                        {
                            TestScene_EnemyController.instance.EnemyAction9();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[10]))
                        {
                            TestScene_EnemyController.instance.EnemyAction10();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[11]))
                        {
                            TestScene_EnemyController.instance.EnemyAction11();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[12]))
                        {
                            TestScene_EnemyController.instance.EnemyAction12();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[13]))
                        {
                            TestScene_EnemyController.instance.EnemyAction13();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[14]))
                        {
                            TestScene_EnemyController.instance.EnemyAction14();
                        }
                        else if (isTouchWithinRect(touch.position, rectTransforms[15]))
                        {
                            TestScene_EnemyController.instance.EnemyAction15();
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
