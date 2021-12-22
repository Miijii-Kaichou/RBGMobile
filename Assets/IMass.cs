using System.Collections;
using UnityEngine;

public interface IMass
{
    public delegate void OnGroundCallback();
    public float Mass { get; set; }
    public float DecentSpeed { get; set; }
    public bool IsGrounded { get; set; }
    public OnGroundCallback GroundedCallback { get; set; }
}

public class MassObject : MonoBehaviour, IMass
{
    public float Mass { get; set; }
    public float DecentSpeed { get; set; }
    public float detectionLength = 1f;
    public Collider2D originCollider;
    public Collider2D collidingWith;
    public bool IsGrounded { get; set; }

    [SerializeField]
    bool _isGrounded;

    public IMass.OnGroundCallback GroundedCallback { get; set; }

    RectTransform rectTransform;
    RaycastHit2D[] raycastResults = new RaycastHit2D[10];
    Vector2 decentVector;
    Vector2 velocity;
    int groundedFlag = 0;
    int previousGroundedFlag = 0;

    public float gridSize = 40.5f;

    float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }
        return pos;
    }

    public void OnValidate()
    {
        originCollider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Awake()
    {
        originCollider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        

        //TODO: Snap into place
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, RoundToNearestGrid(rectTransform.anchoredPosition.y));
        originCollider.attachedRigidbody.velocity = Vector2.zero;
        StartCoroutine(CheckGroundCycle());
        StartCoroutine(MainCycle());
    }

    /// <summary>
    /// This is where the gravity effect of an object takes place
    /// </summary>
    /// <returns></returns>
    IEnumerator MainCycle()
    {
        while (true)
        {
            _isGrounded = IsGrounded;
            if (IsGrounded == false)
            {
                decentVector = new Vector2(0f, -(Mass * GameManager.GravityValue));
                originCollider.attachedRigidbody.velocity = new Vector2(0f, decentVector.y) * Time.fixedDeltaTime;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, RoundToNearestGrid(rectTransform.anchoredPosition.y));

            } else
            {
                //TODO: Snap into place
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, RoundToNearestGrid(rectTransform.anchoredPosition.y));
                originCollider.attachedRigidbody.velocity = Vector2.zero;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator CheckGroundCycle()
    {
        while (true)
        {
            CheckIfGrounded();
            yield return new WaitForFixedUpdate();
        }
    }

    void CheckIfGrounded()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter = filter.NoFilter();
        filter.SetLayerMask(LayerMask.GetMask("PlayFieldCollidables"));
        raycastResults = new RaycastHit2D[30];

        Physics2D.Raycast(transform.position, new Vector2(0f, -((Mass * GameManager.GravityValue))), filter, raycastResults, detectionLength);
        Debug.DrawRay(transform.position, new Vector2(0f, -((Mass * GameManager.GravityValue))) * detectionLength, Color.red);
        collidingWith = raycastResults[1].collider;
        

        if (collidingWith)
        {
            if (previousGroundedFlag != groundedFlag)
            {
                previousGroundedFlag = groundedFlag;
                if (GroundedCallback != null && GroundedCallback.GetInvocationList().Length > 0) GroundedCallback();
            }
            groundedFlag = 1;
            IsGrounded = true;
        }
        else
        {
            groundedFlag = 0;
            IsGrounded = false;
        }  
    }
}