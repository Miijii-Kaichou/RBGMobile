using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingField : MonoBehaviour
{

    GameObject block;

    [SerializeField, Header("Testing")]
    ConceptTest test;

    [SerializeField]
    GameObject[] cachedBlockObjects;

    const int MaxBlockPoolSize = 300;

    static Queue<Block> QueuedBlocks = new Queue<Block>(MaxBlockPoolSize);

    static bool collectionActive = false;
    public static bool CollectionActive => collectionActive;

    private void Start()
    {
        cachedBlockObjects = new GameObject[MaxBlockPoolSize];
        for (int i = 0; i < test.Blocks.Length; i++)
        {
            cachedBlockObjects[i] = test.Blocks[i].gameObject;
        }
        //block = Resources.Load<GameObject>("Block");
        //PoolBlocksIntoScene();
    }

    void PoolBlocksIntoScene()
    {
        cachedBlockObjects = new GameObject[MaxBlockPoolSize];
        for (int i = 0; i < MaxBlockPoolSize; i++)
        {
            GameObject newBlock = Instantiate(block, transform);
            newBlock.SetActive(false);

            #region Calculate and change Block Scaling

            #endregion

            cachedBlockObjects[i] = newBlock;
        }
    }

    /// <summary>
    /// Attempt to eliminate the selected blocks
    /// </summary>
    internal static void AttempChainCollection()
    {
        if (!collectionActive)
        {
            collectionActive = true;
            Block[] blocksToChain = new Block[QueuedBlocks.Count];

            int i = 0;

            //TODO: Dequeue all QueueBlocks, and deselect them
            while (QueuedBlocks.Count > 0)
            {
                Block block = QueuedBlocks.Dequeue();
                blocksToChain[i] = block;
                i++;
            }

            const int validChainCount = 4;
            int chainCount = 0;
            int lastBlockIndex = blocksToChain.Length - 1;
            ColorType startingColor = blocksToChain[0].Color;
            for (int j = 0; j < blocksToChain.Length; j++)
            {
                if (blocksToChain[j].Color == startingColor)
                {
                    chainCount++;

                    if (j == lastBlockIndex)
                    {
                        if (chainCount >= validChainCount)
                        {
                            //TODO: Validate Block Chain
                            ValidateCollection(ref blocksToChain);
                        }
                        else
                        {
                            //Otherwise, invalidate again
                            InvalidateCollection(ref blocksToChain);
                        }
                    }
                }
                else if (blocksToChain[j].Color != startingColor || chainCount < validChainCount)
                {
                    InvalidateCollection(ref blocksToChain);
                    return;
                }
            }
        }
    }

    static void ValidateCollection(ref Block[] referencedChain)
    {
        for (int i = 0; i < referencedChain.Length; i++)
        {
            //TODO: Deselect, and destory blocks / send blocks to opponent
            referencedChain[i].Deselect();
        }

        GameManager.PostChainLength(QueuedBlocks.Count);
        collectionActive = false;
    }

    /// <summary>
    /// Invalidate the Block Chain
    /// </summary>
    /// <param name="referencedChain"></param>
    static void InvalidateCollection(ref Block[] referencedChain)
    {
        for (int i = 0; i < referencedChain.Length; i++)
        {
            referencedChain[i].Deselect();
        }

        GameManager.PostChainLength(QueuedBlocks.Count);
        collectionActive = false;
    }

    /// <summary>
    /// Add selected blocks to the Queue for Chain Collecting
    /// </summary>
    /// <param name="block"></param>
    public static void AddToChain(Block block)
    {
        QueuedBlocks.Enqueue(block);
        GameManager.PostChainLength(QueuedBlocks.Count);
        
    }
}
