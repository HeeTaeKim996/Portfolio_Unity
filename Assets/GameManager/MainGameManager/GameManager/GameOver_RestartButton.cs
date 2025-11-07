using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOver_RestartButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameOverManager gameOverManager;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        gameOverManager = GetComponentInParent<GameOverManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnButton = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && isOnButton)
        {
            gameOverManager.RestartGame();
        }

        isPressed = false;
    }
}
