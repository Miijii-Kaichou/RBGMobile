using UnityEngine;

public class ThemeHandler : Singleton<ThemeHandler>
{
    [SerializeField]
    RGBTheme[] themes = new RGBTheme[10];

    public RGBTheme GetTheme(int index) => themes[index];
}
