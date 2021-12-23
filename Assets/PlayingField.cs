using System.Linq;
using System.Collections;
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

            if (i < xPositions.Length)
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
        StartCoroutine(PostActiveBlocksCycle());
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
        //Check if selected item is greater than 40.8. If it is, this is not close to the current selected block
        if (QueuedBlocks.Count > 0)
        {
            var distance = Mathf.Abs(Vector2.Distance(block.Position, QueuedBlocks.ToArray()[QueuedBlocks.Count - 1].Position));
            if (distance > 40.8f * 2f) return;
        }

        //Select block, since now it's valid
        block.AttachedTouchAction.IsSelected = true;
        block.AttachedTouchAction.SetColor(Color.white);
        SelectionHandler.EnableSlot(block);

        //Enqueue block
        QueuedBlocks.Enqueue(block);

        //Increase the Chain Length to Debug
        GameManager.PostChainLength(QueuedBlocks.Count);

        //Draw out line
        Instance.lineRenderer.positionCount = QueuedBlocks.Count;
        Instance.lineRenderer.SetPosition(QueuedBlocks.Count - 1, block.RectTransform.localPosition);

        //Enable lane highlighting
        AreaHighlightHandler.EnableLane(block.LaneID);

        block.Node.ConnectedMain(block);

        //Connect Chain to previous in queue
        if(QueuedBlocks.Count > 1)
        {
            Block[] blockArray = QueuedBlocks.ToArray();
            //Check what side the previous block is on.
            Block previousBlock = blockArray[QueuedBlocks.Count - 2];
            BlockSide previousBlockSide = BlockSide.Right;
            var Xdifference = (block.RectTransform.anchoredPosition.x - previousBlock.RectTransform.anchoredPosition.x);
            var YDifference = (block.RectTransform.anchoredPosition.y - previousBlock.RectTransform.anchoredPosition.y);

            //Check Right, Bottom, Left, and Top
            if (Xdifference > 0)
                previousBlockSide = BlockSide.Left;
            else if (Xdifference < 0)
                previousBlockSide = BlockSide.Right;
            else if (YDifference > 0)
                previousBlockSide = BlockSide.Down;
            else if (YDifference < 0)
                previousBlockSide = BlockSide.Up;

            block.Node.ConnectBlockToSide(previousBlock, previousBlockSide);
        }
    }

    static void ClearPositions()
    {
        Instance.lineRenderer.positionCount = 0;
    }

    IEnumerator PostActiveBlocksCycle()
    {
        while (true)
        {
            var activeBlocks = (from activeBlock in cachedBlockObjects where activeBlock.activeInHierarchy select activeBlock).ToArray().Length;
            GameManager.PostActiveBlocks(activeBlocks);
            yield return new WaitForSeconds(0.25f);
        }
    }
}