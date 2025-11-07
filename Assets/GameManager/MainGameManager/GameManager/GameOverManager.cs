using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private CanvasGroup canvasGroup;
    private GameOver_RestartButton restartButton;

    private Vector3 respawnPosition;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        canvasGroup = GetComponent<CanvasGroup>();
        restartButton = GetComponentInChildren<GameOver_RestartButton>();

        respawnPosition = playerHealth.transform.position; // 리스폰 위치 지정안할시, 우선은 시작 위치를 리스폰 위치로 임시로 지정함
    }
    private void Start()
    {
        canvasGroup.alpha = 0;
        restartButton.gameObject.SetActive(false);
    }
    public void UpdateRespawnTransform(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    public void ActiveGameOverManager()
    {
        canvasGroup.alpha = 1;
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        playerHealth.transform.position = respawnPosition;
        playerHealth.enabled = true;

        

        canvasGroup.alpha = 0;
        restartButton.gameObject.SetActive(false);
    }

    private IEnumerator ResetChunkCoroutine()
    {
        GameManager.instance.chunkManager.DeActiveAllChunks();
        yield return null;
        GameManager.instance.chunkManager.ReviveAllChunks();
    }
}
