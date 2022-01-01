using UnityEngine;
using System.Text;

/// <summary>
/// The whole Game Ui can be configured based on an RGBTheme
/// Default themes are ClassicWhite and ClassicDarkBlue
/// </summary>
[CreateAssetMenu(fileName = "New RGB Theme", menuName = "RGB Theme")]
public class RGBTheme : ScriptableObject
{
    public string ThemeTag;

    //Different Element ID will be mapped to different UI
    //In the whole game
    public Texture2D[] ThemeUiTextures;

    //This also includes SFX
    public Audio[] AudioSet;
}