using System;
using UnityEngine;

public enum ColorType
{
    Blank = -1,
    R,
    G,
    B
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
    ColorType _color = ColorType.Blank;

    //Colors
    [SerializeField]
    Color[] blockColors = new Color[3];

    BlockTouchAction attachedTouchAction;

    public Vector2 Position => _position;

    public void Update()
    {
        Mass = 3f;
        SetPosition(_rectTransform.anchoredPosition);
    }

    public void AssignData(ColorType color, int instanceID)
    {
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
        _rectTransform.anchoredPosition = new Vector3(_position.x, _position.y + (828.4f - _position.y), 1f);
    }

    public ColorType Color => _color;

    public void ChangeColorType(ColorType type)
    {
        _color = type;
        
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
    }
}
