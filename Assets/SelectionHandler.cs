using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{
    private static SelectionHandler Instance;
    static Image[] selectionUIImg;

    private void Awake()
    {
        Instance = this;
        selectionUIImg = GetComponentsInChildren<Image>();
    }

    public static void EnableSlot(int location)
    {
        if(selectionUIImg[location].enabled == false)
        {
            selectionUIImg[location].enabled = true;
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
