using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldAreaMarker : MonoBehaviour
{
    [SerializeField]
    BoxCollider2D playingArea;


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(playingArea.offset, playingArea.size / 2f);
    }
}
