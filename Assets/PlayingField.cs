using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField, Header("Stat Text")]
    TextMeshProUGUI scoreTMP;

    [SerializeField]
    TextMeshProUGUI bestTMP;

    [SerializeField]
    TextMeshProUGUI maxChainTMP;

    [SerializeField]
    TextMeshProUGUI levelTMP;

    [SerializeField, Header("Game Over Sign")]
    GameObject gameOverObject;

    [SerializeField, Header("Pause Menu")]
    GameObject pauseMenuObject;


    public static int CurrentLevel = 1;

    const int MaxBlockPoolSize = 300;

    static Queue<Block> QueuedBlocks = new Queue<Block>(MaxBlockPoolSize);

    static bool collectionActive = false;
    public static bool CollectionActive => collectionActive;

    public delegate void CollectionValidationCallback();

    [SerializeField]
    float[] xPositions = new float[10];

    //Chain Length Info
    static PlayingFieldStats Stats;

    const int BlockScore = 10;

    bool playerDefeated = false;

    bool gameSessionIsPaused = false;

    internal static void SpawnNewLane()
    {
        int blockCount = 0;

        for (int i = 0; i < Instance.cachedBlockObjects.Length; i++)
        {
            if (blockCount > Instance.xPositions.Length - 1)
                return;

            Block block = Instance.test.Blocks[i];

            if (!block.gameObject.activeInHierarchy)
            {
                block.gameObject.SetActive(true);

                block.AssignData((ColorType)((Random.Range(0, 3) + blockCount) % 3), i);
                block.InitTouchControl();
                block.SetLaneID(blockCount);
                block.RectTransform.anchoredPosition = new Vector2(Instance.xPositions[blockCount], Instance.rectTransform.rect.size.y);
                block.ApplyColor();
                blockCount++;
            }
        }
    }

    internal static void Lose()
    {
        Instance.playerDefeated = true;
        Instance.timer.Stop();
        
    }

    /// <summary>
    /// Update Level Text
    /// </summary>
    internal static void PostLevel()
    {
        if (Stats == null) return;
        Instance.levelTMP.text = string.Format("Level {0}", Stats.Level);
    }

    /// <summary>
    /// Update Score Text
    /// </summary>
    internal static void PostScore()
    {
        if (Stats == null) return;
        Instance.scoreTMP.text = Stats.Score.ToString();
    }

    /// <summary>
    /// Update Best Score
    /// </summary>
    internal static void PostBest()
    {
        if (Stats == null) return;
        Instance.bestTMP.text = string.Format("Best: {0}", Stats.BestScore);
    }

    /// <summary>
    /// Update Max Chain Text
    /// </summary>
    internal static void PostMaxChain()
    {
        if (Stats == null) return;
        Instance.maxChainTMP.text = string.Format("Chain: {0}",Stats.MaxChainLength);
    }

    public static CollectionValidationCallback CollectionValidationCallbackMethod;

    private void Start()
    {
        cachedBlockObjects = new GameObject[MaxBlockPoolSize];

        Stats = PlayingFieldStats.CreateNew();
        
        ConceptTest.Init();

        UpdateXPositions();

        CollectionValidationCallbackMethod = () =>
        {
            GameManager.PostChainLength(Stats.ChainLength);
            Stats.CheckLevel();
            PostLevel();
            collectionActive = false;
            ClearPositions();
            AreaHighlightHandler.Clear();
        };

        timer.StartTimer();


        StartCoroutine(PostActiveBlocksCycle());
    }

    void UpdateXPositions()
    {
        for (int i = 0; i < test.Blocks.Length; i++)
        {
            cachedBlockObjects[i] = test.Blocks[i].gameObject;

            if (i < xPositions.Length)
            {
                xPositions[i] = test.Blocks[i].Position.x;
            }
        }
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
                            Stats.ChainLength = chainCount;
                            PostMaxChain();
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

            Stats.Score += BlockScore;
            PostScore();
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
            if (distance > 40.8f * 1.5f) return;
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
        if (QueuedBlocks.Count > 1)
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

    public void OnPauseToggle()
    {
        gameSessionIsPaused = !gameSessionIsPaused;
        pauseMenuObject.SetActive(gameSessionIsPaused);
        Time.timeScale = gameSessionIsPaused ? 0f : 1f;
    }

    IEnumerator PostActiveBlocksCycle()
    {
        playerDefeated = false;

        while (!playerDefeated)
        {
            var activeBlocks = (from activeBlock in cachedBlockObjects where activeBlock.activeInHierarchy select activeBlock).ToArray().Length;
            GameManager.PostActiveBlocks(activeBlocks);
            yield return new WaitForSeconds(0.25f);
        }
    }
}