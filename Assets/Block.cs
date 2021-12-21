using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Blank = -1,
    R,
    G,
    B
}

public class Block : MonoBehaviour
{
    [SerializeField]
    int _instanceID = 0;

    [SerializeField]
    RectTransform _rectTransform;

    public int InstanceID => _instanceID;
    public RectTransform RectTransform => _rectTransform;

    [SerializeField]
    Vector2 _position;

    [SerializeField]
    ColorType _color = ColorType.Blank;

    BlockTouchAction attachedTouchAction;

    public void AssignData(ColorType color, int instanceID)
    {
        _color = color;
        _instanceID = instanceID;
        attachedTouchAction = GetComponent<BlockTouchAction>();
        attachedTouchAction.Init();
    }

    public void Deselect()
    {
        _position = Vector2.zero;
        attachedTouchAction.RevertToOriginalColor();
        SelectionHandler.DisableSlot(_instanceID);
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public ColorType Color => _color;
}
