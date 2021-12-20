using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Blank = -1,
    R,
    G,
    B
}

public class Block : MonoBehaviour
{
    [SerializeField]
    Vector2 _position;

    [SerializeField]
    ColorType _color = ColorType.Blank;


    public void AssignData(Vector2 position, ColorType color)
    {
        _position = position;
        _color = color;
    }
}
