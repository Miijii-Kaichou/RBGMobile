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
            SetInitColor(blockImage.color);
            SetColor(initColor);
            blockIdentity.SetPosition(blockIdentity.RectTransform.localPosition);
            base.Init();
        } 
        #endregion

    }

    public void SetInitColor(Color color)
    {
        initColor = color;
    }

    public void SetColor(Color color)
    {
        blockImage.color = color;
    }


    // Update is called once per frame
    public override void Main()
    {
        OnTouchEnter(() =>
        {
            if (!isSelected)
            {
                isSelected = true;
                SetColor(selected);
                SelectionHandler.EnableSlot(blockIdentity);
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
        SetColor(initColor);
        isSelected = false;
        cachedData = null;
    }

    
}
