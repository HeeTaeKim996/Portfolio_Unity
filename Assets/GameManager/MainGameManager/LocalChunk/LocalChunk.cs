
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalChunk : MonoBehaviour
{
    private LocalChunkManager localChunkManager;
    private Collider collider1;

    private List<FieldEnemy> chunkEnemies = new List<FieldEnemy>();
    private List<GameObject> nonClassStructures = new List<GameObject>();
    private List<Renderer> terrainRenderers = new List<Renderer>();

    private Coroutine actSuiCoroutine = null;

    [HideInInspector]
    public float activeBatchSize;

    private void Awake()
    {
        localChunkManager = GetComponentInParent<LocalChunkManager>();
        collider1 = GetComponent<Collider>();
    }

    private void Start()
    {
        if (!localChunkManager.loadFinishedChunks.Contains(this))
        {
            localChunkManager.loadFinishedChunks.Add(this);
            localChunkManager.GetBatchSize();
        }
    }

    public void ModifyBatchSize(float newActiveBatchSize)
    {

    }

    public void RegisterNonStructures(List<Transform> newNonClassStructureList)
    {
        foreach(Transform transform in newNonClassStructureList)
        {
            if (collider1.bounds.Contains(transform.transform.position))
            {
                nonClassStructures.Add(transform.gameObject);
            }
        }
    }

    public void RegisterTerrains(List<Renderer> newTerrains)
    {
        foreach(Renderer renderer in newTerrains)
        {
            if (collider1.bounds.Contains(renderer.transform.position))
            {
                terrainRenderers.Add(renderer);
            }
        }
    }


    public void RegisterEnemy(FieldEnemy fieldEnemy)
    {
        if (!chunkEnemies.Contains(fieldEnemy))
        {
            chunkEnemies.Add(fieldEnemy);
        }
    }

    public void UnRegisterEnemy(FieldEnemy fieldEnemy)
    {
        if (chunkEnemies.Contains(fieldEnemy))
        {
            chunkEnemies.Remove(fieldEnemy);
        }
    }

    // 로컬청크 연산효율작업
    public void Active()
    {
        if(actSuiCoroutine != null)
        {
            StopCoroutine(actSuiCoroutine);
        }
        actSuiCoroutine = StartCoroutine(ActiveCoroutine());
    }
    private IEnumerator ActiveCoroutine()
    {
        int count = 0;

        foreach (FieldEnemy fieldEnemy in chunkEnemies)
        {
            if (!fieldEnemy.gameObject.activeSelf)
            {
                fieldEnemy.resetState = ResetState.OnField;
                fieldEnemy.gameObject.SetActive(true);
            }
            else if (fieldEnemy.suicideCoroutine != null)
            {
                fieldEnemy.StopSuicide();
            }

            count += 15;
            if (count>= activeBatchSize)
            {
                count = 0;
                yield return null;
            }
        }

        foreach (GameObject nonClassStructure in nonClassStructures)
        {
            nonClassStructure.SetActive(true);

            count += 3;
            if (count >= activeBatchSize)
            {
                count = 0;
                yield return null;
            }
        }
        foreach (Renderer terrainRenderer in terrainRenderers)
        {
            terrainRenderer.enabled = true;

            count += 3;
            if (count >= activeBatchSize)
            {
                count = 0;
                yield return null;
            }
        }


        actSuiCoroutine = null;
        if (!localChunkManager.loadFinishedChunks.Contains(this))
        {
            localChunkManager.loadFinishedChunks.Add(this);
            localChunkManager.GetBatchSize();
        }
    }
    public void Suicide()
    {
        if (actSuiCoroutine != null)
        {
            StopCoroutine(actSuiCoroutine);
        }
        actSuiCoroutine = StartCoroutine(SuicideCoroutine());
        if (localChunkManager.loadFinishedChunks.Contains(this))
        {
            localChunkManager.loadFinishedChunks.Remove(this);
        }
    }
    private IEnumerator SuicideCoroutine()
    {
        int suicidBatchSize = 50;
        int count = 0;

        foreach (FieldEnemy fieldEnemy in chunkEnemies)
        {
            if (fieldEnemy.gameObject.activeSelf)
            {
                fieldEnemy.Suicide();
            }

            count++;
            if (count >= suicidBatchSize)
            {
                count = 0;
                yield return null;
            }
        }

        foreach (GameObject nonClassStructure in nonClassStructures)
        {
            nonClassStructure.SetActive(false);

            count++;
            if (count >= suicidBatchSize)
            {
                count = 0;
                yield return null;
            }
        }
        foreach (Renderer terrainRenderer in terrainRenderers)
        {
            terrainRenderer.enabled = false;

            count++;
            if (count >= suicidBatchSize)
            {
                count = 0;
                yield return null;
            }
        }


        actSuiCoroutine = null;
    }



    // 재생성작업
    public void DeActive()
    {
        foreach(FieldEnemy fieldEnemy in chunkEnemies)
        {
            if (fieldEnemy.gameObject.activeSelf)
            {
                fieldEnemy.gameObject.SetActive(false);
            }
        }
    }

    public void Revive()
    {
        for(int i = chunkEnemies.Count - 1; i >= 0; i--)
        {
            FieldEnemy fieldEnemy = chunkEnemies[i];

            chunkEnemies.RemoveAt(i);
            fieldEnemy.resetState = ResetState.Revive;
            fieldEnemy.gameObject.SetActive(true);
        }



        /*
        foreach(FieldEnemy fieldEnemy in chunkEnemies)
        {
            if (!fieldEnemy.gameObject.activeSelf)
            {
                FieldEnemy tempForReviveEnemy = fieldEnemy;
                chunkEnemies.Remove(fieldEnemy);
                tempForReviveEnemy.resetState = ResetState.Revive;
                tempForReviveEnemy.gameObject.SetActive(true);
            }
        }
        */
    }

    
}
