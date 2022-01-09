using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeInAnimation : MonoBehaviour
{
    [SerializeField]
    Image selectionUIImg;

    float alphaValue;
    float alphaVelocity = 0f;
    const float MaxAlphaValue = 1;


    Color initColor;
    Color color;

    Vector3 objectScale;
    Vector3 initScale;
    readonly Vector3 targetVector = new Vector3(1.5f, 1.5f, 1f);
    Vector3 scaleVelocity;

    bool isAppearing = false;

    private void Awake()
    {
        selectionUIImg = GetComponent<Image>();
        alphaValue = 0;
        initScale = transform.localScale;
        objectScale = initScale;
        initColor = selectionUIImg.color;
        selectionUIImg.color = new Color(initColor.r, initColor.g, initColor.b, 0f);
        color = initColor;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        isAppearing = true;
        objectScale = new Vector3(2.5f, 2.5f);
        while (isAppearing)
        {
            
            color = selectionUIImg.color;

            transform.localScale = Vector3.SmoothDamp(objectScale, targetVector, ref scaleVelocity, 0.1f);
            objectScale = transform.localScale;
            alphaValue = Mathf.SmoothDamp(alphaValue, MaxAlphaValue, ref alphaVelocity, 0.1f);
            selectionUIImg.color = new Color(color.r, color.b, color.b, alphaValue);

            yield return new WaitForSeconds(1f / 1000f);
        }
    }

    public void Reset()
    {
        isAppearing = false;
        transform.localScale = initScale;
        selectionUIImg.color = initColor;
        alphaValue = 0;
        Awake();
    }
}
