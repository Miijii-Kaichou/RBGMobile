using UnityEngine;
using UnityEngine.UI;

public class ConceptTest : MonoBehaviour
{
    Image[] imageBlocks;
    Block[] blocks;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];

    [SerializeField, Range(1f, 3f)]
    int stackSizeMuliplier = 1;

    const int DefaultStackSize = 29;

    private void OnEnable()
    {
        imageBlocks = GetComponentsInChildren<Image>();
        blocks = GetComponentsInChildren<Block>();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (i > DefaultStackSize + (30 * (stackSizeMuliplier - 1)))
            {
                blocks[i].gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
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
