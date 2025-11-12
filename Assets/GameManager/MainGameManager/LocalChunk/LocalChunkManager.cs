
using System.Collections.Generic;
using UnityEngine;

public class LocalChunkManager : MonoBehaviour
{
    private WorldChunk worldChunk;
    private List<LocalChunk> allChunks = new List<LocalChunk>();
    [HideInInspector]
    public List<LocalChunk> activeChunks = new List<LocalChunk>();
    [HideInInspector]
    public List<LocalChunk> loadFinishedChunks = new List<LocalChunk>();


    private float activeDistance = 25f;
    private float deActiveDistance = 30f;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.OnPlayerPositionUpdate += UpdateChunks;

            GameManager.instance.chunkManager.ReviveAllChunksEvent += ReviveAllChunks;
            GameManager.instance.chunkManager.DeActiveAllChunksEvent += DeActiveAllChunks;
            GameManager.instance.chunkManager.ActiveActiveChunksEvent += ActiveActiveChunks;
            GameManager.instance.chunkManager.SuicideActiveChunksEvent += SuicideActiveChunks;
        }

        worldChunk = GetComponentInParent<WorldChunk>();
        allChunks.AddRange(GetComponentsInChildren<LocalChunk>());
        activeChunks.AddRange(GetComponentsInChildren<LocalChunk>());
    }

    private void Start()
    {
        foreach(LocalChunk chunk in allChunks)
        {
            chunk.RegisterNonStructures(worldChunk.nonClassStuctures);
            chunk.RegisterTerrains(worldChunk.terrains);
        }
    }
    private void OnDestroy()
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.OnPlayerPositionUpdate -= UpdateChunks;

            GameManager.instance.chunkManager.ReviveAllChunksEvent -= ReviveAllChunks;
            GameManager.instance.chunkManager.DeActiveAllChunksEvent -= DeActiveAllChunks;
            GameManager.instance.chunkManager.ActiveActiveChunksEvent -= ActiveActiveChunks;
            GameManager.instance.chunkManager.SuicideActiveChunksEvent -= SuicideActiveChunks;
        }
    }
    public LocalChunk GetChunkAtPosition(Vector3 position)
    {
        foreach (LocalChunk chunk in allChunks)
        {
            if (chunk.GetComponent<Collider>().bounds.Contains(position))
            {
                return chunk;
            }
        }
        return null;
    }


    public void UpdateChunks(Vector3 playerPosition)
    {
        foreach(LocalChunk chunk in allChunks)
        {
            float distance = Vector3.Distance(playerPosition, chunk.transform.position);
            if(distance < activeDistance){


                if (!activeChunks.Contains(chunk))
                {
                    activeChunks.Add(chunk);
                    GetBatchSize();
                    chunk.Active();
                }               
            }
            else if(distance >= deActiveDistance)
            {
                if (activeChunks.Contains(chunk))
                {
                    activeChunks.Remove(chunk);
                    chunk.Suicide();
                }                
            }
        }
    }




    public List<LocalChunk> getActiveChunks()
    {
        return activeChunks;
    }
    

    public void ReviveAllChunks()
    {
        foreach(LocalChunk chunk in allChunks)
        {
            if (!activeChunks.Contains(chunk))
            {
                activeChunks.Add(chunk);
            }
            chunk.Revive();
        }
        Debug.Log("ReviveAllChunksCheck");
    }


    public void DeActiveAllChunks()
    {
        foreach(LocalChunk chunk in allChunks)
        {
            chunk.DeActive();
        }
        Debug.Log("DeActiveAllChunksCheck");
    }

    public void ActiveActiveChunks()
    {
        foreach(LocalChunk chunk in activeChunks)
        {
            chunk.Active();
        }
        Debug.Log("ActiveActiveChunksCheck");
    }
    public void SuicideActiveChunks()
    {
        foreach(LocalChunk chunk in activeChunks)
        {
            chunk.Suicide();
        }
        Debug.Log("SuicideActiveChunksCheck");
    }




    // PrimaryMethods


    public void GetBatchSize()
    {
        int loadingChunkCount = activeChunks.Count - loadFinishedChunks.Count;

        float batchSize = (float) 30f / loadingChunkCount;

        foreach(LocalChunk chunk in activeChunks)
        {
            chunk.activeBatchSize = batchSize;
        }

        if(loadingChunkCount < 0)
        {

        }

    }
}
