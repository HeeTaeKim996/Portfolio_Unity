using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]
    public GameOverManager gameOverManager;
    [HideInInspector]
    public CinemachineController cinemachineController;
    [HideInInspector]
    public DatabaseManager databaseManager;
    private PlayerHealth playerHealth;
    [HideInInspector]
    public PlayerStatusManager playerStatusManager;
    [HideInInspector]
    public PlayerStartPositionTester playerStartPositionTester;
    [HideInInspector]
    public ChunkManager chunkManager;


    public event Action<Vector3> OnPlayerPositionUpdate;

    //FPS
    private int targetFrameRate = 30;

    private void Awake()
    {
        // @@ 하단 2구문은 필요할 때 사용
        //SaveSystem.ResetPlayerJsonData();
        //SaveSystem.ResetEquipmentJsonData();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerHealth = FindObjectOfType<PlayerHealth>();
        gameOverManager = GetComponentInChildren<GameOverManager>();
        cinemachineController = GetComponentInChildren<CinemachineController>();
        databaseManager = FindObjectOfType<DatabaseManager>();
        playerStartPositionTester = FindObjectOfType<PlayerStartPositionTester>();
        chunkManager = GetComponentInChildren<ChunkManager>();
        playerStatusManager = FindObjectOfType<PlayerStatusManager>();

        StartCoroutine(UpdatePlayerPosition());
    }

    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        Time.fixedDeltaTime = 1f / targetFrameRate / 2f;
        AudioListener.volume = 0.5f;
    }

    private IEnumerator UpdatePlayerPosition()
    {
        while (true)
        {
            if (playerHealth != null)
            {           
                OnPlayerPositionUpdate?.Invoke(playerHealth.transform.position);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    
}
