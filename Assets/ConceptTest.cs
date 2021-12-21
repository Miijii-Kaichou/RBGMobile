using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConceptTest : MonoBehaviour
{
    Image[] imageBlocks;
    Block[] blocks;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];


    private void OnEnable()
    {
        imageBlocks = GetComponentsInChildren<Image>();
        blocks = GetComponentsInChildren<Block>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < imageBlocks.Length; i++)
        {
            var value = Random.Range(0, 3);
            imageBlocks[i].color = blockColors[value];
            blocks[i].AssignData( (ColorType)value, i);
        }
    }

    public Block[] Blocks => blocks;
}
