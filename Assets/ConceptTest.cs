using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConceptTest : MonoBehaviour
{
    Image[] imageBlocks;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];


    private void OnEnable()
    {
        imageBlocks = GetComponentsInChildren<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < imageBlocks.Length; i++)
        {
            
            imageBlocks[i].color = blockColors[Random.Range(0, 3)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
