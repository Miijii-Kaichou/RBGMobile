using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockTouchAction : TouchableEntity
{
    [SerializeField]
    Block blockIdentity;

    [SerializeField]
    Image blockImage;

    [SerializeField]
    Color initColor;


    Color selected = Color.white;

    bool initialized = false;

    bool isSelected = false;
    public override void Init()
    {
        #region First-Time Initialization
        if (initialized == false && gameObject.activeInHierarchy)
        {
            initialized = true;
            initColor = blockImage.color;
            blockIdentity.SetPosition(blockIdentity.RectTransform.localPosition);
            base.Init();
        } 
        #endregion

    }


    // Update is called once per frame
    public override void Main()
    {
        OnTouchEnter(() =>
        {
            if (!isSelected)
            {
                isSelected = true;
                blockImage.color = selected;
                SelectionHandler.EnableSlot(blockIdentity.InstanceID);
                PlayingField.AddToChain(blockIdentity);
            }
        });

        OnTouchExit(() =>
        {
            cachedData = null;
        });

        OnTouchRelease(() =>
        {
            if(!PlayingField.CollectionActive)
                PlayingField.AttemptChainCollection();
        });
    }

    internal void RevertToOriginalColor()
    {
        blockImage.color = initColor;
        isSelected = false;
    }

    
}
