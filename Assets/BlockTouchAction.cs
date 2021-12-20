using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockTouchAction : MonoBehaviour
{
    [SerializeField]
    Block blockIdentity;

    [SerializeField]
    Image blockImage;

    [SerializeField]
    Collider2D _collider;

    Color initColor;
    Color selected = Color.white;

    public delegate void TouchCallback();
    TouchCallback touchEvent;

    // Start is called before the first frame update
    void Start()
    {
        initColor = blockImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        OnTouch(() =>
        {
            Debug.Log("You touched me!!!");
            blockImage.color = selected;
            PlayingField.AddToChain(blockIdentity);
        });

        OnTouchExit(() =>
        {
            Debug.Log("Why'd you stop? U w U");
            blockImage.color = initColor;
        });
    }


    void OnTouch(TouchCallback callback)
    {
        if(Input.touchCount > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if(hit.collider == _collider)
            {
                callback();
            }
        }
    }

    void OnTouchEnter(TouchCallback callback)
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.collider == _collider)
            {
                callback();
            }
        }
    }

    void OnTouchExit(TouchCallback callback)
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Canceled)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.collider == _collider)
            {
                callback();
            }
        }
    }
}
