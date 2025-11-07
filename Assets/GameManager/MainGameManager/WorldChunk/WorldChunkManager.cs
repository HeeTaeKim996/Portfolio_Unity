using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager : MonoBehaviour
{
    public Transform playerTransform;
    private float loadDistance = 60f;
    private float unloadDistance = 70f;


    public List<WorldChunkData> worldChunks = new List<WorldChunkData>();

    private Dictionary<Vector3, GameObject> activeChunks = new Dictionary<Vector3, GameObject>();

    private void Start()
    {
        StartCoroutine(UpdateWorldChunks());
    }
    private IEnumerator UpdateWorldChunks()
    {
        while (true)
        {
            UpdateChunks();
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateChunks()
    {
        foreach(WorldChunkData chunkData in worldChunks)
        {
            float distance = Vector3.Distance(playerTransform.position, chunkData.position);

            if(distance <= loadDistance && !activeChunks.ContainsKey(chunkData.position)){
                LoadChunk(chunkData);
            }

            if(distance > unloadDistance && activeChunks.ContainsKey(chunkData.position))
            {
                UnloadChunk(chunkData.position);
            }
        }

    
    }


    private void LoadChunk(WorldChunkData chunkData)
    {
        GameObject chunk = Instantiate(chunkData.prefab, chunkData.position, Quaternion.identity);
        activeChunks[chunkData.position] = chunk; // 이렇게 하여, activeChunks.Add 와 같은 효과(+중복방지효과기능)
    }

    private void UnloadChunk(Vector3 chunkPos)
    {
        if (activeChunks.ContainsKey(chunkPos))
        {
            Destroy(activeChunks[chunkPos]);
            activeChunks.Remove(chunkPos);
        }
    }
}
