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

    public bool IsSelected { get; set; } = false;

    Condition dontInteractCondition;
    public override void Init()
    {
        #region First-Time Initialization
        if (initialized == false && gameObject.activeInHierarchy)
        {
            initialized = true;
            dontInteractCondition = new Condition(() => blockIdentity.IsGrounded == false || PlayingField.PlayerDefeated);
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
        DontInteractIf(ref dontInteractCondition);

        OnTouchEnter(() =>
        {
            if (!IsSelected)
            {
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
        IsSelected = false;
        cachedData = null;
    }
}
