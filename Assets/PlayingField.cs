using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingField : Singleton<PlayingField>
{
    #region Serialized Fields
    [SerializeField, Header("Testing")]
    GameInitializer test;

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

    [SerializeField, Header("Grid Layout Component")]
    private GridLayoutGroup gridLayoutComponent;

    [SerializeField]
    float[] xPositions = new float[10];

    bool gameSessionIsPaused = false;
    #endregion

    public static int CurrentLevel = 1;
    public static bool CollectionActive => collectionActive;
    public static bool ResettingPhase = false;

    public delegate void PlayingFieldCallback();
    public static PlayingFieldCallback CollectionValidationCallbackMethod;
    public static PlayingFieldCallback PlayingFieldResetCallbackMethod;

    //Chain Length Info
    public static bool PlayerDefeated { get; private set; } = false;
    public static RectTransform RectTransform => Instance.rectTransform;

    static PlayingFieldStats Stats;
    static Alarm PlayingFieldAlarm = new Alarm(2);
    static Queue<Block> QueuedBlocks = new Queue<Block>(MaxBlockPoolSize);
    static bool collectionActive = false;
    static bool haveXPositions = false;
    const int MaxBlockPoolSize = 300;
    const int BlockScore = 10;
    public static bool GameSessionStarted = false;

    //Start is called just before any of the Update Method is called the first time
    private void Start()
    {
        InitPlayingField();
    }


    #region Non-Static Methods
    void InitPlayingField()
    {
        PlayingFieldAlarm = new Alarm(3);

        cachedBlockObjects = new GameObject[MaxBlockPoolSize];

        Stats = PlayingFieldStats.CreateNew();

        GameInitializer.Init();

        

        CollectionValidationCallbackMethod = () =>
        {
            GameSessionDebugger.PostChainLength(Stats.ChainLength);
            Stats.CheckLevel();
            PostLevel();
            collectionActive = false;
            ClearPositions();
            AreaHighlightHandler.Clear();
        };

        PlayingFieldAlarm.SetFor(2f, PlayingFieldAlarm.Avaliable, true, () =>
        {

            UpdateXPositions();

            timer.StartTimer();

            StartCoroutine(PostActiveBlocksCycle());
            StartCoroutine(TouchOnScreenCycle());

            ResettingPhase = false;

            GameSessionStarted = true;
        });
    }
    void UpdateXPositions()
    {
        for (int i = 0; i < test.Blocks.Length; i++)
        {
            cachedBlockObjects[i] = test.Blocks[i].gameObject;

            if (haveXPositions) continue;

            if (i < xPositions.Length)
            {
                xPositions[i] = test.Blocks[i].Position.x;
            }
            else
                haveXPositions = true;
        }
    }

    public void OnPauseToggle()
    {
        gameSessionIsPaused = !gameSessionIsPaused;
        pauseMenuObject.SetActive(gameSessionIsPaused);
        Time.timeScale = gameSessionIsPaused ? 0f : 1f;
    }

    /// <summary>
    /// Will Reset the whole playing field.
    /// </summary>
    public void ResetField()
    {
        timer.Stop();

        StopAllCoroutines();

        ResettingPhase = true;

        //TODO: Set all values blank
        TurnActiveBlocksBlank();

        //TODO: Enable all blocks
        for (int i = 0; i < cachedBlockObjects.Length; i++)
        {
            cachedBlockObjects[i].SetActive(true);
        }

        //TODO: Enable GridLayout to resort all objects
        gridLayoutComponent.enabled = true;


        PlayerDefeated = false;

        //Create new Queue
        QueuedBlocks = new Queue<Block>(300);

        InitPlayingField();
        gridLayoutComponent.enabled = false;
    }

    #endregion

    #region Static Methods
    internal static void SpawnNewLane()
    {
        int blockCount = 0;

        Block[] blocksToDrop = new Block[10];

        Instance.UpdateXPositions();

        for (int i = 0; i < Instance.cachedBlockObjects.Length; i++)
        {
            if (blockCount > Instance.xPositions.Length - 1)
                break;

            Block block = Instance.test.Blocks[i];

            if (!block.gameObject.activeInHierarchy)
            {
                block.gameObject.SetActive(true);
                block.DisableGravity();
                block.AssignData((ColorType)((Random.Range(0, 3) + blockCount) % 3), i);
                block.InitTouchControl();
                block.SetLaneID(blockCount);
                block.RectTransform.anchoredPosition = new Vector2(Instance.xPositions[blockCount], Mathf.FloorToInt(Instance.rectTransform.rect.height));
                block.ApplyColor();
                blocksToDrop[blockCount] = block;
                blockCount++;
            }
        }

        Block firstBlock = null;

        for (int i = 0; i < blockCount; i++)
        {
            blocksToDrop[i].IsGrounded = false;
            if (i == 0)
            {
                firstBlock = blocksToDrop[i];

            }
            else
            {
                blocksToDrop[i].SyncYPositionWith(firstBlock);
            }

            blocksToDrop[i].EnableGravity();
        }
    }

    internal static void Lose()
    {
        PlayerDefeated = true;

        ClearPositions();

        Instance.timer.Stop();

        //Create new Queue
        QueuedBlocks = new Queue<Block>(300);

        //TODO: Set all active blocks to blank.
        TurnActiveBlocksBlank();

        PlayingFieldAlarm.SetFor(2f, PlayingFieldAlarm.Avaliable, true, () =>
        {
            //TODO: Disable them one by one
            WipeOutPlayingField(GetAllActiveBlocks());
        });
    }

    /// <summary>
    /// Turns all active blocks in the Playing Field white (signaling that it's Game Over)
    /// </summary>
    static void TurnActiveBlocksBlank()
    {
        var activeBlocks = GetAllActiveBlocks(out GameObject[] objects);
        for (int i = 0; i < activeBlocks.Length; i++)
        {
            activeBlocks[i].Deselect();
            activeBlocks[i].TurnBlank();
        }
    }

    /// <summary>
    /// Removes each active block during a set interval, wiping the entire playing field
    /// </summary>
    static void WipeOutPlayingField(Block[] blocks)
    {
        int count = 0;

        //Alarm-Duration Loop
        PlayingFieldAlarm.SetFor(0.005f, PlayingFieldAlarm.Avaliable, false, () =>
        {
            //Condition
            if (count < blocks.Length)
            {
                blocks[count].gameObject.SetActive(false);
            }
            else
            {
                //Alarm no longer needs to be used.
                //This breaks from the Alarm-Duration Loop
                Instance.gameOverObject.SetActive(true);
                PlayingFieldAlarm.Discard();
            }

            //Increment Value
            count++;
        });
    }

    static Block[] GetAllActiveBlocks(out GameObject[] gameObjects)
    {
        Block[] blocks;
        var activeBlocks = (from activeBlock in Instance.cachedBlockObjects where activeBlock.activeInHierarchy select activeBlock).ToArray();
        gameObjects = activeBlocks;
        blocks = new Block[activeBlocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = activeBlocks[i].GetComponent<Block>();
        }

        return blocks;
    }

    static Block[] GetAllActiveBlocks()
    {
        Block[] blocks;
        var activeBlocks = (from activeBlock in Instance.cachedBlockObjects where activeBlock.activeInHierarchy select activeBlock).ToArray();
        blocks = new Block[activeBlocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = activeBlocks[i].GetComponent<Block>();
        }

        return blocks;
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
        Instance.maxChainTMP.text = string.Format("Chain: {0}", Stats.MaxChainLength);
    }


    /// <summary>
    /// Attempt to eliminate the selected blocks
    /// </summary>
    internal static void AttemptChainCollection()
    {
        Block block = null;

        if (!collectionActive)
        {
            collectionActive = true;
            Block[] blocksToChain = new Block[QueuedBlocks.Count];

            int i = 0;

            //TODO: Dequeue all QueueBlocks, and deselect them
            while (QueuedBlocks.Count > 0)
            {
                block = QueuedBlocks.Dequeue();
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
            var distance = Mathf.Abs(Vector2.Distance(block.Position ,QueuedBlocks.ToArray()[QueuedBlocks.Count - 1].Position));
            if (distance > Instance.gridLayoutComponent.cellSize.x * 1.75f) return;
        }

        //Select block, since now it's valid
        block.AttachedTouchAction.IsSelected = true;
        block.AttachedTouchAction.SetColor(Color.white);
        SelectionHandler.EnableSlot(block);

        //Enqueue block
        AudioManager.Play("BlockSelect", 50f, true);
        QueuedBlocks.Enqueue(block);

        //Increase the Chain Length to Debug
        GameSessionDebugger.PostChainLength(QueuedBlocks.Count);

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
    #endregion

    #region Non-Static Coroutines
    IEnumerator PostActiveBlocksCycle()
    {
        if (!GameManager.EnableDebug) yield return null;

        while (!PlayerDefeated)
        {
            var activeBlocks = (from activeBlock in cachedBlockObjects where activeBlock.activeInHierarchy select activeBlock).ToArray().Length;
            GameSessionDebugger.PostActiveBlocks(activeBlocks);
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator TouchOnScreenCycle()
    {
        while (!PlayerDefeated)
        {
            if (Input.touchCount < 1 && QueuedBlocks.Count > 0 && CollectionActive == false)
            {
                AttemptChainCollection();
            }
            yield return null;
        }
    }
    #endregion
}