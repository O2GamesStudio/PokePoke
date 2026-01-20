using UnityEngine;
using System.Collections;

public class TargetPoint : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] float minRotationSpeed = 30f;
    [SerializeField] float maxRotationSpeed = 120f;

    private bool isCompleted = false;
    private Collider2D pointCollider;
    private WaitForFixedUpdate waitFixed;
    private float rotationSpeed;

    void Awake()
    {
        pointCollider = GetComponent<Collider2D>();
        if (pointCollider == null)
        {
            pointCollider = gameObject.AddComponent<CircleCollider2D>();
            ((CircleCollider2D)pointCollider).radius = 0.3f;
        }
        pointCollider.isTrigger = true;
        waitFixed = new WaitForFixedUpdate();

        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value > 0.5f) rotationSpeed *= -1f;
    }

    void Start()
    {
        StartCoroutine(CheckOverlapNextFrame());
    }

    void Update()
    {
        if (!isCompleted)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator CheckOverlapNextFrame()
    {
        yield return waitFixed;
        CheckOverlapWithObstacles();
    }

    void CheckOverlapWithObstacles()
    {
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        for (int i = 0; i < overlaps.Length; i++)
        {
            if (overlaps[i].CompareTag("StuckObj"))
            {
                StuckObj stuckObj = overlaps[i].GetComponent<StuckObj>();
                if (stuckObj != null && stuckObj.IsStuckToTarget())
                {
                    CompletePoint();
                    return;
                }
            }
        }
    }

    void CompletePoint()
    {
        if (isCompleted) return;

        isCompleted = true;
        TargetPointManager.Instance?.OnPointCompleted(this);
        Destroy(gameObject);
    }

    public bool IsCompleted => isCompleted;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCompleted || !collision.CompareTag("StuckObj")) return;

        StuckObj stuckObj = collision.GetComponent<StuckObj>();
        if (stuckObj != null)
        {
            CompletePoint();
        }
    }
}