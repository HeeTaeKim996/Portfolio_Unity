using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public event Action ReviveAllChunksEvent;
    public event Action DeActiveAllChunksEvent;
    public event Action ActiveActiveChunksEvent;
    public event Action SuicideActiveChunksEvent;



    public void ReviveAllChunks()
    {
        ReviveAllChunksEvent?.Invoke();
    }
    public void DeActiveAllChunks()
    {
        DeActiveAllChunksEvent?.Invoke();
    }
    public void ActiveActiveChunks()
    {
        Debug.Log("Check");
        ActiveActiveChunksEvent?.Invoke();
    }
    public void SuicideActiveChunks()
    {
        Debug.Log("Check");
        SuicideActiveChunksEvent?.Invoke();
    }
}
