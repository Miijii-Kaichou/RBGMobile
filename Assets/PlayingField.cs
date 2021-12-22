using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingField : Singleton<PlayingField>
{
    GameObject block;

    [SerializeField, Header("Testing")]
    ConceptTest test;

    [SerializeField]
    GameObject[] cachedBlockObjects;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    Timer timer;

    public static int CurrentLevel = 1;

    const int MaxBlockPoolSize = 300;

    static Queue<Block> QueuedBlocks = new Queue<Block>(MaxBlockPoolSize);

    static bool collectionActive = false;
    public static bool CollectionActive => collectionActive;

    public delegate void CollectionValidationCallback();

    [SerializeField]
    float[] xPositions = new float[10];

    internal static void SpawnNewLane()
    {
        int blockCount = 0;

        //Check how many blocks are active

        for (int i = 0; i < Instance.cachedBlockObjects.Length; i++)
        {
            if (blockCount > Instance.xPositions.Length - 1)
                return;

            Block block = Instance.test.Blocks[i];

            if (!block.gameObject.activeInHierarchy)
            {
                block.gameObject.SetActive(true);
                
                block.AssignData((ColorType)((UnityEngine.Random.Range(0, 3) + blockCount) % 3), i);
                block.InitTouchControl();
                block.SetLaneID(blockCount);
                block.RectTransform.anchoredPosition = new Vector2(Instance.xPositions[blockCount], Instance.rectTransform.rect.size.y);
                block.ApplyColor();
                blockCount++;
            }
        }
    }

    public static CollectionValidationCallback CollectionValidationCallbackMethod;

    private void Start()
    {
        cachedBlockObjects = new GameObject[MaxBlockPoolSize];
        for (int i = 0; i < test.Blocks.Length; i++)
        {
            cachedBlockObjects[i] = test.Blocks[i].gameObject;
            if(i < xPositions.Length)
            {
                xPositions[i] = test.Blocks[i].Position.x;
            }
        }

        CollectionValidationCallbackMethod = () =>
        {
            GameManager.PostChainLength(QueuedBlocks.Count);
            collectionActive = false;
            ClearPositions();
            AreaHighlightHandler.Clear();
        };

        timer.StartTimer();
    }

    /// <summary>
    /// Attempt to eliminate the selected blocks
    /// </summary>
    internal static void AttemptChainCollection()
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

                        ClearPositions();
                        return;
                    }
                }
                else if (blocksToChain[j].Color != startingColor || chainCount < validChainCount)
                {
                    InvalidateCollection(ref blocksToChain);
                    ClearPositions();
                    return;
                }
            }
        }
    }

    static void ValidateCollection(ref Block[] referencedChain)
    {
        for (int i = 0; i < referencedChain.Length; i++)
        {
            referencedChain[i].SendToTop();
            //TODO: Deselect, and destory blocks / send blocks to opponent
            referencedChain[i].Deselect(true);
        }

        CollectionValidationCallbackMethod();
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

        CollectionValidationCallbackMethod();
    }

    /// <summary>
    /// Add selected blocks to the Queue for Chain Collecting
    /// </summary>
    /// <param name="block"></param>
    public static void AddToChain(Block block)
    {
        QueuedBlocks.Enqueue(block);
        GameManager.PostChainLength(QueuedBlocks.Count);
        Instance.lineRenderer.positionCount = QueuedBlocks.Count;
        Instance.lineRenderer.SetPosition(QueuedBlocks.Count - 1, block.RectTransform.localPosition);
        AreaHighlightHandler.EnableLane(block.LaneID);
    }

    static void ClearPositions()
    {
        Instance.lineRenderer.positionCount = 0;
    }
}