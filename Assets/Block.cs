using System;
using UnityEngine;

public enum ColorType
{
    Blank = -1,
    R,
    G,
    B
}

public enum BlockSide
{
    Right,
    Down,
    Left,
    Up
}

[Serializable]
public class BlockNode
{
    [SerializeField]
    Block _main;

    [SerializeField]
    Block[] _existingSides = new Block[4];

    public void ConnectedMain(Block main)
    {
        _main = main;
    }

    /// <summary>
    /// Connect a block to a selected side
    /// </summary>
    /// <param name="block"></param>
    /// <param name="side"></param>
    public void ConnectBlockToSide(Block block, BlockSide side)
    {
        _existingSides[(int)side] = block;
    }
}

public class Block : MassObject
{
    [SerializeField]
    int _instanceID = 0;

    [SerializeField]
    int _laneID = 0;

    [SerializeField]
    RectTransform _rectTransform;

    public int InstanceID => _instanceID;
    public int LaneID => _laneID;
    public RectTransform RectTransform => _rectTransform;

    [SerializeField]
    Vector2 _position;

    [SerializeField]
    Vector2 initPosition;
    
    [SerializeField]
    ColorType _color = ColorType.Blank;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];

    BlockTouchAction attachedTouchAction;

    Block syncingBlock = null;
    float targetSyncingFrames = 6f;
    float currentSyncingFrames = 0f;
    public BlockTouchAction AttachedTouchAction => attachedTouchAction;

    public BlockNode Node;

    public Vector2 Position => _position;

    public override void Start()
    {
        base.Start();
        initPosition = RectTransform.anchoredPosition;
    }

    public void Update()
    {
        Mass = 3f;

        if (syncingBlock != null)
        {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, syncingBlock._rectTransform.anchoredPosition.y);
            SetPosition(_rectTransform.anchoredPosition);
            currentSyncingFrames++;
            if(currentSyncingFrames >= targetSyncingFrames)
            {
                syncingBlock = null;
                currentSyncingFrames = 0;
            }
            
        } else
            SetPosition(_rectTransform.anchoredPosition);

        #region Lose Condition
        if (PlayingField.ResettingPhase == false && justSpawned == false && IsGrounded && Position.y > PlayingField.RectTransform.rect.height / 2f && PlayingField.PlayerDefeated == false)
        {
            Debug.Log($"Block ID: {InstanceID}");
            PlayingField.Lose();
        } 
        #endregion
    }

    public void AssignData(ColorType color, int instanceID)
    {
        IsGrounded = false;
        ChangeColorType(color);
        _instanceID = instanceID;
    }

    public void SetLaneID(int laneId)
    {
        _laneID = laneId;
    }

    public void InitTouchControl()
    {
        attachedTouchAction = attachedTouchAction ?? GetComponent<BlockTouchAction>();
        attachedTouchAction.SetInitColor(blockColors[(int)_color]);
        attachedTouchAction.Init();
    }

    internal void ApplyColor()
    {
        if (attachedTouchAction == null) return;
        if (_color == ColorType.Blank)
        {
            attachedTouchAction.SetColor(UnityEngine.Color.white);
                return;
        }
        attachedTouchAction.SetColor(blockColors[(int)_color]);
    }

    public void Deselect(bool disable = false)
    {
        _position = Vector2.zero;
        attachedTouchAction.RevertToOriginalColor();
        SelectionHandler.DisableSlot(_instanceID);
        if (disable)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public void SendToTop()
    {
        _rectTransform.anchoredPosition = new Vector2(Position.x, Mathf.FloorToInt(PlayingField.RectTransform.rect.height));
    }

    internal void TurnBlank()
    {
        _color = ColorType.Blank;
        ApplyColor();
    }

    public ColorType Color => _color;

    public void ChangeColorType(ColorType type)
    {
        _color = type;
        ApplyColor();
    }

    public void ReturnToInitialPosition() => RectTransform.anchoredPosition = initPosition;

    public override void OnDisable()
    {
        base.OnDisable();
    }

    internal void SyncYPositionWith(Block block)
    {
        syncingBlock = block;
    }
}
