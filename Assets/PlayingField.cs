using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingField : MonoBehaviour
{

    GameObject block;

    [SerializeField]
    GameObject[] cachedBlocks;

    const int MaxBlockPoolSize = 300;

    static Queue<Block> QueuedBlocks = new Queue<Block>(MaxBlockPoolSize);

    private void Awake()
    {
        block = Resources.Load<GameObject>("Block");
        PoolBlocksIntoScene();
    }

    void PoolBlocksIntoScene()
    {
        cachedBlocks = new GameObject[MaxBlockPoolSize];
        for(int i = 0; i < MaxBlockPoolSize; i++)
        {
            GameObject newBlock = Instantiate(block, transform);
            newBlock.SetActive(false);

            #region Calculate and change Block Scaling

            #endregion

            cachedBlocks[i] = newBlock;
        }
    }

    public static void AddToChain(Block block)
    {
        QueuedBlocks.Enqueue(block);
    }
}
