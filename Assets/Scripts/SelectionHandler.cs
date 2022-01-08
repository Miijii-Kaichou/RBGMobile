using UnityEngine.UI;

public class SelectionHandler : Singleton<SelectionHandler>
{
    static Image[] selectionUIImg;
    static FadeInAnimation[] fadeInAnim;

    public void Start()
    {
        selectionUIImg = GetComponentsInChildren<Image>();
        fadeInAnim = new FadeInAnimation[selectionUIImg.Length];
        for(int i = 0; i < fadeInAnim.Length; i++)
        {
            fadeInAnim[i] = selectionUIImg[i].GetComponent<FadeInAnimation>();
        }
    }

    public static void EnableSlot(Block blockObj)
    {
        int location = blockObj.InstanceID;

        if (selectionUIImg[location].enabled == false)
        {
            selectionUIImg[location].enabled = true;
            fadeInAnim[location].FadeIn();
            selectionUIImg[location].rectTransform.anchoredPosition = blockObj.Position;
        }
    }

    internal static void DisableSlot(int location)
    {
        if (selectionUIImg[location].enabled == true)
        {
            fadeInAnim[location].Reset();
            selectionUIImg[location].enabled = false;
        }
    }
}
