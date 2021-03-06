using UnityEngine;
using UnityEngine.UI;

public class AreaHighlightHandler : Singleton<AreaHighlightHandler>
{
    [SerializeField]
    Image[] cachedLaneHighlights; 

    const float HeightOfHighlight = 828.4f;

    RectTransform initRectTransform;

    public static bool Initialized { get; private set; } = false;

    public void OnEnable()
    {
        cachedLaneHighlights = GetComponentsInChildren<Image>();
        Clear();
    }

    public static void EnableLane(int laneID)
    {
        if(laneID < Instance.cachedLaneHighlights.Length && Instance.cachedLaneHighlights[laneID].enabled == false)
        {
            Instance.cachedLaneHighlights[laneID].enabled = true;
        }
    }

    public static void Clear()
    {
        for (int i = 0; i < Instance.cachedLaneHighlights.Length; i++)
        {
            Instance.cachedLaneHighlights[i].enabled = false;
        }
    }
}
