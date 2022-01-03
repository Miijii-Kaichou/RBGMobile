public class InboxData
{
    public InboxData(string subject, string header, MessageType messageType, string message, bool hasButton, string buttonText, ButtonOnClickCallbackMethod onButtonClick)
    {
        Subject = subject;
        Header = header;
        MessageType = messageType;
        Message = message;
        HasButton = hasButton;
        ButtonText = buttonText;
        OnButtonClick = onButtonClick;
    }

    public string Subject { get; set; }
    public string Header { get; set; }
    MessageType MessageType { get; set; }
    public string Message { get; set; }
    public bool HasButton { get; set; }
    public string ButtonText { get; set; }
    public delegate void ButtonOnClickCallbackMethod();
    public ButtonOnClickCallbackMethod OnButtonClick { get; set; }
    public bool IsRead { get; set; }
    
}