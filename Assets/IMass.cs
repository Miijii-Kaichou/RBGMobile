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
    Vector2 currentPosition;

    [SerializeField]
    Grid grid;

    [SerializeField]
    bool _isGrounded;

    public IMass.OnGroundCallback GroundedCallback { get; set; }

    RectTransform rectTransform;
    RaycastHit2D[] raycastResults = new RaycastHit2D[10];
    Vector2 decentVector;
    Vector2 velocity;
    int groundedFlag = 0;
    int previousGroundedFlag = 0;

    public float gridSize = 40.8f;

    float RoundToNearestGrid(float pos, int dividend = 2)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > (gridSize / dividend))
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
        currentPosition = rectTransform.anchoredPosition;

        //TODO: Snap into place
        rectTransform.anchoredPosition =
            new Vector2(rectTransform.anchoredPosition.x,
             RoundToNearestGrid(Mathf.Floor(rectTransform.anchoredPosition.y)));
        originCollider.attachedRigidbody.velocity = Vector2.zero;
    }

    public void Start()
    {
        //Each time a validation on the playfield is called, all blocks will check if grounded.
        PlayingField.CollectionValidationCallbackMethod += CheckIfGrounded;

        //TODO: Snap into place
        originCollider.attachedRigidbody.velocity = Vector2.zero;
        StartCoroutine(GroundCheckCycle());
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
                originCollider.attachedRigidbody.velocity = new Vector2(0f,decentVector.y) * Time.fixedDeltaTime;
            } else
            {
                //TODO: Snap into place
                rectTransform.anchoredPosition = 
                    new Vector2(rectTransform.anchoredPosition.x,
                     RoundToNearestGrid(Mathf.Floor(rectTransform.anchoredPosition.y)));
                originCollider.attachedRigidbody.velocity = Vector2.zero;
                
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// A ground check routine checking if the object is grounded,
    /// not matter it's current status.
    /// </summary>
    /// <returns></returns>
    IEnumerator GroundCheckCycle()
    {
        while (true)
        {
            CheckIfGrounded();
            yield return new WaitForSeconds(1f / 1000f);
        }
    }

    void CheckIfGrounded()
    {
        raycastResults = new RaycastHit2D[30];
        ContactFilter2D filter = new ContactFilter2D();
        filter = filter.NoFilter();
        filter.useTriggers = true;
        filter.SetLayerMask(LayerMask.GetMask("PlayFieldCollidables"));
        Physics2D.Raycast(transform.position, new Vector2(0f, -((Mass * GameManager.GravityValue))).normalized, filter, raycastResults, detectionLength);
        Debug.DrawRay(transform.position, (new Vector2(0f, -((Mass * GameManager.GravityValue))).normalized) * detectionLength, Color.red);
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
        else if(collidingWith == null || (collidingWith != null && !collidingWith.GetComponent<MassObject>().IsGrounded))
        {
            groundedFlag = 0;
            IsGrounded = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            CheckIfGrounded();
        }
    }
}