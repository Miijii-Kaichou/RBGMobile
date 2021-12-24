using UnityEngine;
using UnityEngine.UI;

public class ConceptTest : Singleton<ConceptTest>
{
    Image[] imageBlocks;
    Block[] blocks;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];

    int stackSizeMuliplier = 1;

    const int DefaultStackSize = 9;

    public static void Init()
    {
        Instance.imageBlocks = Instance.GetComponentsInChildren<Image>();
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

    // Start is called before the first frame update
    void SetUpBlockData()
    {
        for(int i = 0; i < imageBlocks.Length; i++)
        {
            var value = Random.Range(0, 3);
            imageBlocks[i].color = blockColors[value];
            blocks[i].AssignData((ColorType)value, i);
            blocks[i].SetLaneID(i % 10);
            blocks[i].InitTouchControl();
            blocks[i].ApplyColor();
        }
    }

    public Block[] Blocks => blocks;
}
