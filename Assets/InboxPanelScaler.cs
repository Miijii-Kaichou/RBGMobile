using UnityEngine;
using UnityEngine.UI;

public class InboxPanelScaler : Singleton<InboxPanelScaler>
{
    [SerializeField]
    RectTransform rect;

    [SerializeField]
    VerticalLayoutGroup vlg;

    private void OnEnable()
    {
        Resize();
    }
    public static void Resize()
    {
        var newHeight = Instance.GetCombinedHeight();
        Vector2 newSizeDelta = new Vector2(Instance.rect.sizeDelta.x, newHeight);
        Instance.rect.sizeDelta = newSizeDelta;
        Instance.vlg.SetLayoutVertical();
    }

    float GetCombinedHeight()
    {
        InboxPanel[] panels = GetComponentsInChildren<InboxPanel>();
        float height = 0;

        for (int i = 0; i < panels.Length; i++)
        {
            height += panels[i].PanelRect.sizeDelta.y;
        }

        return height;
    }
}
