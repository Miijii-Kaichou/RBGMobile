using System;
using UnityEngine;

/// <summary>
/// A simple class having a collection Texture2D object that's
/// an avatar avaliable in the game
/// </summary>
public class AvatarCollectionLog : Singleton<AvatarCollectionLog>
{
    [SerializeField]
    Texture2D[] avaliableAvatars = new Texture2D[15];

    public static Texture2D GetAvatar(int index) => Instance.avaliableAvatars[index];
}
