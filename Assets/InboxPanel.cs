using System;
using UnityEngine;
using TMPro;


public class InboxPanel : MonoBehaviour
{
    [SerializeField]
    RectTransform _PanelRect;
    public RectTransform PanelRect => _PanelRect;
    [SerializeField]
    MessageType inboxType;

    [SerializeField, Header("Panel Signature")]
    TextMeshProUGUI subjectTMP;
    public string Subject
    {
        get
        {
            return subjectTMP.text;
        }
        set
        {
            subjectTMP.text = value;
        }
    }

    [SerializeField]
    TextMeshProUGUI headerTMP;
    public string Header
    {
        get
        {
            return headerTMP.text;
        }
        set
        {
            headerTMP.text = value;
        }
    }

    [SerializeField, Header("InboxMessage")]
    InboxPanelMessage message;

    public bool Expanded = false;

    float originalHeight;
    Vector2 originalSizeDelta;

    private void OnEnable()
    {
        originalHeight = _PanelRect.rect.height;
        originalSizeDelta = _PanelRect.sizeDelta;
    }

    public void ToggleViewingState()
    {
        Action toggle;
        
        if (Expanded) toggle = Collapse;
        else toggle = Expand;
        toggle();
        Expanded = !Expanded;
        InboxPanelScaler.Resize();
    }

    void Expand()
    {
        if (Expanded) return;
        _PanelRect.sizeDelta = new Vector2(_PanelRect.sizeDelta.x, originalHeight + message.Message.Length + message.MessageButton.Button.image.rectTransform.sizeDelta.y);
    }

    void Collapse()
    {
        if (!Expanded) return;
        _PanelRect.sizeDelta = originalSizeDelta;
    }
}