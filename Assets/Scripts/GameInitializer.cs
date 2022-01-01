using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The full intent of this call is to set up the blocks for the first time, and give them
/// their color-values upon the start of a session
/// </summary>
public class GameInitializer : Singleton<GameInitializer>
{
    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];

    Image[] imageBlocks;
    Block[] blocks;

    int stackSizeMuliplier = 1;

    const int DefaultStackSize = 9;

    /// <summary>
    /// Initialize all blocks in the object pool
    /// </summary>
    public static void Init()
    {
        Instance.imageBlocks =  Instance.GetComponentsInChildren<Image>();
        Instance.imageBlocks = (from imgB in Instance.imageBlocks where imgB.GetComponent<Block>() != null select imgB).ToArray();
        Instance.blocks = Instance.GetComponentsInChildren<Block>();
        Instance.stackSizeMuliplier = GameManager.SelectedConfig.StartingLaneCount;
        for (int i = 0; i < Instance.blocks.Length; i++)
        {
            if (i > DefaultStackSize + (10 * (Instance.stackSizeMuliplier - 1)))
            {
                Instance.blocks[i].gameObject.SetActive(false);
            }
        }

        Instance.SetUpBlockData();
    }

    /// <summary>
    /// Set up initial block data prior to start of game session
    /// </summary>
    void SetUpBlockData()
    {
        for(int i = 0; i < imageBlocks.Length; i++)
        {
            var value = Random.Range(0, 3);
            imageBlocks[i].color = blockColors[value];
            blocks[i].AssignData((ColorType)value, i);
            blocks[i].SetLaneID(i % 10);
            blocks[i].ApplyColor();
            
            if(blocks[i].gameObject.activeInHierarchy)
                blocks[i].InitTouchControl();
        }
    }

    public Block[] Blocks => blocks;
}
