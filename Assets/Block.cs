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
        Mass = 2f;
        _color = color;
        _instanceID = instanceID;
        attachedTouchAction = GetComponent<BlockTouchAction>();
        attachedTouchAction.Init();
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
}
