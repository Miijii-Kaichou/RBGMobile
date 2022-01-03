using UnityEngine;
using TMPro;

public class InboxPanelMessage : MonoBehaviour
{
    [SerializeField]
    Rect messageRect;
    public Rect MessageRect => messageRect;

    [SerializeField]
    TextMeshProUGUI messageTMP;
    public TextMeshProUGUI MessageTMP => messageTMP;
    public string Message
    {
        get
        {
            return messageTMP.text;
        }
        set
        {
            messageTMP.text = value;
        }
    }

    [SerializeField]
    InboxPanelButton messageButton;
    public InboxPanelButton MessageButton => messageButton;
    string MessageContent
    {
        get
        {
            return messageButton.ButtonText;
        } set
        {
            messageButton.ButtonText = value;
        }
    }
}