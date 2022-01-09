using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITouchable
{
    public delegate void TouchCallback();

    /// <summary>
    /// Is the player still touching the object
    /// </summary>
    public bool IsTouching { get; }

    /// <summary>
    /// The very first time that a user touches the object.
    /// </summary>
    /// <param name="evt"></param>
    public void OnTouchStart(TouchCallback evt);

    /// <summary>
    /// When the user's touch is stationary to the interacting object.
    /// </summary>
    /// <param name="evt"></param>
    public void OnTouchStay(TouchCallback evt);

    /// <summary>
    /// A touch that has interacted with an object (usually when 
    /// the user's still touching the screen)
    /// </summary>
    /// <param name="evt"></param>
    public void OnTouchEnter(TouchCallback evt);

    /// <summary>
    /// A touch that has left an object (usually when the user 
    /// slides their finger off from the interaction object).
    /// </summary>
    /// <param name="evt"></param>
    public void OnTouchExit(TouchCallback evt);

    /// <summary>
    /// The very instance that the player release their touch
    /// from the object
    /// </summary>
    /// <param name="evt"></param>
    public void OnTouchRelease(TouchCallback evt);
}

public abstract class TouchableEntity : MonoBehaviour, ITouchable
{
    [SerializeField]
    protected Collider2D entityCollider;

    [SerializeField]
    Camera mainGameplayCamera;

    public bool IsTouching { get => cachedData != null; }

    [SerializeField]
    protected Collider2D cachedData;

    RaycastHit2D hit;

    protected bool initialized;

    Condition uninteractableCondition;

    public void OnTouchStay(ITouchable.TouchCallback callback)
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Stationary && uninteractableCondition.WasMet == false)
        {
            if (hit.collider == entityCollider)
            {
                callback();
                cachedData = entityCollider;
            }
        }
    }

    public void OnTouchEnter(ITouchable.TouchCallback callback)
    {
        if (Input.touchCount > 0 && uninteractableCondition.WasMet == false)
        {
            if (!IsTouching && hit.collider == entityCollider)
            {
                callback();
                cachedData = entityCollider; 
            }
        }
    }

    public void OnTouchExit(ITouchable.TouchCallback callback)
    {
        if (Input.touchCount > 0 && uninteractableCondition.WasMet == false)
        {
            if (IsTouching && hit.collider != entityCollider)
            {
                cachedData = null;
                callback();
            }
        }
    }

    public void OnTouchRelease(ITouchable.TouchCallback callback)
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended && IsTouching && uninteractableCondition.WasMet == false)
        {
            cachedData = null;
            callback();
        }
    }
    public void OnTouchStart(ITouchable.TouchCallback callback)
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && uninteractableCondition.WasMet == false)
        {
            if (!IsTouching && hit.collider == entityCollider)
            {
                callback();
                cachedData = entityCollider;
            }
        }
    }

    public void Init()
    {
        if (initialized == false)
        {
            initialized = true;
            OnInitialized();
            StartCoroutine(TouchCycle());
        }
    }

    public virtual void OnInitialized()
    {

    }

    protected void DontInteractIf(ref Condition condition)
    {
        uninteractableCondition = condition;
    }

    private void OnEnable()
    {
        if (initialized && PlayingField.GameSessionStarted)
            StartCoroutine(TouchCycle());
    }

    public virtual void Main()
    {

    }

    IEnumerator TouchCycle()
    {
        while (true)
        {
            try
            {
                if (Input.touchCount > 0)
                    hit = Physics2D.Raycast(mainGameplayCamera.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);

                Main();
            }
            catch
            {
                yield break;
            }

            yield return null;

        }
    }
}