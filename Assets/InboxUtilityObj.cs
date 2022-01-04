using UnityEngine;

public class InboxUtilityObj : MonoBehaviour
{
    public void GoBack()
    {
        GameOverlay.EnableOverlay(1);
        GameSceneManager.LoadPrevious();
    }

    public void MarkAllAsRead()
    {
        //TODO: Go Through all InputPanels and mark as "Read"
    }
}
