// Controller

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private RectTransform rectTransform;

    private Vector2 touchStartPosition;
    private Vector2 touchcurrentPosition;
    private float buffer;
    [HideInInspector]
    public Vector2 refinedMoveVector;

    private float backgroundRadius;
    private float circleProportion = 0.9f;
    private float controllerRadius;

    public RectTransform moveRectTransform;
    public RectTransform backRectTransform;


    private bool didItTouchMovingButton = false;
    [HideInInspector]
    public bool isMoving = false;

    private int activeTouchId = -1;

    // dodge
    public RectTransform dodgeRectTransform;
    private float dodgeRadius;
    private int activeTouchId2 = -1;

    private bool didItTouchDodgeButton = false;

    [HideInInspector]
    public bool isShielding = false;

    [HideInInspector]
    public bool runCountStart = false;


    // Attack
    public RectTransform attackRectTransform;
    private float attackRadius;
    private int activeTouchId3 = -1;
    private bool didItTouchAttackButton = false;
    private bool isHeavyCharging = false;

    // UseItem
    public RectTransform itemRectTransform;
    private float itemRadius;
    private int activeTouchId4 = -1;
    private bool didItTouchItemButton = false;


    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        controllerRadius = moveRectTransform.rect.width / 2f * circleProportion;

        backgroundRadius = backRectTransform.rect.width / 2f * circleProportion;

        // dodge
        dodgeRadius = dodgeRectTransform.rect.width / 2f * circleProportion;

        // attack
        attackRadius = attackRectTransform.rect.width / 2f * circleProportion;

        // Item
        itemRadius = itemRectTransform.rect.width / 2f * circleProportion;

    }

    private void FixedUpdate()
    {
        isMoving = false;
        isShielding = false;
        runCountStart = false;

        
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (activeTouchId == -1 || activeTouchId == touch.fingerId)
            {

                if (touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, backRectTransform, backRectTransform.rect.center, backgroundRadius))
                {
                    activeTouchId = touch.fingerId;
                    didItTouchMovingButton = true;
                    touchStartPosition = touch.position;
                    moveRectTransform.anchoredPosition = touchStartPosition;
                    backRectTransform.anchoredPosition = touchStartPosition;
                }

                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (didItTouchMovingButton)
                    {
                        touchcurrentPosition = touch.position;
                        Vector2 moveVector = touchcurrentPosition - touchStartPosition;

                        if (controllerRadius < moveVector.magnitude)
                        {
                            buffer = moveVector.magnitude;
                        }
                        else
                        {
                            buffer = controllerRadius;
                        }
                        refinedMoveVector = moveVector / buffer;
                        playerMovement.InvokeMoving(refinedMoveVector);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    didItTouchMovingButton = false;
                    refinedMoveVector = Vector2.zero;
                    moveRectTransform.anchoredPosition = new Vector2(400f, 350f);
                    backRectTransform.anchoredPosition = new Vector2(400f, 350f);
                    activeTouchId = -1;
                }
            }

            //dodge
            if (activeTouchId2 == -1 || activeTouchId2 == touch.fingerId)
            {
                if (touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, dodgeRectTransform, dodgeRectTransform.rect.center, dodgeRadius))
                {
                    activeTouchId2 = touch.fingerId;
                    didItTouchDodgeButton = true;
                    dodgeRectTransform.anchoredPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (didItTouchDodgeButton)
                    {
                        if (isTouchWithinCircle(touch.position, dodgeRectTransform, dodgeRectTransform.rect.center, dodgeRadius))
                        {
                            playerMovement.InvokeShielding();
                        }
                        else
                        {
                            playerMovement.InvokeRunning();
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchDodgeButton)
                    {
                        if (isTouchWithinCircle(touch.position, dodgeRectTransform, dodgeRectTransform.rect.center, dodgeRadius))
                        {
                            playerMovement.InvokeDodge();
                        }
                        didItTouchDodgeButton = false;
                        dodgeRectTransform.anchoredPosition = new Vector2(1650f, 300f);
                        activeTouchId2 = -1;
                    }
                }
            }

            // Attack
            if (activeTouchId3 == -1 || activeTouchId3 == touch.fingerId)
            {
                if (touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, attackRectTransform, attackRectTransform.rect.center, attackRadius))
                {
                    activeTouchId3 = touch.fingerId;
                    didItTouchAttackButton = true;
                    attackRectTransform.anchoredPosition = touch.position;
                }

                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (didItTouchAttackButton)
                    {
                        if (!isTouchWithinCircle(touch.position, attackRectTransform, attackRectTransform.rect.center, attackRadius))
                        {
                            isHeavyCharging = true;
                            playerMovement.InvokeHeavyAttackCharging();
                        }
                        else if (isHeavyCharging)
                        {
                            playerMovement.InvokeHeavyAttackCharging();
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchAttackButton)
                    {
                        if (!isHeavyCharging)
                        {
                            playerMovement.InvokeLightAttack();
                        }
                        else
                        {
                            playerMovement.InvokeHeavyAttack();
                        }

                        InvokeAttackButtonEndPhase();
                    }
                }
            }
                // UseITem
            if (activeTouchId4 == -1 || activeTouchId4 == touch.fingerId)
            {
                if(touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, itemRectTransform, itemRectTransform.rect.center, itemRadius))
                {
                    activeTouchId4 = touch.fingerId;
                    didItTouchItemButton = true;
                    itemRectTransform.anchoredPosition = touch.position;
                }

                else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchItemButton)
                    {
                        playerMovement.InvokeHealthPotionDrink();

                        didItTouchItemButton = false;
                        itemRectTransform.anchoredPosition = new Vector2(2150f, 900f);
                        activeTouchId4 = -1;
                    }
                }        
            }                                 
        }
    }
    
    private bool isTouchWithinCircle(Vector2 touchPosition, RectTransform rectTransform, Vector2 circleCenter, float radius)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, null, out localPoint);
        float distance = Vector2.Distance(localPoint, circleCenter);
        return distance <= radius;
    }

    public void InvokeAttackButtonEndPhase()
    {
        didItTouchAttackButton = false;
        isHeavyCharging = false;
        attackRectTransform.anchoredPosition = new Vector2(1950f, 550f);
        activeTouchId3 = -1;
    }
}