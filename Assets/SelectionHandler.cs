using UnityEngine.UI;

public class SelectionHandler : Singleton<SelectionHandler>
{
    static Image[] selectionUIImg;

    public void Start()
    {
        selectionUIImg = GetComponentsInChildren<Image>();
    }

    public static void EnableSlot(Block blockObj)
    {
        int location = blockObj.InstanceID;

        if (selectionUIImg[location].enabled == false)
        {
            selectionUIImg[location].enabled = true;
            selectionUIImg[location].rectTransform.anchoredPosition = blockObj.Position;
        }
    }

    internal static void DisableSlot(int location)
    {
        if (selectionUIImg[location].enabled == true)
        {
            selectionUIImg[location].enabled = false;
        }
    }
}
